using System;
using System.Collections.Generic;

namespace WhampsChallenge.Core.Maps
{
    public class FixedSizeMap : DynamicSizeMap
    {
        public int SizeX { get; }

        public int SizeY { get; }

        public int SizeZ { get; }

        public FixedSizeMap(int sizeX, int sizeY, int sizeZ, Func<Coordinate, object> initialFieldContentFactory) : base(initialFieldContentFactory)
        {
            SizeX = sizeX;
            SizeY = sizeY;
            SizeZ = sizeZ;
        }

        public override IField this[Coordinate pos]
        {
            get
            {
                var (x, y, z) = pos;

                if (x < 0 || x >= SizeX) return null;
                if (y < 0 || y >= SizeY) return null;
                if (z < 0 || z >= SizeZ) return null;

                return base[pos];
            }
        }
    }
}