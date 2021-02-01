using WhampsChallenge.Core.Common.Events;
using WhampsChallenge.Core.Extensions;
using WhampsChallenge.Core.Level1.Fields;
using WhampsChallenge.Core.Markers;

namespace WhampsChallenge.Core.Level1.Actions
{
    [Action]
    [Level(1)]
    public class Pickup : IAction
    {
        public void Execute(Game game)
        {
            if (game.State.Map[game.State.PlayerPosition].Content.IsNot<Gold>()) return;

            game.AddPerception(new Win());
            game.GameState = Common.GameState.Win;
        }
    }
}
