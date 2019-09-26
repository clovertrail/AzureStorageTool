using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzSignalR.Monitor.Storage.Entities
{
    /// <summary>
    /// An Attribute to enable us write complex objects into Azure table
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ConvertableEntityPropertyAttribute : Attribute
    {
        public Type ConvertToType;

        public ConvertableEntityPropertyAttribute()
        {

        }
        public ConvertableEntityPropertyAttribute(Type convertToType)
        {
            ConvertToType = convertToType;
        }

        public virtual string Serialize(object value)
        {
            if (value != null && value.GetType().IsEnum)
            {
                return value.ToString();
            }
            return JsonConvert.SerializeObject(value);
        }

        public virtual object Deserialize(string value, Type resultType)
        {
            if (resultType.IsEnum)
            {
                return Enum.Parse(resultType, value);
            }
            return JsonConvert.DeserializeObject(value, resultType);
        }
    }
}
