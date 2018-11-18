using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace WhampsChallenge
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> Repeat<T>(Func<T> generator, int count)
        {
            return Enumerable.Range(1, count).Select(_ => generator());
        }
    }
}