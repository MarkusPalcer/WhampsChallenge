// Generated file, so ReSharper-Checks are disabled.
// ReSharper disable all

using System;
using System.Collections.Generic;
using WhampsChallenge.Core.Common;
using WhampsChallenge.Messaging.Common;


namespace WhampsChallenge.Runner.Shared {
	

	public static class LevelTypes {
		public enum Levels {
			Level3,
			Level2,
			Level1,
		}

		public static readonly IReadOnlyDictionary<Levels, Type> GameEngines;
		public static readonly IReadOnlyDictionary<Levels, Type> ActionDecoders;

		static LevelTypes() {
			GameEngines = new Dictionary<Levels, Type> {
				{ Levels.Level3, typeof(WhampsChallenge.Core.Level3.Game) },
				{ Levels.Level2, typeof(WhampsChallenge.Core.Level2.Game) },
				{ Levels.Level1, typeof(WhampsChallenge.Core.Level1.Game) },
			};

			ActionDecoders = new Dictionary<Levels, Type> {
				{ Levels.Level3, typeof(WhampsChallenge.Messaging.Level3.Actions.ActionDecoder) },
				{ Levels.Level2, typeof(WhampsChallenge.Messaging.Level2.Actions.ActionDecoder) },
				{ Levels.Level1, typeof(WhampsChallenge.Messaging.Level1.Actions.ActionDecoder) },
			};
		}
	}
}