using AzSignalR.Monitor.Storage.Entities;
using Microsoft.WindowsAzure.Storage.Table;

namespace AzSignalR.Monitor.Storage.Tables
{
    public class MetadataTable : BaseMonitorTable<TableEntity>
    {
        public MetadataTable(CloudTableClient storageTableClient, string tableName = "meta") : base(tableName, storageTableClient) { }
    }
}
