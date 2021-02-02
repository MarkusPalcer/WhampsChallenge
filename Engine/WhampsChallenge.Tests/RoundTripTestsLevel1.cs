using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WhampsChallenge.Core.Common;
using WhampsChallenge.Core.Level1;
using WhampsChallenge.Library.Level1.Events;
using WhampsChallenge.Library.Shared.Enums;
using WhampsChallenge.Messaging.Common;
using WhampsChallenge.Runner.Shared.Direct;
using GameState = WhampsChallenge.Core.Common.GameState;

namespace WhampsChallenge.Tests
{
    [TestClass]
    public class RoundTripTestsLevel1
    {
        private Game game;
        private DirectCommunicator communicator;
        private Library.Level1.Game gameProxy;

        [TestInitialize]
        public void Initialize()
        {
            game = new Game();
            var decoder = new ActionDecoder(1);
            communicator = new DirectCommunicator(decoder);
            gameProxy = new Library.Level1.Game(communicator);
        }

        [TestMethod]
        public async Task MoveOnEmptySpot()
        {
            game.Seed = 0;
            game.Initialize();
            game.State.PlayerPosition = (0, 0);

            var responseTask = gameProxy.MoveAsync(Direction.East);
            var decodedAction = await communicator.ReceiveFromContestantAsync();
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
            game.Seed = 0;
            game.Initialize();
            game.State.PlayerPosition = (0, 0);

            var responseTask = gameProxy.MoveAsync(Direction.North);
            var decodedAction = await communicator.ReceiveFromContestantAsync();
            var sentResponse = game.Execute(decodedAction);
            sentResponse.As<Result>().GameState.PlayerPosition.Should().Be((0, 0));
            responseTask.IsCompleted.Should().BeFalse();
            communicator.SendToContestant(sentResponse);
            var decodedResponse = await responseTask;
            decodedResponse.Perceptions.Should().Contain(x => x.GetType() == typeof(Bump));
            decodedResponse.GameState.MovesLeft.Should().Be(99);
            game.GameState.Should().Be(GameState.Running);
        }

        [TestMethod]
        public async Task MoveOnGold()
        {
            game.Seed = 0;
            game.Initialize();
            game.State.PlayerPosition = (3, 1);

            var responseTask = gameProxy.MoveAsync(Direction.South);
            var decodedAction = await communicator.ReceiveFromContestantAsync();
            var sentResponse = game.Execute(decodedAction);
            sentResponse.As<Result>().GameState.PlayerPosition.Should().Be((3, 2));
            responseTask.IsCompleted.Should().BeFalse();
            communicator.SendToContestant(sentResponse);
            var decodedResponse = await responseTask;
            decodedResponse.Perceptions.OfType<Glitter>().Should().NotBeEmpty();
            decodedResponse.GameState.MovesLeft.Should().Be(99);
            game.GameState.Should().Be(GameState.Running);
        }

        [TestMethod]
        public async Task PickupGold()
        {
            game.Seed = 0;
            game.Initialize();
            game.State.PlayerPosition = (3, 2);

            var responseTask = gameProxy.PickupAsync();
            var decodedResponse = await ExecuteRequestedAction(communicator, game, responseTask);
            decodedResponse.Perceptions.Should().Contain(x => x.GetType() == typeof(Win));
            decodedResponse.GameState.MovesLeft.Should().Be(99);
            game.GameState.Should().Be(GameState.Win);
        }

        [TestMethod]
        public async Task PickupNothing()
        {
            game.Seed = 0;
            game.Initialize();

            var responseTask = gameProxy.PickupAsync();
            var decodedResponse = await ExecuteRequestedAction(communicator, game, responseTask);
            decodedResponse.Perceptions.Should().BeEmpty();
            decodedResponse.GameState.MovesLeft.Should().Be(99);
            game.GameState.Should().Be(GameState.Running);
        }

        [TestMethod]
        public async Task RunOutOfMoves()
        {
            game.Seed = 0;
            game.Initialize();
            game.State.MovesLeft = 1;

            var responseTask = gameProxy.PickupAsync();
            var decodedResponse = await ExecuteRequestedAction(communicator, game, responseTask);
            decodedResponse.Perceptions.Should().Contain(x => x.GetType() == typeof(Death));
            decodedResponse.Perceptions.Should().NotContain(x => x.GetType() == typeof(Win));
            decodedResponse.GameState.MovesLeft.Should().Be(0);
            game.GameState.Should().Be(GameState.Lose);
        }

        [TestMethod]
        public async Task PickupGoldInLastMove()
        {
            game.Seed = 0;
            game.Initialize();
            game.State.PlayerPosition = (3, 2);
            game.State.MovesLeft = 1;

            var responseTask = gameProxy.PickupAsync();
            var decodedResponse = await ExecuteRequestedAction(communicator, game, responseTask);
            decodedResponse.Perceptions.Should().Contain(x => x.GetType() == typeof(Win));
            decodedResponse.GameState.MovesLeft.Should().Be(0);
            game.GameState.Should().Be(GameState.Win);
        }

        private static async Task<Library.Level1.Types.Result> ExecuteRequestedAction(
            DirectCommunicator communicator,
            IGame game,
            Task<Library.Level1.Types.Result> responseTask)
        {
            var decodedAction = await communicator.ReceiveFromContestantAsync();
            var sentResponse = game.Execute(decodedAction);
            responseTask.IsCompleted.Should().BeFalse();
            communicator.SendToContestant(sentResponse);
            var decodedResponse = await responseTask;
            return decodedResponse;
        }
    }
}
