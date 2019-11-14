using AzSignalR.Monitor.Storage;
using AzSignalR.Monitor.Storage.Entities;
using McMaster.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace AzureStorageTable
{
    [HelpOption("--help")]
    [Subcommand(
        typeof(ProdDeleteCommandOptions),
        typeof(ProdLoadTestCommandOptions),
        typeof(ProdQueryCommandOptions),
        typeof(SearchResourceCommandOptions),
        typeof(StagingDeleteCommandOptions),
        typeof(StagingQueryCommandOptions),
        typeof(StagingResourceQueryCommandOptions),
        typeof(GetAllNamesCommandOptions),
        typeof(GetProdAllNamesCommandOptions))]
    internal class CommandLineOptions : BaseOption
    {
        public string GetVersion()
                => typeof(CommandLineOptions).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;

        protected override Task OnExecuteAsync(CommandLineApplication app)
        {
            app.ShowHelp();
            return Task.CompletedTask;
        }
    }

    [HelpOption("--help")]
    internal abstract class BaseOption
    {
        protected virtual Task OnExecuteAsync(CommandLineApplication app)
        {
            return Task.CompletedTask;
        }

        protected static void ReportError(Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine($"Unexpected error: {ex}");
            Console.ResetColor();
        }

        [Option("-b|--onOrBefore", Description = "Specify from how many days before (@today - value). Default value is 30 (days)")]
        public int DaysBeforeNow { get; set; } = 30;

        [Option("-a|--onOrAfter", Description = "Specify from how many days after (@today - value). Default value is 40 (days)")]
        public int DaysAfterNow { get; set; } = 40;

        [Option("-n|--tableName", Description = "Specify the table name: 'deployments', 'jobstatus', 'meta', 'nameindexes', 'names', 'pods', 'respack', 'services', 'versions', 'virtualmachines', Default is 'services'")]
        public string TableName { get; set; } = "services";

        // Utilities
        protected void DumpTypedEntities(IEnumerable<NameEntity> entities, ResourceType rt)
        {
            var selectedEntities = entities.Where(t => t.Type == rt);
            DumpEntities(rt.ToString(), selectedEntities);
        }

        protected void DumpEntities(string entityType, IEnumerable<NameEntity> entities)
        {
            Console.WriteLine($"======{entityType}");
            foreach (var entity in entities)
            {
                Console.WriteLine($"{entity.EffectiveTime} {entity.NameRegion} {entity.RowKey}");
            }
        }
    }
}
