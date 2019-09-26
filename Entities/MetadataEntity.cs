using Newtonsoft.Json;
using System;

namespace AzSignalR.Monitor.Storage.Entities
{
    public interface IMetadata
    {
        string Key { get; }
    }

    public class MetadataEntity : CompoundTableEntity
    {
        public MetadataEntity() : base() { }
        public MetadataEntity(string partitionKey, string rowKey) : base(partitionKey, rowKey) { }

        public string JsonData { get; set; }

        public void SetMetadata<T>(T metadata) where T : IMetadata
        {
            JsonData = JsonConvert.SerializeObject(metadata);
        }

        public T As<T>() where T : IMetadata
        {
            return JsonConvert.DeserializeObject<T>(JsonData);
        }
    }

    public class SignalRRefreshMetadata : IMetadata
    {
        public string Key { get; set; }
        public DateTime LatestTimestamp { get; set; }

        public static string JobName(string region, string tableName)
        {
            return $"Refresh-SignalR-{region}-{tableName}";
        }
    }

    public class MonitorRefreshMetadata : IMetadata
    {
        public const string KEY = nameof(MonitorRefreshMetadata);

        public string Key { get; } = KEY;

        public DateTime StartTime { get; set; }
        public string Version { get; set; }
        public DateTime VersionTime { get; set; }
    }
}
