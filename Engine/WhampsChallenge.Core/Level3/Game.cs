using System.Collections.Generic;
using System.Linq;
using WhampsChallenge.Core.Common;
using WhampsChallenge.Core.Common.Events;
using WhampsChallenge.Core.Extensions;
using WhampsChallenge.Core.Level3.Events;
using WhampsChallenge.Core.Level3.Fields;

namespace WhampsChallenge.Core.Level3
{
    public class Game : Level2.Game
    {
        internal new GameState State = new GameState();
        
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

            perceptions = new List<IEvent>();

            return result;
        }

        protected override void PostProcessAction()
        {
            base.PostProcessAction();

            // Die when stepping on whamps
            if (State.Map[State.PlayerPosition].Content is Whamps)
            {
                AddPerception(new Death());
                GameState = Common.GameState.Lose;
                return;
            }

            // Feel stench when adjacent to whamps
            foreach (var adjacentFieldContent in GetAdjacentFieldsOf(State.Map[State.PlayerPosition]).Select(x => x.Content))
            {
                if (adjacentFieldContent is Whamps) AddPerception(new Stench());
            }
        }

        protected override bool IsSquareFree(int x, int y)
        {
            // Check that there is no stench on the field
            return base.IsSquareFree(x, y) && GetAdjacentFieldsOf(State.Map[(x,y)]).SelectMany(GetAdjacentFieldsOf).All(f => f.Content.IsNot<Whamps>());
        }
    }
}
