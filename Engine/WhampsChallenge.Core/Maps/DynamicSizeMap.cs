using System;
using System.Collections.Generic;

namespace WhampsChallenge.Core.Maps
{
    public class DynamicSizeMap : IMap
    {
        private readonly Func<Coordinate, object> initialFieldContentFactory;

        private readonly Dictionary<Coordinate, IField> data = new Dictionary<Coordinate, IField>();

        public DynamicSizeMap(Func<Coordinate, object> initialFieldContentFactory)
        {
            this.initialFieldContentFactory = initialFieldContentFactory;
        }

        public virtual IField this[Coordinate pos]
        {
            get
            {
                if (data.TryGetValue(pos, out var field)) return field;

                field = new Field(pos.X, pos.Y, pos.Z, this)
                {
                    Content = initialFieldContentFactory(pos)
                };
                
                data[pos] = field;

                return field;
            }
        }
    }
}
