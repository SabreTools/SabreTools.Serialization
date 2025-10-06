using System;
using System.Collections.Generic;
using System.IO;
using SabreTools.CommandLine;
using SabreTools.CommandLine.Inputs;
using SabreTools.IO.Extensions;
using SabreTools.Serialization;
using SabreTools.Serialization.Wrappers;

namespace ExtractionTool
{
    class Program
    {
        #region Constants

        private const string _debugName = "debug";
        private const string _helpName = "help";
        private const string _outputPathName = "output-path";

        #endregion

        static void Main(string[] args)
        {
#if NET462_OR_GREATER || NETCOREAPP
            // Register the codepages
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
#endif

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
                OutputPath = commandSet.GetString(_outputPathName) ?? string.Empty,
            };

            // Validate the output path
            if (!options.ValidateExtractionPath())
            {
                commandSet.OutputAllHelp();
                return;
            }

            // Loop through the input paths
            for (int i = firstFileIndex; i < args.Length; i++)
            {
                string arg = args[i];
                ExtractPath(arg, options);
            }
        }

        /// <summary>
        /// Create the command set for the program
        /// </summary>
        private static CommandSet CreateCommands()
        {
            List<string> header = [
                "Extraction Tool",
                string.Empty,
                "ExtractionTool <options> file|directory ...",
                string.Empty,
            ];

            var commandSet = new CommandSet(header);

            commandSet.Add(new FlagInput(_helpName, ["-?", "-h", "--help"], "Display this help text"));
            commandSet.Add(new FlagInput(_debugName, ["-d", "--debug"], "Enable debug mode"));
            commandSet.Add(new StringInput(_outputPathName, ["-o", "--outdir"], "Set output path for extraction (required)"));

            return commandSet;
        }

        /// <summary>
        /// Wrapper to extract data for a single path
        /// </summary>
        /// <param name="path">File or directory path</param>
        /// <param name="options">User-defined options</param>
        private static void ExtractPath(string path, Options options)
        {
            // Normalize by getting the full path
            path = Path.GetFullPath(path);
            Console.WriteLine($"Checking possible path: {path}");

            // Check if the file or directory exists
            if (File.Exists(path))
            {
                ExtractFile(path, options);
            }
            else if (Directory.Exists(path))
            {
                foreach (string file in IOExtensions.SafeEnumerateFiles(path, "*", SearchOption.AllDirectories))
                {
                    ExtractFile(file, options);
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
        /// <param name="path">File path</param>
        /// <param name="options">User-defined options</param>
        private static void ExtractFile(string file, Options options)
        {
            Console.WriteLine($"Attempting to extract all files from {file}");
            using Stream stream = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            // Get the extension for certain checks
            string extension = Path.GetExtension(file).ToLower().TrimStart('.');

            // Get the first 16 bytes for matching
            byte[] magic = new byte[16];
            try
            {
                int read = stream.Read(magic, 0, 16);
                stream.Seek(0, SeekOrigin.Begin);
            }
            catch (Exception ex)
            {
                if (options.Debug) Console.Error.WriteLine(ex);
                return;
            }

            // Get the file type
            WrapperType ft = WrapperFactory.GetFileType(magic, extension);
            var wrapper = WrapperFactory.CreateWrapper(ft, stream);

            // Create the output directory
            Directory.CreateDirectory(options.OutputPath);

            // Print the preamble
            Console.WriteLine($"Attempting to extract from '{wrapper?.Description() ?? "UNKNOWN"}'");
            Console.WriteLine();

            switch (wrapper)
            {
                // 7-zip
                case SevenZip sz:
                    sz.Extract(options.OutputPath, options.Debug);
                    break;

                // BFPK archive
                case BFPK bfpk:
                    bfpk.Extract(options.OutputPath, options.Debug);
                    break;

                // BSP
                case BSP bsp:
                    bsp.Extract(options.OutputPath, options.Debug);
                    break;

                // bzip2
                case BZip2 bzip2:
                    bzip2.Extract(options.OutputPath, options.Debug);
                    break;

                // CFB
                case CFB cfb:
                    cfb.Extract(options.OutputPath, options.Debug);
                    break;

                // GCF
                case GCF gcf:
                    gcf.Extract(options.OutputPath, options.Debug);
                    break;

                // gzip
                case GZip gzip:
                    gzip.Extract(options.OutputPath, options.Debug);
                    break;

                // InstallShield Archive V3 (Z)
                case InstallShieldArchiveV3 isv3:
                    isv3.Extract(options.OutputPath, options.Debug);
                    break;

                // IS-CAB archive
                case InstallShieldCabinet iscab:
                    iscab.Extract(options.OutputPath, options.Debug);
                    break;

                // LZ-compressed file, KWAJ variant
                case LZKWAJ kwaj:
                    kwaj.Extract(options.OutputPath, options.Debug);
                    break;

                // LZ-compressed file, QBasic variant
                case LZQBasic qbasic:
                    qbasic.Extract(options.OutputPath, options.Debug);
                    break;

                // LZ-compressed file, SZDD variant
                case LZSZDD szdd:
                    szdd.Extract(options.OutputPath, options.Debug);
                    break;

                // Microsoft Cabinet archive
                case MicrosoftCabinet mscab:
                    mscab.Extract(options.OutputPath, options.Debug);
                    break;

                // MoPaQ (MPQ) archive
                case MoPaQ mpq:
                    mpq.Extract(options.OutputPath, options.Debug);
                    break;

                // New Executable
                case NewExecutable nex:
                    nex.Extract(options.OutputPath, options.Debug);
                    break;

                // PAK
                case PAK pak:
                    pak.Extract(options.OutputPath, options.Debug);
                    break;

                // PFF
                case PFF pff:
                    pff.Extract(options.OutputPath, options.Debug);
                    break;

                // PKZIP
                case PKZIP pkzip:
                    pkzip.Extract(options.OutputPath, options.Debug);
                    break;

                // Portable Executable
                case PortableExecutable pex:
                    pex.Extract(options.OutputPath, options.Debug);
                    break;

                // Quantum
                case Quantum quantum:
                    quantum.Extract(options.OutputPath, options.Debug);
                    break;

                // RAR
                case RAR rar:
                    rar.Extract(options.OutputPath, options.Debug);
                    break;

                // SGA
                case SGA sga:
                    sga.Extract(options.OutputPath, options.Debug);
                    break;

                // Tape Archive
                case TapeArchive tar:
                    tar.Extract(options.OutputPath, options.Debug);
                    break;

                // VBSP
                case VBSP vbsp:
                    vbsp.Extract(options.OutputPath, options.Debug);
                    break;

                // VPK
                case VPK vpk:
                    vpk.Extract(options.OutputPath, options.Debug);
                    break;

                // WAD3
                case WAD3 wad:
                    wad.Extract(options.OutputPath, options.Debug);
                    break;

                // xz
                case XZ xz:
                    xz.Extract(options.OutputPath, options.Debug);
                    break;

                // XZP
                case XZP xzp:
                    xzp.Extract(options.OutputPath, options.Debug);
                    break;

                // Everything else
                default:
                    Console.WriteLine("Not a supported extractable file format, skipping...");
                    Console.WriteLine();
                    break;
            }
        }
    }
}
