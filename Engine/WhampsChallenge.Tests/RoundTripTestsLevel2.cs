using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WhampsChallenge.Core.Common;
using WhampsChallenge.Core.Common.Discovery;
using WhampsChallenge.Core.Level2;
using WhampsChallenge.Core.Level2.Fields;
using WhampsChallenge.Library.Level2.Events;
using WhampsChallenge.Library.Shared.Enums;
using WhampsChallenge.Runner.Shared.Direct;

namespace WhampsChallenge.Tests
{
    [TestClass]
    public class RoundTripTestsLevel2
    {
        private Game game;
        private DirectCommunicator communicator;
        private Library.Level2.Game gameProxy;

        [TestInitialize]
        public void Initialize()
        {
            game = new Game();
            communicator = new DirectCommunicator(new Discoverer(), 2);
            gameProxy = new Library.Level2.Game(communicator);
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
            game.State.PlayerPosition = (2, 0);

            var responseTask = gameProxy.MoveAsync(Direction.North);
            var decodedAction = await communicator.ReceiveFromContestantAsync();
            var sentResponse = game.Execute(decodedAction);
            sentResponse.GameState.PlayerPosition.Should().Be((2, 0));
            sentResponse.Events.OfType<Core.Level1.Events.Bump>().Should().NotBeEmpty();

            responseTask.IsCompleted.Should().BeFalse();
            communicator.SendToContestant(sentResponse);
            var decodedResponse = await responseTask;
            decodedResponse.Events.OfType<Bump>().Should().NotBeEmpty();
            decodedResponse.GameState.MovesLeft.Should().Be(99);
            game.GameCompletionState.Should().Be(GameCompletionStates.Running);
        }

        [TestMethod]
        public async Task MoveIntoWind()
        {
            game.Seed = 0;
            game.Initialize();
            game.State.PlayerPosition = (1, 0);

            var responseTask = gameProxy.MoveAsync(Direction.West);
            var decodedAction = await communicator.ReceiveFromContestantAsync();
            var sentResponse = game.Execute(decodedAction);
            sentResponse.GameState.PlayerPosition.Should().Be((0, 0));
            responseTask.IsCompleted.Should().BeFalse();
            communicator.SendToContestant(sentResponse);
            var decodedResponse = await responseTask;
            decodedResponse.Events.OfType<Wind>().Should().NotBeEmpty();
            decodedResponse.GameState.MovesLeft.Should().Be(99);
            game.GameCompletionState.Should().Be(GameCompletionStates.Running);
        }

        [TestMethod]
        public void NoMoreThanOneWindPerField()
        {
            game.Seed = 0;
            game.Initialize();

            for (var x = 0; x < 5; x ++)
            for (var y = 0; y < 5; y++)
            {
                Game.GetAdjacentFieldsOf(game.State.Map[(x, y)]).Select(field => field.Content)
                    .OfType<Trap>().Count().Should()
                    .BeLessOrEqualTo(1, "the field on [{0},{1}] should have only one wind on it", x, y);
            }
        }

        [TestMethod]
        public async Task MoveOnGold()
        {
            game.Seed = 0;
            game.Initialize();
            game.State.PlayerPosition = (3, 3);

            var responseTask = gameProxy.MoveAsync(Direction.North);
            var decodedAction = await communicator.ReceiveFromContestantAsync();
            var sentResponse = game.Execute(decodedAction);
            sentResponse.GameState.PlayerPosition.Should().Be((3, 2));
            responseTask.IsCompleted.Should().BeFalse();
            communicator.SendToContestant(sentResponse);
            var decodedResponse = await responseTask;
            decodedResponse.Events.Select(x => x.GetType()).Should().BeEquivalentTo(typeof(Glitter), typeof(Wind));
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
            decodedResponse.Events.OfType<Win>().Should().NotBeEmpty();
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
            decodedResponse.Events.OfType<Death>().Should().NotBeEmpty();
            decodedResponse.Events.OfType<Win>().Should().BeEmpty();
            decodedResponse.GameState.MovesLeft.Should().Be(0);
            game.GameCompletionState.Should().Be(GameCompletionStates.Lose);
        }

        [TestMethod]
        public async Task MoveOnTrap()
        {
            game.Seed = 0;
            game.Initialize();
            game.State.PlayerPosition = (0, 0);

            var responseTask = gameProxy.MoveAsync(Direction.South);
            var decodedResponse = await ExecuteRequestedAction(communicator, game, responseTask);
            decodedResponse.Events.OfType<Death>().Should().NotBeEmpty();
            decodedResponse.Events.OfType<Win>().Should().BeEmpty();
            decodedResponse.GameState.MovesLeft.Should().Be(99);
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
            decodedResponse.Events.OfType<Win>().Should().NotBeEmpty();
            decodedResponse.Events.OfType<Death>().Should().BeEmpty();
            decodedResponse.GameState.MovesLeft.Should().Be(0);
            game.GameCompletionState.Should().Be(GameCompletionStates.Win);
        }

        private static async Task<Library.Shared.Types.ActionResult> ExecuteRequestedAction(DirectCommunicator communicator,
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
