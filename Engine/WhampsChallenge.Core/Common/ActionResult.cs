using WhampsChallenge.Core.Common.Events;
using WhampsChallenge.Core.Markers;

namespace WhampsChallenge.Core.Common
{
    [Result]
    [Shared]
    public class ActionResult
    {
        public IEvent[] Events { get; init; }

        public GameState GameState { get; init; }
    }
}
