using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using WhampsChallenge.Core.Common;

namespace WhampsChallenge.Messaging.Common
{
    public class ActionDecoder : IActionDecoder
    {
        protected readonly Dictionary<string, Type> RegisteredTypes;

        public ActionDecoder(int level)
        {
            var discoverer = new LevelDiscoverer();
            var levelData = discoverer[level];
            RegisteredTypes = levelData.Actions.ToDictionary(x => x.Name);
        }

        public IAction Decode(JObject message)
        {
            var actionName = message["Action"].Value<string>();
            var actionType = RegisteredTypes[actionName];
            return (IAction) message.ToObject(actionType);
        }
    }
}
