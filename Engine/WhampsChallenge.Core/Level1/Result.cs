using WhampsChallenge.Core.Common.Events;
using WhampsChallenge.Core.Markers;

namespace WhampsChallenge.Core.Level1
{
    [Result]
    [Level(1)]
    public class Result
    {
        public IEvent[] Perceptions { get; set; }

        public GameState GameState { get; set; }
    }
}
