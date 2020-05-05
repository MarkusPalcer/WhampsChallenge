using WhampsChallenge.Core.Level3;
using WhampsChallenge.Core.Markers;

namespace WhampsChallenge.Core.Level1.Actions
{
    [Action(1)]
    public class Pickup : IAction
    {
        public void Execute(Game game)
        {
            if (game.State.Map[game.State.PlayerPosition].Content == FieldContent.Gold)
            {
                game.AddPerception(Perception.Win);
                game.GameState = Common.GameState.Win;
            }
        }
    }
}