using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WhampsChallenge.Core.Common;
using WhampsChallenge.Core.Common.Discovery;
using WhampsChallenge.Core.Level1;
using WhampsChallenge.Library.Level1.Events;
using WhampsChallenge.Library.Shared.Enums;
using WhampsChallenge.Runner.Shared.Direct;

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
            communicator = new DirectCommunicator(new Discoverer(), 1);
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
            sentResponse.GameState.PlayerPosition.Should().Be((1, 0));
            responseTask.IsCompleted.Should().BeFalse();
            communicator.SendToContestant(sentResponse);
            var decodedResponse = await responseTask;
            decodedResponse.Events.Should().BeEmpty();
            decodedResponse.GameState.MovesLeft.Should().Be(99);
            game.GameCompletionState.Should().Be(GameCompletionStates.Running);
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
            sentResponse.GameState.PlayerPosition.Should().Be((0, 0));
            responseTask.IsCompleted.Should().BeFalse();
            communicator.SendToContestant(sentResponse);
            var decodedResponse = await responseTask;
            decodedResponse.Events.Should().Contain(x => x.GetType() == typeof(Bump));
            decodedResponse.GameState.MovesLeft.Should().Be(99);
            game.GameCompletionState.Should().Be(GameCompletionStates.Running);
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
            sentResponse.GameState.PlayerPosition.Should().Be((3, 2));
            responseTask.IsCompleted.Should().BeFalse();
            communicator.SendToContestant(sentResponse);
            var decodedResponse = await responseTask;
            decodedResponse.Events.OfType<Glitter>().Should().NotBeEmpty();
            decodedResponse.GameState.MovesLeft.Should().Be(99);
            game.GameCompletionState.Should().Be(GameCompletionStates.Running);
        }

        [TestMethod]
        public async Task PickupGold()
        {
            game.Seed = 0;
            game.Initialize();
            game.State.PlayerPosition = (3, 2);

            var responseTask = gameProxy.PickupAsync();
            var decodedResponse = await ExecuteRequestedAction(communicator, game, responseTask);
            decodedResponse.Events.Should().Contain(x => x.GetType() == typeof(Win));
            decodedResponse.GameState.MovesLeft.Should().Be(99);
            game.GameCompletionState.Should().Be(GameCompletionStates.Win);
        }

        [TestMethod]
        public async Task PickupNothing()
        {
            game.Seed = 0;
            game.Initialize();

            var responseTask = gameProxy.PickupAsync();
            var decodedResponse = await ExecuteRequestedAction(communicator, game, responseTask);
            decodedResponse.Events.Should().BeEmpty();
            decodedResponse.GameState.MovesLeft.Should().Be(99);
            game.GameCompletionState.Should().Be(GameCompletionStates.Running);
        }

        [TestMethod]
        public async Task RunOutOfMoves()
        {
            game.Seed = 0;
            game.Initialize();
            game.State.MovesLeft = 1;

            var responseTask = gameProxy.PickupAsync();
            var decodedResponse = await ExecuteRequestedAction(communicator, game, responseTask);
            decodedResponse.Events.Should().Contain(x => x.GetType() == typeof(Death));
            decodedResponse.Events.Should().NotContain(x => x.GetType() == typeof(Win));
            decodedResponse.GameState.MovesLeft.Should().Be(0);
            game.GameCompletionState.Should().Be(GameCompletionStates.Lose);
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
            decodedResponse.Events.Should().Contain(x => x.GetType() == typeof(Win));
            decodedResponse.GameState.MovesLeft.Should().Be(0);
            game.GameCompletionState.Should().Be(GameCompletionStates.Win);
        }

        private static async Task<Library.Shared.Types.ActionResult> ExecuteRequestedAction(
            DirectCommunicator communicator,
            IGame game,
            Task<Library.Shared.Types.ActionResult> responseTask)
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
