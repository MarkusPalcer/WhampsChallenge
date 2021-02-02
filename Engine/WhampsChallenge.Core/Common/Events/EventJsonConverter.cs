using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WhampsChallenge.Core.Common.Discovery;
using WhampsChallenge.Shared.Communication;

namespace WhampsChallenge.Core.Common.Events
{
    public class EventJsonConverter : Decoder<IEvent>
    {
        public EventJsonConverter(IDiscoverer discoverer, int level) : base("Event")
        {
            var levelData = discoverer[level];
            foreach (var item in levelData.Events)
            {
                Add(item.Name, item);
            }
        }
    }
}
