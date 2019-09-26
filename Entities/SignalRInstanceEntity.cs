using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace AzSignalR.Monitor.Storage.Entities
{
    public class SignalRInstanceEntity : CompoundTableEntity, IPackableEntity
    {

        public SignalRInstanceEntity() { }
        public SignalRInstanceEntity(string partitionKey, string rowKey) : base(partitionKey, rowKey) { }

        public static string GenerateRowKey(string resourceGroup, string resourceName, string rowGuid)
        {
            var rk = $"{resourceGroup}-{resourceName}-{rowGuid}";
            return rk.ToLower();
        }

        /// <summary>
        /// Standardized Azure resource Id.
        /// </summary>
        [IgnoreProperty]
        public string ResourceId
        {
            get
            {
                return $"/subscriptions/{UserSubscriptionId}/resourceGroups/{ResourceGroup}/providers/Microsoft.SignalRService/SignalR/{ResourceName}";
            }
        }

        public string ResourceName { get; set; }

        /// <summary>
        /// Try using PartitionKey as much as possible which is the lowercase of UserSubscriptionId
        /// </summary>
        public string UserSubscriptionId { get; set; }

        [IgnoreProperty]
        public string UserSubscriptionIdLowercase
        {
            get
            {
                return UserSubscriptionId?.ToLower();
            }
        }
        public string ResourceType { get; set; }
        public string ResourceGroup { get; set; }
        public string Location { get; set; }

        public int Status { get; set; }
        public bool IsSuspended { get; set; }
        public bool IsStopped { get; set; }
        /// <summary>
        /// Count of billing units
        ///
        /// Note that there is a factor between this value and actual kubernetes pods count
        /// according to the SKU user chooses
        /// </summary>
        public int SignalRInstances { get; set; }
        public string SignalRExternalIP { get; set; }
        public string SignalRLeafDomainLabel { get; set; }
        public string SignalRFQDN { get; set; }
        public string ACSInstanceRowKey { get; set; }
        public string ExternalAgentPoolName { get; set; }
        [BackwardCompatibleEntityProperty("ExternalAvailabilitySetName")]
        public string ExternalVMSetName { get; set; }
        public string RedisCacheRowKey { get; set; }
        public string AccessKeysId { get; set; }
        public string CustomDomainsId { get; set; }
        public string CustomCertificatesId { get; set; }
        /// <summary>
        /// Current asynchronous operation associated with this resource
        /// </summary>
        public string AsyncOperationId { get; set; }
        /// <summary>
        /// The docker image for this SignalR resource
        /// </summary>
        public string ContainerImage { get; set; }
        public DateTime CreatedTime { get; set; }

        [ConvertableEntityProperty]
        public Dictionary<string, string> Tags { get; set; }

        public string SkuName { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public SignalRArchitecture Arch { get; set; }

        /// <summary>
        /// An ID used by service runtime to identify itself
        ///
        /// This should never change once a resource is created
        /// </summary>
        [BackwardCompatibleEntityProperty("KubeId")]
        public string ServiceId { get; set; }

        #region Kubernetes deployment metadata
        /// <summary>
        /// An ID to locate corresponding Kubenetes resources
        ///
        /// This may change during resource relocation/re-creation
        /// </summary>
        public string KubeId { get; set; }

        public string KubeNamespace { get; set; }
        public string KubeDeploymentName { get; set; }
        public string KubeServiceName { get; set; }
        public string KubeIngressName { get; set; }
        public string KubeIngressControllerName { get; set; }
        public int PublicPort { get; set; }
        public int ServerPort { get; set; }
        #endregion

        #region IPackableEntity
        [IgnoreProperty]
        ResourceType IPackableEntity.Type => Storage.ResourceType.SignalR;
        [IgnoreProperty]
        string IPackableEntity.ResourceIdentifier => RowKey;
        [IgnoreProperty]
        string IPackableEntity.Name => ResourceName;
        [IgnoreProperty]
        List<string> IPackableEntity.AlternaltiveNames => null;
        [IgnoreProperty]
        string IPackableEntity.NameRegion => null;
        [IgnoreProperty]
        DateTime IPackableEntity.ResourceVersionTime => Timestamp.DateTime.ToUniversalTime();
        public DateTime CheckTime { get; set; }
        [IgnoreProperty]
        bool IPackableEntity.IsDeleted => Status == SignalRResourceStatus.Deleted || Status == SignalRResourceStatus.Moved;
        #endregion
    }

    public class SignalRResourceStatus
    {
        public const int Active = 0;

        public const int Creating = 10;
        public const int CreateFailed = 11;
        public const int CreateDeploymentInACS = 12;
        public const int CreateNetworkConfiguration = 13;

        public const int Updating = 20;
        public const int UpdateFailed = 21;
        public const int UpdateFailedRetryable = 22;

        public const int Deleting = 30;
        public const int DeleteFailed = 31;
        public const int Deleted = 32;

        public const int Moving = 60;
        public const int MoveFailed = 61;
        public const int Moved = 62;
    }

    public enum SignalRArchitecture
    {
        Layer4,
        NginxIngress,
    }
}
