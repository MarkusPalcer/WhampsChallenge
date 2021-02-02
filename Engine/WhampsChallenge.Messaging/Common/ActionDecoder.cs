using System.Linq;
using WhampsChallenge.Core.Common;
using WhampsChallenge.Core.Common.Discovery;
using WhampsChallenge.Shared.Communication;

namespace WhampsChallenge.Messaging.Common
{
    public class ActionDecoder : Decoder<IAction>
    {
        public ActionDecoder(IDiscoverer discoverer, int level) : base("Action")
        {
            var levelData = discoverer[level];
            foreach (var action in levelData.Actions)
            {
                Add(action.Name, action);
            }
        }
    }
}
