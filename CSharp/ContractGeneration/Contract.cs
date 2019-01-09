using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

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

        public string ResultType { get; };

        public Dictionary<string, Dictionary<string, string>> Types { get; } = new Dictionary<string, Dictionary<string, string>>();

        private void AddEnum(Type enumType)
        {
            if (!enumType.IsEnum) throw new ArgumentOutOfRangeException(nameof(enumType));

            if (Enums.ContainsKey(enumType.Name)) return;

            Enums.Add(enumType.Name, Enum.GetNames(enumType));
        }

        private void AddAction(MethodBase action)
        {
            if (Actions.ContainsKey(action.Name)) return;

            var result = new Action();

            // Process parameters
            foreach (var parameter in action.GetParameters())
            {
                if (!parameter.ParameterType.IsEnum) continue;

                AddEnum(parameter.ParameterType);
                result.Parameters.Add(parameter.Name, parameter.ParameterType.Name);
            }

            // Ignore return value

            // Add to contract
            Actions.Add(action.Name, result);
        }

        private void ProcessResultType(Type resultType)
        {
            ResultType = AddType(resultType);
        }



        private string AddType(Type type)
        {
            if (type.IsEnum)
            {
                return AddEnumType(type);
            }

            foreach (var property in resultType.GetProperties().Where(x => x.GetCustomAttribute<JsonIgnoreAttribute>() != null))
            {
                if (property.PropertyType.IsEnum)
                {

                }
            }
        }

        private string AddEnumType(Type type)
        {
            AddEnum(type);
            return type.Name;
        }

        public static Contract Generate()
        {
            var result = new Contract();

            foreach (var action in typeof(WhampsChallenge.Contracts.IActions).GetMethods())
            {
                result.AddAction(action);
            } 

            return result;
        }

        public static string GenerateJson()
        {
            return JsonConvert.SerializeObject(Generate(), Formatting.Indented);
        }
    }
}
