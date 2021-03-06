﻿using System;
using Newtonsoft.Json.Linq;
using PantherDI.ContainerCreation;
using WhampsChallenge.Core.Common;
using WhampsChallenge.Library;
using WhampsChallenge.Messaging.Common;
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
            var builder = new ContainerBuilder();
            builder.Register(communicator).WithContract(typeof(ICommunicator));

            builder.Register(LevelTypes.GameEngines[level]).As<IGame>().WithConstructors();
            builder.Register(LevelTypes.ActionDecoders[level]).As<IActionDecoder>().WithConstructors();

            builder.WithSupportForUnregisteredTypes();

            var container = builder.Build();
            var game = container.Resolve<IGame>();
            var actionDecoder = container.Resolve<IActionDecoder>();

            game.Initialize();
            Console.Out.WriteLine($"STRT: {level}");
            communicator.Send(new StartLevel
            {
                Level = level.ToString()
            });

            while (game.GameState == GameState.Running)
            {
                var receivedMessage = communicator.Receive<JObject>();
                var decodedAction = actionDecoder.Decode(receivedMessage);
                var response = game.Execute(decodedAction);
                communicator.Send(response);
            }

            return (
                Died: game.GameState == GameState.Lose,
                Score: game.Score,
                Seed: game.Seed
            );
        }
    }
}