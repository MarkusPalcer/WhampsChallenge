using Newtonsoft.Json.Linq;
using WhampsChallenge.Common;

namespace WhampsChallenge.Messaging.Common
{
    public interface IActionDecoder
    {
        IAction Decode(JObject message);
    }
}