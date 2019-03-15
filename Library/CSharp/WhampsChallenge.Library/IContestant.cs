using PantherDI.Attributes;

namespace WhampsChallenge.Library
{
    [Contract]
    public interface IContestant
    {
        void Run();
    }
}