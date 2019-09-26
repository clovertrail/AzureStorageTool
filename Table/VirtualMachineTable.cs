using AzSignalR.Monitor.Storage.Entities;
using Microsoft.WindowsAzure.Storage.Table;

namespace AzSignalR.Monitor.Storage.Tables
{
    public class VirtualMachineTable : BaseMonitorTable<TableEntity>
    {
        public VirtualMachineTable(CloudTableClient storageTableClient, string tableName = "virtualmachines") :
            base(tableName, storageTableClient)
        { }
    }
}
