

namespace WhampsChallenge.Common
{
    public interface IGame
    {
        object Execute(IAction action);

        void Initialize();
    }
}