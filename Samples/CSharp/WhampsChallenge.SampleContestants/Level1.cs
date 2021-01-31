using System.Linq;
using WhampsChallenge.Library;
using WhampsChallenge.Library.Level1;
using WhampsChallenge.Library.Level1.Enums;
using WhampsChallenge.Library.Shared.Enums;

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
            var perceptions = new Perception[]{};

            while (!perceptions.Contains(Perception.Bump))
            {
                perceptions = game.Move(Direction.North).Perceptions;
            }

            // Move all the way to the left
            perceptions = new Perception[]{};

            while (!perceptions.Contains(Perception.Bump))
            {
                perceptions = game.Move(Direction.West).Perceptions;
            }

            // Zigzag through the game
            var direction = Direction.East;

            // First move
            perceptions = game.Move(direction).Perceptions;

            while (true)
            {
                // We lost
                if (perceptions.Contains(Perception.Death)) return;

                // We won
                if (perceptions.Contains(Perception.Win)) return;

                // We ran into the left or right wall, go south one and reverse direction
                if (perceptions.Contains(Perception.Bump))
                {
                    direction = direction == Direction.East ? Direction.West : Direction.East;
                    perceptions = game.Move(Direction.South).Perceptions;
                    continue; // Reevaluate perceptions after move
                }

                // We stepped on the gold
                if (perceptions.Contains(Perception.Glitter))
                {
                    perceptions = game.Pickup().Perceptions;
                    continue; // Reevaluate perceptions after pickup
                }

                // No special perception -> Move on
                perceptions = game.Move(direction).Perceptions;
            }
        }
    }
}