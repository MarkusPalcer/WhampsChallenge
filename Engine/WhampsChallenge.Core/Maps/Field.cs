using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace WhampsChallenge.Core.Maps
{
    [DebuggerDisplay("{Position} = {Content}")]
    public class Field<TFieldContent> : IField<TFieldContent>
    {
        public int X => Position.X;

        public int Y => Position.Y;

        public Coordinate Position { get; }

        private readonly Dictionary<Direction, Lazy<IField<TFieldContent>>> adjacents;
        private static readonly Direction[] Directions = {Direction.North, Direction.East, Direction.South, Direction.West};

        internal Field(int x, int y, IMap<TFieldContent> map)
        {
            Position = (x, y);

            adjacents = Directions.ToDictionary(
                d => d, 
                d => new Lazy<IField<TFieldContent>>(() => map[Position.Go(d)]));
        }

        public TFieldContent Content { get; set; }

        public IField<TFieldContent> this[Direction direction] => adjacents[direction].Value;

        public IEnumerable<IField<TFieldContent>> AdjacentFields
        {
            get
            {
                return Directions.Select(x => this[x]).Where(x => x != null);
            }
        }
    }
}