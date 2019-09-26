using AzSignalR.Monitor.Storage;
using AzSignalR.Monitor.Storage.Entities;
using AzSignalR.Monitor.Storage.Tables;
using AzSignalR.Monitor.ViewModel;
using McMaster.Extensions.CommandLineUtils;
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

        static async Task Main(string[] args)
        {
            await CommandLineApplication.ExecuteAsync<CommandLineOptions>(args);
        }
    }
}
