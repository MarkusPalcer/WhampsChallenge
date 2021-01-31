namespace WhampsChallenge.Core.Maps
{
    public interface IMap
    {
        IField this[Coordinate pos] { get; }
    }
}