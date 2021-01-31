using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace WhampsChallenge.Core.Maps
{
    [DebuggerDisplay("{Position} = {Content}")]
    public class Field : IField
    {
        public int X => Position.X;

        public int Y => Position.Y;

        public int Z => Position.Z;

        public Coordinate Position { get; }

        private readonly Dictionary<Direction, Lazy<IField>> adjacents;

        private static readonly Direction[] Directions = Enum.GetValues(typeof(Direction)).OfType<Direction>().ToArray();

        internal Field(int x, int y, int z, IMap map)
        {
            Position = (x, y, z);

            adjacents = Directions.ToDictionary(
                d => d, 
                d => new Lazy<IField>(() => map[Position.Go(d)]));
        }

        public object Content { get; set; }

        public IField this[Direction direction] => adjacents[direction].Value;

        public IEnumerable<IField> AdjacentFields
        {
            get
            {
                return Directions.Select(x => this[x]).Where(x => x != null);
            }
        }
    }
}