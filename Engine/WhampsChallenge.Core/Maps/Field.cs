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

        public int Z => Position.Z;

        public Coordinate Position { get; }

        private readonly Dictionary<Direction, Lazy<IField<TFieldContent>>> adjacents;

        private static readonly Direction[] Directions = Enum.GetValues(typeof(Direction)).OfType<Direction>().ToArray();

        internal Field(int x, int y, int z, IMap<TFieldContent> map)
        {
            Position = (x, y, z);

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