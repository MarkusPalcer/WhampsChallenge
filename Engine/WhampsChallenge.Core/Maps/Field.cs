using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace WhampsChallenge.Core.Maps
{
    [DebuggerDisplay("{Position} = {Content}")]
    public class Field<TFieldContent> : IField<TFieldContent>
    {
        public int X { get; }
        
        public int Y { get; }

        public (int X, int Y) Position => (X, Y);

        private readonly Lazy<IField<TFieldContent>> north;
        private readonly Lazy<IField<TFieldContent>> east;
        private readonly Lazy<IField<TFieldContent>> south;
        private readonly Lazy<IField<TFieldContent>> west;

        internal Field(int x, int y, IMap<TFieldContent> map)
        {
            X = x;
            Y = y;

            north = new Lazy<IField<TFieldContent>>(() => map[x, y - 1]);
            east = new Lazy<IField<TFieldContent>>(() => map[x + 1, y]);
            south = new Lazy<IField<TFieldContent>>(() => map[x, y + 1]);
            west = new Lazy<IField<TFieldContent>>(() => map[x - 1, y]);
        }

        public TFieldContent Content { get; set; }

        public IField<TFieldContent> this[Direction direction]
        {
            get
            {
                switch (direction)
                {
                    case Direction.North:
                        return north.Value;
                    case Direction.East:
                        return east.Value;
                    case Direction.South:
                        return south.Value;
                    case Direction.West:
                        return west.Value;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
                }
            }
        }

        public IEnumerable<IField<TFieldContent>> AdjacentFields
        {
            get
            {
                if (north.Value != null) yield return north.Value;
                if (east.Value != null) yield return east.Value;
                if (south.Value != null) yield return south.Value;
                if (west.Value != null) yield return west.Value;
            }
        }
    }
}