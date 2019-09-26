using AzSignalR.Monitor.Storage.Entities;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzSignalR.Monitor.Storage.Tables
{
    public interface IVersionTable : IAzureTable<VersionEntity>
    {
        Task<VersionEntity> GetCurrent(ResourceType resourceType);
        Task<List<VersionEntity>> GetRecentVersionEntities(ResourceType resourceType, int days = 7, int hours = 0);
        Task<List<VersionEntity>> GetVersionEntitiesBetween(ResourceType resourceType, DateTime startTime, DateTime endTime);
    }

    public class VersionTable : AzureTable<VersionEntity>, IVersionTable
    {
        public VersionTable(CloudStorageAccount storageAccount, string tableName) : base(storageAccount, tableName) { }

        public async Task<VersionEntity> GetCurrent(ResourceType resourceType)
        {
            var query = new TableQuery<VersionEntity>()
                .Where(TableQuery.GenerateFilterCondition(TableConstants.PartitionKey, QueryComparisons.Equal, resourceType.ToString()))
                .Take(1);
            var queryResults = await ExecuteQuerySegmentedAsync(query, null as TableContinuationToken);
            return queryResults.FirstOrDefault();
        }

        public async Task<List<VersionEntity>> GetRecentVersionEntities(ResourceType resourceType,
            int days = 7, int hours = 0)
        {
            var lastMonthInverseTimeKey = Utils.InversedTimeKey(DateTime.UtcNow - new TimeSpan(days, hours, 0, 0));
            var query = new TableQuery<VersionEntity>().Where(
                TableQuery.CombineFilters(
                    TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, resourceType.ToString()),
                    TableOperators.And,
                    TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.LessThanOrEqual,
                        lastMonthInverseTimeKey)
                )
            );
            var versionEntities = new List<VersionEntity>();
            await ExecuteQuerySegmentedAsync(query, (segment) =>
            {
                versionEntities.AddRange(segment);
            });
            return versionEntities;
        }

        public async Task<List<VersionEntity>> GetVersionEntitiesBetween(ResourceType resourceType,
            DateTime startTime, DateTime endTime)
        {
            var formerVersion = Utils.InversedTimeKey(startTime);
            var laterVersion = Utils.InversedTimeKey(endTime);
            var query = new TableQuery<VersionEntity>().Where(
                TableQuery.CombineFilters(
                    TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, resourceType.ToString()),
                    TableOperators.And,
                    TableQuery.CombineFilters(
                        TableQuery.GenerateFilterCondition("RowKey",
                            QueryComparisons.LessThanOrEqual, formerVersion),
                        TableOperators.And,
                        TableQuery.GenerateFilterCondition("RowKey",
                            QueryComparisons.GreaterThanOrEqual, laterVersion)
                    )
                )
            );
            var versionEntities = new List<VersionEntity>();
            await ExecuteQuerySegmentedAsync(query, (segment) =>
            {
                versionEntities.AddRange(segment);
            });
            return versionEntities;
        }

    }
}
