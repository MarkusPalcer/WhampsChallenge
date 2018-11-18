using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ContestantContracts.Game;

namespace WhampsChallenge
{
    internal class Map<T>
    {
        internal class Field
        {
            private readonly int _x;
            private readonly int _y;
            private readonly Map<T> _map;

            internal Field(int x, int y, Map<T> map)
            {
                _x = x;
                _y = y;
                _map = map;

                North = map[x, y - 1];
                East = map[x + 1, y];
                South = map[x, y + 1];
                West = map[x - 1, y];
            }

            public T Value
            {
                get => _map._data[_x][_y];
                set => _map._data[_x][_y] = value;
            }

            public Field North { get; }

            public Field East { get; }

            public Field South { get; }

            public Field West { get; }

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