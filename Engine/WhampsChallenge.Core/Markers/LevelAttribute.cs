using System;
using System.Collections;
using System.Collections.Generic;

namespace WhampsChallenge.Core.Markers
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class LevelAttribute : Attribute, IEnumerable<int>
    {
        private readonly IEnumerable<int> levels;

        public LevelAttribute(int level)
        {
            levels = new[] {level};
        }

        public IEnumerator<int> GetEnumerator()
        {
            return levels.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) levels).GetEnumerator();
        }
    }
}