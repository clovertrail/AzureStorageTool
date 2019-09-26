using AzSignalR.Monitor.Storage.Entities;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AzSignalR.Monitor.Storage.Tables
{
    public interface IRegionConfigTable : IAzureTable<RegionConfigEntity>
    {
        Task<List<RegionConfigEntity>> GetAllAsync(CancellationToken cancellationToken);
    }

    public class RegionConfigTable : AzureTable<RegionConfigEntity>, IRegionConfigTable
    {
        public RegionConfigTable(CloudStorageAccount storageTableClient, string tableName) :
            base(storageTableClient, tableName)
        { }

        public async Task<List<RegionConfigEntity>> GetAllAsync(CancellationToken cancellationToken)
        {
            var results = new List<RegionConfigEntity>();
            var query = new TableQuery<RegionConfigEntity>();
            await ExecuteQuerySegmentedAsync(query, action: (segment) =>
            {
                results.AddRange(segment);
            }, cancellationToken: cancellationToken);
            return results;
        }
    }
}
