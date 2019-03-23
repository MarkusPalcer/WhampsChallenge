using WhampsChallenge.Core.Markers;

namespace WhampsChallenge.Core.Level3
{
    [Result]
    public class Result
    {
        public Perception[] Perceptions { get; set; }

        public GameState GameState { get; set; }
    }
}