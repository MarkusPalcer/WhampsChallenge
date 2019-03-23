using System;

namespace WhampsChallenge.Shared.Extensions
{
    public static class RandomExtensions
    {
        public static Random GetNewChild(this Random parent)
        {
            return new Random(parent.Next(Int32.MinValue, Int32.MaxValue));
        }
    }
}