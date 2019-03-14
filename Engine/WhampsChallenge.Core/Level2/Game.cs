using System;
using System.Collections.Generic;
using System.Linq;
using WhampsChallenge.Common;
using WhampsChallenge.Level3;

namespace WhampsChallenge.Level2
{
    public class Game : Level1.Game
    {
        private List<Perception> _perceptions = new List<Perception>();
        
        private readonly Dictionary<Level1.Perception, Perception> _perceptionMappings = new Dictionary<Level1.Perception, Perception>
        {
            {Level1.Perception.Bump, Perception.Bump},
            {Level1.Perception.Death, Perception.Death},
            {Level1.Perception.Glitter, Perception.Glitter},
            {Level1.Perception.Win, Perception.Win}
        };

        public override object Execute(IAction action)
        {
            ((Level2.Actions.IAction) action).Execute(this);
            PostProcessAction();

            var result = new Result()
            {
                GameState = State,
                Perceptions = _perceptions.ToArray()
            };

            _perceptions = new List<Perception>();

            return result;
        }

        public override void Initialize()
        {
            // Create Level 1 style map
            base.Initialize();
            
            // Add 2 Traps to Level 1 style map
            State.Map[GetFreeSquare()].Content = FieldContent.Trap;
            State.Map[GetFreeSquare()].Content = FieldContent.Trap;
        }

        protected override void PostProcessAction()
        {
            base.PostProcessAction();

            // Die when stepping on a trap
            if (State.Map[State.PlayerPosition].Content == FieldContent.Trap)
            {
                AddPerception(Perception.Death);
                IsGameOver = true;
                return;
            }

            // Feel wind when adjacent to a trap
            foreach (var adjacentFieldContent in State.Map[State.PlayerPosition].AdjacentFields.Values.Select(x => x.Content))
            {
                if (adjacentFieldContent == FieldContent.Trap) AddPerception(Perception.Wind);
            }
        }

        internal override void AddPerception(Level1.Perception perception)
        {
            // Map Level 1 Perceptions to Level 2 Perceptions
            AddPerception(_perceptionMappings[perception]);
        }

        internal virtual void AddPerception(Perception perception)
        {
            _perceptions.Add(perception);
        }

        protected override bool IsSquareFree(int x, int y)
        {
            // Check that there is no wind adjacent to this field 
            // This means that all fields adjacend to the adjacent fields must be trap-free
            return base.IsSquareFree(x, y) && State.Map[x, y].AdjacentFields.Values.SelectMany(f => f.AdjacentFields.Values).All(f => f.Content != FieldContent.Trap);
        }
    }
}
