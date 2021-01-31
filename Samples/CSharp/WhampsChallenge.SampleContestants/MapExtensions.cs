using System;
using System.Collections.Generic;
using System.Linq;
using WhampsChallenge.Shared.Maps.FourDirections;

namespace WhampsChallenge.SampleContestants
{
    public static class MapExtensions
    {
        private static readonly Direction[] Directions =
            {Direction.North, Direction.East, Direction.South, Direction.West};

        public static IEnumerable<Direction> GetShortestPathToField<TFieldContent>(
            this IDictionary<(int,int), TFieldContent> map, (int,int) startCoordinate, Predicate<TFieldContent> selectionPredicate,
            Predicate<TFieldContent> abortionPredicate)
        {
            var q = new Queue<((int, int), Direction[])>(Directions.Select(x => (startCoordinate.Go(x), new[] {x})));
            var visited = new HashSet<(int, int)>();

            while (q.Any())
            {
                var (currentField, currentPath) = q.Dequeue();
                if (visited.Contains(currentField)) continue;

                visited.Add(currentField);

                if (!map.ContainsKey(currentField)) continue;
                if (selectionPredicate(map[currentField])) return currentPath;
                if (abortionPredicate(map[currentField])) continue;

                foreach (var direction in Directions)
                {
                    q.Enqueue((currentField.Go(direction), currentPath.Append(direction).ToArray()));
                }
            }

            // Could not find any Path to a field that matches the predicate
            return null;
        }

        public static IEnumerable<(int, int)> AdjacentFields(this (int, int) self)
        {
            return Directions.Select(x => self.Go(x));
        }

        public static (int, int) Go(this (int, int) self, Direction direction)
        {
            var (x, y) = self;

            switch (direction)
            {
                case Direction.North:
                    return (x, y - 1);
                case Direction.East:
                    return (x + 1, y);
                case Direction.South:
                    return (x, y + 1);
                case Direction.West:
                    return (x - 1, y);
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
        }

    }
}