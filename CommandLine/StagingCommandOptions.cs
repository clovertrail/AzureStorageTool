using AzSignalR.Monitor.Storage;
using AzSignalR.Monitor.Storage.Tables;
using AzureStorageTable.CommandLine;
using McMaster.Extensions.CommandLineUtils;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace AzureStorageTable
{
    [Command(Name = "get-staging-names", FullName = "get all names", Description = "Get all names")]
    internal class GetAllNamesCommandOptions : StagingCommandOptions
    {
        protected override async Task OnExecuteAsync(CommandLineApplication app)
        {
            ValidateParameters();
            var nameTable = new NameTable(AzureStorageAccount.GetStorageTable(ConnectionString));
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            var entities = await nameTable.GetPeroidAsync(DaysBeforeNow, DaysAfterNow, cts.Token);
            DumpTypedEntities(entities, ResourceType.SignalR);
            DumpTypedEntities(entities, ResourceType.Deployment);
        }
    }

    [Command(Name = "clear-staging-tbl", FullName = "clear staging storage table", Description = "Command options for staging environment")]
    internal class StagingCommandOptions : BaseOption
    {
        [Option("-c|--connectionString", Description = "Specify the storage account connection string")]
        public string ConnectionString { get; set; }

        protected void ValidateParameters()
        {
            if (string.IsNullOrEmpty(ConnectionString))
            {
                ReportError(new ArgumentException("Missing storage account's connectionString"));
            }
        }
    }

    [Command(Name = "staging-load-test", FullName = "load test for staging storage table", Description = "Command options for staging environment")]
    internal class StagingLoadTestCommandOptions : StagingCommandOptions
    {
        protected override async Task OnExecuteAsync(CommandLineApplication app)
        {
            ValidateParameters();
            var storageAccount = AzureStorageAccount.CreateStorageAccountFromConnectionString(ConnectionString);
            var serviceTable = new ServiceTable(AzureStorageAccount.GetStorageTable(ConnectionString));
            var versionTable = new VersionTable(AzureStorageAccount.GetStorageTable(ConnectionString));
            await Helper.LoadTestForQuery(serviceTable, versionTable);
        }
    }

    [Command(Name = "staging-query-tbl", FullName = "query for staging storage table", Description = "Perf check for batch query of staging environment")]
    internal class StagingQueryCommandOptions : StagingCommandOptions
    {
        protected override async Task OnExecuteAsync(CommandLineApplication app)
        {
            ValidateParameters();
            var monitorTable = Helper.GenMonitorTable(TableName, AzureStorageAccount.GetStorageTable(ConnectionString));
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var entries = await monitorTable.GetBasedTimestamp(DaysBeforeNow);
            stopWatch.Stop();
            Console.WriteLine($"Get {entries.Count} entries takes {stopWatch.ElapsedMilliseconds} milli-seconds");
        }
    }

    [Command(Name = "staging-query-resource", FullName = "query a SignalR resource for staging storage table", Description = "Query SignalR resource of staging environment")]
    internal class StagingResourceQueryCommandOptions : StagingCommandOptions
    {
        [Option("-s|--SubscriptionId", Description = "Specify the subscription ID of the SignalR resource")]
        public string SubscriptionId { get; set; }

        [Option("-r|--ResourceName", Description = "Specify the SignalR resource name")]
        public string ResourceName { get; set; }

        protected override async Task OnExecuteAsync(CommandLineApplication app)
        {
            ValidateParameters();
            if (string.IsNullOrEmpty(SubscriptionId) || string.IsNullOrEmpty(ResourceName))
            {
                ReportError(new ArgumentException("Missing subscriptionId or resource name"));
            }
            var signalrInstanceTbl = new SignalRInstanceTable(SignalRConstants.SignalRInstanceTableName, AzureStorageAccount.GetStorageTable(ConnectionString));
            await Helper.SearchEntity(signalrInstanceTbl, SubscriptionId, ResourceName);
        }
    }

    internal class StagingDeleteCommandOptions : StagingCommandOptions
    {
        protected override async Task OnExecuteAsync(CommandLineApplication app)
        {
            ValidateParameters();
            var storageAccount = AzureStorageAccount.CreateStorageAccountFromConnectionString(ConnectionString);
            var monitorTable = Helper.GenMonitorTable(TableName, AzureStorageAccount.GetStorageTable(ConnectionString));
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var deleted = await monitorTable.DeleteOldEntities(DaysBeforeNow);
            stopWatch.Stop();
            Console.WriteLine($"Delete {deleted} entries takes {stopWatch.ElapsedMilliseconds} milli-seconds");
        }
    }
}
