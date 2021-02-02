using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WhampsChallenge.Core.Common;

namespace WhampsChallenge.Messaging.Common
{
    public class ActionDecoder : JsonConverter<IAction>
    {
        private readonly Dictionary<string, Type> registeredTypes;

        public ActionDecoder(int level)
        {
            var discoverer = new Discoverer();
            var levelData = discoverer[level];
            registeredTypes = levelData.Actions.ToDictionary(x => x.Name);
        }

        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, IAction value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override IAction ReadJson(JsonReader reader, Type objectType, IAction existingValue, bool hasExistingValue,
                                         JsonSerializer serializer)
        {
            var obj = JObject.Load(reader);
            var actionName = obj.Value<string>("Action");
            var actionType = registeredTypes[actionName];
            return (IAction) obj.ToObject(actionType);
        }

        /// <inheritdoc />
        public override bool CanWrite => false;
    }
}
