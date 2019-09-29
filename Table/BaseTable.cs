using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace AzSignalR.Monitor.Storage.Tables
{
    public abstract class BaseTable
    {
        public string TableName { get; }
        public CloudTable Table { get; }

        protected BaseTable(string tableName, CloudTableClient storageTableClient)
        {
            this.TableName = tableName;
            this.Table = storageTableClient.GetTableReference(TableName);
            Table.CreateIfNotExistsAsync();
        }

        protected async Task<TEntity> ExecuteSearchEntity<TEntity>(
            TableQuery<TEntity> query,
            Func<TEntity, bool> filter)
            where TEntity : TableEntity, new()
        {
            bool found = false;
            TEntity entity = default(TEntity);
            TableContinuationToken continuationToken = null;
            do
            {
                var queryResults = await Table.ExecuteQuerySegmentedAsync(query, continuationToken);
                if (filter != null)
                {
                    foreach (var en in queryResults)
                    {
                        if (filter(en))
                        {
                            entity = en;
                            found = true;
                            break;
                        }
                    }
                }
                continuationToken = queryResults.ContinuationToken;
            } while (continuationToken != null && !found);
            return entity;
        }

        protected async Task<List<TEntity>> ExecuteSegmentedQueryAsync<TEntity>(TableQuery<TEntity> query)
            where TEntity : TableEntity, new()
        {
            var results = new List<TEntity>();
            TableContinuationToken continuationToken = null;
            var queryResults = await Table.ExecuteQuerySegmentedAsync(query, continuationToken);
            results.AddRange(queryResults.Results);
            return results;
        }

        protected async Task<List<TEntity>> ExecuteSegmentedQueryAndGetAllResultsAsync<TEntity>(TableQuery<TEntity> query)
            where TEntity : TableEntity, new()
        {
            var results = new List<TEntity>();
            TableContinuationToken continuationToken = null;
            do
            {
                var queryResults = await Table.ExecuteQuerySegmentedAsync(query, continuationToken);
                continuationToken = queryResults.ContinuationToken;
                results.AddRange(queryResults.Results);
            } while (continuationToken != null);
            return results;
        }

        protected async Task<int> ExecuteBatchDeleteAsync<TEntity>(TableQuery<TEntity> query)
            where TEntity : TableEntity, new()
        {
            int deleted = 0, r = 0, pos = 0;
            TableContinuationToken continuationToken = null;
            do
            {
                var queryResults = await Table.ExecuteQuerySegmentedAsync(query, continuationToken);
                continuationToken = queryResults.ContinuationToken;
                var aggregatedEntities = from entity in queryResults.Results
                                         group entity by entity.PartitionKey into newEntity
                                         orderby newEntity.Key
                                         select newEntity;
                if (aggregatedEntities.Count() > 0)
                {
                    foreach (var entities in aggregatedEntities)
                    {   
                        if (entities.Count() < TableConstants.TableServiceBatchMaximumOperations)
                        {
                            var tableBatchOperation = new TableBatchOperation();
                            foreach (var entity in entities)
                            {
                                tableBatchOperation.Add(TableOperation.Delete(entity));
                            }
                            Console.WriteLine($"Patch delete for {tableBatchOperation.Count}");
                            var result = await Table.ExecuteBatchAsync(tableBatchOperation);
                            for (int i = 0; i < result.Count; i++)
                            {
                                deleted += (result[i].HttpStatusCode == 204 ? 1 : 0);
                            }
                        }
                        else
                        {
                            var entityList = entities.ToList();
                            do
                            {
                                int i;
                                var tableBatchOperation = new TableBatchOperation();
                                for (i = pos; i < pos + TableConstants.TableServiceBatchMaximumOperations && i < entityList.Count; i++)
                                {
                                    tableBatchOperation.Add(TableOperation.Delete(entityList[i]));
                                }
                                pos = i;
                                Console.WriteLine($"Patch delete for {tableBatchOperation.Count}");
                                var result = await Table.ExecuteBatchAsync(tableBatchOperation);
                                for (i = 0; i < result.Count; i++)
                                {
                                    deleted += (result[i].HttpStatusCode == 204 ? 1 : 0);
                                }
                            } while (pos < entities.Count());
                            pos = 0;
                        }
                        
                        Console.WriteLine($"Deleted {deleted} entries");
                    }
                    Console.WriteLine($"Finish {r} round of batch delete");
                }
                r++;
            } while (continuationToken != null);
            return deleted;
        }

        protected async Task<int> ExecuteDeleteAsync<TEntity>(TableQuery<TEntity> query)
            where TEntity : TableEntity, new()
        {
            int deleted = 0;
            TableContinuationToken continuationToken = null;
            do
            {
                var queryResults = await Table.ExecuteQuerySegmentedAsync(query, continuationToken);
                continuationToken = queryResults.ContinuationToken;
                foreach (var queryResult in queryResults)
                {
                    var tableOperation = TableOperation.Delete(queryResult);
                    var result = await Table.ExecuteAsync(tableOperation);
                    if (result.HttpStatusCode == 204)
                    {
                        deleted += 1;
                    }
                }
            } while (continuationToken != null);
            return deleted;
        }
    }
}
