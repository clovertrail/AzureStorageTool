using AzSignalR.Monitor.Storage.Entities;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AzSignalR.Monitor.Storage.Tables
{
    public class NameTable : BaseMonitorTable<TableEntity>
    {
        public NameTable(CloudTableClient storageTableClient, string tableName = "names") : base(tableName, storageTableClient) { }

        public async Task<List<NameEntity>> GetPeroidAsync(int onOrBeforeDays, int onOrAfterDays, CancellationToken cancellationToken)
        {
            string dateOnOrAfter = null, dateOnOrBefore = null;
            if (onOrAfterDays != 0)
            {
                dateOnOrAfter = TableQuery.GenerateFilterConditionForDate(
                    "Timestamp",
                    QueryComparisons.GreaterThanOrEqual,
                    DateTimeOffset.Now.AddDays(-onOrAfterDays).Date);
            }
            if (onOrBeforeDays != 0)
            {
                dateOnOrBefore = TableQuery.GenerateFilterConditionForDate(
                    "Timestamp",
                    QueryComparisons.LessThanOrEqual,
                    DateTimeOffset.Now.AddDays(-onOrBeforeDays).Date);
            }
            TableQuery<NameEntity> query = null;
            if (dateOnOrAfter != null && dateOnOrBefore == null)
            {
                query = new TableQuery<NameEntity>().Where(
                    TableQuery.CombineFilters(dateOnOrAfter,
                    TableOperators.And,
                    TableQuery.GenerateFilterCondition(
                        TableConstants.PartitionKey,QueryComparisons.NotEqual,NameEntity.IndexPartition)));
            }
            else if (dateOnOrAfter == null && dateOnOrBefore != null)
            {
                query = new TableQuery<NameEntity>().Where(
                    TableQuery.CombineFilters(dateOnOrBefore,
                    TableOperators.And,
                    TableQuery.GenerateFilterCondition(
                        TableConstants.PartitionKey, QueryComparisons.NotEqual, NameEntity.IndexPartition)));
            }
            else if (dateOnOrAfter != null && dateOnOrBefore != null)
            {
                query = new TableQuery<NameEntity>().Where(
                    TableQuery.CombineFilters(
                        TableQuery.CombineFilters(dateOnOrAfter, TableOperators.And, dateOnOrBefore),
                        TableOperators.And,
                        TableQuery.GenerateFilterCondition(
                            TableConstants.PartitionKey, QueryComparisons.NotEqual, NameEntity.IndexPartition)));
            }
            else
            {
                query = new TableQuery<NameEntity>().Where(
                    TableQuery.GenerateFilterCondition(
                        TableConstants.PartitionKey,
                        QueryComparisons.NotEqual,
                        NameEntity.IndexPartition));
            }
            var results = new List<NameEntity>();
            await ExecuteQuerySegmentedAsync(query, segment =>
            {
                results.AddRange(segment);
            }, cancellationToken);
            return results;
        }

        public async Task<List<NameEntity>> GetAllAsync(CancellationToken cancellationToken)
        {
            var query = new TableQuery<NameEntity>().Where(
                TableQuery.GenerateFilterCondition(
                    TableConstants.PartitionKey,
                    QueryComparisons.NotEqual,
                    NameEntity.IndexPartition));
            var results = new List<NameEntity>();
            await ExecuteQuerySegmentedAsync(query, segment =>
            {
                results.AddRange(segment);
            }, cancellationToken);
            return results;
        }

        private async Task ExecuteQuerySegmentedAsync(
            TableQuery<NameEntity> query,
            Action<TableQuerySegment<NameEntity>> action,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            TableContinuationToken continuationTable = null;
            do
            {
                var queryResult = await Table.ExecuteQuerySegmentedAsync(query, continuationTable);
                action?.Invoke(queryResult);
                continuationTable = queryResult.ContinuationToken;
            } while (continuationTable != null);
        }
    }
}
