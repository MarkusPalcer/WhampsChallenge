

namespace WhampsChallenge.Common
{
    public interface IGame 
    {
        /// <summary>
        /// Gets the seed of the current game.
        /// </summary>
        int Seed { get; }
    }
}