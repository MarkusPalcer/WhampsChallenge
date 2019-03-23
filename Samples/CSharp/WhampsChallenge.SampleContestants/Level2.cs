using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using WhampsChallenge.Library;
using WhampsChallenge.Library.Level2;
using WhampsChallenge.Library.Level2.Enums;
using WhampsChallenge.Shared.Maps;
using WhampsChallenge.Shared.Maps.FourDirections;

namespace WhampsChallenge.SampleContestants
{
    public class Level2 : IAgent
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

        public Level2(Game game)
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
                    Debug.WriteLine($"Changing ({currentField.Position}) from {currentField.Content} to Visited");
                    currentField.Content = FieldData.Visited;

                    if (perceptions.Contains(Perception.Glitter))
                    {
                        perceptions = game.Pickup().Perceptions;

                        // Stop when the game ends
                        if (perceptions.Contains(Perception.Win) || perceptions.Contains(Perception.Death)) return;
                    }

                    // If there is wind, flag unknown fields as dangerous, if there is none, flag all fields as safe
                    if (perceptions.Contains(Perception.Wind))
                    {
                        foreach (var field in currentField.AdjacentFields.Where(x => x.Content == FieldData.Unknown))
                        {
                            Debug.WriteLine($"Changing ({field.X}, {field.Y}) from {field.Content} to Dangerous");
                            field.Content = FieldData.Dangerous;
                        }
                    }
                    else
                    {
                        foreach (var field in currentField.AdjacentFields.Where(x => x.Content != FieldData.Visited && x.Content != FieldData.Wall))
                        {
                            Debug.WriteLine($"Changing ({field.X}, {field.Y}) from {field.Content} to Safe");
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
