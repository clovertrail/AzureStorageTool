using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;

namespace AzSignalR.Monitor.Storage.Entities
{
    public class VirtualMachineEntity : CompoundTableEntity, IPackableEntity
    {
        public const string MasterConnectivity = "masterConnectivity";
        public const string AgentConnectivity = "agentConnectivity";
        public const string Cpu = "cpu";
        public const string Memory = "memory";
        public const string Disk = "disk";
        public const string Mount = "mount";
        public const string Docker = "docker";
        public const string Kubelet = "kubelet";
        public const string KubeApiserver = "kubeApiserver";
        public const string KubeProxy = "kubeProxy";
        public const string Azsecd = "azsecd";
        public const string Etcd = "etcd";
        public const string ResultState = "processState";

        public VirtualMachineEntity() { }
        public VirtualMachineEntity(string partitionKey, string rowKey) : base(partitionKey, rowKey) { }

        public string Region { get; set; }
        public string ResourceGroup { get; set; }
        public string Name { get; set; }
        public string ComputerName { get; set; }
        public string MasterIP { get; set; }
        public string PowerState { get; set; }
        public string ProvisioningState { get; set; }
        public string OSType { get; set; }
        public string Tags { get; set; }
        public string VMId { get; set; }
        public int OSDiskSize { get; set; }
        public string AvailabilitySetId { get; set; }
        public string OSDiskID { get; set; }
        public string LicenseType { get; set; }

        public string Version { get; set; }
        public string SubscriptionId { get; set; }

        [ConvertableEntityProperty]
        public IDictionary<string, Property> HealthItems { get; set; } = new Dictionary<string, Property>();

        #region IPackableEntity
        [IgnoreProperty]
        ResourceType IPackableEntity.Type => ResourceType.VM;
        [IgnoreProperty]
        string IPackableEntity.ResourceIdentifier => PackableEntityUtil.ResourceIdentifierForACS(ComputerName?.ToLower(), ResourceGroup?.ToLower());
        [IgnoreProperty]
        string IPackableEntity.Name => ComputerName?.ToLower();
        [IgnoreProperty]
        List<string> IPackableEntity.AlternaltiveNames => (!string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(ComputerName) && Name != ComputerName) ? new List<string> { Name } : null;
        [IgnoreProperty]
        string IPackableEntity.NameRegion => Region;
        public DateTime CreatedTime { get; set; }
        public DateTime ResourceVersionTime { get; set; }
        DateTime IPackableEntity.CheckTime => ResourceVersionTime;
        [IgnoreProperty]
        bool IPackableEntity.IsDeleted => false;
        #endregion
        /*
        public VirtualMachineEntity LoadFrom(IVirtualMachineAdaptor vm)
        {
            Region = RegionUtil.Normalize(vm.RegionName);
            Name = vm.Name;
            ComputerName = vm.ComputerName;
            Tags = JsonConvert.SerializeObject(vm.Tags);
            VMId = vm.VMId;
            OSDiskID = vm.OSDiskId;
            LicenseType = vm.LicenseType;
            PowerState = vm.PowerState?.ToString();
            OSType = vm.OSType.ToString();
            OSDiskSize = vm.OSDiskSize;
            ProvisioningState = vm.ProvisioningState;
            AvailabilitySetId = vm.AvailabilitySetId;

            return this;
        }

        public static VirtualMachineEntity FromVirtualMachine(IVirtualMachineAdaptor vm, string resourceGroup, string version, DateTime versionTime)
        {
            if (Utils.InversedTimeKey(versionTime) != version)
            {
                throw new ArgumentException("version and versionTime do not match");
            }
            var entity = new VirtualMachineEntity(version, $"{resourceGroup} | {vm.Name}");
            entity.LoadFrom(vm);
            entity.ResourceGroup = resourceGroup;
            entity.Version = version;
            entity.ResourceVersionTime = versionTime;
            entity.CreatedTime = versionTime;
            return entity;
        }
        */
    }
}
