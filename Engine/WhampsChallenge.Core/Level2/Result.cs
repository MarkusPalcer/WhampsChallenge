using WhampsChallenge.Core.Level1;
using WhampsChallenge.Core.Markers;

namespace WhampsChallenge.Core.Level2
{
    [Result]
    public class Result
    {
        public Perception[] Perceptions { get; set; }

        public GameState GameState { get; set; }
    }
}