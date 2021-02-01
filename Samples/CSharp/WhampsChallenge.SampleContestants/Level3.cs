using System.Collections.Generic;
using System.Linq;
using WhampsChallenge.Library;
using WhampsChallenge.Library.Level3;
using WhampsChallenge.Library.Level3.Events;
using WhampsChallenge.Library.Shared.Enums;

namespace WhampsChallenge.SampleContestants
{
    public class Level3 : IAgent
    {
        enum FieldData
        {
            Safe,
            Dangerous,
            Visited,
            Wall
        }   

        private readonly Game game;
        private (int,int)  currentField;
        private readonly Dictionary<(int,int), FieldData> map = new Dictionary<(int, int), FieldData>();

        public Level3(Game game)
        {
            this.game = game;
        }

        public void Run()
        {
            // Promise: The field you start on is completely empty and already visited.
            map[(0, 0)] = FieldData.Visited;

            // Thus the Adjacent fields are safe to enter
            foreach (var field in (0, 0).AdjacentFields())
            {
                map[field] = FieldData.Safe;
            }

            currentField = (0, 0);

            while (true)
            {
                var path = GetNextPath().ToArray();

                foreach (var direction in path.Take(path.Length - 1))
                {
                    var perceptions = game.Move(direction).Perceptions;
                    currentField = currentField.Go(direction);

                    // Stop when the game ends
                    if (perceptions.OfType<Win>().Any() || perceptions.OfType<Death>().Any()) return;
                }

                {
                    var direction = path.Last();
                    var perceptions = game.Move(direction).Perceptions;

                    // Stop when the game ends
                    if (perceptions.OfType<Win>().Any() || perceptions.OfType<Death>().Any()) return;
                    

                    if (perceptions.OfType<Bump>().Any())
                    {
                        map[currentField.Go(direction)] = FieldData.Wall;
                        continue;
                    }

                    currentField = currentField.Go(direction);
                    map[currentField] = FieldData.Visited;

                    if (perceptions.OfType<Glitter>().Any())
                    {
                        perceptions = game.Pickup().Perceptions;

                        // Stop when the game ends
                        if (perceptions.OfType<Win>().Any() || perceptions.OfType<Death>().Any()) return;
                    }

                    // If there is wind or stench, flag unknown fields as dangerous, if there is none, flag all fields as safe
                    // This one is a pacifist, it will never use the arrow.
                    if (perceptions.OfType<Wind>().Any() || perceptions.OfType<Stench>().Any())
                    {
                        foreach (var field in currentField.AdjacentFields().Where(x => !map.ContainsKey(x)))
                        {
                            map[field] = FieldData.Dangerous;
                        }
                    }
                    else
                    {
                        foreach (var field in currentField.AdjacentFields().Where(x => (!map.ContainsKey(x)) || (map[x] != FieldData.Visited && map[x] != FieldData.Wall)))
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
