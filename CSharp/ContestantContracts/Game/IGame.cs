using ContestantContracts.Perceptions;

namespace ContestantContracts.Game
{
    public interface IGame
    {
        /// <summary>
        /// Gets a value indicating the number of moves left until game over
        /// </summary>
        int MovesLeft { get; }

        /// <summary>
        /// Gets a value indicting whether the player has an arrow
        /// </summary>
        bool HasArrow { get; }

        /// <summary>
        /// Gets the seed of the current game.
        /// </summary>
        int Seed { get;  }

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
}