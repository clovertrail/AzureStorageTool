using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzSignalR.Monitor.Storage.Tables
{
    /// <summary>
    /// A copy of https://github.com/Azure/azure-storage-net/blob/master/Lib/Common/Table/Protocol/TableConstants.cs
    /// </summary>
    public static class TableConstants
    {
        /// <summary>
        /// Stores the maximum number of operations allowed in a TableBatchOperation.
        /// </summary>
        public const int TableServiceBatchMaximumOperations = 100;

        /// <summary>
        /// Stores the header prefix for continuation information.
        /// </summary>
        public const string TableServicePrefixForTableContinuation = "x-ms-continuation-";

        /// <summary>
        /// Stores the header suffix for the next partition key.
        /// </summary>
        public const string TableServiceNextPartitionKey = "NextPartitionKey";

        /// <summary>
        /// Stores the header suffix for the next row key.
        /// </summary>
        public const string TableServiceNextRowKey = "NextRowKey";

        /// <summary>
        /// Stores the table suffix for the next table name.
        /// </summary>
        public const string TableServiceNextTableName = "NextTableName";

        /// <summary>
        /// Stores the maximum results the Table service can return.
        /// </summary>
        public const int TableServiceMaxResults = 1000;

        /// <summary>
        /// The maximum size of a string property for the Table service in bytes.
        /// </summary>
        public const int TableServiceMaxStringPropertySizeInBytes = 64 * 1024;

        /// <summary>
        /// The maximum size of a string property for the Table service in bytes.
        /// </summary>
        public const long TableServiceMaxPayload = 20 * 1024 * 1024;

        /// <summary>
        /// The maximum size of a string property for the Table service in chars.
        /// </summary>
        public const int TableServiceMaxStringPropertySizeInChars = TableServiceMaxStringPropertySizeInBytes / 2;

        /// <summary>
        /// The name of the special table used to store tables.
        /// </summary>
        public const string TableServiceTablesName = "Tables";

        /// <summary>
        /// The name of the partition key property.
        /// </summary>
        public const string PartitionKey = "PartitionKey";

        /// <summary>
        /// The name of the row key property.
        /// </summary>
        public const string RowKey = "RowKey";

        /// <summary>
        /// The name of the Timestamp property.
        /// </summary>
        public const string Timestamp = "Timestamp";

        /// <summary>
        /// The name of the ETag property.
        /// </summary>
        public const string Etag = "ETag";

        /// <summary>
        /// The name of the property that stores the table name.
        /// </summary>
        public const string TableName = "TableName";

        /// <summary>
        /// The query filter clause name.
        /// </summary>
        public const string Filter = "$filter";

        /// <summary>
        /// The query top clause name.
        /// </summary>
        public const string Top = "$top";

        /// <summary>
        /// The query select clause name.
        /// </summary>
        public const string Select = "$select";

        /// <summary>
        /// The minimum DateTime supported.
        /// </summary>
        public static readonly DateTimeOffset MinDateTime = new DateTimeOffset(1601, 1, 1, 0, 0, 0, TimeSpan.Zero);
    }
}
