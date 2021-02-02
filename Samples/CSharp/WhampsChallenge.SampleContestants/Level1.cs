using System.Linq;
using WhampsChallenge.Library;
using WhampsChallenge.Library.Level1;
using WhampsChallenge.Library.Level1.Events;
using WhampsChallenge.Library.Shared.Enums;
using WhampsChallenge.Library.Shared.Events;

namespace WhampsChallenge.SampleContestants
{
    public class Level1 : IAgent
    {
        private readonly Game game;

        public Level1(Game game)
        {
            this.game = game;
        }

        public void Run()
        {
            // Move all the way up
            var events = System.Array.Empty<Event>();

            while (!events.OfType<Bump>().Any())
            {
                events = game.Move(Direction.North).Events;
            }

            // Move all the way to the left
            events = System.Array.Empty<Event>();

            while (!events.OfType<Bump>().Any())
            {
                events = game.Move(Direction.West).Events;
            }

            // Zigzag through the game
            var direction = Direction.East;

            // First move
            events = game.Move(direction).Events;

            while (true)
            {
                // We lost
                if (events.OfType<Death>().Any()) return;

                // We won
                if (events.OfType<Win>().Any()) return;

                // We ran into the left or right wall, go south one and reverse direction
                if (events.OfType<Bump>().Any())
                {
                    direction = direction == Direction.East ? Direction.West : Direction.East;
                    events = game.Move(Direction.South).Events;
                    continue; // Reevaluate Events after move
                }

                // We stepped on the gold
                if (events.OfType<Glitter>().Any())
                {
                    events = game.Pickup().Events;
                    continue; // Reevaluate Events after pickup
                }

                // No special perception -> Move on
                events = game.Move(direction).Events;
            }
        }
    }
}
