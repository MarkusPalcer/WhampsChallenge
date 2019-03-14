using WhampsChallenge.Level1;
using WhampsChallenge.Markers;

namespace WhampsChallenge.Level3.Actions
{
    [Action]
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
                var field = game.State.Map[game.State.PlayerPosition].AdjacentFields[Direction];
                while (field != null)
                {
                    if (field.Content == FieldContent.Whamps)
                    {
                        game.AddPerception(Perception.Scream);
                        field.Content = FieldContent.Empty;
                    }

                    field = field.AdjacentFields[Direction];
                }
            }
        }
    }
}