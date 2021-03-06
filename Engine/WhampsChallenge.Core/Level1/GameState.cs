﻿using Newtonsoft.Json;
using WhampsChallenge.Core.Level3;
using WhampsChallenge.Shared.Maps.FourDirections;

namespace WhampsChallenge.Core.Level1
{
    public class GameState
    {
        /// <summary>
        /// Gets a value indicating the number of moves left until game over
        /// </summary>
        public int MovesLeft { get; set; }

        [JsonIgnore]
        internal FixedSizeMap<FieldContent> Map { get; set; }

        [JsonIgnore]
        internal (int, int) PlayerPosition { get; set; }

        [JsonIgnore]
        internal int Seed { get; set; }
    }
}