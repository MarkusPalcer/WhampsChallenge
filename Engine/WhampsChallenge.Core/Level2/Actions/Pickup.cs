using WhampsChallenge.Core.Markers;

namespace WhampsChallenge.Core.Level2.Actions
{
    [Action]
    public class Pickup: Level1.Actions.Pickup, IAction
    {
        public void Execute(Game game)
        {
            Execute((Level1.Game)game);
        }
    }
}