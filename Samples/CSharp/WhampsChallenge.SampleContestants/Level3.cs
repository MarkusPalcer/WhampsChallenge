using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using WhampsChallenge.Library;
using WhampsChallenge.Library.Level3;
using WhampsChallenge.Library.Level3.Enums;
using WhampsChallenge.Shared.Maps;
using WhampsChallenge.Shared.Maps.FourDirections;

namespace WhampsChallenge.SampleContestants
{
    public class Level3 : IAgent
    {
        enum FieldData
        {
            Unknown,
            Safe,
            Dangerous,
            Visited,
            Wall
        }   

        private readonly Game game;
        private IField<Direction, FieldData> currentField;
        private readonly DynamicSizeMap<FieldData> map = new DynamicSizeMap<FieldData>((_, __) => FieldData.Unknown);

        public Level3(Game game)
        {
            this.game = game;
        }

        public void Run()
        {
            // Promise: The field you start on is completely empty and already visited.
            map[0, 0].Content = FieldData.Visited;

            // Thus the Adjacent fields are safe to enter
            foreach (var field in map[0,0].AdjacentFields)
            {
                field.Content = FieldData.Safe;
            }

            currentField = map[0, 0];

            while (true)
            {
                var path = GetNextPath().ToArray();

                foreach (var direction in path.Take(path.Length - 1))
                {
                    var perceptions = game.Move(direction).Perceptions;
                    currentField = currentField[direction];

                    // Stop when the game ends
                    if (perceptions.Contains(Perception.Win) || perceptions.Contains(Perception.Death)) return;
                }

                {
                    var direction = path.Last();
                    var perceptions = game.Move(direction).Perceptions;

                    // Stop when the game ends
                    if (perceptions.Contains(Perception.Win) || perceptions.Contains(Perception.Death)) return;
                    

                    if (perceptions.Contains(Perception.Bump))
                    {
                        currentField[direction].Content = FieldData.Wall;
                        continue;
                    }

                    currentField = currentField[direction];
                    currentField.Content = FieldData.Visited;

                    if (perceptions.Contains(Perception.Glitter))
                    {
                        perceptions = game.Pickup().Perceptions;

                        // Stop when the game ends
                        if (perceptions.Contains(Perception.Win) || perceptions.Contains(Perception.Death)) return;
                    }

                    // If there is wind or stench, flag unknown fields as dangerous, if there is none, flag all fields as safe
                    // This one is a pacifist, it will never use the arrow.
                    if (perceptions.Contains(Perception.Wind) || perceptions.Contains(Perception.Stench))
                    {
                        foreach (var field in currentField.AdjacentFields.Where(x => x.Content == FieldData.Unknown))
                        {
                            field.Content = FieldData.Dangerous;
                        }
                    }
                    else
                    {
                        foreach (var field in currentField.AdjacentFields.Where(x => x.Content != FieldData.Visited && x.Content != FieldData.Wall))
                        {
                            field.Content = FieldData.Safe;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Calculates the path to the next visited field
        /// </summary>
        /// <returns>The directions to move into in order to get to the safe field</returns>
        private IEnumerable<Direction> GetNextPath()
        {
            // Try to find the shortest way to a safe field. 
            // If none is found, get the shortest way to a dangerous field.
            // Only traverse visited fields 
            return currentField.GetShortestPathToField(x => x.Content == FieldData.Safe, x => x.Content != FieldData.Visited) ??
                   currentField.GetShortestPathToField(x => x.Content == FieldData.Dangerous, x => x.Content != FieldData.Visited);
        }
    }
}
