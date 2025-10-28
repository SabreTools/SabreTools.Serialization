using System;
using System.IO;
using System.Text;
using SabreTools.CommandLine;
using SabreTools.CommandLine.Inputs;
using SabreTools.Hashing;
using SabreTools.IO.Extensions;
using SabreTools.Serialization;
using SabreTools.Serialization.Wrappers;

namespace InfoPrint.Features
{
    internal sealed class MainFeature : Feature
    {
        #region Feature Definition

        public const string DisplayName = "main";

        /// <remarks>Flags are unused</remarks>
        private static readonly string[] _flags = [];

        /// <remarks>Description is unused</remarks>
        private const string _description = "";

        #endregion

        #region Inputs

        private const string _debugName = "debug";
        internal readonly FlagInput DebugInput = new(_debugName, ["-d", "--debug"], "Enable debug mode");

        private const string _fileOnlyName = "file-only";
        internal readonly FlagInput FileOnlyInput = new(_fileOnlyName, ["-f", "--file"], "Print to file only");

        private const string _hashName = "hash";
        internal readonly FlagInput HashInput = new(_hashName, ["-c", "--hash"], "Output file hashes");

#if NETCOREAPP
        private const string _jsonName = "json";
        internal readonly FlagInput JsonInput = new(_jsonName, ["-j", "--json"], "Print info as JSON");
#endif

        #endregion

        /// <summary>
        /// Enable debug output for relevant operations
        /// </summary>
        public bool Debug { get; private set; }

        /// <summary>
        /// Output information to file only, skip printing to console
        /// </summary>
        public bool FileOnly { get; private set; }

        /// <summary>
        /// Print external file hashes
        /// </summary>
        public bool Hash { get; private set; }

#if NETCOREAPP
        /// <summary>
        /// Enable JSON output
        /// </summary>
        public bool Json { get; private set; }
#endif

        public MainFeature()
            : base(DisplayName, _flags, _description)
        {
            RequiresInputs = true;

            Add(DebugInput);
            Add(HashInput);
            Add(FileOnlyInput);
#if NETCOREAPP
            Add(JsonInput);
#endif
        }

        /// <inheritdoc/>
        public override bool Execute()
        {
            // Get the options from the arguments
            Debug = GetBoolean(_debugName);
            Hash = GetBoolean(_hashName);
            FileOnly = GetBoolean(_fileOnlyName);
#if NETCOREAPP
            Json = GetBoolean(_jsonName);
#endif

            // Loop through the input paths
            for (int i = 0; i < Inputs.Count; i++)
            {
                string arg = Inputs[i];
                PrintPathInfo(arg);
            }

            return true;
        }

        /// <inheritdoc/>
        public override bool VerifyInputs() => Inputs.Count > 0;

        /// <summary>
        /// Wrapper to print information for a single path
        /// </summary>
        /// <param name="path">File or directory path</param>
        private void PrintPathInfo(string path)
        {
            Console.WriteLine($"Checking possible path: {path}");

            // Check if the file or directory exists
            if (File.Exists(path))
            {
                PrintFileInfo(path);
            }
            else if (Directory.Exists(path))
            {
                foreach (string file in path.SafeEnumerateFiles("*", SearchOption.AllDirectories))
                {
                    PrintFileInfo(file);
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
        private void PrintFileInfo(string file)
        {
            Console.WriteLine($"Attempting to print info for {file}");

            // Get the base info output name
            string filenameBase = $"info-{DateTime.Now:yyyy-MM-dd_HHmmss.ffff}";

            // If we have the hash flag
            if (Hash)
            {
                var hashBuilder = PrintHashInfo(file);
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
                stream.SeekIfPossible(0, SeekOrigin.Begin);

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
                if (Json)
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
                if (!FileOnly)
                    Console.WriteLine(builder);

                using var sw = new StreamWriter(File.OpenWrite($"{filenameBase}.txt"));
                sw.WriteLine(file);
                sw.WriteLine();
                sw.WriteLine(builder.ToString());
                sw.Flush();
            }
            catch (Exception ex)
            {
                Console.WriteLine(Debug ? ex : "[Exception opening file, please try again]");
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Print hash information for a single file, if possible
        /// </summary>
        /// <param name="file">File path</param>
        /// <returns>StringBuilder representing the hash information, if possible</returns>
        private StringBuilder? PrintHashInfo(string file)
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
                    if (Debug) Console.WriteLine($"Hashes for {file} could not be retrieved");
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
                Console.WriteLine(Debug ? ex : "[Exception opening file, please try again]");
                return null;
            }
        }
    }
}
