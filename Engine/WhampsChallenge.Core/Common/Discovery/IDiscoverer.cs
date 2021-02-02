using System;
using System.Collections.Generic;

namespace WhampsChallenge.Core.Common.Discovery
{
    public interface IDiscoverer
    {
        List<Type> SharedTypes { get; }

        IEnumerable<int> Levels { get; }

        LevelData this[int key] { get; }
    }
}
