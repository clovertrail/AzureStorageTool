﻿using AzSignalR.Monitor;
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

    [Command(Name = "prod-load-test", FullName = "load test for the production", Description = "Command options for production environment")]
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

    internal class ProdCommandOptions : BaseOption
    {
        [Option("-t|--tenantId", Description = "Specify the tenant ID. Default is '33e01921-4d64-4f8c-a055-5bdaffd5e33d'")]
        public string TenantId { get; set; } = "33e01921-4d64-4f8c-a055-5bdaffd5e33d";

        [Option("-k|--keyvault", Description = "Specify the keyvault name. Default is 'srprodkvmonitor'")]
        public string KeyVaultName { get; set; } = "srprodkvmonitor";
    }
}