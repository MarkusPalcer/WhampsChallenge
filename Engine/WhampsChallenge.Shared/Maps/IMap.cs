using System;
using WhampsChallenge.Shared.Maps.FourDirections;

namespace WhampsChallenge.Shared.Maps
{
    public interface IMap<in TDirections,TFieldContent>
    {
        IField<TDirections, TFieldContent> this[int x, int y] { get; }

        IField<TDirections, TFieldContent> this[ValueTuple<int, int> position] { get; }
    }
}