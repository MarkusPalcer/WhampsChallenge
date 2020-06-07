using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using WhampsChallenge.Core.Maps.FourDirections;
using WhampsChallenge.Core.Markers;

namespace WhampsChallenge.Core.Level1.Actions
{
    [Action]
    [Level(1)]
    public class Move : IAction
    {
        public Direction Direction { get; set; }

        public void Execute(Game game)
        {
            ValueTuple<int, int> newPosition;

            switch (Direction)
            {
                case Direction.North:
                    newPosition = (game.State.PlayerPosition.Item1, game.State.PlayerPosition.Item2 - 1);
                    break;
                case Direction.South:
                    newPosition = (game.State.PlayerPosition.Item1, game.State.PlayerPosition.Item2 + 1);
                    break;
                case Direction.West:
                    newPosition = (game.State.PlayerPosition.Item1 - 1, game.State.PlayerPosition.Item2);
                    break;
                case Direction.East:
                    newPosition = (game.State.PlayerPosition.Item1 + 1, game.State.PlayerPosition.Item2);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var newField = game.State.Map[newPosition];
            if (newField == null)
            {
                game.AddPerception(Perception.Bump);
            }
            else
            {
                game.State.PlayerPosition = newPosition;
            }
        }
    }
}