using AzSignalR.Monitor.Extensions;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace AzSignalR.Monitor.Storage.Entities
{
    public class PodEntity : CompoundTableEntity, IPackableEntity
    {
        public const string Logs = "logs";
        public const string NotEvicted = "notEvicted";
        public const string Restart = "restart";
        public const string Condition = "condition";
        public const string GenevaMetrics = "genevaMetrics";

        public static class Delta
        {
            public const string Restart = "RestartDelta";
        }

        public PodEntity(string partitionKey, string rowKey) : base(partitionKey, rowKey) { }
        public PodEntity() { }

        public string Region { get; set; }
        public string ResourceGroup { get; set; }
        // from Metadata
        public string Name { get; set; }
        public string NamespaceProperty { get; set; }
        public string ResourceVersion { get; set; }
        public string Labels { get; set; } // from IDictionary<string, string>
        public string DeletionTimestamp { get; set; }
        public long DeletionGracePeriodSeconds { get; set; }
        public string CreationTimestamp { get; set; }
        public string Annotations { get; set; } // from IDictionary<string, string>
        public string SelfLink { get; set; }
        public string Uid { get; set; }
        public string ClusterName { get; set; }

        // from Spec
        public string Subdomain { get; set; }
        public string ServiceAccount { get; set; }
        public string ServiceAccountName { get; set; }
        public string SchedulerName { get; set; }
        public string RestartPolicy { get; set; }
        public string PriorityClassName { get; set; }
        public string NodeSelector { get; set; } // from IDictionary<string, string>
        public string NodeName { get; set; }
        public string Hostname { get; set; }
        public string DnsPolicy { get; set; }
        /*
        // from Status
        [IgnoreProperty]
        public IList<V1PodCondition> Conditions { get; set; }
        [IgnoreProperty]
        public IList<V1ContainerStatus> ContainerStatuses { get; set; }
        */
        public string HostIP { get; set; }
        public string Message { get; set; }
        public string NominatedNodeName { get; set; }
        public string Phase { get; set; }
        public string PodIP { get; set; }
        public string QosClass { get; set; }
        public string Reason { get; set; }
        public string StartTime { get; set; }


        [ConvertableEntityProperty]
        public IDictionary<string, Property> HealthItems { get; set; } = new Dictionary<string, Property>();

        // Utils
        public string Version { get; set; }
        public string SubscriptionId { get; set; }

        public void RecordDelta(PodEntity prev)
        {
            var restart = HealthItems.Try<int>(Restart);
            if (restart == 0)
            {
                return;
            }
            var prevRestart = prev?.HealthItems.Try<int>(Restart) ?? 0;
            var delta = restart - prevRestart;
            if (delta > 0)
            {
                HealthItems.Add(Delta.Restart, new Property(PropertyType.Number, delta, "Restart Delta", PropertySeverity.Info));
            }
        }

        #region IPackableEntity
        [IgnoreProperty]
        ResourceType IPackableEntity.Type => ResourceType.Pod;
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
        // methods
        public static PodEntity FromV1Pod(V1Pod item, string resourceGroup, string version, DateTime versionTime)
        {
            if (Utils.InversedTimeKey(versionTime) != version)
            {
                throw new ArgumentException("version and versionTime do not match");
            }
            var pod = new PodEntity(version, BuildRowKey(resourceGroup, item.Metadata.Name))
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
                Subdomain = item.Spec.Subdomain,
                ServiceAccount = item.Spec.ServiceAccount,
                ServiceAccountName = item.Spec.ServiceAccountName,
                SchedulerName = item.Spec.SchedulerName,
                RestartPolicy = item.Spec.RestartPolicy,
                PriorityClassName = item.Spec.PriorityClassName,
                NodeSelector = JsonConvert.SerializeObject(item.Spec.NodeSelector),
                NodeName = item.Spec.NodeName,
                Hostname = item.Spec.Hostname,
                DnsPolicy = item.Spec.DnsPolicy,
                //Conditions = item.Status.Conditions,
                //ContainerStatuses = item.Status.ContainerStatuses,
                HostIP = item.Status.HostIP,
                Message = item.Status.Message,
                NominatedNodeName = item.Status.NominatedNodeName,
                Phase = item.Status.Phase,
                PodIP = item.Status.PodIP,
                QosClass = item.Status.QosClass,
                Reason = item.Status.Reason,
                StartTime = item.Status.StartTime?.ToString(),
                Version = version,
                ResourceVersionTime = versionTime,
                CreatedTime = item.Metadata.CreationTimestamp ?? versionTime,
            };
            return pod;
        }
        */
        public static string BuildRowKey(string cluster, string name)
        {
            return $"{cluster} | {name}";
        }

    }
}
