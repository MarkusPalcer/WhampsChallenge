using WhampsChallenge.Core.Common.Events;
using WhampsChallenge.Core.Markers;

namespace WhampsChallenge.Core.Level3
{
    [Result]
    [Level(3)]
    public class Result
    {
        public IEvent[] Events { get; set; }

        public GameState GameState { get; set; }
    }
}
