using System.Collections.Generic;
using System.Linq;
using WhampsChallenge.Core.Common;

namespace ContractGenerator
{
    public class Contract
    {
        public VersionInfo Version => new VersionInfo(2,0,0);

        public Dictionary<string, Level> Levels;

        public readonly SharedTypes Shared = new SharedTypes();

        public Contract()
        {
            var discoverer = new Discoverer();
            Levels = discoverer.Levels.ToDictionary(x => $"Level{x}", x => new Level(discoverer[x]) {Index = x});

            discoverer.SharedTypes.ForEach(x => Shared.AddType(x));
        }
    }
}
