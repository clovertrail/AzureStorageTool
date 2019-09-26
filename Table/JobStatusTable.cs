using AzSignalR.Monitor.Storage.Entities;
using Microsoft.WindowsAzure.Storage.Table;

namespace AzSignalR.Monitor.Storage.Tables
{
    public class JobStatusTable : BaseMonitorTable<TableEntity>
    {
        public JobStatusTable(CloudTableClient storageTableClient, string tableName = "jobstatus") : base(tableName, storageTableClient) { }
    }
}
