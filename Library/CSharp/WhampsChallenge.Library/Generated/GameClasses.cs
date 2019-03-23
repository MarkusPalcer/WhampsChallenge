// Generated file, so ReSharper-Checks are disabled. 
// ReSharper disable all


using System.Threading.Tasks;

using WhampsChallenge.Shared.Communication;
using WhampsChallenge.Shared.Extensions;

namespace WhampsChallenge.Library.Level3 {

	using WhampsChallenge.Library.Level3.Enums;
	using WhampsChallenge.Library.Level3.Types;	
	using WhampsChallenge.Library.Level3.Actions;	

	public class Game {
		private readonly ICommunicator _communicator;

	    public Game(ICommunicator communicator)
	    {
	        _communicator = communicator;
	    }

		public Result Move(WhampsChallenge.Shared.Maps.FourDirections.Direction direction) {
			var message = new Move {
				Direction = direction,
			};

			return _communicator.SendAndReceive<Result>(message);
		}

		public async Task<Result> MoveAsync(WhampsChallenge.Shared.Maps.FourDirections.Direction direction) {
			var message = new Move {
				Direction = direction,
			};

			return await _communicator.SendAndReceiveAsync<Result>(message);
		}

		public Result Pickup() {
			var message = new Pickup {
			};

			return _communicator.SendAndReceive<Result>(message);
		}

		public async Task<Result> PickupAsync() {
			var message = new Pickup {
			};

			return await _communicator.SendAndReceiveAsync<Result>(message);
		}

		public Result Shoot(WhampsChallenge.Shared.Maps.FourDirections.Direction direction) {
			var message = new Shoot {
				Direction = direction,
			};

			return _communicator.SendAndReceive<Result>(message);
		}

		public async Task<Result> ShootAsync(WhampsChallenge.Shared.Maps.FourDirections.Direction direction) {
			var message = new Shoot {
				Direction = direction,
			};

			return await _communicator.SendAndReceiveAsync<Result>(message);
		}

	}
}

namespace WhampsChallenge.Library.Level2 {

	using WhampsChallenge.Library.Level2.Enums;
	using WhampsChallenge.Library.Level2.Types;	
	using WhampsChallenge.Library.Level2.Actions;	

	public class Game {
		private readonly ICommunicator _communicator;

	    public Game(ICommunicator communicator)
	    {
	        _communicator = communicator;
	    }

		public Result Move(WhampsChallenge.Shared.Maps.FourDirections.Direction direction) {
			var message = new Move {
				Direction = direction,
			};

			return _communicator.SendAndReceive<Result>(message);
		}

		public async Task<Result> MoveAsync(WhampsChallenge.Shared.Maps.FourDirections.Direction direction) {
			var message = new Move {
				Direction = direction,
			};

			return await _communicator.SendAndReceiveAsync<Result>(message);
		}

		public Result Pickup() {
			var message = new Pickup {
			};

			return _communicator.SendAndReceive<Result>(message);
		}

		public async Task<Result> PickupAsync() {
			var message = new Pickup {
			};

			return await _communicator.SendAndReceiveAsync<Result>(message);
		}

	}
}

namespace WhampsChallenge.Library.Level1 {

	using WhampsChallenge.Library.Level1.Enums;
	using WhampsChallenge.Library.Level1.Types;	
	using WhampsChallenge.Library.Level1.Actions;	

	public class Game {
		private readonly ICommunicator _communicator;

	    public Game(ICommunicator communicator)
	    {
	        _communicator = communicator;
	    }

		public Result Move(WhampsChallenge.Shared.Maps.FourDirections.Direction direction) {
			var message = new Move {
				Direction = direction,
			};

			return _communicator.SendAndReceive<Result>(message);
		}

		public async Task<Result> MoveAsync(WhampsChallenge.Shared.Maps.FourDirections.Direction direction) {
			var message = new Move {
				Direction = direction,
			};

			return await _communicator.SendAndReceiveAsync<Result>(message);
		}

		public Result Pickup() {
			var message = new Pickup {
			};

			return _communicator.SendAndReceive<Result>(message);
		}

		public async Task<Result> PickupAsync() {
			var message = new Pickup {
			};

			return await _communicator.SendAndReceiveAsync<Result>(message);
		}

	}
}

