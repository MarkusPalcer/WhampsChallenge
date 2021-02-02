using WhampsChallenge.Core.Common;
using WhampsChallenge.Shared.Communication;

namespace WhampsChallenge.Messaging.Common
{
    public class ActionDecoder : Decoder<IAction>
    {
        public ActionDecoder(int level) : base("Action")
        {
            var discoverer = new Discoverer();
            var levelData = discoverer[level];
            foreach (var action in levelData.Actions)
            {
                Add(action.Name, action);
            }
        }
    }
}
