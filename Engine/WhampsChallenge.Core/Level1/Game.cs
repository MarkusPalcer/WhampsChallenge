using System;
using System.Collections.Generic;
using System.Linq;
using WhampsChallenge.Core.Common;
using WhampsChallenge.Core.Common.Events;
using WhampsChallenge.Core.Level1.Events;
using WhampsChallenge.Core.Level1.Fields;
using WhampsChallenge.Core.Maps;
using WhampsChallenge.Shared.Extensions;
using GameState = WhampsChallenge.Core.Common.GameState;

namespace WhampsChallenge.Core.Level1
{
    public class Game : IGame
    {
        private Dictionary<Randomizers, Random> randomizers;
        private bool isStarted;
        internal GameState State = new();
        protected List<IEvent> Events = new();

        public int Seed
        {
            get => State.Seed;
            set
            {
                if (isStarted) throw new InvalidOperationException("Seed cannot be changed when game is already started.");
                State.Seed = value;
            }
        }

        public Game()
        {
            Seed = new Random().Next(int.MinValue, int.MaxValue);
        }

        public virtual void Initialize()
        {
            if (isStarted) throw new InvalidOperationException("Game can only be started once.");
            isStarted = true;

            var mainRandom = new Random(Seed);
            randomizers = Enum.GetValues(typeof(Randomizers))
                .OfType<Randomizers>()
                .ToDictionary(x => x, _ => mainRandom.GetNewChild());

            // Create 5x5 array of empty fields
            State.Map = new FixedSizeMap(5, 5, 1,_ => new Empty());

            // Put Gold somewhere
            State.Map[GetFreeSquare()].Content = new Gold();

            State.PlayerPosition = GetFreeSquare();
            State.MovesLeft = 100;
            GameCompletionState = GameCompletionStates.Running;
        }

        public int Score => State.MovesLeft;

        internal virtual void AddEvent(IEvent ev)
        {
            Events.Add(ev);
        }

        protected virtual void PostProcessAction()
        {
            State.MovesLeft--;
            
            if (GameCompletionState != GameCompletionStates.Running) return;

            if (State.MovesLeft < 1)
            {
                AddEvent(new Death());
                GameCompletionState = GameCompletionStates.Lose;
                return;
            }

            if (State.Map[State.PlayerPosition].Content is Gold) AddEvent(new Glitter());
        }

        protected virtual bool IsSquareFree(int x, int y)
        {
            // Check if chosen field is empty
            return State.Map[(x, y)].Content is Empty;
        }

        protected ValueTuple<int,int> GetFreeSquare()
        {
            while (true)
            {
                var x = randomizers[Randomizers.Level].Next(0, 4);
                var y = randomizers[Randomizers.Level].Next(0, 4);

                if (!IsSquareFree(x, y)) continue;

                return (x,y);
            }
        }
        
        public GameCompletionStates GameCompletionState { get; internal set; }

        public virtual ActionResult Execute(IAction action)
        {
            ((Level1.Actions.IAction) action).Execute(this);
            PostProcessAction();

            var result = new ActionResult()
            {
                GameState = State,
                Events = Events.ToArray()
            };

            Events = new List<IEvent>();

            return result;
        }
    }
}
