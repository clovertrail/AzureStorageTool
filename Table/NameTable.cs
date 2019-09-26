using AzSignalR.Monitor.Storage.Entities;
using Microsoft.WindowsAzure.Storage.Table;

namespace AzSignalR.Monitor.Storage.Tables
{
    public class NameTable : BaseMonitorTable<TableEntity>
    {
        public NameTable(CloudTableClient storageTableClient, string tableName = "names") : base(tableName, storageTableClient) { }

    }
}
