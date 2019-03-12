using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using WhampsChallenge;
using WhampsChallenge.Markers;
using static System.String;

namespace ContractGeneration
{
    public class Contract 
    {
        public Dictionary<string, string[]> Enums { get; } = new Dictionary<string, string[]>();

        public class Action
        {
            public Dictionary<string, string> Parameters { get; } = new Dictionary<string, string>();
        }

        public Dictionary<string, Action> Actions { get; } = new Dictionary<string, Action>();

        public string ResultType { get; private set; }

        public Dictionary<string, Dictionary<string, string>> Types { get; } = new Dictionary<string, Dictionary<string, string>>();

        private void AddAction(Type action)
        {
            if (Actions.ContainsKey(action.Name)) return;

            var result = new Action();

            // Process parameters
            foreach (var parameter in action.GetProperties())
            {
                AddType(parameter.PropertyType);
                result.Parameters.Add(parameter.Name, parameter.PropertyType.Name);
            }

            // Ignore return value

            // Add to contract
            Actions.Add(action.Name, result);
        }

        private string AddType(Type type)
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
                .ToDictionary(x => x.Name, x=> AddType(x.PropertyType));

            Types.Add(typeName, typeData);

            return typeName;
        }

        private void AddEnumType(Type type)
        {
            if (!type.IsEnum) throw new ArgumentOutOfRangeException(nameof(type));

            if (Enums.ContainsKey(type.Name)) return;

            Enums.Add(type.Name, Enum.GetNames(type));
        }

        public static Contract Generate(IEnumerable<Type> levelTypes)
        {
            var typesByMarkerInterfaces = SplitTypesByMarkerInterfaces(levelTypes);

            if (typesByMarkerInterfaces[typeof(ResultAttribute)].Count > 1)
            {
                var offendingTypeNames = Join(", ", typesByMarkerInterfaces[typeof(ResultAttribute)].Select(x => x.FullName));
                throw new InvalidOperationException("Found multiple result types: [" + offendingTypeNames + "]");
            } 
            else if (!typesByMarkerInterfaces[typeof(ResultAttribute)].Any())
            {
                throw new InvalidOperationException("No result types found");
            }

            var result = new Contract();

            foreach (var action in typesByMarkerInterfaces[typeof(ActionAttribute)])
            {
                result.AddAction(action);
            }

            result.ResultType = result.AddType(typesByMarkerInterfaces[typeof(ResultAttribute)].Single());

            return result;
        }

        private static Dictionary<Type, List<Type>> SplitTypesByMarkerInterfaces(IEnumerable<Type> levelTypes)
        {
            var typesByMarker = new Dictionary<Type, List<Type>>
            {
                {typeof(ActionAttribute), new List<Type>()},
                {typeof(ResultAttribute), new List<Type>()}
            };

            foreach (var type in levelTypes)
            {
                foreach (var attribute in type.GetCustomAttributes())
                {
                    typesByMarker.Add(attribute.GetType(), type);
                }
            }

            return typesByMarker;
        }

        public static object Generate()
        {
            var foundTypes = new Dictionary<int, List<Type>>();

            foreach (var type in typeof(ActionAttribute).Assembly.GetTypes())
            {
                if (type.Namespace == null) continue;

                var match = Regex.Match(type.Namespace, "WhampsChallenge\\.Level(?'level'\\d+)");
                if (match.Success)
                {
                    foundTypes.Add(int.Parse(match.Groups["level"].Value), type);
                }
            }

            return foundTypes.ToDictionary(x => $"Level{x.Key}", x => Generate(x.Value));
        }
    }
}
