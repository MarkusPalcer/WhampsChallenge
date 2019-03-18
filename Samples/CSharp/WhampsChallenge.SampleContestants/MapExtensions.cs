using System;
using System.Collections.Generic;
using System.Linq;
using WhampsChallenge.Shared.Maps;
using WhampsChallenge.Shared.Maps.FourDirections;

namespace WhampsChallenge.SampleContestants
{
    public static class MapExtensions
    {
        private static readonly Direction[] Directions = {Direction.North, Direction.East, Direction.South, Direction.West};

        public static IEnumerable<Direction> GetShortestPathToField<TFieldContent>(this IField<Direction, TFieldContent> self, Predicate<IField<Direction, TFieldContent>> selectionPredicate, Predicate<IField<Direction, TFieldContent>> abortionPredicate)
        {
            var q = new Queue<(IField<Direction, TFieldContent>, Direction[])>(Directions.Select(x => (self[x], new []{x})));
            var visited = new HashSet<(int, int)>();

            while (q.Any())
            {
                var (currentField, currentPath) = q.Dequeue();
                if (visited.Contains(currentField.Position)) continue;

                visited.Add(currentField.Position);

                if (selectionPredicate(currentField)) return currentPath;
                if (abortionPredicate(currentField)) continue;

                foreach (var direction in Directions)
                {
                    q.Enqueue((currentField[direction], currentPath.Append(direction).ToArray()));
                }
            }

            // Could not find any Path to a field that matches the predicate
            return null;
        }
    }
}