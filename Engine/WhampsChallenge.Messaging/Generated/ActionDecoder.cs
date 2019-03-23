// Generated file, so ReSharper-Checks are disabled. 
// ReSharper disable all

using System;
using System.Collections.Generic; 
using Newtonsoft.Json.Linq; 
using WhampsChallenge.Core.Common;

using WhampsChallenge.Messaging.Common;


namespace WhampsChallenge.Messaging.Level3.Actions {
	using WhampsChallenge.Core.Level3.Actions;

	public class ActionDecoder : IActionDecoder {
		private readonly Dictionary<string, Func<JObject, WhampsChallenge.Core.Common.IAction>> _decoders = 
			new Dictionary<string, Func<JObject, WhampsChallenge.Core.Common.IAction>> {
				{ "Move", DecodeMove },
				{ "Pickup", DecodePickup },
				{ "Shoot", DecodeShoot },
			};

		public WhampsChallenge.Core.Common.IAction Decode(JObject message)
	    {
	        return _decoders[message["Action"].Value<string>()](message);
	    }

		private static WhampsChallenge.Core.Common.IAction DecodeMove(JObject message) {
			return message.ToObject<Move>();
		}
		private static WhampsChallenge.Core.Common.IAction DecodePickup(JObject message) {
			return message.ToObject<Pickup>();
		}
		private static WhampsChallenge.Core.Common.IAction DecodeShoot(JObject message) {
			return message.ToObject<Shoot>();
		}

	}
}

namespace WhampsChallenge.Messaging.Level2.Actions {
	using WhampsChallenge.Core.Level2.Actions;

	public class ActionDecoder : IActionDecoder {
		private readonly Dictionary<string, Func<JObject, WhampsChallenge.Core.Common.IAction>> _decoders = 
			new Dictionary<string, Func<JObject, WhampsChallenge.Core.Common.IAction>> {
				{ "Move", DecodeMove },
				{ "Pickup", DecodePickup },
			};

		public WhampsChallenge.Core.Common.IAction Decode(JObject message)
	    {
	        return _decoders[message["Action"].Value<string>()](message);
	    }

		private static WhampsChallenge.Core.Common.IAction DecodeMove(JObject message) {
			return message.ToObject<Move>();
		}
		private static WhampsChallenge.Core.Common.IAction DecodePickup(JObject message) {
			return message.ToObject<Pickup>();
		}

	}
}

namespace WhampsChallenge.Messaging.Level1.Actions {
	using WhampsChallenge.Core.Level1.Actions;

	public class ActionDecoder : IActionDecoder {
		private readonly Dictionary<string, Func<JObject, WhampsChallenge.Core.Common.IAction>> _decoders = 
			new Dictionary<string, Func<JObject, WhampsChallenge.Core.Common.IAction>> {
				{ "Move", DecodeMove },
				{ "Pickup", DecodePickup },
			};

		public WhampsChallenge.Core.Common.IAction Decode(JObject message)
	    {
	        return _decoders[message["Action"].Value<string>()](message);
	    }

		private static WhampsChallenge.Core.Common.IAction DecodeMove(JObject message) {
			return message.ToObject<Move>();
		}
		private static WhampsChallenge.Core.Common.IAction DecodePickup(JObject message) {
			return message.ToObject<Pickup>();
		}

	}
}

