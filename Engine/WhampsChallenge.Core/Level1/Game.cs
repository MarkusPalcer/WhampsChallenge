using System;
using System.Collections.Generic;
using System.Linq;
using WhampsChallenge.Common;
using WhampsChallenge.Level3;
using WhampsChallenge.Shared.Extensions;

namespace WhampsChallenge.Level1
{
    public class Game : IGame
    {
        private Dictionary<Randomizers, Random> _randomizers;
        private bool _isStarted;
        internal GameState State = new GameState();
        private List<Perception> _perceptions = new List<Perception>();

        public bool IsGameOver { get; internal set; }

        public int Seed
        {
            get => State.Seed;
            set
            {
                if (_isStarted) throw new InvalidOperationException("Seed cannot be changed when game is already started.");
                State.Seed = value;
            }
        }

        public Game()
        {
            Seed = new Random().Next(int.MinValue, int.MaxValue);
        }

        public virtual void Initialize()
        {
            if (_isStarted) throw new InvalidOperationException("Game can only be started once.");
            _isStarted = true;

            var mainRandom = new Random(Seed);
            _randomizers = Enum.GetValues(typeof(Randomizers))
                .OfType<Randomizers>()
                .ToDictionary(x => x, _ => mainRandom.GetNewChild());

            // Create 5x5 array of empty fields
            State.Map = new Map<FieldContent>(5, 5);

            // Put Gold somewhere
            State.Map[GetFreeSquare()].Content = FieldContent.Gold;

            State.PlayerPosition = GetFreeSquare();
            State.MovesLeft = 100;
            IsGameOver = false;
        }

        internal virtual void AddPerception(Perception perception)
        {
            _perceptions.Add(perception);
        }

        protected virtual void PostProcessAction()
        {
            State.MovesLeft--;
            
            if (IsGameOver) return;

            if (State.MovesLeft < 1)
            {
                AddPerception(Perception.Death);
                IsGameOver = true;
                return;
            }

            if (State.Map[State.PlayerPosition].Content == FieldContent.Gold) AddPerception(Perception.Glitter);
        }

        protected virtual bool IsSquareFree(int x, int y)
        {
            // Check if chosen field is empty
            return State.Map[x, y].Content == FieldContent.Empty;
        }

        protected ValueTuple<int,int> GetFreeSquare()
        {
            while (true)
            {
                var x = _randomizers[Randomizers.Level].Next(0, 4);
                var y = _randomizers[Randomizers.Level].Next(0, 4);

                if (!IsSquareFree(x, y)) continue;

                return (x,y);
            }
        }

        public virtual object Execute(Common.IAction action)
        {
            ((Level1.Actions.IAction) action).Execute(this);
            PostProcessAction();

            var result = new Result()
            {
                GameState = State,
                Perceptions = _perceptions.ToArray()
            };

            _perceptions = new List<Perception>();

            return result;
        }
    }
}
