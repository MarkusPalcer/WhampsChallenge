using WhampsChallenge.Core.Common.Events;
using WhampsChallenge.Core.Markers;

namespace WhampsChallenge.Core.Level1.Events
{
    /// <summary>
    /// Something is glittering on the ground.
    /// Maybe you should pick it up?
    /// </summary>
    [Level(1, 3)]
    public class Glitter : IEvent
    {
    }
}