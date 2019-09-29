using McMaster.Extensions.CommandLineUtils;
using System;
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
        typeof(StagingResourceQueryCommandOptions))]
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

        [Option("-d|--daysBeforeNow", Description = "Specify from how many days before now. Default is 30 (days)")]
        public int DaysBeforeNow { get; set; } = 30;

        [Option("-n|--tableName", Description = "Specify the table name: 'deployments', 'jobstatus', 'meta', 'nameindexes', 'names', 'pods', 'respack', 'services', 'versions', 'virtualmachines', Default is 'services'")]
        public string TableName { get; set; } = "services";
    }
}
