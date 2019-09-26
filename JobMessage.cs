using AzSignalR.Monitor.Storage;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace AzureStorageTable
{
    public class JobMessage
    {
        public string Version { get; set; }
        public DateTime VersionTime { get; set; }
        public string Region { get; set; }
        public string SubscriptionID { get; set; }

        #region ACS Jobs (VM, Service, Deployment, Pod, etc)
        public string KubeconfigName { get; set; }
        public ACSEntry ACSEntry { get; set; }
        #endregion

        #region Regional Jobs (SignalR)
        public string StorageAccount { get; set; }
        public string TableName { get; set; }
        #endregion

        [JsonConverter(typeof(StringEnumConverter))]
        public ResourceType Type { get; set; }

        [JsonIgnore]
        public string Key
        {
            get
            {
                if (Type.IsPerACS())
                {
                    return $"{Type}-{ACSEntry?.RowKey}";
                }
                else if (Type.IsPerRegion())
                {
                    return $"{Type}-{Region}";
                }
                else
                {
                    return $"{Type}-{SubscriptionID}";
                }
            }
        }

        [JsonIgnore]
        public string LockId
        {
            get
            {
                if (Type.IsPerACS())
                {
                    return $"{Type}-{ACSEntry?.Name}";
                }
                else if (Type.IsPerRegion())
                {
                    return $"{Type}-{Region}";
                }
                else
                {
                    return $"{Type}-{SubscriptionID}";
                }
            }
        }

        public override string ToString()
        {
            return $"{VersionTime}/{Region}/{ACSEntry?.Name}/{Type}/{Version}";
        }
    }
}
