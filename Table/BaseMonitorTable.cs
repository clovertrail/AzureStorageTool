using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AzSignalR.Monitor.Storage.Tables
{
    public abstract class BaseMonitorTable<TEntity> : BaseTable where TEntity : TableEntity, new()
    {
        protected BaseMonitorTable(string tableName, CloudTableClient storageTableClient) :
            base(tableName, storageTableClient)
        { }

        public async Task<List<TEntity>> GetFromVersion(string version)
        {
            var query = new TableQuery<TEntity>().Where(
                    TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, version));
            var results = await ExecuteSegmentedQueryAndGetAllResultsAsync(query);
            return results;
        }

        public async Task<List<TEntity>> GetBasedTimestamp(DateTimeOffset dateTimeOffset)
        {
            var query = new TableQuery<TEntity>().Where(
                    TableQuery.GenerateFilterConditionForDate("Timestamp", QueryComparisons.LessThan, dateTimeOffset));
            var results = await ExecuteSegmentedQueryAndGetAllResultsAsync(query);
            return results;
        }


        public async Task<int> DeleteOldEntities(int daysBefore)
        {
            var query = new TableQuery<TEntity>().Where(
                    TableQuery.GenerateFilterConditionForDate("Timestamp",
                    QueryComparisons.LessThan, DateTimeOffset.Now.AddDays(-daysBefore).Date));
            var result = await ExecuteBatchDeleteAsync(query);
            return result;
        }

        public async Task<TEntity> PointQuery(string partitionKey, string rowKey)
        {
            var retrieveOperation = TableOperation.Retrieve<TEntity>(partitionKey, rowKey);
            var retrieveResult = await Table.ExecuteAsync(retrieveOperation);
            var entity = retrieveResult.Result as TEntity;
            return entity;
        }

        public async Task InsertOrReplaceAllAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
        {
            const int batchSize = 20;
            var batch = new TableBatchOperation();
            foreach (var entity in entities)
            {
                batch.InsertOrReplace(entity);
                if (batch.Count > batchSize)
                {
                    await Table.ExecuteBatchAsync(batch);
                    batch.Clear();
                }
            }
            if (batch.Count > 0)
            {
                await Table.ExecuteBatchAsync(batch);
            }
        }
    }
}
