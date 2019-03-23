using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using WhampsChallenge.Core.Markers;
using WhampsChallenge.Shared.Maps;
using WhampsChallenge.Shared.Maps.FourDirections;

namespace WhampsChallenge.Core.Level1.Actions
{
    [Action]
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