using WhampsChallenge.Level3;
using WhampsChallenge.Markers;

namespace WhampsChallenge.Level1.Actions
{
    [Action]
    public class Pickup : IAction
    {
        public void Execute(Game game)
        {
            if (game.State.Map[game.State.PlayerPosition].Content == FieldContent.Gold)
            {
                game.AddPerception(Perception.Win);
                game.IsGameOver = true;
            }
        }
    }
}