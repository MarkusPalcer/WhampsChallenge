using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using WhampsChallenge.Core.Common;
using WhampsChallenge.Core.Markers;

// Disabled inspections due to this being a serialized file
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable PropertyCanBeMadeInitOnly.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable CollectionNeverQueried.Global

namespace ContractGenerator
{
    public class Level
    {
        public Dictionary<string, string[]> Enums { get; } = new();

        public class Action
        {
            public Dictionary<string, string> Parameters { get; } = new();
        }

        public Dictionary<string, Action> Actions { get; } = new();

        public string ResultType { get; } = nameof(ActionResult);

        public Dictionary<string, Dictionary<string, string>> Types { get; } =  new();

        public Dictionary<string, Dictionary<string, string>> Events { get; } =  new();

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
                return AddType(type.GetElementType()) + "[]";
            }

            if (type.GetCustomAttribute<IgnoreAttribute>() == null) Types.Add(typeName, GenerateTypeData(type));

            return typeName;
        }

        private void AddEvent(string name, Type type)
        {
            if (type.GetCustomAttribute<SharedAttribute>() != null)
            {
                return;
            }

            Events.Add(name, GenerateTypeData(type));
        }

        private Dictionary<string, string> GenerateTypeData(Type type)
        {
            var properties = type
                .GetProperties();
            var filteredProperties = properties
                .Where(x => x.GetCustomAttribute<JsonIgnoreAttribute>() == null);
            var typeData = filteredProperties
                .ToDictionary(x => x.Name, x => AddType(x.PropertyType));
            return typeData;
        }

        private void AddEnumType(Type type)
        {
            if (!type.IsEnum) throw new ArgumentOutOfRangeException(nameof(type));

            if (Enums.ContainsKey(type.Name)) return;

            Enums.Add(type.Name, Enum.GetNames(type));
        }

        public Level(Discoverer.LevelData data)
        {
            foreach (var action in data.Actions)
            {
                AddAction(action);
            }

            foreach (var ev in data.Events)
            {
                AddEvent(ev.Name, ev);
            }
        }
    }
}
