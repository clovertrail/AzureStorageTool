using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace AzSignalR.Monitor.Storage.Entities
{
    public class VersionEntity : CompoundTableEntity
    {
        public VersionEntity() { }

        /// <summary>
        ///
        /// </summary>
        /// <param name="partitionKey">Resource type</param>
        /// <param name="rowKey">Inversed time key as the version.</param>
        public VersionEntity(ResourceType resourceType, string rowKey) : base(resourceType.ToString(), rowKey)
        {
            ResourceType = resourceType;
        }

        [IgnoreProperty]
        public string Version => RowKey;

        [ConvertableEntityProperty]
        [JsonConverter(typeof(StringEnumConverter))]
        public ResourceType ResourceType { get; set; }

        /// <summary>
        /// The time when we start the check for this version.
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// The time when we complete the check fot this version.
        /// </summary>
        public DateTime EndTime { get; set; }
    }
}
