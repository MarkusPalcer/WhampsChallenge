using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using WhampsChallenge.Core.Common;

namespace WhampsChallenge.Messaging.Common
{
    public class ActionDecoder : IActionDecoder
    {
        private readonly Dictionary<string, Type> registeredTypes;

        public ActionDecoder(int level)
        {
            var discoverer = new Discoverer();
            var levelData = discoverer[level];
            registeredTypes = levelData.Actions.ToDictionary(x => x.Name);
        }

        public IAction Decode(JObject message)
        {
            var actionName = message["Action"].Value<string>();
            var actionType = registeredTypes[actionName];
            return (IAction) message.ToObject(actionType);
        }
    }
}
