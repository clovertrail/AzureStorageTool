using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzSignalR.Monitor.Storage.Entities
{
    public class Property
    {
        public Property()
        {
        }

        public Property(PropertyType type, object value, object detail = null,
            PropertySeverity severity = PropertySeverity.None)
        {
            Type = type;
            Value = value;
            Detail = detail;
            Severity = severity;
        }

        public PropertyType Type { get; set; }
        public object Value { get; set; }
        public object Detail { get; set; }
        public PropertySeverity Severity { get; set; }

        public void Set(PropertyType type, object value, object detail = null,
            PropertySeverity severity = PropertySeverity.None)
        {
            Type = type;
            Value = value;
            Detail = detail;
            Severity = severity;
        }

        public static Property FromString(string value)
        {
            return new Property(PropertyType.String, value);
        }

        public static Property FromNumber(double value)
        {
            return new Property(PropertyType.Number, value);
        }

        public static Property FromBool(bool value)
        {
            return new Property(PropertyType.Bool, value);
        }

        public static Property FromObject(object obj)
        {
            return new Property(PropertyType.Object, obj);
        }

        public static Property FromDictionary(IDictionary<string, Property> obj, string name)
        {
            return obj.TryGetValue(name, out var value) ? value : new Property();
        }
    }

    public enum PropertyType
    {
        None, Object, Icon, Bool, Number, String
    }

    public enum PropertyIcon
    {
        None, True, False
    }

    public enum PropertySeverity
    {
        None, Success, Info, Warning, Error
    }
}
