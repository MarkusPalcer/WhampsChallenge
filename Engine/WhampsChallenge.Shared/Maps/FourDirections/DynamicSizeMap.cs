﻿using System;
using System.Collections.Generic;

namespace WhampsChallenge.Shared.Maps.FourDirections
{
    public class DynamicSizeMap<TFieldContent> : IMap<Direction, TFieldContent>
    {
        private readonly Func<int, int, TFieldContent> initialFieldContentFactory;

        private readonly Dictionary<(int,int), IField<Direction, TFieldContent>> data = new Dictionary<(int, int), IField<Direction, TFieldContent>>();

        public DynamicSizeMap(Func<int, int, TFieldContent> initialFieldContentFactory)
        {
            this.initialFieldContentFactory = initialFieldContentFactory;
        }

        public virtual IField<Direction, TFieldContent> this[int x, int y]
        {
            get
            {
                if (data.TryGetValue((x, y), out var field)) return field;

                field = new Field<TFieldContent>(x, y, this);
                data[(x, y)] = field;

                return field;
            }
        }

        public IField<Direction, TFieldContent> this[(int, int) position] => this[position.Item1, position.Item2];
    }
}