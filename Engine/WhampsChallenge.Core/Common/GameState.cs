using Newtonsoft.Json;
using WhampsChallenge.Core.Maps;
using WhampsChallenge.Core.Markers;

namespace WhampsChallenge.Core.Common
{
    [Shared]
    public class GameState
    {
        /// <summary>
        /// Gets a value indicating the number of moves left until game over
        /// </summary>
        public int MovesLeft { get; set; }

        [JsonIgnore] internal FixedSizeMap Map { get; set; }

        [JsonIgnore] internal (int, int) PlayerPosition { get; set; }

        [JsonIgnore] internal int Seed { get; set; }

        /// <summary>
        /// Gets a value indicting whether the player has an arrow
        /// </summary>
        public bool HasArrow { get; set; }
    }
}
