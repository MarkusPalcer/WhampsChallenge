using System;
using System.Threading.Tasks;
using PantherDI.ContainerCreation;
using WhampsChallenge.Core.Common;
using WhampsChallenge.Core.Common.Discovery;
using WhampsChallenge.Library;
using WhampsChallenge.Messaging.Common;
using WhampsChallenge.Shared.Communication;

namespace WhampsChallenge.Runner.Shared.Direct
{
    public class DirectRunner
    {
        public class Result
        {
            public int Score { get; set; }
            public bool Died { get; set; }

            public int Seed { get; set; }
        }

        private readonly Levels level;
        private readonly Type agentType;
        private IAgent contestant;
        private IGame game;
        private DirectCommunicator communicator;

        public DirectRunner(Levels level, Type agentType)
        {
            this.level = level;
            this.agentType = agentType;

            ScanWholeContestantAssembly = false;
        }

        public static DirectRunner Create<TAgent>(Levels level) where TAgent : IAgent
        {
            return new DirectRunner(level, typeof(TAgent));
        }

        public bool ScanWholeContestantAssembly { get; set; }

        public int? Seed { get; set; }

        public async Task<Result> Run()
        {
            var builder = new ContainerBuilder();

            if (ScanWholeContestantAssembly)
            {
                builder.WithAssemblyOf(agentType);
            }
            else
            {
                builder.WithType(agentType);
            }

            communicator = new DirectCommunicator(new Discoverer(), (int) level);

            builder.Register(communicator).WithContract(typeof(ICommunicator));

            builder.Register(LevelTypes.GameEngines[level]).As<IGame>().WithConstructors();

            builder.WithSupportForUnregisteredTypes();

            var container = builder.Build();

            contestant = container.Resolve<IAgent>(agentType);
            game = container.Resolve<IGame>();

            var contestantTask = Task.Run(() => contestant.Run());

            if (Seed.HasValue) game.Seed = Seed.Value;
            game.Initialize();
            while (game.GameCompletionState == GameCompletionStates.Running)
            {
                var decodedAction = await communicator.ReceiveFromContestantAsync();
                var response = game.Execute(decodedAction);
                communicator.SendToContestant(response);
            }

            communicator.Dispose();

            // Ensure that the contestant has stopped running
            await contestantTask;

            return new Result
            {
                Died = game.GameCompletionState == GameCompletionStates.Lose,
                Score = game.Score,
                Seed = game.Seed
            };
        }
    }
}
