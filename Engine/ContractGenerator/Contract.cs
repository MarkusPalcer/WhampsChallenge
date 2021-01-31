using System.Collections.Generic;
using System.Linq;
using WhampsChallenge.Core.Common;

namespace ContractGeneration
{
    public class Contract
    {
        public struct VersionInfo
        {
            public int Major;
            public int Minor;
            public int Revision;

            public VersionInfo(int major, int minor, int revision)
            {
                Major = major;
                Minor = minor;
                Revision = revision;
            }
        }

        public VersionInfo Version => new VersionInfo(2,0,0);

        public Dictionary<string, Level> Levels;

        public Contract()
        {
            var discoverer = new LevelDiscoverer();
            Levels = discoverer.Levels.ToDictionary(x => $"Level{x}", x => new Level(discoverer[x]) {Index = x});
        }
    }
}
