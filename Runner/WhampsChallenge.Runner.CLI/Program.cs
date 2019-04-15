using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using PantherDI.ContainerCreation;
using WhampsChallenge.Library;
using WhampsChallenge.Runner.Shared.Direct;
using WhampsChallenge.Shared.Communication;
using WhampsChallenge.Shared.Communication.Messages;

namespace WhampsChallenge.Runner.CLI
{
    static class Program
    {
        private static readonly Dictionary<string, Func<string[], int>> Modes = new Dictionary<string, Func<string[], int>>
        {
            {"assembly", RunAssembly},
            {"entry", RunEntry},
            {"level", RunLevel},
            {"pipe", RunPipe},
            {"exitcodes", PrintExitCodes}
        };

        private static int RunPipe(string[] args)
        {
            if (args.Length < 2)
            {
                PrintUsage();
                return 1;
            }

            var (process, helloMessage, communicator) = StartExternalProcess(args);

            foreach (var level in helloMessage.Levels)
            {
                var runner = new Shared.Runner(communicator);
                var result = runner.Run((Levels) Enum.Parse(typeof(Levels), level));
                Console.Out.WriteLine("RSLT: " + JsonConvert.SerializeObject(new
                {
                    Level = level,
                    result.Died,
                    result.Score,
                    result.Seed
                }));

                if (!process.HasExited) process.Kill();
                (process, _, communicator) = StartExternalProcess(args);
            }

            return 0;
        }

        private static (Process Process, Hello HelloMessage, ICommunicator Communicator) StartExternalProcess(string[] args)
        {
            var process = new Process
            {
                StartInfo =
                {
                    FileName = args[1],
                    CreateNoWindow = true,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true
                }
            };

            foreach (var argument in args.Skip(2))
            {
                process.StartInfo.ArgumentList.Add(argument);
            }

            process.Start();

            var communicator = Communicator.Create(process.StandardOutput, process.StandardInput);

            // Read hello-message
            var hello = JsonConvert.DeserializeObject<Hello>(communicator.Receive());

            var processData = (Process: process, HelloMessage: hello, Communicator: communicator);
            return processData;
        }

        private static int Main(string[] args)
        {
            if (args.Length < 1)
            {
                PrintUsage();
                return 1;
            }

            if (!Modes.TryGetValue(args[0].ToLowerInvariant(), out var modeHandler))
            {
                PrintUsage();
                return 1;
            }

            return modeHandler(args);
        }

        private static int RunAssembly(string[] args)
        {
            if (args.Length < 2)
            {
                PrintUsage();
                return 1;
            }

            var (contestantAssembly, exitCode1) = LoadAssembly(args);
            if (contestantAssembly == null) return exitCode1;

            var entries = new ContainerBuilder().WithAssembly(contestantAssembly).WithGenericResolvers().Build().Resolve<IEnumerable<IContestEntry>>().ToArray();
            if (entries.Length == 0)
            {
                Console.WriteLine("No contest entries found.");
                return 4;
            }

            foreach (var contestEntry in entries)
            {
                var runResult = RunContestEntry(contestEntry);
                if (runResult != 0)
                    return runResult;
            }

            return 0;
        }

        private static int RunEntry(string[] args)
        {
            if (args.Length < 3)
            {
                PrintUsage();
                return 1;
            }

            var (contestantAssembly, exitCode1) = LoadAssembly(args);
            if (contestantAssembly == null) return exitCode1;

            var (entryType, exitCode2) = GetType<IContestEntry>(args, contestantAssembly);
            if (entryType == null) return exitCode2;

            return RunContestEntry((IContestEntry) Activator.CreateInstance(entryType));
        }

        private static int RunLevel(string[] args)
        {
            if (args.Length < 4)
            {
                PrintUsage();
                return 1;
            }

            var (contestantAssembly, exitCode1) = LoadAssembly(args);
            if (contestantAssembly == null) return exitCode1;

            var (agentType, exitCode2) = GetType<IAgent>(args, contestantAssembly);
            if (agentType == null) return exitCode2;

            if (!Enum.TryParse<Levels>(args[3], true, out var level))
            {
                Console.WriteLine("Invalid level '{0}'.", args[3]);
                PrintUsage();
                return 8;
            }

            return RunContestAgent(agentType, level);
        }

        private static (Type agentType, int runLevel) GetType<TOut>(string[] args, Assembly contestantAssembly)
        {
            var agentType = contestantAssembly.GetTypes().SingleOrDefault(x =>
                x.FullName != null && (x.FullName.ToLowerInvariant().Equals(args[2].ToLowerInvariant()) ||
                                       x.Name.ToLowerInvariant().Equals(args[2].ToLowerInvariant())));

            if (agentType == null)
            {
                Console.WriteLine("Could not find type '{0}' in '{1}'.", args[1], args[2]);
                {
                    return (null, 6);
                }
            }

            if (!agentType.GetInterfaces().Contains(typeof(TOut)))
            {
                Console.WriteLine("The type '{0}' is not implementing {1}", agentType.FullName, typeof(TOut).Name);
                {
                    return (null, 7);
                }
            }

            return (agentType, 0);
        }

        private static (Assembly contestantAssembly, int runLevel) LoadAssembly(string[] args)
        {
            if (!File.Exists(args[1]))
            {
                Console.WriteLine("The file '{0}' could not be found", args[1]);
                {
                    return (null, 2);
                }
            }

            Assembly contestantAssembly;
            try
            {
                contestantAssembly = Assembly.LoadFile(Path.GetFullPath(args[1]));
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                Console.Error.WriteLine(ex.StackTrace);
                Console.WriteLine("The file '{0}' could not be loaded as .NET-Assembly", args[1]);
                {
                    return (null, 3);
                }
            }

            return (contestantAssembly, 0);
        }

        private static int RunContestEntry(IContestEntry contestEntry)
        {
            Console.WriteLine("Running {0} (by {1}):", contestEntry.ContestantName, contestEntry.Author);
            foreach (var entryAgent in contestEntry.Agents)
            {
                
                var runResult = RunContestAgent(entryAgent.Value, entryAgent.Key);
                if (runResult != 0)
                    return runResult;
            }

            return 0;
        }

        private static int RunContestAgent(Type agentType, Levels level)
        {
            Console.WriteLine("{0} vs. {1}:",agentType.FullName, level);
            var runner = new DirectRunner(level, agentType);
            try
            {
                var result = runner.Run().Result;
                Console.WriteLine("RSLT: {0}", JsonConvert.SerializeObject(result));
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                Console.Error.WriteLine(ex.StackTrace);
                Console.WriteLine("Error while running {0} on level {1}.", agentType.FullName, level);
                return 5;
            }

            return 0; 
        }

        private static void PrintUsage()
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("WhampsChallenge.Runner.CLI.exe <mode> <parameters>");
            Console.WriteLine();
            Console.WriteLine("Please choose one of the modes and specify the given parameters dependent on the mode:");
            Console.WriteLine();
            Console.WriteLine("Assembly <filename> - Scans the given Assembly (e.g. DLL-File) for implementations of");
            Console.WriteLine("                      IContestEntry and executes all of them for all levels defined therein.");
            Console.WriteLine("Entry <filename> <classname> - Loads the given implementation of IContestEntry and executes");
            Console.WriteLine("                               all levels defined therein.");
            Console.WriteLine("Level <filename> <classname> <level> - Loads the given implementation of IAgent and executes it");
            Console.WriteLine("                                       against the given level.");
            Console.WriteLine("Pipe <commandline> - Runs the given command line and attaches to STDIN, STDOUT and STDERR of the");
            Console.WriteLine("                     started process to communicate and log.");
            Console.WriteLine("Exitcodes - Prints all exitcodes and their meanings");
            Console.WriteLine();
            Console.WriteLine("This implementation of the CLI-Runner knows the following levels:");
            Console.WriteLine(string.Join("\n", Enum.GetNames(typeof(Levels))));
        }

        private static int PrintExitCodes(string[] arg)
        {
            Console.WriteLine("0 - All went fine");
            Console.WriteLine("1 - Invalid arguments given");
            Console.WriteLine("2 - File not found");
            Console.WriteLine("3 - Assembly could not be loaded");
            Console.WriteLine("4 - No contest entries found");
            Console.WriteLine("5 - Exception while running contest");
            Console.WriteLine("6 - Specified type not found in assembly");
            Console.WriteLine("7 - Specified type does not implement mandatory interface");
            Console.WriteLine("8 - Specified level is unknown");

            return 0;
        }
    }
}
