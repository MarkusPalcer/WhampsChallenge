using System.Collections.Generic;

namespace WhampsChallenge.Shared.Maps
{
    public interface IField<in TDirection, TFieldContent>
    {
        int X { get; }
        int Y { get; }

        (int X, int Y) Position { get; }

        TFieldContent Content { get; set; }

        IField<TDirection, TFieldContent> this[TDirection direction] { get; }

        IEnumerable<IField<TDirection, TFieldContent>> AdjacentFields { get; }
    }
}