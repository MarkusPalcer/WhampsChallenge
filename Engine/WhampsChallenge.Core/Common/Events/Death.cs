using WhampsChallenge.Core.Markers;

namespace WhampsChallenge.Core.Common.Events
{
    /// <summary>
    /// You died and subsequently lost the game
    /// </summary>
    [Level(1)]
    [Level(2)]
    [Level(3)]
    public class Death : IEvent
    {
    }
}