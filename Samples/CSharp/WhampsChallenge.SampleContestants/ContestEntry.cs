using System;
using System.Collections.Generic;
using WhampsChallenge.Library;

namespace WhampsChallenge.SampleContestants
{
    public class ContestEntry : IContestEntry
    {
        public IReadOnlyDictionary<Levels, Type> Agents { get; } = new Dictionary<Levels, Type>
        {
            {Levels.Level1, typeof(Level1)},
            {Levels.Level2, typeof(Level2)},
            {Levels.Level3, typeof(Level3)},
        };

        public string Author => "Markus Palcer";

        public string ContestantName => "Sample Contestant";
    }
}