using System;

namespace WhampsChallenge.Core.Maps
{
    public interface IMap<TFieldContent>
    {
        IField<TFieldContent> this[Coordinate pos] { get; }
    }
}