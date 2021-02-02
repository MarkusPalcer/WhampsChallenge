using System;
using Newtonsoft.Json;
using PantherDI.ContainerCreation;
using WhampsChallenge.Core.Common;
using WhampsChallenge.Core.Common.Discovery;
using WhampsChallenge.Core.Common.Events;
using WhampsChallenge.Library;
using WhampsChallenge.Shared.Communication;
using WhampsChallenge.Shared.Communication.Messages;
using WhampsChallenge.Shared.Extensions;

namespace WhampsChallenge.Runner.Shared
{
    public class Runner
    {
        private readonly ICommunicator communicator;

        public Runner(ICommunicator communicator)
        {
            this.communicator = communicator;
        }

        public (bool Died, int Score, int Seed) Run(Levels level)
        {
            var discoverer = new Discoverer();
            var actionDecoder = new KnownTypeJsonConverter<Action>("Action", discoverer[(int) level].Actions);
            var eventDecoder = new KnownTypeJsonConverter<IEvent>("Event", discoverer[(int) level].Events);

            var builder = new ContainerBuilder();
            builder.Register(communicator).WithContract(typeof(ICommunicator));

            builder.Register(LevelTypes.GameEngines[level]).As<IGame>().WithConstructors();

            builder.WithSupportForUnregisteredTypes();

            var container = builder.Build();
            var game = container.Resolve<IGame>();

            game.Initialize();
            Console.Out.WriteLine($"START: {level}");
            communicator.Send(JsonConvert.SerializeObject(new StartLevel
            {
                Level = level.ToString()
            }));

            while (game.GameCompletionState == GameCompletionStates.Running)
            {
                var receivedMessage = communicator.Receive();
                var decodedAction = JsonConvert.DeserializeObject<IAction>(receivedMessage, actionDecoder);
                var response = game.Execute(decodedAction);
                var encodedResponse = JsonConvert.SerializeObject(response, eventDecoder);
                communicator.Send(encodedResponse);
            }

            return (
                Died: game.GameCompletionState == GameCompletionStates.Lose,
                Score: game.Score,
                Seed: game.Seed
            );
        }
    }
}
