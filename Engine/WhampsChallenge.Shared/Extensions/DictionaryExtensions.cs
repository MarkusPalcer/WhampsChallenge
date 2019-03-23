using System.Collections.Generic;

namespace WhampsChallenge.Shared.Extensions
{
    public static class DictionaryExtensions
    {
        public static void Add<TKey, TValue, TCollection>(this Dictionary<TKey, TCollection> self, TKey key, TValue value) where TCollection : ICollection<TValue>, new()
        {
            if (!self.TryGetValue(key, out var list))
            {
                list = new TCollection();
                self[key] = list;
            }

            list.Add(value);
        }

        
    }
}