using System.Collections.Generic;
using System.Linq;
using WhampsChallenge.Core.Common;
using WhampsChallenge.Core.Common.Events;
using WhampsChallenge.Core.Extensions;
using WhampsChallenge.Core.Level2.Events;
using WhampsChallenge.Core.Level2.Fields;
using WhampsChallenge.Core.Maps;

namespace WhampsChallenge.Core.Level2
{
    public class Game : Level1.Game
    {
        internal static IEnumerable<IField> GetAdjacentFieldsOf(IField source)
        {
            return new[] {Direction.North, Direction.East, Direction.South, Direction.West}.Select(x => source[x]).Where(x => x != null);
        }

        public override ActionResult Execute(IAction action)
        {
            ((Level2.Actions.IAction) action).Execute(this);
            PostProcessAction();

            var result = new ActionResult()
            {
                GameState = State,
                Events = Events.ToArray()
            };

            Events = new List<IEvent>();

            return result;
        }

        public override void Initialize()
        {
            // Create Level 1 style map
            base.Initialize();
            
            // Add 2 Traps to Level 1 style map
            State.Map[GetFreeSquare()].Content = new Trap();
            State.Map[GetFreeSquare()].Content = new Trap();
        }

        protected override void PostProcessAction()
        {
            base.PostProcessAction();

            // Die when stepping on a trap
            if (State.Map[State.PlayerPosition].Content is Trap)
            {
                AddEvent(new Death());
                GameCompletionState = GameCompletionStates.Lose;
                return;
            }

            // Feel wind when adjacent to a trap
            foreach (var adjacentFieldContent in GetAdjacentFieldsOf(State.Map[State.PlayerPosition]).Select(x => x.Content))
            {
                if (adjacentFieldContent is Trap) AddEvent(new Wind());
            }
        }

        protected override bool IsSquareFree(int x, int y)
        {
            // Check that there is no wind adjacent to this field 
            // This means that all fields adjacend to the adjacent fields must be trap-free
            return base.IsSquareFree(x, y)
                && GetAdjacentFieldsOf(State.Map[(x, y)]).SelectMany(GetAdjacentFieldsOf).All(f => f.Content.IsNot<Trap>())
                && !State.PlayerPosition.Equals((x, y));
        }
    }
}
