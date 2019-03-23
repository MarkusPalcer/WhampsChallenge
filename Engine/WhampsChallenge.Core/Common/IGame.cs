

namespace WhampsChallenge.Core.Common
{
    public enum GameState
    {
        Running,
        Win,
        Lose
    }
    public interface IGame
    {
        
        GameState GameState { get; }

        object Execute(IAction action);

        int Seed { get; set; }

        void Initialize();

        int Score { get; }
    }
}