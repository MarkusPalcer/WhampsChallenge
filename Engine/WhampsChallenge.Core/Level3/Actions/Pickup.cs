using WhampsChallenge.Core.Markers;

namespace WhampsChallenge.Core.Level3.Actions
{
    [Action]
    [Level(3)]
    public class Pickup : Level2.Actions.Pickup, IAction
    {
        public void Execute(Game game)
        {
            Execute((Level2.Game)game);
        }
    }
}