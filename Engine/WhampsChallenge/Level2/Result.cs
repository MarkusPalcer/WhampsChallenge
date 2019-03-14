using WhampsChallenge.Level1;
using WhampsChallenge.Markers;

namespace WhampsChallenge.Level2
{
    [Result]
    public class Result
    {
        public Perception[] Perceptions { get; set; }

        public GameState GameState { get; set; }
    }
}