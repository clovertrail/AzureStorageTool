using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzSignalR.Monitor.Storage.Entities
{
    /// <summary>
    /// An attribute to help us rename a property while keeps backward compatibility to existing data
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public class BackwardCompatibleEntityPropertyAttribute : Attribute
    {
        public string CompatibleName { get; private set; }

        public BackwardCompatibleEntityPropertyAttribute(string compatibleName)
        {
            CompatibleName = compatibleName;
        }
    }
}
