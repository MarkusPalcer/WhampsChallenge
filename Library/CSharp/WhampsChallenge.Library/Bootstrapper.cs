using System;
using System.Linq;
using Newtonsoft.Json;
using PantherDI.ContainerCreation;
using WhampsChallenge.Shared.Communication;
using WhampsChallenge.Shared.Communication.Messages;
using WhampsChallenge.Shared.Extensions;

namespace WhampsChallenge.Library
{
    /// <summary>
    /// Bootstraps a client wrapped into an executable application
    /// </summary>
    public class Bootstrapper
    {
        private readonly ICommunicator communicator;
        private readonly IContestEntry contestEntry;

        public Bootstrapper(ICommunicator communicator, IContestEntry contestEntry)
        {
            this.communicator = communicator;
            this.contestEntry = contestEntry;

            ScanWholeContestantAssembly = false;
        }

        /// <summary>
        /// Determines whether the whole assembly of the agent will be scanned by the <see cref="ContainerBuilder"/> or if just the type of the agent will be added.
        ///
        /// This is only needed if the agent resolves a dependency in its constructor via an interface instead of the type directly
        /// </summary>
        public bool ScanWholeContestantAssembly { get; set; }

        public void Run()
        {
            var hello = new Hello
            {
                Author = contestEntry.Author,
                ContestantName = contestEntry.ContestantName,
                Levels = contestEntry.Agents.Keys.Select(x => x.ToString()).ToArray(),
            };

            communicator.Send(JsonConvert.SerializeObject(hello));
            var runInfo = JsonConvert.DeserializeObject<StartLevel>(communicator.Receive());

            var builder = new ContainerBuilder();
            
            var agentType = contestEntry.Agents[(Levels)Enum.Parse(typeof(Levels), runInfo.Level)];

            if (ScanWholeContestantAssembly)
            {
                builder.WithAssemblyOf(agentType);
            }
            else
            {
                builder.WithType(agentType);
            }

            builder.Register(communicator).WithContract(typeof(ICommunicator));

            builder.WithSupportForUnregisteredTypes();

            var container = builder.Build();

            var agent = container.Resolve<IAgent>(agentType);
            agent.Run();

        }
    }
}
