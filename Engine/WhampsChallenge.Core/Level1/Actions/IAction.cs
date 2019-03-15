namespace WhampsChallenge.Core.Level1.Actions
{
    public interface IAction : Common.IAction
    {
        void Execute(Game game);
    }
}