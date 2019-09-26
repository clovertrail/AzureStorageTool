using AzSignalR.Monitor.ViewModel;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzSignalR.Monitor.Storage.Entities
{
    public class RegionConfigEntity : CompoundTableEntity
    {
        public RegionConfigEntity() : base() { }
        /// <summary>
        ///
        /// </summary>
        /// <param name="partitionKey">Lower cased region name without space. This is the same with the ACS instance partition key.</param>
        /// <param name="rowKey">The regional subscription ID.</param>
        public RegionConfigEntity(string partitionKey, string rowKey) : base(partitionKey, rowKey) { }

        [IgnoreProperty]
        public string Region => PartitionKey;

        [IgnoreProperty]
        public string SubscriptionID => RowKey;

        /// <summary>
        /// The storage account in which RP stores the ACS instance data.
        /// </summary>
        public string StorageAccount { get; set; }

        public string ACSTableName { get; set; }

        public string SignalRInstanceTableName { get; set; } = "azsresource";

        [ConvertableEntityProperty]
        public List<ACSEntry> AcsEntries { get; set; } = new List<ACSEntry>();
    }
}
