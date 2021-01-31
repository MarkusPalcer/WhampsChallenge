using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using WhampsChallenge.Core.Common;

namespace WhampsChallenge.Messaging.Common
{
    public abstract class ActionDecoder : IActionDecoder
    {
        protected readonly Dictionary<string, Type> RegisteredTypes = new();

        public IAction Decode(JObject message)
        {
            var actionName = message["Action"].Value<string>();
            var actionType = RegisteredTypes[actionName];
            return (IAction) message.ToObject(actionType);
        }
    }
}
