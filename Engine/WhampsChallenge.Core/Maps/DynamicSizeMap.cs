using System;
using System.Collections.Generic;

namespace WhampsChallenge.Core.Maps
{
    public class DynamicSizeMap<TFieldContent> : IMap<TFieldContent>
    {
        private readonly Func<int, int, TFieldContent> initialFieldContentFactory;

        private readonly Dictionary<(int,int), IField<TFieldContent>> data = new Dictionary<(int, int), IField<TFieldContent>>();

        public DynamicSizeMap(Func<int, int, TFieldContent> initialFieldContentFactory)
        {
            this.initialFieldContentFactory = initialFieldContentFactory;
        }

        public virtual IField<TFieldContent> this[int x, int y]
        {
            get
            {
                if (data.TryGetValue((x, y), out var field)) return field;

                field = new Field<TFieldContent>(x, y, this);
                data[(x, y)] = field;

                return field;
            }
        }

        public IField<TFieldContent> this[Coordinate pos] => this[pos.X, pos.Y];
    }
}