using Newtonsoft.Json;

namespace AzSignalR.Monitor.Storage.Entities
{
    /// <summary>
    /// An entity with a payload of other resource entities, such as VirtualMachineEntity,
    /// with the NameIdentifier as the partition key and version as the row key.
    ///
    /// This kind of replication allows us to query the target resource in a given time range quickly.
    /// </summary>
    public class ResourcePackEntity : CompoundTableEntity
    {
        public ResourcePackEntity() : base() { }
        public ResourcePackEntity(string partitionKey, string rowKey) : base(partitionKey, rowKey) { }

        /// <summary>
        /// The type of the resource.
        ///
        /// Generally, this should be reflected in the NameIdentifier.
        /// </summary>
        [ConvertableEntityProperty]
        public ResourceType Type { get; set; }

        /// <summary>
        /// JSON serialized entity data.
        /// </summary>
        public string JsonPayload { get; set; }

        public T To<T>() where T : IPackableEntity
        {
            return JsonConvert.DeserializeObject<T>(JsonPayload);
        }
    }
}
