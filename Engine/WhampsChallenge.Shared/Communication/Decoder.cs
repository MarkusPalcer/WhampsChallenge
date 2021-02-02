using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WhampsChallenge.Shared.Communication
{
    public class Decoder<T> : JsonConverter<T>, IDictionary<string, Type>
    {
        private readonly Dictionary<string, Type> knownTypes = new();

        private readonly string typeNameProperty;

        public Decoder(string typeNameProperty)
        {
            this.typeNameProperty = typeNameProperty;
        }

        public override void WriteJson(JsonWriter writer, T value, JsonSerializer serializer)
        {
            var result = JObject.FromObject(value);
            result.Add(typeNameProperty, value.GetType().Name);
            result.WriteTo(writer);
        }

        public override T ReadJson(JsonReader reader, Type objectType, T existingValue, bool hasExistingValue,
                                   JsonSerializer serializer)
        {
            var tempObject = serializer.Deserialize<JObject>(reader);
            return (T) tempObject.ToObject(knownTypes[tempObject[typeNameProperty].Value<string>()]);
        }

        #region IDictionary
        /// <inheritdoc />
        IEnumerator<KeyValuePair<string, Type>> IEnumerable<KeyValuePair<string, Type>>.GetEnumerator()
        {
            return knownTypes.GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) knownTypes).GetEnumerator();
        }

        /// <inheritdoc />
        void ICollection<KeyValuePair<string, Type>>.Add(KeyValuePair<string, Type> item)
        {
            ((ICollection < KeyValuePair<string, Type>>)knownTypes).Add(item);
        }

        /// <inheritdoc />
        void ICollection<KeyValuePair<string, Type>>.Clear()
        {
            knownTypes.Clear();
        }

        /// <inheritdoc />
        bool ICollection<KeyValuePair<string, Type>>.Contains(KeyValuePair<string, Type> item)
        {
            return ((ICollection<KeyValuePair<string, Type>>) knownTypes).Contains(item);
        }

        /// <inheritdoc />
        void ICollection<KeyValuePair<string, Type>>.CopyTo(KeyValuePair<string, Type>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<string, Type>>) knownTypes).CopyTo(array, arrayIndex);
        }

        /// <inheritdoc />
        bool ICollection<KeyValuePair<string, Type>>.Remove(KeyValuePair<string, Type> item)
        {
            return ((ICollection<KeyValuePair<string, Type>>) knownTypes).Remove(item);
        }

        /// <inheritdoc />
        int ICollection<KeyValuePair<string, Type>>.Count => knownTypes.Count;

        /// <inheritdoc />
        bool ICollection<KeyValuePair<string, Type>>.IsReadOnly =>
            ((ICollection<KeyValuePair<string, Type>>) knownTypes).IsReadOnly;

        /// <inheritdoc />
        public void Add(string key, Type value)
        {
            knownTypes.Add(key, value);
        }

        /// <inheritdoc />
        bool IDictionary<string, Type>.ContainsKey(string key)
        {
            return knownTypes.ContainsKey(key);
        }

        /// <inheritdoc />
        bool IDictionary<string, Type>.Remove(string key)
        {
            return knownTypes.Remove(key);
        }

        /// <inheritdoc />
        bool IDictionary<string, Type>.TryGetValue(string key, out Type value)
        {
            return knownTypes.TryGetValue(key, out value);
        }

        /// <inheritdoc />
        Type IDictionary<string, Type>.this[string key]
        {
            get => knownTypes[key];
            set => knownTypes[key] = value;
        }

        /// <inheritdoc />
        ICollection<string> IDictionary<string, Type>.Keys => knownTypes.Keys;

        /// <inheritdoc />
        ICollection<Type> IDictionary<string, Type>.Values => knownTypes.Values;
        #endregion
    }
}
