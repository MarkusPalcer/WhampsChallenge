using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WhampsChallenge.Core.Common;
using WhampsChallenge.Core.Common.Discovery;
using WhampsChallenge.Core.Level2.Fields;
using WhampsChallenge.Core.Level3;
using WhampsChallenge.Library.Level3.Events;
using WhampsChallenge.Library.Shared.Enums;
using WhampsChallenge.Runner.Shared.Direct;

namespace WhampsChallenge.Tests
{
    
    [TestClass]
    public class RoundTripTestsLevel3
    {
        private Game game;
        private DirectCommunicator communicator;
        private Library.Level3.Game gameProxy;

        [TestInitialize]
        public void Initialize()
        {
            game = new Game();
            communicator = new DirectCommunicator(new Discoverer(), 3);
            gameProxy = new Library.Level3.Game(communicator);
        }
        
        [TestMethod]
        public async Task MoveOnEmptySpot()
        {
            game.Seed = 0;
            game.Initialize();
            game.State.PlayerPosition = (0, 4);

            var responseTask = gameProxy.MoveAsync(Direction.North);
            var decodedAction = await communicator.ReceiveFromContestantAsync();
            var sentResponse = game.Execute(decodedAction);
            sentResponse.GameState.PlayerPosition.Should().Be((0, 3));
            responseTask.IsCompleted.Should().BeFalse();
            communicator.SendToContestant(sentResponse);
            var decodedResponse = await responseTask;
            decodedResponse.Events.Should().BeEmpty();
            decodedResponse.GameState.MovesLeft.Should().Be(99);
            decodedResponse.GameState.HasArrow.Should().BeTrue();
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
            responseTask.IsCompleted.Should().BeFalse();
            communicator.SendToContestant(sentResponse);
            var decodedResponse = await responseTask;
            decodedResponse.Events.OfType<Bump>().Should().NotBeEmpty();
            decodedResponse.GameState.MovesLeft.Should().Be(99);
            decodedResponse.GameState.HasArrow.Should().BeTrue();
            game.GameCompletionState.Should().Be(GameCompletionStates.Running);
        }

        [TestMethod]
        public async Task MoveIntoWind()
        {
            game.Seed = 0;
            game.Initialize();
            game.State.PlayerPosition = (1, 2);

            var responseTask = gameProxy.MoveAsync(Direction.North);
            var decodedAction = await communicator.ReceiveFromContestantAsync();
            var sentResponse = game.Execute(decodedAction);
            sentResponse.GameState.PlayerPosition.Should().Be((1, 1));
            responseTask.IsCompleted.Should().BeFalse();
            communicator.SendToContestant(sentResponse);
            var decodedResponse = await responseTask;
            decodedResponse.Events.OfType<Wind>().Should().NotBeEmpty();
            decodedResponse.GameState.MovesLeft.Should().Be(99);
            decodedResponse.GameState.HasArrow.Should().BeTrue();
            game.GameCompletionState.Should().Be(GameCompletionStates.Running);
        }

        [TestMethod]
        public void NoMoreThanOneWindPerField()
        {
            game.Initialize();

            for (var x = 0; x < 5; x ++)
            for (var y = 0; y < 5; y++)
            {
                Core.Level2.Game.GetAdjacentFieldsOf(game.State.Map[(x, y)]).Select(field => field.Content).OfType<Trap>().Count().Should()
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
            decodedResponse.GameState.HasArrow.Should().BeTrue();
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
            decodedResponse.Events.OfType<Death>().Should().BeEmpty();
            decodedResponse.GameState.MovesLeft.Should().Be(99);
            decodedResponse.GameState.HasArrow.Should().BeTrue();
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
            decodedResponse.GameState.HasArrow.Should().BeTrue();
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
            decodedResponse.GameState.HasArrow.Should().BeTrue();
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
            decodedResponse.GameState.HasArrow.Should().BeTrue();
            game.GameCompletionState.Should().Be(GameCompletionStates.Lose);
        }

        [TestMethod]
        public async Task MoveOnWhamps()
        {
            game.Seed = 0;
            game.Initialize();
            game.State.PlayerPosition = (0, 1);

            var responseTask = gameProxy.MoveAsync(Direction.North);
            var decodedResponse = await ExecuteRequestedAction(communicator, game, responseTask);
            decodedResponse.Events.OfType<Death>().Should().NotBeEmpty();
            decodedResponse.Events.OfType<Win>().Should().BeEmpty();
            decodedResponse.GameState.MovesLeft.Should().Be(99);
            decodedResponse.GameState.HasArrow.Should().BeTrue();
            game.GameCompletionState.Should().Be(GameCompletionStates.Lose);
        }

        [TestMethod]
        public async Task ShootWhamps()
        {
            game.Seed = 0;
            game.Initialize();
            game.State.PlayerPosition = (0, 2);

            var responseTask = gameProxy.ShootAsync(Direction.North);
            var decodedResponse = await ExecuteRequestedAction(communicator, game, responseTask);
            decodedResponse.Events.OfType<Scream>().Should().NotBeEmpty();
            decodedResponse.Events.OfType<Twang>().Should().BeEmpty();
            decodedResponse.Events.OfType<Win>().Should().BeEmpty();
            decodedResponse.Events.OfType<Death>().Should().BeEmpty();
            decodedResponse.GameState.MovesLeft.Should().Be(99);
            decodedResponse.GameState.HasArrow.Should().BeFalse();

            responseTask = gameProxy.ShootAsync(Direction.North);
            decodedResponse = await ExecuteRequestedAction(communicator, game, responseTask);
            decodedResponse.Events.OfType<Scream>().Should().BeEmpty();
            decodedResponse.Events.OfType<Twang>().Should().NotBeEmpty();
            decodedResponse.Events.OfType<Win>().Should().BeEmpty();
            decodedResponse.Events.OfType<Death>().Should().BeEmpty();
            decodedResponse.GameState.MovesLeft.Should().Be(98);
            decodedResponse.GameState.HasArrow.Should().BeFalse();

            game.State.PlayerPosition = (0, 1);
            responseTask = gameProxy.MoveAsync(Direction.North);
            decodedResponse = await ExecuteRequestedAction(communicator, game, responseTask);
            decodedResponse.Events.OfType<Scream>().Should().BeEmpty();
            decodedResponse.Events.OfType<Twang>().Should().BeEmpty();
            decodedResponse.Events.OfType<Win>().Should().BeEmpty();
            decodedResponse.Events.OfType<Death>().Should().BeEmpty();
            decodedResponse.GameState.MovesLeft.Should().Be(97);
            decodedResponse.GameState.HasArrow.Should().BeFalse();

            game.GameCompletionState.Should().Be(GameCompletionStates.Running);
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
            decodedResponse.GameState.HasArrow.Should().BeTrue();
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
