using System;
using System.Collections.Generic;
using PantherDI.Attributes;

namespace WhampsChallenge.Library
{
    [Contract]
    public interface IContestEntry
    {
        IReadOnlyDictionary<Levels, Type> Agents { get; }

        string Author { get; }

        string ContestantName { get; }
    }
}