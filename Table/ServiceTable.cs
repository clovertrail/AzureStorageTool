using AzSignalR.Monitor.Storage.Entities;
using Microsoft.WindowsAzure.Storage.Table;

namespace AzSignalR.Monitor.Storage.Tables
{
    public class ServiceTable : BaseMonitorTable<ServiceEntity>
    {
        public ServiceTable(string tableName, CloudTableClient storageTableClient) :
            base(tableName, storageTableClient)
        { }
    }
}
