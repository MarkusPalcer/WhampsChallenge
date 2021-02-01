using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

namespace ContractGenerator
{
    public class SharedTypes
    {
        public Dictionary<string, string[]> Enums { get; } = new();

        public Dictionary<string, Dictionary<string, string>> Types { get; } = new();

        public Dictionary<string, Dictionary<string, string>> Events { get; } = new();

        public Dictionary<string, Dictionary<string, string>> Actions { get; } = new();

        public string AddType(Type type)
        {
            if (type == typeof(object)) return "object";

            if (type.IsEnum)
            {
                AddEnumType(type);
                return type.Name;
            }

            var typeName = type.Name;
            if (type.IsInterface) typeName = typeName.Substring(1);
            if (type.IsPrimitive) return typeName;

            if (Types.ContainsKey(typeName)) return typeName;

            if (type.IsArray)
            {
                AddType(type.GetElementType());
                return typeName;
            }

            var properties = type
                .GetProperties();
            var filteredProperties = properties
                .Where(x => x.GetCustomAttribute<JsonIgnoreAttribute>() == null);
            var typeData = filteredProperties
                .ToDictionary(x => x.Name, x => AddType(x.PropertyType));

            Types.Add(typeName, typeData);

            return typeName;
        }

        private void AddEnumType(Type type)
        {
            if (!type.IsEnum) throw new ArgumentOutOfRangeException(nameof(type));

            if (Enums.ContainsKey(type.Name)) return;

            Enums.Add(type.Name, Enum.GetNames(type));
        }
    }
}
