// This file contains all enums as declared by the contracts.json file

// Generated file, so ReSharper-Checks are disabled.
// ReSharper disable all

using System;


namespace WhampsChallenge.Messaging.Level3 {
	public class GameState {
		Boolean HasArrow { get; set; }
		Int32 MovesLeft { get; set; }
	}

	public class Result {
		Perception[] Perceptions { get; set; }
		GameState GameState { get; set; }
	}

}
namespace WhampsChallenge.Messaging.Level2 {
	public class GameState {
		Int32 MovesLeft { get; set; }
	}

	public class Result {
		Perception[] Perceptions { get; set; }
		GameState GameState { get; set; }
	}

}
namespace WhampsChallenge.Messaging.Level1 {
	public class GameState {
		Int32 MovesLeft { get; set; }
	}

	public class Result {
		Perception[] Perceptions { get; set; }
		GameState GameState { get; set; }
	}

}
