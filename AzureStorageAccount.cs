using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;

namespace AzureStorageTable
{
    public class AzureStorageAccount
    {
        public static CloudStorageAccount CreateStorageAccountFromAccountKey(string account, string key)
        {
            var storageCredential = new StorageCredentials(account, key);
            var cloudStorageAccount = new CloudStorageAccount(storageCredential, true);
            return cloudStorageAccount;
        }

        public static CloudStorageAccount CreateStorageAccountFromConnectionString(string connectionString)
        {
            CloudStorageAccount.TryParse(connectionString, out CloudStorageAccount cloudStorageAccount);
            return cloudStorageAccount;
        }

        public static CloudTableClient GetStorageTable(string connectionString)
        {
            var storageAccount = CreateStorageAccountFromConnectionString(connectionString);
            var tableClient = storageAccount.CreateCloudTableClient();
            return tableClient;
        }
    }
}
