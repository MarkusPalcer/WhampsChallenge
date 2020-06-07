using System;

namespace WhampsChallenge.Core.Maps
{
    public interface IMap<in TDirections,TFieldContent>
    {
        IField<TDirections, TFieldContent> this[int x, int y] { get; }

        IField<TDirections, TFieldContent> this[ValueTuple<int, int> position] { get; }
    }
}