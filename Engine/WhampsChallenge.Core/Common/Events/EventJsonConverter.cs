using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WhampsChallenge.Core.Common.Events
{
    public class EventJsonConverter : JsonConverter<IEvent>
    {
        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, IEvent value, JsonSerializer serializer)
        {
            var t = JToken.FromObject(value);

            t["Event"] = value.GetType().Name;

            t.WriteTo(writer);
        }

        /// <inheritdoc />
        public override IEvent ReadJson(JsonReader reader, Type objectType, IEvent existingValue, bool hasExistingValue,
                                        JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override bool CanRead => false;
    }
}
