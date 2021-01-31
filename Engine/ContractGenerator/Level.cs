using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using WhampsChallenge.Core.Common;
using WhampsChallenge.Core.Markers;

namespace ContractGeneration
{
    public class Level
    {
        public Dictionary<string, string[]> Enums { get; } = new Dictionary<string, string[]>();

        public class Action
        {
            public Dictionary<string, string> Parameters { get; } = new Dictionary<string, string>();
        }

        public Dictionary<string, Action> Actions { get; } = new Dictionary<string, Action>();

        public string ResultType { get; private set; }

        public Dictionary<string, Dictionary<string, string>> Types { get; } =
            new Dictionary<string, Dictionary<string, string>>();

        public int Index { get; set; }
        
        private void AddAction(Type action)
        {
            if (Actions.ContainsKey(action.Name)) return;

            var result = new Action();

            // Process parameters
            foreach (var parameter in action.GetProperties())
            {
                AddType(parameter.PropertyType);
                result.Parameters.Add(parameter.Name, AddType(parameter.PropertyType));
            }

            // Ignore return value

            // Add to contract
            Actions.Add(action.Name, result);
        }

        private string AddType(Type type)
        {
            if (type == typeof(object)) return "object";

            if (type.GetCustomAttribute<SharedAttribute>() != null)
            {
                return type.Name;
            }

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
                .Where(x => CustomAttributeExtensions.GetCustomAttribute<JsonIgnoreAttribute>((MemberInfo) x) == null);
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

        public Level()
        {
        }

        public Level(Discoverer.LevelData data)
        {
            foreach (var action in data.Actions)
            {
                AddAction(action);
            }

            ResultType = AddType(data.Result);
        }
    }
}
