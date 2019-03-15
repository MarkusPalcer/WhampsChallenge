using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using WhampsChallenge.Core.Level1;
using WhampsChallenge.Shared.Extensions;

namespace WhampsChallenge.Core
{
    internal class Map<T>
    {
        internal class Field
        {
            private readonly int x;
            private readonly int y;
            private readonly Map<T> map;

            private readonly Lazy<Field> north;
            private readonly Lazy<Field> east;
            private readonly Lazy<Field> south;
            private readonly Lazy<Field> west;

            internal Field(int x, int y, Map<T> map)
            {
                this.x = x;
                this.y = y;
                this.map = map;

                north = new Lazy<Field>(() => map[x, y - 1]);
                east = new Lazy<Field>(() => map[x + 1, y]);
                south = new Lazy<Field>(() => map[x, y + 1]);
                west = new Lazy<Field>(() => map[x - 1, y]);
            }

            public T Content
            {
                get => map.data[x][y];
                set => map.data[x][y] = value;
            }

            public Field North => north.Value;

            public Field East => east.Value;

            public Field South => south.Value;

            public Field West => west.Value;

            public IReadOnlyDictionary<Direction, Field> AdjacentFields => new AdjacentFieldsDictionary(this);

            private class AdjacentFieldsDictionary : IReadOnlyDictionary<Direction, Field>
            {
                private Field data;

                public AdjacentFieldsDictionary(Field data)
                {
                    this.data = data;
                }

                public IEnumerator<KeyValuePair<Direction, Field>> GetEnumerator()
                {
                    return Keys.Select(direction => new KeyValuePair<Direction, Field>(direction, this[direction])).GetEnumerator();
                }

                IEnumerator IEnumerable.GetEnumerator()
                {
                    return GetEnumerator();
                }

                public int Count => Keys.Count();

                public bool ContainsKey(Direction key)
                {
                    return this[key] != null;
                }

                public bool TryGetValue(Direction key, out Field value)
                {
                    value = this[key];
                    return value != null;
                }

                public Field this[Direction key]
                {
                    get
                    {
                        switch (key)
                        {
                            case Direction.North:
                                return data.North;
                            case Direction.East:
                                return data.East;
                            case Direction.South:
                                return data.South;
                            case Direction.West:
                                return data.West;
                            default:
                                throw new ArgumentOutOfRangeException(nameof(key), key, null);
                        }
                    }
                }

                public IEnumerable<Direction> Keys => Enum.GetValues(typeof(Direction)).OfType<Direction>().Where(direction => this[direction] != null);

                public IEnumerable<Field> Values => Keys.Select(direction => this[direction]);
            }
        }

        public int SizeX { get; }
        public int SizeY { get; }

        private readonly T[][] data;

        public Map(int sizeX, int sizeY, T initialFieldContent = default(T))
        {
            SizeX = sizeX;
            SizeY = sizeY;
            data = EnumerableExtensions.Repeat(() => Enumerable.Repeat(initialFieldContent, sizeY).ToArray(), sizeX).ToArray();
        }

        internal Field this[ValueTuple<int, int> position] => this[position.Item1, position.Item2];

        internal Field this[int x, int y]
        {
            get
            {
                if (x < 0 || x >= data.Length) return null;
                if (y < 0 || y >= data[0].Length) return null;

                return new Field(x, y, this);
            }
        }

    }
}