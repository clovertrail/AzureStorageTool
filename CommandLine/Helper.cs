using AzSignalR.Monitor.Storage;
using AzSignalR.Monitor.Storage.Entities;
using AzSignalR.Monitor.Storage.Tables;
using AzSignalR.Monitor.ViewModel;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace AzureStorageTable.CommandLine
{
    public class Helper
    {
        public static async Task LoadTestForQuery(ServiceTable serviceTable, VersionTable versionTable)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var version = await versionTable.GetCurrent(ResourceType.Service);
            if (version != null)
            {
                stopWatch.Stop();
                Console.WriteLine($"verion: {version.RowKey}, takes {stopWatch.ElapsedMilliseconds} milli-seconds");
            }
            stopWatch.Start();
            var services = await serviceTable.GetFromParitionKey(version.RowKey);
            var models = new List<ServiceViewModel>();
            foreach (var entity in services)
            {
                try
                {
                    models.Add(ServiceViewModel.FromEntity((ServiceEntity)entity));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            stopWatch.Stop();
            Console.WriteLine($"query {models.Count} takes {stopWatch.ElapsedMilliseconds} milli-seconds");
        }

        public static BaseMonitorTable<TableEntity> GenMonitorTable(string table, CloudTableClient cloudTableClient)
        {
            switch (table)
            {
                case "deployments":
                    return new DeploymentTable(cloudTableClient);
                case "jobstatus":
                    return new JobStatusTable(cloudTableClient);
                case "meta":
                    return new MetadataTable(cloudTableClient);
                case "nameindexes":
                    return new NameIndexTable(cloudTableClient);
                case "names":
                    return new NameTable(cloudTableClient);
                case "pods":
                    return new PodTable(cloudTableClient);
                case "respack":
                    return new ResourcePackTable(cloudTableClient);

                case "services":
                    return new ServiceTable(cloudTableClient);

                case "versions":
                    return new VersionTable(cloudTableClient);
                case "virtualmachines":
                    return new VirtualMachineTable(cloudTableClient);
            }
            return null;
        }
    }
}
