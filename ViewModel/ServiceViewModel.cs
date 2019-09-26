using AzSignalR.Monitor.Storage.Entities;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace AzSignalR.Monitor.ViewModel
{
    public class ServiceViewModel
    {
        public Property PartitionKey { get; set; }
        public Property RowKey { get; set; }
        public Property Timestamp { get; set; }

        public Property Version { get; set; }
        public Property SubscriptionId { get; set; }
        public Property ResourceGroup { get; set; }
        public Property ResourceName { get; set; }
        public Property ResourceRowGuid { get; set; }
        public Property ResourceKubeId { get; set; }
        public Property NamespaceProperty { get; set; }
        public Property Region { get; set; }
        public Property Name { get; set; }
        public Property LoadBalancer { get; set; }
        public Property Labels { get; set; }
        public Property ExternalIP { get; set; }
        public Property ResourceVersion { get; set; }
        public Property ClusterName { get; set; }
        public Property DeletionTimestamp { get; set; }
        public Property DeletionGracePeriodSecond { get; set; }
        public Property CreationTimestamp { get; set; }
        public Property Annotations { get; set; }
        public Property SelfLink { get; set; }
        public Property Uid { get; set; }
        public Property ClusterIP { get; set; }
        public Property ExternalName { get; set; }
        public Property ExternalTrafficPolicy { get; set; }
        public Property LoadBalancerIP { get; set; }
        public Property IngressString { get; set; }
        public Property Etag { get; set; }

        public Property Connectivity { get; set; }

        public static ServiceViewModel FromEntity(ServiceEntity entity)
        {
            var viewModel = new ServiceViewModel()
            {
                SubscriptionId = Property.FromString(entity.SubscriptionId),
                Version = Property.FromString(entity.Version),
                PartitionKey = Property.FromString(entity.PartitionKey),
                RowKey = Property.FromString(entity.RowKey),
                Timestamp = Property.FromString(entity.ResourceVersionTime.ToString()),
                ResourceGroup = Property.FromString(entity.ResourceGroup),
                ResourceName = Property.FromString(entity.ResourceName),
                ResourceRowGuid = Property.FromString(entity.ResourceRowGuid),
                ResourceKubeId = Property.FromString(entity.ResourceKubeId),
                NamespaceProperty = Property.FromString(entity.NamespaceProperty),
                Region = Property.FromString(entity.Region),
                Name = Property.FromString(entity.Name),
                LoadBalancer = Property.FromString(entity.LoadBalancer),
                Labels = Property.FromObject(JsonConvert.DeserializeObject(entity.Labels)),
                ExternalIP = Property.FromString(entity.ExternalIP),
                ResourceVersion = Property.FromString(entity.ResourceVersion),
                ClusterName = Property.FromString(entity.ClusterName),
                DeletionTimestamp = Property.FromString(entity.DeletionTimestamp),
                DeletionGracePeriodSecond = Property.FromNumber(entity.DeletionGracePeriodSeconds),
                CreationTimestamp = Property.FromString(entity.CreationTimestamp),
                Annotations = Property.FromObject(JsonConvert.DeserializeObject(entity.Annotations)),
                SelfLink = Property.FromString(entity.SelfLink),
                Uid = Property.FromString(entity.Uid),
                ClusterIP = Property.FromString(entity.ClusterIP),
                ExternalName = Property.FromString(entity.ExternalName),
                ExternalTrafficPolicy = Property.FromString(entity.ExternalTrafficPolicy),
                LoadBalancerIP = Property.FromString(entity.LoadBalancerIP),
                IngressString = Property.FromString(entity.IngressString),
                Etag = Property.FromString(entity.ETag)
            };

            viewModel.Connectivity = Property.FromDictionary(entity.HealthItems, ServiceEntity.Connectivity);
            return viewModel;
        }

    }
}
