using log4net;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using System;
using System.Threading.Tasks;

namespace AzSignalR.Monitor.Storage
{
    public interface IStorageAccountProvider
    {
        Task<CloudStorageAccount> GetRPTableReader(string accountName);
    }

    public class StorageAccountProvider : IStorageAccountProvider
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(StorageAccountProvider));

        public AzureHelper AzureHelper { get; set; }

        string _keyVaultURI;

        public StorageAccountProvider(AzureHelper azureHelper, string keyVaultURI)
        {
            AzureHelper = azureHelper;
            _keyVaultURI = keyVaultURI;
        }

        public async Task<CloudStorageAccount> GetRPTableReader(string accountName)
        {
            var secretName = $"{accountName}-tableRead1D";
            string sasToken = null;
            try
            {
                sasToken = await AzureHelper.GetSecretValue(_keyVaultURI, secretName);
            }
            catch (Exception e)
            {
                Logger.Error($"Error: failed to fetch SAS token forl {secretName} {e}");
                throw e;
            }
            if (sasToken == null)
            {
                throw new ArgumentException($"Cannot fetch SAS token for {secretName}");
            }
            var credentials = new StorageCredentials(sasToken);
            return new CloudStorageAccount(credentials, accountName, null, true);
        }
    }
}
