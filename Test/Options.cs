using System;
using System.Collections.Generic;

namespace Test
{
    /// <summary>
    /// Set of options for the test executable
    /// </summary>
    internal sealed class Options
    {
        #region Properties

        /// <summary>
        /// Enable debug output for relevant operations
        /// </summary>
        public bool Debug { get; private set; } = false;

        /// <summary>
        /// Set of input paths to use for operations
        /// </summary>
        public List<string> InputPaths { get; private set; } = [];

#if NETCOREAPP3_1_OR_GREATER
        /// <summary>
        /// Enable JSON output
        /// </summary>
        public bool Json { get; private set; } = false;
#endif

        #endregion

        /// <summary>
        /// Parse commandline arguments into an Options object
        /// </summary>
        public static Options? ParseOptions(string[] args)
        {
            // If we have invalid arguments
            if (args == null || args.Length == 0)
                return null;

            // Create an Options object
            var options = new Options();

            // Parse the features
            int index = 0;
            for (; index < args.Length; index++)
            {
                string arg = args[index];
                bool featureFound = false;
                switch (arg)
                {
                    case "-?":
                    case "-h":
                    case "--help":
                        return null;

                    default:
                        break;
                }

                // If the flag wasn't a feature
                if (!featureFound)
                    break;
            }

            // Parse the options and paths
            for (; index < args.Length; index++)
            {
                string arg = args[index];
                switch (arg)
                {
                    case "-d":
                    case "--debug":
                        options.Debug = true;
                        break;

                    case "-j":
                    case "--json":
#if NET6_0_OR_GREATER
                        options.Json = true;
#else
                        Console.WriteLine("JSON output not available in .NET Framework");
#endif
                        break;

                    default:
                        options.InputPaths.Add(arg);
                        break;
                }
            }

            // Validate we have any input paths to work on
            if (options.InputPaths.Count == 0)
            {
                Console.WriteLine("At least one path is required!");
                return null;
            }

            return options;
        }

        /// <summary>
        /// Display help text
        /// </summary>
        public static void DisplayHelp()
        {
            Console.WriteLine("SabreTools.Printing Test Program");
            Console.WriteLine();
            Console.WriteLine("test.exe <options> file|directory ...");
            Console.WriteLine();
            Console.WriteLine("Options:");
            Console.WriteLine("-?, -h, --help           Display this help text and quit");
            Console.WriteLine("-d, --debug              Enable debug mode");
#if NET6_0_OR_GREATER
            Console.WriteLine("-j, --json               Print executable info as JSON");
#endif
        }
    }
}