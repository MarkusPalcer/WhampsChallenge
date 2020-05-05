using System;

namespace WhampsChallenge.Core.Markers
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class LevelAttribute : Attribute
    {
        public LevelAttribute(int level)
        {
            Level = level;
        }

        public int Level { get; }
    }
}