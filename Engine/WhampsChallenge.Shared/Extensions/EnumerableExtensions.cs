using System;
using System.Collections.Generic;
using System.Linq;

namespace WhampsChallenge.Shared.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> Repeat<T>(this Func<T> generator, int count)
        {
            return Enumerable.Range(1, count).Select(_ => generator());
        }
    }
}