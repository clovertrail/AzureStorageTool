using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace AzSignalR.Monitor.Storage.Entities
{
    public class DeploymentEntity : TableEntity, IPackableEntity
    {
        public DeploymentEntity() { }
        public DeploymentEntity(string partitionKey, string rowKey) : base(partitionKey, rowKey) { }

        public string Region { get; set; }
        public string ResourceGroup { get; set; }
        // from Metadata
        public string Name { get; set; }
        public string NamespaceProperty { get; set; }
        public string ClusterName { get; set; }
        public string ResourceVersion { get; set; }
        public string Labels { get; set; } // from IDictionary<string, string>
        public string DeletionTimestamp { get; set; }
        public long DeletionGracePeriodSeconds { get; set; }
        public string CreationTimestamp { get; set; }
        public string Annotations { get; set; } // from IDictionary<string, string>
        public string SelfLink { get; set; }
        public string Uid { get; set; }

        // from Spec
        public int MinReadySeconds { get; set; }
        public bool Paused { get; set; }
        public int ProgressDeadlineSeconds { get; set; }
        public int SpecReplicas { get; set; }
        public int RevisionHistoryLimit { get; set; }

        // from Status
        public int AvailableReplicas { get; set; }
        public int CollisionCount { get; set; }
        public long ObservedGeneration { get; set; }
        public int ReadyReplicas { get; set; }
        public int StatusReplicas { get; set; }
        public int UnavailableReplicas { get; set; }
        public int UpdatedReplicas { get; set; }

        public string Version { get; set; }
        public string SubscriptionId { get; set; }

        #region IPackableEntity
        [IgnoreProperty]
        ResourceType IPackableEntity.Type => ResourceType.Deployment;
        [IgnoreProperty]
        string IPackableEntity.ResourceIdentifier => PackableEntityUtil.ResourceIdentifierForACS(Name, ResourceGroup);
        [IgnoreProperty]
        string IPackableEntity.Name => Name;
        [IgnoreProperty]
        List<string> IPackableEntity.AlternaltiveNames => null;
        [IgnoreProperty]
        string IPackableEntity.NameRegion => Region;
        public DateTime CreatedTime { get; set; }
        public DateTime ResourceVersionTime { get; set; }
        [IgnoreProperty]
        DateTime IPackableEntity.CheckTime => ResourceVersionTime;
        [IgnoreProperty]
        bool IPackableEntity.IsDeleted => false;
        #endregion
        /*
        public static DeploymentEntity FromAppsV1Beta1Deployment(Appsv1beta1Deployment item, string resourceGroup, string version, DateTime versionTime)
        {
            if (Utils.InversedTimeKey(versionTime) != version)
            {
                throw new ArgumentException("version and versionTime do not match");
            }
            var deployment = new DeploymentEntity(version, $"{resourceGroup} | {item.Metadata.Name}")
            {
                Name = item.Metadata.Name,
                NamespaceProperty = item.Metadata.NamespaceProperty,
                ClusterName = resourceGroup,
                ResourceGroup = resourceGroup,
                ResourceVersion = item.Metadata.ResourceVersion,
                Labels = JsonConvert.SerializeObject(item.Metadata.Labels),
                DeletionTimestamp = item.Metadata.DeletionTimestamp?.ToString(),
                DeletionGracePeriodSeconds = item.Metadata.DeletionGracePeriodSeconds ?? 0,
                CreationTimestamp = item.Metadata.CreationTimestamp?.ToString(),
                Annotations = JsonConvert.SerializeObject(item.Metadata.Annotations),
                SelfLink = item.Metadata.SelfLink,
                Uid = item.Metadata.Uid,
                MinReadySeconds = item.Spec.MinReadySeconds ?? 0,
                Paused = item.Spec.Paused ?? false,
                ProgressDeadlineSeconds = item.Spec.ProgressDeadlineSeconds ?? 0,
                SpecReplicas = item.Spec.Replicas ?? 0,
                RevisionHistoryLimit = item.Spec.RevisionHistoryLimit ?? 0,
                AvailableReplicas = item.Status.AvailableReplicas ?? 0,
                CollisionCount = item.Status.CollisionCount ?? 0,
                ObservedGeneration = item.Status.ObservedGeneration ?? 0,
                ReadyReplicas = item.Status.ReadyReplicas ?? 0,
                StatusReplicas = item.Status.Replicas ?? 0,
                UnavailableReplicas = item.Status.UnavailableReplicas ?? 0,
                UpdatedReplicas = item.Status.UpdatedReplicas ?? 0,
                Version = version,
                ResourceVersionTime = versionTime,
                CreatedTime = item.Metadata.CreationTimestamp ?? versionTime,
            };
            return deployment;
        }
        */
    }
}
