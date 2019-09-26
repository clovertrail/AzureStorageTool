using AzSignalR.Monitor.Storage.Entities;
using Microsoft.WindowsAzure.Storage.Table;

namespace AzSignalR.Monitor.Storage.Tables
{
    public class DeploymentTable : BaseMonitorTable<TableEntity>
    {
        public DeploymentTable(CloudTableClient storageTableClient, string tableName="deployments") :
            base(tableName, storageTableClient)
        { }
    }
}
