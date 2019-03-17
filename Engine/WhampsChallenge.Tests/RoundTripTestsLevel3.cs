using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WhampsChallenge.Core.Common;
using WhampsChallenge.Core.Level3;
using WhampsChallenge.Shared.Maps.FourDirections;
using FieldContent = WhampsChallenge.Core.Level3.FieldContent;
using GameState = WhampsChallenge.Core.Common.GameState;

namespace WhampsChallenge.Tests
{
    [TestClass]
    public class RoundTripTestsLevel3
    {
        [TestMethod]
        public async Task MoveOnEmptySpot()
        {
            var game = new Game();
            var communicator = new TestCommunicator();
            var gameProxy = new Library.Level3.Game(communicator);
            var decoder = new Messaging.Level3.Actions.ActionDecoder();

            game.Seed = 0;
            game.Initialize();
            game.State.PlayerPosition = (0, 4);

            var responseTask = gameProxy.MoveAsync(Direction.North);
            var decodedAction = decoder.Decode(await communicator.GetLastMessage());
            var sentResponse = game.Execute(decodedAction);
            sentResponse.As<Result>().GameState.PlayerPosition.Should().Be((0, 3));
            responseTask.IsCompleted.Should().BeFalse();
            communicator.SetResponse(sentResponse);
            var decodedResponse = await responseTask;
            decodedResponse.Perceptions.Should().BeEmpty();
            decodedResponse.GameState.MovesLeft.Should().Be(99);
            game.GameState.Should().Be(GameState.Running);
        }

        [TestMethod]
        public async Task MoveIntoWall()
        {
            var game = new Game();
            var communicator = new TestCommunicator();
            var gameProxy = new Library.Level3.Game(communicator);
            var decoder = new Messaging.Level3.Actions.ActionDecoder();

            game.Seed = 0;
            game.Initialize();
            game.State.PlayerPosition = (2, 0);

            var responseTask = gameProxy.MoveAsync(Direction.North);
            var decodedAction = decoder.Decode(await communicator.GetLastMessage());
            var sentResponse = game.Execute(decodedAction);
            sentResponse.As<Result>().GameState.PlayerPosition.Should().Be((2, 0));
            responseTask.IsCompleted.Should().BeFalse();
            communicator.SetResponse(sentResponse);
            var decodedResponse = await responseTask;
            decodedResponse.Perceptions.Should().BeEquivalentTo(Perception.Bump);
            decodedResponse.GameState.MovesLeft.Should().Be(99);
            game.GameState.Should().Be(GameState.Running);
        }

        [TestMethod]
        public async Task MoveIntoWind()
        {
            var game = new Game();
            var communicator = new TestCommunicator();
            var gameProxy = new Library.Level3.Game(communicator);
            var decoder = new Messaging.Level3.Actions.ActionDecoder();

            game.Seed = 0;
            game.Initialize();
            game.State.PlayerPosition = (1, 2);

            var responseTask = gameProxy.MoveAsync(Direction.North);
            var decodedAction = decoder.Decode(await communicator.GetLastMessage());
            var sentResponse = game.Execute(decodedAction);
            sentResponse.As<Result>().GameState.PlayerPosition.Should().Be((1, 1));
            responseTask.IsCompleted.Should().BeFalse();
            communicator.SetResponse(sentResponse);
            var decodedResponse = await responseTask;
            decodedResponse.Perceptions.Should().BeEquivalentTo(Perception.Wind);
            decodedResponse.GameState.MovesLeft.Should().Be(99);
            game.GameState.Should().Be(GameState.Running);
        }

        [TestMethod]
        public void NoMoreThanOneWindPerField()
        {
            var game = new Game();
            game.Seed = 0;
            game.Initialize();

            for (var x = 0; x < 5; x ++)
            for (var y = 0; y < 5; y++)
            {
                game.State.Map[x, y].AdjacentFields.Count(f => f.Content == FieldContent.Trap).Should()
                    .BeLessOrEqualTo(1, "the field on [{0},{1}] should have only one wind on it", x, y);
            }
        }

        [TestMethod]
        public async Task MoveOnGold()
        {
            var game = new Game();
            var communicator = new TestCommunicator();
            var gameProxy = new Library.Level3.Game(communicator);
            var decoder = new Messaging.Level3.Actions.ActionDecoder();

            game.Seed = 0;
            game.Initialize();
            game.State.PlayerPosition = (3, 3);

            var responseTask = gameProxy.MoveAsync(Direction.North);
            var decodedAction = decoder.Decode(await communicator.GetLastMessage());
            var sentResponse = game.Execute(decodedAction);
            sentResponse.As<Result>().GameState.PlayerPosition.Should().Be((3, 2));
            responseTask.IsCompleted.Should().BeFalse();
            communicator.SetResponse(sentResponse);
            var decodedResponse = await responseTask;
            decodedResponse.Perceptions.Should().BeEquivalentTo(Perception.Glitter, Perception.Wind);
            decodedResponse.GameState.MovesLeft.Should().Be(99);
            game.GameState.Should().Be(GameState.Running);
        }

        [TestMethod]
        public async Task PickupGold()
        {
            var game = new Game();
            var communicator = new TestCommunicator();
            var gameProxy = new Library.Level3.Game(communicator);
            var decoder = new Messaging.Level3.Actions.ActionDecoder();

            game.Seed = 0;
            game.Initialize();
            game.State.PlayerPosition = (3, 2);

            var responseTask = gameProxy.PickupAsync();
            var decodedResponse = await ExecuteRequestedAction(decoder, communicator, game, responseTask);
            decodedResponse.Perceptions.Should().Contain(Library.Level3.Enums.Perception.Win);
            decodedResponse.Perceptions.Should().NotContain(Library.Level3.Enums.Perception.Death);
            decodedResponse.GameState.MovesLeft.Should().Be(99);
            game.GameState.Should().Be(GameState.Win);
        }

        [TestMethod]
        public async Task PickupNothing()
        {
            var game = new Game();
            var communicator = new TestCommunicator();
            var gameProxy = new Library.Level3.Game(communicator);
            var decoder = new Messaging.Level3.Actions.ActionDecoder();

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
            var communicator = new TestCommunicator();
            var gameProxy = new Library.Level3.Game(communicator);
            var decoder = new Messaging.Level3.Actions.ActionDecoder();

            game.Seed = 0;
            game.Initialize();
            game.State.MovesLeft = 1;

            var responseTask = gameProxy.PickupAsync();
            var decodedResponse = await ExecuteRequestedAction(decoder, communicator, game, responseTask);
            decodedResponse.Perceptions.Should().Contain(Library.Level3.Enums.Perception.Death);
            decodedResponse.Perceptions.Should().NotContain(Library.Level3.Enums.Perception.Win);
            decodedResponse.GameState.MovesLeft.Should().Be(0);
            game.GameState.Should().Be(GameState.Lose);
        }

        [TestMethod]
        public async Task MoveOnTrap()
        {
            var game = new Game();
            var communicator = new TestCommunicator();
            var gameProxy = new Library.Level3.Game(communicator);
            var decoder = new Messaging.Level3.Actions.ActionDecoder();

            game.Seed = 0;
            game.Initialize();
            game.State.PlayerPosition = (0, 0);

            var responseTask = gameProxy.MoveAsync(Direction.South);
            var decodedResponse = await ExecuteRequestedAction(decoder, communicator, game, responseTask);
            decodedResponse.Perceptions.Should().Contain(Library.Level3.Enums.Perception.Death);
            decodedResponse.Perceptions.Should().NotContain(Library.Level3.Enums.Perception.Win);
            decodedResponse.GameState.MovesLeft.Should().Be(99);
            game.GameState.Should().Be(GameState.Lose);
        }

        [TestMethod]
        public async Task MoveOnWhamps()
        {
            var game = new Game();
            var communicator = new TestCommunicator();
            var gameProxy = new Library.Level3.Game(communicator);
            var decoder = new Messaging.Level3.Actions.ActionDecoder();

            game.Seed = 0;
            game.Initialize();
            game.State.PlayerPosition = (0, 1);

            var responseTask = gameProxy.MoveAsync(Direction.North);
            var decodedResponse = await ExecuteRequestedAction(decoder, communicator, game, responseTask);
            decodedResponse.Perceptions.Should().Contain(Library.Level3.Enums.Perception.Death);
            decodedResponse.Perceptions.Should().NotContain(Library.Level3.Enums.Perception.Win);
            decodedResponse.GameState.MovesLeft.Should().Be(99);
            game.GameState.Should().Be(GameState.Lose);
        }

        [TestMethod]
        public async Task PickupGoldInLastMove()
        {
            var game = new Game();
            var communicator = new TestCommunicator();
            var gameProxy = new Library.Level3.Game(communicator);
            var decoder = new Messaging.Level3.Actions.ActionDecoder();

            game.Seed = 0;
            game.Initialize();
            game.State.PlayerPosition = (3, 2);
            game.State.MovesLeft = 1;

            var responseTask = gameProxy.PickupAsync();
            var decodedResponse = await ExecuteRequestedAction(decoder, communicator, game, responseTask);
            decodedResponse.Perceptions.Should().Contain(Library.Level3.Enums.Perception.Win);
            decodedResponse.Perceptions.Should().NotContain(Library.Level3.Enums.Perception.Death);
            decodedResponse.GameState.MovesLeft.Should().Be(0);
            game.GameState.Should().Be(GameState.Win);
        }

        private static async Task<Library.Level3.Types.Result> ExecuteRequestedAction(
            Messaging.Level3.Actions.ActionDecoder decoder, 
            TestCommunicator communicator, 
            IGame game,
            Task<Library.Level3.Types.Result> responseTask)
        {
            var decodedAction = decoder.Decode(await communicator.GetLastMessage());
            var sentResponse = game.Execute(decodedAction);
            responseTask.IsCompleted.Should().BeFalse();
            communicator.SetResponse(sentResponse);
            var decodedResponse = await responseTask;
            return decodedResponse;
        }
    }
}
