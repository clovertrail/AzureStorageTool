using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace AzSignalR.Monitor.Storage.Entities
{
    public class JobStatusEntity : CompoundTableEntity
    {
        public JobStatusEntity() : base() { }
        public JobStatusEntity(string partitionKey, string rowKey) : base(partitionKey, rowKey) { }

        [IgnoreProperty]
        public string Version => PartitionKey;

        [IgnoreProperty]
        public string JobKey => RowKey;

        public DateTime StartTime { get; set; }
        public string Region { get; set; }
        public string SubscriptionID { get; set; }
        public string KubeconfigName { get; set; }

        [ConvertableEntityProperty]
        public ACSEntry ACSEntry { get; set; }

        [ConvertableEntityProperty]
        public ResourceType JobType { get; set; }

        [ConvertableEntityProperty]
        public JobStatus Status { get; set; }
    }

    public enum JobStatus
    {
        Unknown,
        Running,
        Completed,
    }
}
