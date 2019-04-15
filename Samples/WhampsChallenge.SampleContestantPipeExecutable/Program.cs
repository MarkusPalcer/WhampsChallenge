
using WhampsChallenge.Library;
using WhampsChallenge.SampleContestants;
using WhampsChallenge.Shared.Communication;

namespace WhampsChallenge.SampleContestantPipeExecutable
{
    class Program
    {
        static void Main()
        {
            // Run the contestant communicating through STDIN and STDOUT
            new Bootstrapper(Communicator.FromConsole(), new ContestEntry()).Run();
        }
    }
}
