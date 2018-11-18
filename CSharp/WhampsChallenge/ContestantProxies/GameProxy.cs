using System;
using System.Threading.Tasks;
using ContestantContracts.Game;
using ContestantContracts.Perceptions;
using WhampsChallenge.Exceptions;

namespace WhampsChallenge.ContestantProxies
{
    /// <summary>
    /// Hides the actual <see cref="Game"/> object from the contestant by proxying all calls onto it.
    /// This object also does the Thread synchronization between contestant and engine.
    /// Most of its code will be executed on the contestants thread. 
    /// </summary>
    public class GameProxy : IGame
    {
        private TaskCompletionSource<Func<IGame, Perception[]>> _contestantTaskSource;
        private TaskCompletionSource<ValueTuple<Perception[], IGame>> _engineTaskSource;

        #region Game loop
        /// <summary>
        /// Proxies an action to the engine.
        /// </summary>
        internal Perception[] ProxyAction(Func<IGame, Perception[]> action)
        {
            if (_engineTaskSource != null) throw new InternalException("Contestant was running while engine was running.");
            if (_contestantTaskSource == null) throw new InternalException("Internal error: Contestant was not meant to run, but executes action on engine.");

            _engineTaskSource = new TaskCompletionSource<(Perception[], IGame)>();

            var contestantTask = _contestantTaskSource;
            _contestantTaskSource = null;

            contestantTask.SetResult(action);
            _engineTaskSource.Task.Wait();

            UpdateProxyProperties(_engineTaskSource.Task.Result.Item2);
            
            return _engineTaskSource.Task.Result.Item1;
        }

        /// <summary>
        /// Reports the result of an action and returns the task that waits for the next action
        /// </summary>
        /// <remarks>
        /// Runs on the engine thread
        /// </remarks>
        internal Task<Func<IGame, Perception[]>> ReportResult(Perception[] result, IGame game)
        {
            if (_contestantTaskSource != null) throw new InternalException("Contestant was running while engine was running.");
            if (_engineTaskSource == null) throw new InternalException("Internal error: Engine was not meant to run, but executes action.");

            _contestantTaskSource = new TaskCompletionSource<Func<IGame, Perception[]>>();

            var engineTask = _engineTaskSource;
            _engineTaskSource = null;

            engineTask.SetResult((result, game));
            return _contestantTaskSource.Task;
        }

        /// <summary>
        /// Updates the properties of this proxy object and all sub-proxies
        /// </summary>
        private void UpdateProxyProperties(IGame game)
        {
            MovesLeft = game.MovesLeft;
            HasArrow = game.HasArrow;
            Seed = game.Seed;
        }
        #endregion

        #region IGame
        public int MovesLeft { get; private set; }
        public bool HasArrow { get; private set; }
        public int Seed { get; private set; }

        public Perception[] Move(Direction direction)
        {
            return ProxyAction(g => g.Move(direction));
        }

        public Perception[] Shoot(Direction direction)
        {
            return ProxyAction(g => g.Shoot(direction));
        }

        public Perception[] Pickup()
        {
            return ProxyAction(g => g.Pickup());
        }
        #endregion
    }
}