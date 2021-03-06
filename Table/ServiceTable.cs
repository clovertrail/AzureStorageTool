using AzSignalR.Monitor.Storage.Entities;
using Microsoft.WindowsAzure.Storage.Table;

namespace AzSignalR.Monitor.Storage.Tables
{
    public class ServiceTable : BaseMonitorTable<TableEntity>
    {
        public ServiceTable(CloudTableClient storageTableClient, string tableName = "services") :
            base(tableName, storageTableClient)
        { }
    }
}
