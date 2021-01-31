using System.Collections.Generic;
using System.Linq;
using WhampsChallenge.Core.Common;
using WhampsChallenge.Core.Level3;
using GameState = WhampsChallenge.Core.Common.GameState;

namespace WhampsChallenge.Core.Level2
{
    public class Game : Level1.Game
    {
        private List<Perception> perceptions = new List<Perception>();
        
        private readonly Dictionary<Level1.Perception, Perception> perceptionMappings = new Dictionary<Level1.Perception, Perception>
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
                Perceptions = perceptions.ToArray()
            };

            perceptions = new List<Perception>();

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
                GameState = GameState.Lose;
                return;
            }

            // Feel wind when adjacent to a trap
            foreach (var adjacentFieldContent in State.Map[State.PlayerPosition].AdjacentFields.Select(x => x.Content))
            {
                if (adjacentFieldContent == FieldContent.Trap) AddPerception(Perception.Wind);
            }
        }

        internal override void AddPerception(Level1.Perception perception)
        {
            // Map Level 1 Perceptions to Level 2 Perceptions
            AddPerception(perceptionMappings[perception]);
        }

        internal virtual void AddPerception(Perception perception)
        {
            perceptions.Add(perception);
        }

        protected override bool IsSquareFree(int x, int y)
        {
            // Check that there is no wind adjacent to this field 
            // This means that all fields adjacend to the adjacent fields must be trap-free
            return base.IsSquareFree(x, y)
                && State.Map[x, y].AdjacentFields.SelectMany(f => f.AdjacentFields).All(f => f.Content != FieldContent.Trap)
                && !State.PlayerPosition.Equals((x, y));
        }
    }
}
