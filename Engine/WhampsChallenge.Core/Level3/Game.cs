using System.Collections.Generic;
using System.Linq;
using WhampsChallenge.Core.Common;
using WhampsChallenge.Core.Extensions;
using WhampsChallenge.Core.Level3.Fields;

namespace WhampsChallenge.Core.Level3
{
    public class Game : Level2.Game
    {
        private List<Perception> perceptions = new List<Perception>();

        internal new GameState State = new GameState();
        
        private readonly Dictionary<Level2.Perception, Perception> perceptionMappings = new Dictionary<Level2.Perception, Perception>
        {
            {Level2.Perception.Bump, Perception.Bump},
            {Level2.Perception.Death, Perception.Death},
            {Level2.Perception.Glitter, Perception.Glitter},
            {Level2.Perception.Win, Perception.Win},
            {Level2.Perception.Wind, Perception.Wind}
        };

        public override void Initialize()
        {
            base.State = State;

            base.Initialize();

            // Add Monster to Level 2 styled map
            State.Map[GetFreeSquare()].Content = new Whamps();

            State.HasArrow = true;
        }

        public override object Execute(IAction action)
        {
            ((Level3.Actions.IAction) action).Execute(this);
            PostProcessAction();

            var result = new Result()
            {
                GameState = State,
                Perceptions = perceptions.ToArray()
            };

            perceptions = new List<Perception>();

            return result;
        }

        protected override void PostProcessAction()
        {
            base.PostProcessAction();

            // Die when stepping on whamps
            if (State.Map[State.PlayerPosition].Content is Whamps)
            {
                AddPerception(Perception.Death);
                GameState = Common.GameState.Lose;
                return;
            }

            // Feel stench when adjacent to whamps
            foreach (var adjacentFieldContent in GetAdjacentFieldsOf(State.Map[State.PlayerPosition]).Select(x => x.Content))
            {
                if (adjacentFieldContent is Whamps) AddPerception(Perception.Stench);
            }
        }

        internal override void AddPerception(Level2.Perception perception)
        {
            AddPerception(perceptionMappings[perception]);
        }

        public virtual void AddPerception(Perception perception)
        {
            perceptions.Add(perception);
        }

        protected override bool IsSquareFree(int x, int y)
        {
            // Check that there is no stench on the field
            return base.IsSquareFree(x, y) && GetAdjacentFieldsOf(State.Map[(x,y)]).SelectMany(GetAdjacentFieldsOf).All(f => f.Content.IsNot<Whamps>());
        }
    }
}
