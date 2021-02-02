namespace WhampsChallenge.Core.Maps
{
using System;

    public readonly struct Coordinate
    {
        public readonly int X;
        public readonly int Y;
        public readonly int Z;


        public Coordinate(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static implicit operator Coordinate((int, int, int) other)
        {
            return new Coordinate(other.Item1, other.Item2, other.Item3);
        }

        public static implicit operator Coordinate((int, int) other)
        {
            return (other.Item1, other.Item2, 0);
        }

        public void Deconstruct(out int x, out int y, out int z)
        {
            x = X;
            y = Y;
            z = Z;
        }

        public void Deconstruct(out int x, out int y)
        {
            x = X;
            y = Y;
        }

        public Coordinate Go(Direction direction)
        {
            switch (direction)
            {
                case Direction.North:
                    return (X, Y - 1, Z);
                case Direction.East:
                    return (X + 1, Y, Z);
                case Direction.South:
                    return (X, Y + 1, Z);
                case Direction.West:
                    return (X - 1, Y, Z);
                case Direction.NorthEast:
                    return Go(Direction.North).Go(Direction.East);
                case Direction.SouthEast:
                    return Go(Direction.South).Go(Direction.East);
                case Direction.NorthWest:
                    return Go(Direction.North).Go(Direction.West);
                case Direction.SouthWest:
                    return Go(Direction.South).Go(Direction.West);
                case Direction.Up:
                    return (X, Y, Z - 1);
                case Direction.Down:
                    return (X, Y, Z + 1);
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
        }

        public bool Equals(Coordinate other)
        {
            var (x, y, z) = other;
            return X == x && Y == y && Z == z;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (!(obj is Coordinate c)) return false;
            return Equals(c);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = X;
                hashCode = (hashCode * 397) ^ Y;
                hashCode = (hashCode * 397) ^ Z;
                return hashCode;
            }
        }
    }
}
