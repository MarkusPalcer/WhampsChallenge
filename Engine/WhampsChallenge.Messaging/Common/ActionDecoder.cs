using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using WhampsChallenge.Core.Common;

namespace WhampsChallenge.Messaging.Common
{
    public abstract class ActionDecoder : IActionDecoder
    {
        protected Dictionary<string, Type> registeredTypes = new Dictionary<string, Type>();

        public IAction Decode(JObject message)
        {
            var actionName = message["Action"].Value<string>();
            var actionType = registeredTypes[actionName];
            return (IAction) message.ToObject(actionType);
        }
    }
}