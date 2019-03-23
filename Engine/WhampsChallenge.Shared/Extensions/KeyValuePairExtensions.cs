using System.Collections.Generic;

namespace WhampsChallenge.Shared.Extensions
{
    public static class KeyValuePairExtensions
    {
        public static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> self, out TKey key,
            out TValue value)
        {
            key = self.Key;
            value = self.Value;
        }
    }
}