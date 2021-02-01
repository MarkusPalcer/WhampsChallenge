using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WhampsChallenge.Library.Shared.Events;

namespace WhampsChallenge.Library
{
    public abstract class EventDecoder : JsonConverter<Event>
    {
        protected Dictionary<string, Func<Event>> Constructors { get; } = new();

        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, Event value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override Event ReadJson(JsonReader reader, Type objectType, Event existingValue, bool hasExistingValue,
                                       JsonSerializer serializer)
        {
            var obj  = JObject.Load(reader);
            var type = obj.Value<string>("Event");

            var result = Constructors[type]();
            serializer.Populate(obj.CreateReader(), result);

            return result;
        }

        /// <inheritdoc />
        public override bool CanWrite => false;
    }
}
