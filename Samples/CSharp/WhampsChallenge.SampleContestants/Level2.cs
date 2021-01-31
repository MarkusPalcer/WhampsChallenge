using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using WhampsChallenge.Library;
using WhampsChallenge.Library.Level2;
using WhampsChallenge.Library.Level2.Enums;
using WhampsChallenge.Library.Shared.Enums;


namespace WhampsChallenge.SampleContestants
{
    public class Level2 : IAgent
    {
        enum FieldData
        {
            Safe,
            Dangerous,
            Visited,
            Wall
        }   

        private readonly Game game;
        private (int,int) currentField;
        private readonly Dictionary<(int,int), FieldData> map = new Dictionary<(int, int), FieldData>();

        public Level2(Game game)
        {
            this.game = game;
        }

        public void Run()
        {
            // Promise: The field you start on is completely empty and already visited.
            map[(0, 0)] = FieldData.Visited;

            // Thus the Adjacent fields are safe to enter
            foreach (var field in (0,0).AdjacentFields())
            {
                
                map[field] = FieldData.Safe;
            }

            currentField = (0,0);

            while (true)
            {
                var path = GetNextPath().ToArray();

                foreach (var direction in path.Take(path.Length - 1))
                {
                    var perceptions = game.Move(direction).Perceptions;
                    currentField = currentField.Go(direction);

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
                        map[currentField.Go(direction)] = FieldData.Wall;
                        continue;
                    }

                    currentField = currentField.Go(direction);

                    Debug.WriteLine($"Changing ({currentField})  to Visited");
                    map[currentField] = FieldData.Visited;

                    if (perceptions.Contains(Perception.Glitter))
                    {
                        perceptions = game.Pickup().Perceptions;

                        // Stop when the game ends
                        if (perceptions.Contains(Perception.Win) || perceptions.Contains(Perception.Death)) return;
                    }

                    // If there is wind, flag unknown fields as dangerous, if there is none, flag all fields as safe
                    if (perceptions.Contains(Perception.Wind))
                    {
                        foreach (var field in currentField.AdjacentFields().Where(x =>  !map.ContainsKey(x)))
                        {
                            map[field] = FieldData.Dangerous;
                        }
                    }
                    else
                    {
                        foreach (var field in currentField.AdjacentFields().Where(x => !map.ContainsKey(x) || (map[x] != FieldData.Visited && map[x] != FieldData.Wall)))
                        {
                            map[field] = FieldData.Safe;
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
            return map.GetShortestPathToField(currentField, x => x == FieldData.Safe, x => x != FieldData.Visited) ??
                   map.GetShortestPathToField(currentField, x => x == FieldData.Dangerous, x => x != FieldData.Visited);
        }
    }
}
