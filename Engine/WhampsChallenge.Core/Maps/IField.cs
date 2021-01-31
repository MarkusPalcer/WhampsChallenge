using System.Collections.Generic;

namespace WhampsChallenge.Core.Maps
{
    public interface IField
    {
        int X { get; }
        int Y { get; }

        Coordinate Position { get; }

        object Content { get; set; }

        IField this[Direction direction] { get; }

        IEnumerable<IField> AdjacentFields { get; }
    }
}