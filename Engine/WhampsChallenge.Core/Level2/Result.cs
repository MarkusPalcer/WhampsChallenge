using WhampsChallenge.Core.Common.Events;
using WhampsChallenge.Core.Level1;
using WhampsChallenge.Core.Markers;

namespace WhampsChallenge.Core.Level2
{
    [Result]
    [Level(2)]
    public class Result
    {
        public IEvent[] Events { get; set; }

        public GameState GameState { get; set; }
    }
}