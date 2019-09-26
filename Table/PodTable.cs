using AzSignalR.Monitor.Storage.Entities;
using Microsoft.WindowsAzure.Storage.Table;

namespace AzSignalR.Monitor.Storage.Tables
{
    public class PodTable : BaseMonitorTable<TableEntity>
    {
        public PodTable(CloudTableClient storageTableClient, string tableName = "pods") :
            base(tableName, storageTableClient)
        { }
    }
}
