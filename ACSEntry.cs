namespace AzSignalR.Monitor.Storage
{
    public class ACSEntry
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public string ResourceGroup { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return $"{ResourceGroup}/{Name}";
        }
    }
}
