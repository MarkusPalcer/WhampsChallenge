using System.Collections.Generic;
using System.Linq;
using WhampsChallenge.Common;

namespace WhampsChallenge.Level3
{
    public class Game : Level2.Game
    {
        private List<Perception> _perceptions = new List<Perception>();

        internal new GameState State = new GameState();
        
        private readonly Dictionary<Level2.Perception, Perception> _perceptionMappings = new Dictionary<Level2.Perception, Perception>
        {
            {Level2.Perception.Bump, Perception.Bump},
            {Level2.Perception.Death, Perception.Death},
            {Level2.Perception.Glitter, Perception.Glitter},
            {Level2.Perception.Win, Perception.Win}
        };

        public override void Initialize()
        {
            base.State = State;

            base.Initialize();

            // Add Monster to Level 2 styled map
            State.Map[GetFreeSquare()].Content = FieldContent.Whamps;
        }

        public override object Execute(IAction action)
        {
            ((Level3.Actions.IAction) action).Execute(this);
            PostProcessAction();

            var result = new Result()
            {
                GameState = State,
                Perceptions = _perceptions.ToArray()
            };

            _perceptions = new List<Perception>();

            return result;
        }

        protected override void PostProcessAction()
        {
            base.PostProcessAction();

            // Die when stepping on whamps
            if (State.Map[State.PlayerPosition].Content == FieldContent.Whamps)
            {
                AddPerception(Perception.Death);
                IsGameOver = true;
                return;
            }

            // Feel stench when adjacent to whamps
            foreach (var adjacentFieldContent in State.Map[State.PlayerPosition].AdjacentFields.Values.Select(x => x.Content))
            {
                if (adjacentFieldContent == FieldContent.Whamps) AddPerception(Perception.Stench);
            }
        }

        internal override void AddPerception(Level2.Perception perception)
        {
            AddPerception(_perceptionMappings[perception]);
        }

        public virtual void AddPerception(Perception perception)
        {
            _perceptions.Add(perception);
        }
    }
}
