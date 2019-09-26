using AzSignalR.Monitor.Storage.Entities;
using Microsoft.WindowsAzure.Storage.Table;

namespace AzSignalR.Monitor.Storage.Tables
{
    /// <summary>
    /// This is a table used to retrieve SignalR instance information in different regions.
    /// It's intended to be instantiated manually and should not used by IoC container.
    /// </summary>
    public class SignalRInstanceTable : BaseMonitorTable<TableEntity>
    {
        public SignalRInstanceTable(string tableName, CloudTableClient storageTableClient) : base(tableName, storageTableClient) { }
    }
}
