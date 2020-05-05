using System;

namespace WhampsChallenge.Core.Markers
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ActionAttribute : Attribute
    {
        public ActionAttribute(int level)
        {
            Level = level;
        }

        public int Level { get; }
    }
}