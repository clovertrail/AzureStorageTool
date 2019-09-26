using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzSignalR.Monitor.Storage.Entities
{
    public class EntityPropertySerializer
    {
        public static void Serialize<TEntity>(TEntity entity, IDictionary<string, EntityProperty> results)
        {
            foreach (var property in entity.GetType().GetProperties())
            {
                var convertableAttribute = (ConvertableEntityPropertyAttribute)Attribute.GetCustomAttribute(property, typeof(ConvertableEntityPropertyAttribute));
                if (convertableAttribute != null)
                {
                    var propertyValue = entity.GetType().GetProperty(property.Name)?.GetValue(entity);
                    results.Add(property.Name, new EntityProperty(convertableAttribute.Serialize(propertyValue)));
                }
            }
        }

        public static void DeSerialize<TEntity>(TEntity entity, IDictionary<string, EntityProperty> properties)
        {
            foreach (var property in entity.GetType().GetProperties())
            {
                // Convertable property
                var convertableAttribute = (ConvertableEntityPropertyAttribute)Attribute.GetCustomAttribute(property, typeof(ConvertableEntityPropertyAttribute));
                if (convertableAttribute != null && properties.ContainsKey(property.Name))
                {
                    Type resultType = property.PropertyType;
                    if (convertableAttribute.ConvertToType != null)
                    {
                        resultType = convertableAttribute.ConvertToType;
                    }

                    var objectValue = convertableAttribute.Deserialize(properties[property.Name].StringValue, resultType);
                    // Set property only when deserialized value is not null,
                    // otherwise leave it to be the default value as constructed
                    if (objectValue != null)
                    {
                        entity.GetType().GetProperty(property.Name)?.SetValue(entity, objectValue);
                    }
                }

                // BackwardCompatible property
                if (!properties.ContainsKey(property.Name))
                {
                    var backwardCompatibleAttributes = Attribute.GetCustomAttributes(property, typeof(BackwardCompatibleEntityPropertyAttribute));
                    foreach (BackwardCompatibleEntityPropertyAttribute attribute in backwardCompatibleAttributes)
                    {
                        if (properties.TryGetValue(attribute.CompatibleName, out EntityProperty value))
                        {
                            entity.GetType().GetProperty(property.Name)?.SetValue(entity, value.PropertyAsObject);
                            break;
                        }
                    }
                }
            }
        }
    }
}
