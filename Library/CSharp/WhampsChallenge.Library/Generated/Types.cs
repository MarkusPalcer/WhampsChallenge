// Generated file, so ReSharper-Checks are disabled.
// ReSharper disable all

using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;


namespace WhampsChallenge.Library.Level3.Types {

	using WhampsChallenge.Library.Level3.Enums;

	public class GameState {
		public Boolean HasArrow { get; set; }
		public Int32 MovesLeft { get; set; }
	}

	public class Result {
		public Perception[] Perceptions { get; set; }
		public GameState GameState { get; set; }
	}

}
namespace WhampsChallenge.Library.Level2.Types {

	using WhampsChallenge.Library.Level2.Enums;

	public class GameState {
		public Int32 MovesLeft { get; set; }
	}

	public class Result {
		public Perception[] Perceptions { get; set; }
		public GameState GameState { get; set; }
	}

}
namespace WhampsChallenge.Library.Level1.Types {

	using WhampsChallenge.Library.Level1.Enums;

	public class GameState {
		public Int32 MovesLeft { get; set; }
	}

	public class Result {
		public Perception[] Perceptions { get; set; }
		public GameState GameState { get; set; }
	}

}
