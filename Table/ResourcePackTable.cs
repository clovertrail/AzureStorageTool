using AzSignalR.Monitor.Storage.Entities;
using Microsoft.WindowsAzure.Storage.Table;

namespace AzSignalR.Monitor.Storage.Tables
{
    public class ResourcePackTable : BaseMonitorTable<TableEntity>
    {
        public ResourcePackTable(CloudTableClient storageTableClient, string tableName = "respack") : base(tableName, storageTableClient) { }

    }
}
