using AzSignalR.Monitor.Storage;
using AzSignalR.Monitor.Storage.Tables;
using AzSignalR.Monitor.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace AzureStorageTable
{
    class Program
    {
        static void IterateAllEntities(List<ServiceViewModel> allModels)
        {
            int i = 0;
            foreach (var model in allModels)
            {
                Console.WriteLine($"{i} {model.Region} {model.ResourceName} {model.Connectivity.Detail}");
                i++;
            }
        }

        static async Task LoadTimespanForQueryService(string connectionString)
        {
            var table = "services";
            var storageAccount = AzureStorageAccount.CreateStorageAccountFromConnectionString(connectionString);
            var serviceTable = new ServiceTable(table, AzureStorageAccount.GetStorageTable(connectionString));
            
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var services = await serviceTable.GetBasedTimestamp(DateTimeOffset.Now.AddDays(-30).Date);
            var models = new List<ServiceViewModel>();
            foreach (var entity in services)
            {
                try
                {
                    models.Add(ServiceViewModel.FromEntity(entity));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            stopWatch.Stop();
            Console.WriteLine($"query {models.Count} takes {stopWatch.ElapsedMilliseconds} milli-seconds");
        }

        static async Task LoadTestForQueryService(string connectionString)
        {
            var table = "services";
            var storageAccount = AzureStorageAccount.CreateStorageAccountFromConnectionString(connectionString);
            var serviceTable = new ServiceTable(table, AzureStorageAccount.GetStorageTable(connectionString));
            var versionTable = new VersionTable(storageAccount, "versions");

            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var version = await versionTable.GetCurrent(ResourceType.Service);
            if (version != null)
            {
                stopWatch.Stop();
                Console.WriteLine($"verion: {version.RowKey}, takes {stopWatch.ElapsedMilliseconds} milli-seconds");
            }
            stopWatch.Start();
            var services = await serviceTable.GetFromVersion(version?.Version);
            var models = new List<ServiceViewModel>();
            foreach (var entity in services)
            {
                try
                {
                    models.Add(ServiceViewModel.FromEntity(entity));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            stopWatch.Stop();
            Console.WriteLine($"query {models.Count} takes {stopWatch.ElapsedMilliseconds} milli-seconds");
        }
        
        static async Task DeleteOldEntries(string connectionString)
        {
            var table = "services";
            var storageAccount = AzureStorageAccount.CreateStorageAccountFromConnectionString(connectionString);
            var serviceTable = new ServiceTable(table, AzureStorageAccount.GetStorageTable(connectionString));

            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var deleted = await serviceTable.DeleteOldEntities(30);
            stopWatch.Stop();
            Console.WriteLine($"Delete {deleted} entries takes {stopWatch.ElapsedMilliseconds} milli-seconds");
        }

        static async Task Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Input storage connection string");
                return;
            }
            //await LoadTestForQueryService(args[0]);
            //await LoadTimespanForQueryService(args[0]);
            await DeleteOldEntries(args[0]);
        }
    }
}
