using System;
using System.Collections.Generic;

namespace WhampsChallenge.Core.Maps
{
    public class DynamicSizeMap<TFieldContent> : IMap<TFieldContent>
    {
        private readonly Func<Coordinate, TFieldContent> initialFieldContentFactory;

        private readonly Dictionary<Coordinate, IField<TFieldContent>> data = new Dictionary<Coordinate, IField<TFieldContent>>();

        public DynamicSizeMap(Func<Coordinate, TFieldContent> initialFieldContentFactory)
        {
            this.initialFieldContentFactory = initialFieldContentFactory;
        }

        public virtual IField<TFieldContent> this[Coordinate pos]
        {
            get
            {
                if (data.TryGetValue(pos, out var field)) return field;

                field = new Field<TFieldContent>(pos.X, pos.Y, pos.Z, this)
                {
                    Content = initialFieldContentFactory(pos)
                };
                
                data[pos] = field;

                return field;
            }
        }
    }
}
