﻿using System;
using System.Collections.Generic;

namespace WhampsChallenge.Shared.Maps.FourDirections
{
    public class FixedSizeMap<TFieldContent> : DynamicSizeMap<TFieldContent>
    {
        public int SizeX { get; }

        public int SizeY { get; }

        private readonly Dictionary<(int,int), IField<Direction, TFieldContent>> data = new Dictionary<(int, int), IField<Direction, TFieldContent>>();

        public FixedSizeMap(int sizeX, int sizeY, Func<int,int,TFieldContent> initialFieldContentFactory) : base(initialFieldContentFactory)
        {
            SizeX = sizeX;
            SizeY = sizeY;
        }

        public override IField<Direction, TFieldContent> this[int x, int y]
        {
            get
            {
                if (x < 0 || x >= SizeX) return null;
                if (y < 0 || y >= SizeY) return null;

                return base[x, y];
            }
        }
    }
}