using System;
using System.Collections.Generic;

namespace WhampsChallenge.Core.Common.Discovery
{
    public class LevelData
    {
        public readonly List<Type> Actions = new();
        public readonly List<Type> Events = new();
        public Type Result;
    }
}
