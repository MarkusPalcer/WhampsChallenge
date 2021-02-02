namespace WhampsChallenge.Core.Common
{
    public interface IGame
    {
        
        GameCompletionStates GameCompletionState { get; }

        ActionResult Execute(IAction action);

        int Seed { get; set; }

        void Initialize();

        int Score { get; }
    }
}
