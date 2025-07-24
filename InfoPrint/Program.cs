using System;
using System.IO;
using SabreTools.IO.Extensions;
using SabreTools.Serialization;
using SabreTools.Serialization.Wrappers;

namespace InfoPrint
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            // Get the options from the arguments
            var options = Options.ParseOptions(args);

            // If we have an invalid state
            if (options == null)
            {
                Options.DisplayHelp();
                return;
            }

            // Loop through the input paths
            foreach (string inputPath in options.InputPaths)
            {
#if NETFRAMEWORK || NETSTANDARD2_0_OR_GREATER
                PrintPathInfo(inputPath, false, options.Debug);
#else
                PrintPathInfo(inputPath, options.Json, options.Debug);
#endif
            }
        }

        /// <summary>
        /// Wrapper to print information for a single path
        /// </summary>
        /// <param name="path">File or directory path</param>
        /// <param name="json">Enable JSON output, if supported</param>
        /// <param name="debug">Enable debug output</param>
        private static void PrintPathInfo(string path, bool json, bool debug)
        {
            Console.WriteLine($"Checking possible path: {path}");

            // Check if the file or directory exists
            if (File.Exists(path))
            {
                PrintFileInfo(path, json, debug);
            }
            else if (Directory.Exists(path))
            {
                foreach (string file in IOExtensions.SafeEnumerateFiles(path, "*", SearchOption.AllDirectories))
                {
                    PrintFileInfo(file, json, debug);
                }
            }
            else
            {
                Console.WriteLine($"{path} does not exist, skipping...");
            }
        }

        /// <summary>
        /// Print information for a single file, if possible
        /// </summary>
        private static void PrintFileInfo(string file, bool json, bool debug)
        {
            Console.WriteLine($"Attempting to print info for {file}");

            try
            {
                using Stream stream = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                // Read the first 8 bytes
                byte[]? magic = stream.ReadBytes(8);
                stream.Seek(0, SeekOrigin.Begin);

                // Get the file type
                string extension = Path.GetExtension(file).TrimStart('.');
                WrapperType ft = WrapperFactory.GetFileType(magic ?? [], extension);

                // Print out the file format
                Console.WriteLine($"File format found: {ft}");

                // Setup the wrapper to print
                var wrapper = WrapperFactory.CreateWrapper(ft, stream);

                // If we don't have a wrapper
                if (wrapper == null)
                {
                    Console.WriteLine($"Either {ft} is not supported or something went wrong during parsing!");
                    Console.WriteLine();
                    return;
                }

                // Get the base info output name
                string filenameBase = $"info-{DateTime.Now:yyyy-MM-dd_HHmmss.ffff}";

#if NETCOREAPP
                // If we have the JSON flag
                if (json)
                {
                    // Create the output data
                    string serializedData = wrapper.ExportJSON();
                    Console.WriteLine(serializedData);

                    // Write the output data
                    using var jsw = new StreamWriter(File.OpenWrite($"{filenameBase}.json"));
                    jsw.WriteLine(serializedData);
                }
#endif

                // Create the output data
                var builder = wrapper.ExportStringBuilder();
                if (builder == null)
                {
                    Console.WriteLine("No item information could be generated");
                    return;
                }

                // Write the output data
                Console.WriteLine(builder);
                using var sw = new StreamWriter(File.OpenWrite($"{filenameBase}.txt"));
                sw.WriteLine(builder.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(debug ? ex : "[Exception opening file, please try again]");
                Console.WriteLine();
            }
        }
    }
}
