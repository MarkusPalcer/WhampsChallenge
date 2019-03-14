using WhampsChallenge.Markers;

namespace WhampsChallenge.Level2.Actions
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