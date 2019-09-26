using AzSignalR.Monitor.Storage.Entities;
using Microsoft.WindowsAzure.Storage.Table;

namespace AzSignalR.Monitor.Storage.Tables
{
    public class NameIndexTable : BaseMonitorTable<TableEntity>
    {
        public NameIndexTable(CloudTableClient storageTableClient, string tableName = "nameindexes") : base(tableName, storageTableClient) { }
    }
}
