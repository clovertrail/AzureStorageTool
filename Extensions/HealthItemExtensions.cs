using AzSignalR.Monitor.Storage.Entities;
using System;
using System.Collections.Generic;

namespace AzSignalR.Monitor.Extensions
{
    public static class HealthItemExtensions
    {
        public static T Try<T>(this IDictionary<string, Property> healthItems, string name)
        {
            if (healthItems.TryGetValue(name, out var value))
            {
                try
                {
                    // int after save/load may become long, which may fail type check.
                    // So just try to do the conversion directly and catch possible error.
                    return (T)Convert.ChangeType(value.Value, typeof(T));
                }
                catch
                {
                    // ignore
                }
            }
            return default(T);
        }
    }
}
