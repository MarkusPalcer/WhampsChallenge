using System;
using System.Collections.Generic;
using System.Linq;
using WhampsChallenge.Core.Common;
using WhampsChallenge.Core.Level3;
using WhampsChallenge.Shared.Extensions;

namespace WhampsChallenge.Core.Level1
{
    public class Game : IGame
    {
        private Dictionary<Randomizers, Random> randomizers;
        private bool isStarted;
        internal GameState State = new GameState();
        private List<Perception> perceptions = new List<Perception>();

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
            State.Map = new Map<FieldContent>(5, 5, (_,__) => FieldContent.Empty);

            // Put Gold somewhere
            State.Map[GetFreeSquare()].Content = FieldContent.Gold;

            State.PlayerPosition = GetFreeSquare();
            State.MovesLeft = 100;
            GameState = Common.GameState.Running;
        }

        public int Score => State.MovesLeft;

        internal virtual void AddPerception(Perception perception)
        {
            perceptions.Add(perception);
        }

        protected virtual void PostProcessAction()
        {
            State.MovesLeft--;
            
            if (GameState != Common.GameState.Running) return;

            if (State.MovesLeft < 1)
            {
                AddPerception(Perception.Death);
                GameState = Common.GameState.Lose;
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
                var x = randomizers[Randomizers.Level].Next(0, 4);
                var y = randomizers[Randomizers.Level].Next(0, 4);

                if (!IsSquareFree(x, y)) continue;

                return (x,y);
            }
        }
        
        public Common.GameState GameState { get; internal set; }

        public virtual object Execute(Common.IAction action)
        {
            ((Level1.Actions.IAction) action).Execute(this);
            PostProcessAction();

            var result = new Result()
            {
                GameState = State,
                Perceptions = perceptions.ToArray()
            };

            perceptions = new List<Perception>();

            return result;
        }
    }
}
