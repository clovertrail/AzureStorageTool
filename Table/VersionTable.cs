using AzSignalR.Monitor.Storage.Entities;
using Microsoft.WindowsAzure.Storage.Table;
using System.Linq;
using System.Threading.Tasks;

namespace AzSignalR.Monitor.Storage.Tables
{
    public class VersionTable : BaseMonitorTable<TableEntity>
    {
        public VersionTable(CloudTableClient storageTableClient, string tableName = "versions") : base(tableName, storageTableClient) { }

        public async Task<TableEntity> GetCurrent(ResourceType resourceType)
        {
            var query = new TableQuery<TableEntity>()
                .Where(TableQuery.GenerateFilterCondition(TableConstants.PartitionKey, QueryComparisons.Equal, resourceType.ToString()))
                .Take(1);
            var queryResults = await ExecuteQueryAsync(query);
            return queryResults.FirstOrDefault();
        }
    }
}
