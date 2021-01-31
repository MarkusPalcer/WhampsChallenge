using WhampsChallenge.Core.Extensions;
using WhampsChallenge.Core.Level1.Fields;
using WhampsChallenge.Core.Level3.Fields;
using WhampsChallenge.Core.Maps;
using WhampsChallenge.Core.Markers;

namespace WhampsChallenge.Core.Level3.Actions
{
    [Action]
    [Level(3)]
    public class Shoot : IAction
    {
        public Direction Direction { get; set; }

        public void Execute(Game game)
        {
            if (!game.State.HasArrow)
            {
                game.AddPerception(Perception.Twang);
            }
            else
            {
                // Move into the direction until the end of the playing field.
                game.State.HasArrow = false;
                var field = game.State.Map[game.State.PlayerPosition][Direction];
                while (field != null && field.IsNot<Wall>())
                {
                    if (field.Content is Whamps)
                    {
                        game.AddPerception(Perception.Scream);
                        field.Content = new Empty();
                        return;
                    }

                    field = field[Direction];
                }
            }
        }
    }
}