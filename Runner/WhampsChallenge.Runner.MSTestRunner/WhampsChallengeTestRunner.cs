using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WhampsChallenge.Library;
using WhampsChallenge.Runner.Shared.Direct;
using WhampsChallenge.SampleContestants;

namespace WhampsChallenge.Runner.MSTestRunner
{

    /// <summary>
    /// Runs a contestant against a level leveraging the MSTest-Framework for debugging the contestant.
    /// </summary>
    [TestClass]
    public class WhampsChallengeTestRunner
    {
        [TestMethod]
        public async Task RunLevel3()
        {
            var runner = DirectRunner.Create<Level3>(Levels.Level3);
            runner.Seed = -133356616;
            var result = await runner.Run();
            Console.WriteLine("Game over. Result: {0} Score: {1} Seed: {2}", result.Died ? "Dead" : "Alive", result.Score, result.Seed);
        }

        [TestMethod]
        public async Task RunLevel2()
        {
            var runner = DirectRunner.Create<Level2>(Levels.Level2);
            runner.Seed = -133356616;
            var result = await runner.Run();
            Console.WriteLine("Game over. Result: {0} Score: {1} Seed: {2}", result.Died ? "Dead" : "Alive", result.Score, result.Seed);
        }

        [TestMethod]
        public async Task RunLevel1()
        {
            var runner = DirectRunner.Create<Level1>(Levels.Level1);
            runner.Seed = -133356616;
            var result = await runner.Run();
            Console.WriteLine("Game over. Result: {0} Score: {1} Seed: {2}", result.Died ? "Dead" : "Alive", result.Score, result.Seed);
        }
    }
}
