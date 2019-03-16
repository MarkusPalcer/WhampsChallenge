using System;
using System.Collections.Generic;
using WhampsChallenge.Core.Level1;
using WhampsChallenge.Shared.Extensions;

namespace WhampsChallenge.Core
{
    internal class Map<T>
    {
        private readonly Func<int, int, T> initialFieldContentFactory;

        internal class Field
        {
            public int X { get; }
            public int Y { get; }

            private readonly Lazy<Field> north;
            private readonly Lazy<Field> east;
            private readonly Lazy<Field> south;
            private readonly Lazy<Field> west;

            internal Field(int x, int y, Map<T> map)
            {
                X = x;
                Y = y;

                north = new Lazy<Field>(() => map[x, y - 1]);
                east = new Lazy<Field>(() => map[x + 1, y]);
                south = new Lazy<Field>(() => map[x, y + 1]);
                west = new Lazy<Field>(() => map[x - 1, y]);
            }

            public T Content { get; set; }

            public Field North => north.Value;

            public Field East => east.Value;

            public Field South => south.Value;

            public Field West => west.Value;

            public Field this[Direction direction]
            {
                get
                {
                    switch (direction)
                    {
                        case Direction.North:
                            return North;
                        case Direction.East:
                            return East;
                        case Direction.South:
                            return South;
                        case Direction.West:
                            return West;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
                    }
                }
            }

            public IEnumerable<Field> AdjacentFields
            {
                get
                {
                    if (North != null) yield return North;
                    if (East != null) yield return East;
                    if (South != null) yield return South;
                    if (West != null) yield return West;
                }
            }
        }

        public int SizeX { get; }
        public int SizeY { get; }

        private readonly Dictionary<int, Dictionary<int, Field>> data = new Dictionary<int, Dictionary<int, Field>>();

        public Map(int sizeX, int sizeY, Func<int,int,T> initialFieldContentFactory)
        {
            this.initialFieldContentFactory = initialFieldContentFactory;
            SizeX = sizeX;
            SizeY = sizeY;
        }

        internal Field this[ValueTuple<int, int> position] => this[position.Item1, position.Item2];

        internal Field this[int x, int y]
        {
            get
            {
                if (x < 0 || x >= SizeX) return null;
                if (y < 0 || y >= SizeY) return null;

                if (!data.TryGetValue(x, out var row))
                {
                    row = new Dictionary<int, Field>();
                    data[x] = row;
                }

                if (!row.TryGetValue(y, out var field))
                {
                    field = new Field(x, y, this) {Content = initialFieldContentFactory(x, y)};
                    row[y] = field;
                }

                return field;
            }
        }

    }
}