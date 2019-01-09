

namespace WhampsChallenge.Contracts
{
    public interface IActions
    {
        /// <summary>
        /// Moves one square into the desired direction
        /// </summary>
        /// <returns>An array containing all perceptions following the action</returns>
        Perception[] Move(Direction direction);

        /// <summary>
        /// Shoots an arrow into the desired direction
        /// </summary>
        /// <returns>An array containing all perceptions following the action</returns>
        Perception[] Shoot(Direction direction);

        /// <summary>
        /// Picks up whatever lies on the current space
        /// </summary>
        /// <returns>An array containing all perceptions following the action</returns>
        Perception[] Pickup();
    }

    public interface IGame : IActions, IGameState
    {
        /// <summary>
        /// Gets the seed of the current game.
        /// </summary>
        int Seed { get; }
    }
}