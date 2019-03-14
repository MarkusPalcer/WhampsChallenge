using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using WhampsChallenge.Level1;
using WhampsChallenge.Shared.Extensions;

namespace WhampsChallenge
{
    internal class Map<T>
    {
        internal class Field
        {
            private readonly int _x;
            private readonly int _y;
            private readonly Map<T> _map;

            private readonly Lazy<Field> _north;
            private readonly Lazy<Field> _east;
            private readonly Lazy<Field> _south;
            private readonly Lazy<Field> _west;

            internal Field(int x, int y, Map<T> map)
            {
                _x = x;
                _y = y;
                _map = map;

                _north = new Lazy<Field>(() => map[x, y - 1]);
                _east = new Lazy<Field>(() => map[x + 1, y]);
                _south = new Lazy<Field>(() => map[x, y + 1]);
                _west = new Lazy<Field>(() => map[x - 1, y]);
            }

            public T Content
            {
                get => _map._data[_x][_y];
                set => _map._data[_x][_y] = value;
            }

            public Field North => _north.Value;

            public Field East => _east.Value;

            public Field South => _south.Value;

            public Field West => _west.Value;

            public IReadOnlyDictionary<Direction, Field> AdjacentFields => new AdjacentFieldsDictionary(this);

            private class AdjacentFieldsDictionary : IReadOnlyDictionary<Direction, Field>
            {
                private Field _data;

                public AdjacentFieldsDictionary(Field data)
                {
                    _data = data;
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
                                return _data.North;
                            case Direction.East:
                                return _data.East;
                            case Direction.South:
                                return _data.South;
                            case Direction.West:
                                return _data.West;
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

        private readonly T[][] _data;

        public Map(int sizeX, int sizeY, T initialFieldContent = default(T))
        {
            SizeX = sizeX;
            SizeY = sizeY;
            _data = EnumerableExtensions.Repeat(() => Enumerable.Repeat(initialFieldContent, sizeY).ToArray(), sizeX).ToArray();
        }

        internal Field this[ValueTuple<int, int> position] => this[position.Item1, position.Item2];

        internal Field this[int x, int y]
        {
            get
            {
                if (x < 0 || x >= _data.Length) return null;
                if (y < 0 || y >= _data[0].Length) return null;

                return new Field(x, y, this);
            }
        }

    }
}