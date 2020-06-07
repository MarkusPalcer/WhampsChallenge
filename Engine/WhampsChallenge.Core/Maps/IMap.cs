using System;

namespace WhampsChallenge.Core.Maps
{
    public interface IMap<TFieldContent>
    {
        IField<TFieldContent> this[int x, int y] { get; }

        IField<TFieldContent> this[ValueTuple<int, int> position] { get; }
    }
}