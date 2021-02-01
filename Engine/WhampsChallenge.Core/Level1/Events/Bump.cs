﻿using WhampsChallenge.Core.Common.Events;
using WhampsChallenge.Core.Markers;

namespace WhampsChallenge.Core.Level1.Events
{
    /// <summary>
    /// You ran against a wall.
    /// Ouch!
    /// </summary>
    [Level(1)]
    [Level(2)]
    [Level(3)]
    public class Bump : IEvent
    {
    }
}