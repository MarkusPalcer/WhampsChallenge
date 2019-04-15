using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WhampsChallenge.Core.Common;
using WhampsChallenge.Core.Level1;
using WhampsChallenge.Runner.Shared.Direct;
using WhampsChallenge.Shared.Maps.FourDirections;
using GameState = WhampsChallenge.Core.Common.GameState;

namespace WhampsChallenge.Tests
{
    [TestClass]
    public class RoundTripTestsLevel1
    {
        [TestMethod]
        public async Task MoveOnEmptySpot()
        {
            var game = new Game();
            var communicator = new DirectCommunicator();
            var gameProxy = new Library.Level1.Game(communicator);
            var decoder = new Messaging.Level1.Actions.ActionDecoder();

            game.Seed = 0;
            game.Initialize();
            game.State.PlayerPosition = (0, 0);

            var responseTask = gameProxy.MoveAsync(Direction.East);
            var decodedAction = decoder.Decode(await communicator.ReceiveFromContestantAsync());
            var sentResponse = game.Execute(decodedAction);
            sentResponse.As<Result>().GameState.PlayerPosition.Should().Be((1, 0));
            responseTask.IsCompleted.Should().BeFalse();
            communicator.SendToContestant(sentResponse);
            var decodedResponse = await responseTask;
            decodedResponse.Perceptions.Should().BeEmpty();
            decodedResponse.GameState.MovesLeft.Should().Be(99);
            game.GameState.Should().Be(GameState.Running);
        }

        [TestMethod]
        public async Task MoveIntoWall()
        {
            var game = new Game();
            var communicator = new DirectCommunicator();
            var gameProxy = new Library.Level1.Game(communicator);
            var decoder = new Messaging.Level1.Actions.ActionDecoder();

            game.Seed = 0;
            game.Initialize();
            game.State.PlayerPosition = (0, 0);

            var responseTask = gameProxy.MoveAsync(Direction.North);
            var decodedAction = decoder.Decode(await communicator.ReceiveFromContestantAsync());
            var sentResponse = game.Execute(decodedAction);
            sentResponse.As<Result>().GameState.PlayerPosition.Should().Be((0, 0));
            responseTask.IsCompleted.Should().BeFalse();
            communicator.SendToContestant(sentResponse);
            var decodedResponse = await responseTask;
            decodedResponse.Perceptions.Should().BeEquivalentTo(Perception.Bump);
            decodedResponse.GameState.MovesLeft.Should().Be(99);
            game.GameState.Should().Be(GameState.Running);
        }

        [TestMethod]
        public async Task MoveOnGold()
        {
            var game = new Game();
            var communicator = new DirectCommunicator();
            var gameProxy = new Library.Level1.Game(communicator);
            var decoder = new Messaging.Level1.Actions.ActionDecoder();

            game.Seed = 0;
            game.Initialize();
            game.State.PlayerPosition = (3, 1);

            var responseTask = gameProxy.MoveAsync(Direction.South);
            var decodedAction = decoder.Decode(await communicator.ReceiveFromContestantAsync());
            var sentResponse = game.Execute(decodedAction);
            sentResponse.As<Result>().GameState.PlayerPosition.Should().Be((3, 2));
            responseTask.IsCompleted.Should().BeFalse();
            communicator.SendToContestant(sentResponse);
            var decodedResponse = await responseTask;
            decodedResponse.Perceptions.Should().BeEquivalentTo(Perception.Glitter);
            decodedResponse.GameState.MovesLeft.Should().Be(99);
            game.GameState.Should().Be(GameState.Running);
        }

        [TestMethod]
        public async Task PickupGold()
        {
            var game = new Game();
            var communicator = new DirectCommunicator();
            var gameProxy = new Library.Level1.Game(communicator);
            var decoder = new Messaging.Level1.Actions.ActionDecoder();

            game.Seed = 0;
            game.Initialize();
            game.State.PlayerPosition = (3, 2);

            var responseTask = gameProxy.PickupAsync();
            var decodedResponse = await ExecuteRequestedAction(decoder, communicator, game, responseTask);
            decodedResponse.Perceptions.Should().Contain(Library.Level1.Enums.Perception.Win);
            decodedResponse.GameState.MovesLeft.Should().Be(99);
            game.GameState.Should().Be(GameState.Win);
        }

        [TestMethod]
        public async Task PickupNothing()
        {
            var game = new Game();
            var communicator = new DirectCommunicator();
            var gameProxy = new Library.Level1.Game(communicator);
            var decoder = new Messaging.Level1.Actions.ActionDecoder();

            game.Seed = 0;
            game.Initialize();

            var responseTask = gameProxy.PickupAsync();
            var decodedResponse = await ExecuteRequestedAction(decoder, communicator, game, responseTask);
            decodedResponse.Perceptions.Should().BeEmpty();
            decodedResponse.GameState.MovesLeft.Should().Be(99);
            game.GameState.Should().Be(GameState.Running);
        }

        [TestMethod]
        public async Task RunOutOfMoves()
        {
            var game = new Game();
            var communicator = new DirectCommunicator();
            var gameProxy = new Library.Level1.Game(communicator);
            var decoder = new Messaging.Level1.Actions.ActionDecoder();

            game.Seed = 0;
            game.Initialize();
            game.State.MovesLeft = 1;

            var responseTask = gameProxy.PickupAsync();
            var decodedResponse = await ExecuteRequestedAction(decoder, communicator, game, responseTask);
            decodedResponse.Perceptions.Should().Contain(Library.Level1.Enums.Perception.Death);
            decodedResponse.Perceptions.Should().NotContain(Library.Level1.Enums.Perception.Win);
            decodedResponse.GameState.MovesLeft.Should().Be(0);
            game.GameState.Should().Be(GameState.Lose);
        }

        [TestMethod]
        public async Task PickupGoldInLastMove()
        {
            var game = new Game();
            var communicator = new DirectCommunicator();
            var gameProxy = new Library.Level1.Game(communicator);
            var decoder = new Messaging.Level1.Actions.ActionDecoder();

            game.Seed = 0;
            game.Initialize();
            game.State.PlayerPosition = (3, 2);
            game.State.MovesLeft = 1;

            var responseTask = gameProxy.PickupAsync();
            var decodedResponse = await ExecuteRequestedAction(decoder, communicator, game, responseTask);
            decodedResponse.Perceptions.Should().BeEquivalentTo(Perception.Win);
            decodedResponse.GameState.MovesLeft.Should().Be(0);
            game.GameState.Should().Be(GameState.Win);
        }

        private static async Task<Library.Level1.Types.Result> ExecuteRequestedAction(
            Messaging.Level1.Actions.ActionDecoder decoder, 
            DirectCommunicator communicator, 
            IGame game,
            Task<Library.Level1.Types.Result> responseTask)
        {
            var decodedAction = decoder.Decode(await communicator.ReceiveFromContestantAsync());
            var sentResponse = game.Execute(decodedAction);
            responseTask.IsCompleted.Should().BeFalse();
            communicator.SendToContestant(sentResponse);
            var decodedResponse = await responseTask;
            return decodedResponse;
        }
    }
}
