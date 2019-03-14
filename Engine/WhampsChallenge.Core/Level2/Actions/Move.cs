using WhampsChallenge.Markers;

namespace WhampsChallenge.Level2.Actions
{
    [Action]
    public class Move : Level1.Actions.Move, IAction
    {
        public void Execute(Game game)
        {
            Execute((Level1.Game)game);
        }
    }
}