using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using WhampsChallenge.Core.Common.Events;
using WhampsChallenge.Core.Markers;

namespace WhampsChallenge.Core.Common.Discovery
{
    public class Discoverer : IDiscoverer
    {
        private readonly Dictionary<int, LevelData> data = new();

        public List<Type> SharedTypes { get; }= new();

        public Discoverer()
        {
            foreach (var type in typeof(Discoverer).Assembly.DefinedTypes)
            {
                var levelAttributes = type.GetCustomAttributes<LevelAttribute>();
                foreach (var level in levelAttributes.SelectMany(x => x))
                {
                    var entry = this[level];
                    if (type.GetCustomAttribute<ActionAttribute>() != null)
                    {
                        entry.Actions.Add(type);
                    }
                    else if (typeof(IEvent).IsAssignableFrom(type))
                    {
                        entry.Events.Add(type);
                    }
                    else if (type.GetCustomAttribute<ResultAttribute>() != null)
                    {
                        entry.Result = type;
                    }
                }

                if (type.GetCustomAttribute<SharedAttribute>() != null)
                {
                    SharedTypes.Add(type);
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
