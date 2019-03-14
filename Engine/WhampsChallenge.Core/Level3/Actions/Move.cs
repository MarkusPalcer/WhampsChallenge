using WhampsChallenge.Level1;
using WhampsChallenge.Markers;

namespace WhampsChallenge.Level3.Actions
{
    [Action]
    public class Move : Level2.Actions.Move, IAction
    {
        public void Execute(Game game)
        {
            Execute((Level2.Game)game);
        }
    }
}