﻿using System;
using System.Collections.Generic;
using System.Reflection;
using WhampsChallenge.Core.Markers;

namespace WhampsChallenge.Core.Common
{
    public class LevelDiscoverer
    {
        private Dictionary<int, LevelData> data = new Dictionary<int, LevelData>();

        public class LevelData
        {
            public readonly List<Type> Actions = new List<Type>();
            public Type Result = null;
        }

        public LevelDiscoverer()
        {
            foreach (var type in typeof(LevelDiscoverer).Assembly.DefinedTypes)
            {
                var levelAttributes = type.GetCustomAttributes<LevelAttribute>();
                foreach (var levelAttribute in levelAttributes)
                {
                    var entry = this[levelAttribute.Level];
                    if (type.GetCustomAttribute<ActionAttribute>() != null)
                    {
                        entry.Actions.Add(type);
                    } else if (type.GetCustomAttribute<ResultAttribute>() != null)
                    {
                        entry.Result = type;
                    }
                }
            }
        }

        public IEnumerable<int> Levels => data.Keys;

        public LevelData this[int key]
        {
            get
            {
                if (data.TryGetValue(key, out LevelData value))
                {
                    return value;
                }
                else
                {
                    value = new LevelData();
                    data.Add(key, value);
                    return value;
                }

            }
        }
    }
}