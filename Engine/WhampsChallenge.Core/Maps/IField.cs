using System.Collections.Generic;

namespace WhampsChallenge.Core.Maps
{
    public interface IField<TFieldContent>
    {
        int X { get; }
        int Y { get; }

        Coordinate Position { get; }

        TFieldContent Content { get; set; }

        IField<TFieldContent> this[Direction direction] { get; }

        IEnumerable<IField<TFieldContent>> AdjacentFields { get; }
    }
}