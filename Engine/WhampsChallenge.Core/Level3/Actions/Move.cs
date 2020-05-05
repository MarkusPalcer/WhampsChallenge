using WhampsChallenge.Core.Markers;

namespace WhampsChallenge.Core.Level3.Actions
{
    [Action(3)]
    public class Move : Level2.Actions.Move, IAction
    {
        public void Execute(Game game)
        {
            Execute((Level2.Game)game);
        }
    }
}