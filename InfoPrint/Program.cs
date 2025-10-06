using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SabreTools.CommandLine;
using SabreTools.CommandLine.Inputs;
using SabreTools.Hashing;
using SabreTools.IO.Extensions;
using SabreTools.Serialization;
using SabreTools.Serialization.Wrappers;

namespace InfoPrint
{
    public static class Program
    {
        #region Constants

        private const string _debugName = "debug";
        private const string _fileOnlyName = "file-only";
        private const string _hashName = "hash";
        private const string _helpName = "help";
#if NETCOREAPP
        private const string _jsonName = "json";
#endif

        #endregion

        public static void Main(string[] args)
        {
            // Create the command set
            var commandSet = CreateCommands();

            // If we have no args, show the help and quit
            if (args == null || args.Length == 0)
            {
                commandSet.OutputAllHelp();
                return;
            }

            // Loop through and process the options
            int firstFileIndex = 0;
            for (; firstFileIndex < args.Length; firstFileIndex++)
            {
                string arg = args[firstFileIndex];

                var input = commandSet.GetTopLevel(arg);
                if (input == null)
                    break;

                input.ProcessInput(args, ref firstFileIndex);
            }

            // If help was specified
            if (commandSet.GetBoolean(_helpName))
            {
                commandSet.OutputAllHelp();
                return;
            }

            // Get the options from the arguments
            var options = new Options
            {
                Debug = commandSet.GetBoolean(_debugName),
                Hash = commandSet.GetBoolean(_hashName),
                FileOnly = commandSet.GetBoolean(_fileOnlyName),
#if NETCOREAPP
                Json = commandSet.GetBoolean(_jsonName),
#endif
            };

            // Loop through the input paths
            for (int i = firstFileIndex; i < args.Length; i++)
            {
                string arg = args[i];
                PrintPathInfo(arg, options);
            }
        }

        /// <summary>
        /// Create the command set for the program
        /// </summary>
        private static CommandSet CreateCommands()
        {
            List<string> header = [
                "Information Printing Program",
                string.Empty,
                "InfoPrint <options> file|directory ...",
                string.Empty,
            ];

            var commandSet = new CommandSet(header);

            commandSet.Add(new FlagInput(_helpName, ["-?", "-h", "--help"], "Display this help text"));
            commandSet.Add(new FlagInput(_debugName, ["-d", "--debug"], "Enable debug mode"));
            commandSet.Add(new FlagInput(_hashName, ["-c", "--hash"], "Output file hashes"));
            commandSet.Add(new FlagInput(_fileOnlyName, ["-f", "--file"], "Print to file only"));
#if NETCOREAPP
            commandSet.Add(new FlagInput(_jsonName, ["-j", "--json"], "Print info as JSON"));
#endif

            return commandSet;
        }

        /// <summary>
        /// Wrapper to print information for a single path
        /// </summary>
        /// <param name="path">File or directory path</param>
        /// <param name="options">User-defined options</param>
        private static void PrintPathInfo(string path, Options options)
        {
            Console.WriteLine($"Checking possible path: {path}");

            // Check if the file or directory exists
            if (File.Exists(path))
            {
                PrintFileInfo(path, options);
            }
            else if (Directory.Exists(path))
            {
                foreach (string file in IOExtensions.SafeEnumerateFiles(path, "*", SearchOption.AllDirectories))
                {
                    PrintFileInfo(file, options);
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
        /// <param name="file">File path</param>
        /// <param name="options">User-defined options</param>
        private static void PrintFileInfo(string file, Options options)
        {
            Console.WriteLine($"Attempting to print info for {file}");

            // Get the base info output name
            string filenameBase = $"info-{DateTime.Now:yyyy-MM-dd_HHmmss.ffff}";

            // If we have the hash flag
            if (options.Hash)
            {
                var hashBuilder = PrintHashInfo(file, options.Debug);
                if (hashBuilder != null)
                {
                    // Create the output data
                    string hashData = hashBuilder.ToString();

                    // Write the output data
                    using var hsw = new StreamWriter(File.OpenWrite($"{filenameBase}.hashes"));
                    hsw.WriteLine(hashData);
                    hsw.Flush();
                }
            }

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

#if NETCOREAPP
                // If we have the JSON flag
                if (options.Json)
                {
                    // Create the output data
                    string serializedData = wrapper.ExportJSON();

                    // Write the output data
                    using var jsw = new StreamWriter(File.OpenWrite($"{filenameBase}.json"));
                    jsw.WriteLine(serializedData);
                    jsw.Flush();
                }
#endif

                // Create the output data
                var builder = wrapper.ExportStringBuilder();
                if (builder == null)
                {
                    Console.WriteLine("No item information could be generated");
                    return;
                }

                // Only print to console if enabled
                if (!options.FileOnly)
                    Console.WriteLine(builder);

                using var sw = new StreamWriter(File.OpenWrite($"{filenameBase}.txt"));
                sw.WriteLine(file);
                sw.WriteLine();
                sw.WriteLine(builder.ToString());
                sw.Flush();
            }
            catch (Exception ex)
            {
                Console.WriteLine(options.Debug ? ex : "[Exception opening file, please try again]");
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Print hash information for a single file, if possible
        /// </summary>
        /// <param name="file">File path</param>
        /// <param name="debug">Enable debug output</param>
        /// <returns>StringBuilder representing the hash information, if possible</returns>
        private static StringBuilder? PrintHashInfo(string file, bool debug)
        {
            // Ignore missing files
            if (!File.Exists(file))
                return null;

            Console.WriteLine($"Attempting to hash {file}, this may take a while...");

            try
            {
                // Get all file hashes for flexibility
                var hashes = HashTool.GetFileHashes(file);
                if (hashes == null)
                {
                    if (debug) Console.WriteLine($"Hashes for {file} could not be retrieved");
                    return null;
                }

                // Output subset of available hashes
                var builder = new StringBuilder();
                if (hashes.TryGetValue(HashType.CRC16, out string? crc16) && crc16 != null)
                    builder.AppendLine($"CRC-16 checksum: {crc16}");
                if (hashes.TryGetValue(HashType.CRC32, out string? crc32) && crc32 != null)
                    builder.AppendLine($"CRC-32 checksum: {crc32}");
                if (hashes.TryGetValue(HashType.MD2, out string? md2) && md2 != null)
                    builder.AppendLine($"MD2 hash: {md2}");
                if (hashes.TryGetValue(HashType.MD4, out string? md4) && md4 != null)
                    builder.AppendLine($"MD4 hash: {md4}");
                if (hashes.TryGetValue(HashType.MD5, out string? md5) && md5 != null)
                    builder.AppendLine($"MD5 hash: {md5}");
                if (hashes.TryGetValue(HashType.RIPEMD128, out string? ripemd128) && ripemd128 != null)
                    builder.AppendLine($"RIPEMD-128 hash: {ripemd128}");
                if (hashes.TryGetValue(HashType.RIPEMD160, out string? ripemd160) && ripemd160 != null)
                    builder.AppendLine($"RIPEMD-160 hash: {ripemd160}");
                if (hashes.TryGetValue(HashType.SHA1, out string? sha1) && sha1 != null)
                    builder.AppendLine($"SHA-1 hash: {sha1}");
                if (hashes.TryGetValue(HashType.SHA256, out string? sha256) && sha256 != null)
                    builder.AppendLine($"SHA-256 hash: {sha256}");
                if (hashes.TryGetValue(HashType.SHA384, out string? sha384) && sha384 != null)
                    builder.AppendLine($"SHA-384 hash: {sha384}");
                if (hashes.TryGetValue(HashType.SHA512, out string? sha512) && sha512 != null)
                    builder.AppendLine($"SHA-512 hash: {sha512}");

                return builder;
            }
            catch (Exception ex)
            {
                Console.WriteLine(debug ? ex : "[Exception opening file, please try again]");
                return null;
            }
        }
    }
}
