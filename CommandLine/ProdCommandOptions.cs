using AzSignalR.Monitor;
using AzSignalR.Monitor.Storage;
using AzSignalR.Monitor.Storage.Tables;
using AzureStorageTable.CommandLine;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace AzureStorageTable
{
    [Command(Name = "clear-prod-tbl", FullName = "clear production table", Description = "Command options for production environment")]
    internal class ProdDeleteCommandOptions : ProdCommandOptions
    {
        protected override async Task OnExecuteAsync(CommandLineApplication app)
        {
            var keyvaultAddress = $"https://{KeyVaultName}.vault.azure.net/";
            var azureHelper = new AzureHelper(TenantId);

            var storageAccount = CloudStorageAccount.Parse(
                azureHelper.GetSecretValue(
                    keyvaultAddress, "StorageAccountConnectionString").GetAwaiter().GetResult());
            var tableClient = storageAccount.CreateCloudTableClient();

            var monitorTable = Helper.GenMonitorTable(TableName, tableClient);
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var deleted = await monitorTable.DeleteOldEntities(DaysBeforeNow);
            stopWatch.Stop();
            Console.WriteLine($"Delete {deleted} entries takes {stopWatch.ElapsedMilliseconds} milli-seconds");
        }
    }

    [Command(Name = "prod-query-resource", FullName = "Query SignalR resource for the production", Description = "Query SignalR resource of production environment")]
    internal class SearchResourceCommandOptions : ProdCommandOptions
    {
        [Option("-s|--SubscriptionId", Description = "Specify the subscription ID of the SignalR resource")]
        public string SubscriptionId { get; set; }

        [Option("-r|--ResourceName", Description = "Specify the SignalR resource name")]
        public string ResourceName { get; set; }

        protected override async Task OnExecuteAsync(CommandLineApplication app)
        {
            if (string.IsNullOrEmpty(SubscriptionId) || string.IsNullOrEmpty(ResourceName))
            {
                ReportError(new ArgumentException("Missing subscriptionId or resource name"));
            }
            var keyvaultAddress = $"https://{KeyVaultName}.vault.azure.net/";
            var azureHelper = new AzureHelper(TenantId);
            var strAccountProvider = new StorageAccountProvider(azureHelper, keyvaultAddress);
            var storageAccount = await strAccountProvider.GetRPTableReader(SignalRConstants.SignalRInstanceStorageAccount);
            var signalrInstanceTbl = new SignalRInstanceTable(SignalRConstants.SignalRInstanceTableName, storageAccount.CreateCloudTableClient());
            await Helper.SearchEntity(signalrInstanceTbl, SubscriptionId, ResourceName);
        }
    }

    [Command(Name = "prod-load-test", FullName = "load test for the production", Description = "Load test for production environment")]
    internal class ProdLoadTestCommandOptions : ProdCommandOptions
    {
        protected override async Task OnExecuteAsync(CommandLineApplication app)
        {
            var keyvaultAddress = $"https://{KeyVaultName}.vault.azure.net/";
            var azureHelper = new AzureHelper(TenantId);

            var storageAccount = CloudStorageAccount.Parse(
                azureHelper.GetSecretValue(
                    keyvaultAddress, "StorageAccountConnectionString").GetAwaiter().GetResult());
            var tableClient = storageAccount.CreateCloudTableClient();

            var serviceTable = new ServiceTable(tableClient);
            var versionTable = new VersionTable(tableClient);
            await Helper.LoadTestForQuery(serviceTable, versionTable);
        }
    }

    [Command(Name = "prod-query-tbl", FullName = "query for production storage table", Description = "Perf test for querying in production environment")]
    internal class ProdQueryCommandOptions : ProdCommandOptions
    {
        protected override async Task OnExecuteAsync(CommandLineApplication app)
        {
            var keyvaultAddress = $"https://{KeyVaultName}.vault.azure.net/";
            var azureHelper = new AzureHelper(TenantId);

            var storageAccount = CloudStorageAccount.Parse(
                azureHelper.GetSecretValue(
                    keyvaultAddress, "StorageAccountConnectionString").GetAwaiter().GetResult());
            var tableClient = storageAccount.CreateCloudTableClient();
            var monitorTable = Helper.GenMonitorTable(TableName, tableClient);
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var entries = await monitorTable.GetBasedTimestamp(DaysBeforeNow);
            stopWatch.Stop();
            Console.WriteLine($"Get {entries.Count} entries takes {stopWatch.ElapsedMilliseconds} milli-seconds");
        }
    }

    internal class ProdCommandOptions : BaseOption
    {
        [Option("-t|--tenantId", Description = "Specify the tenant ID. Default is '33e01921-4d64-4f8c-a055-5bdaffd5e33d'")]
        public string TenantId { get; set; } = "33e01921-4d64-4f8c-a055-5bdaffd5e33d";

        [Option("-k|--keyvault", Description = "Specify the keyvault name. Default is 'srprodkvmonitor'")]
        public string KeyVaultName { get; set; } = "srprodkvmonitor";
    }
}
