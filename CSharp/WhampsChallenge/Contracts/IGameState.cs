using Newtonsoft.Json;

namespace WhampsChallenge.Contracts
{
    public struct GameState
    {
        /// <summary>
        /// Gets a value indicating the number of moves left until game over
        /// </summary>
        public int MovesLeft { get; set; }

        /// <summary>
        /// Gets a value indicting whether the player has an arrow
        /// </summary>
        public bool HasArrow { get; set; }

        [JsonIgnore]
        internal Map<FieldContent> Map { get; set; }

        [JsonIgnore]
        internal (int, int) PlayerPosition { get; set; }

        [JsonIgnore]
        internal int Seed { get; set; }
    }
}