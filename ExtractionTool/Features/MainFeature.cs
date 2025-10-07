using System;
using System.IO;
using SabreTools.CommandLine;
using SabreTools.CommandLine.Inputs;
using SabreTools.IO.Extensions;
using SabreTools.Serialization;
using SabreTools.Serialization.Wrappers;

namespace ExtractionTool.Features
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

        private const string _outputPathName = "output-path";
        internal readonly StringInput OutputPathInput = new(_outputPathName, ["-o", "--outdir"], "Set output path for extraction (required)");

        #endregion

        #region Properties

        /// <summary>
        /// Enable debug output for relevant operations
        /// </summary>
        public bool Debug { get; private set; }

        /// <summary>
        /// Output path for archive extraction
        /// </summary>
        public string OutputPath { get; private set; } = string.Empty;

        #endregion

        public MainFeature()
            : base(DisplayName, _flags, _description)
        {
            RequiresInputs = true;

            Add(DebugInput);
            Add(OutputPathInput);
        }

        /// <inheritdoc/>
        public override bool Execute()
        {
            // Get the options from the arguments
            Debug = GetBoolean(_debugName);
            OutputPath = GetString(_outputPathName) ?? string.Empty;

            // Validate the output path
            if (!ValidateExtractionPath())
                return false;

            // Loop through the input paths
            for (int i = 0; i < Inputs.Count; i++)
            {
                string arg = Inputs[i];
                ExtractPath(arg);
            }

            return true;
        }

        /// <inheritdoc/>
        public override bool VerifyInputs() => Inputs.Count > 0;

        /// <summary>
        /// Wrapper to extract data for a single path
        /// </summary>
        /// <param name="path">File or directory path</param>
        private void ExtractPath(string path)
        {
            // Normalize by getting the full path
            path = Path.GetFullPath(path);
            Console.WriteLine($"Checking possible path: {path}");

            // Check if the file or directory exists
            if (File.Exists(path))
            {
                ExtractFile(path);
            }
            else if (Directory.Exists(path))
            {
                foreach (string file in path.SafeEnumerateFiles("*", SearchOption.AllDirectories))
                {
                    ExtractFile(file);
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
        private void ExtractFile(string file)
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
                if (Debug) Console.Error.WriteLine(ex);
                return;
            }

            // Get the file type
            WrapperType ft = WrapperFactory.GetFileType(magic, extension);
            var wrapper = WrapperFactory.CreateWrapper(ft, stream);

            // Create the output directory
            Directory.CreateDirectory(OutputPath);

            // Print the preamble
            Console.WriteLine($"Attempting to extract from '{wrapper?.Description() ?? "UNKNOWN"}'");
            Console.WriteLine();

            switch (wrapper)
            {
                // 7-zip
                case SevenZip sz:
                    sz.Extract(OutputPath, Debug);
                    break;

                // BFPK archive
                case BFPK bfpk:
                    bfpk.Extract(OutputPath, Debug);
                    break;

                // BSP
                case BSP bsp:
                    bsp.Extract(OutputPath, Debug);
                    break;

                // bzip2
                case BZip2 bzip2:
                    bzip2.Extract(OutputPath, Debug);
                    break;

                // CFB
                case CFB cfb:
                    cfb.Extract(OutputPath, Debug);
                    break;

                // GCF
                case GCF gcf:
                    gcf.Extract(OutputPath, Debug);
                    break;

                // gzip
                case GZip gzip:
                    gzip.Extract(OutputPath, Debug);
                    break;

                // InstallShield Archive V3 (Z)
                case InstallShieldArchiveV3 isv3:
                    isv3.Extract(OutputPath, Debug);
                    break;

                // IS-CAB archive
                case InstallShieldCabinet iscab:
                    iscab.Extract(OutputPath, Debug);
                    break;

                // LZ-compressed file, KWAJ variant
                case LZKWAJ kwaj:
                    kwaj.Extract(OutputPath, Debug);
                    break;

                // LZ-compressed file, QBasic variant
                case LZQBasic qbasic:
                    qbasic.Extract(OutputPath, Debug);
                    break;

                // LZ-compressed file, SZDD variant
                case LZSZDD szdd:
                    szdd.Extract(OutputPath, Debug);
                    break;

                // Microsoft Cabinet archive
                case MicrosoftCabinet mscab:
                    mscab.Extract(OutputPath, Debug);
                    break;

                // MoPaQ (MPQ) archive
                case MoPaQ mpq:
                    mpq.Extract(OutputPath, Debug);
                    break;

                // New Executable
                case NewExecutable nex:
                    nex.Extract(OutputPath, Debug);
                    break;

                // PAK
                case PAK pak:
                    pak.Extract(OutputPath, Debug);
                    break;

                // PFF
                case PFF pff:
                    pff.Extract(OutputPath, Debug);
                    break;

                // PKZIP
                case PKZIP pkzip:
                    pkzip.Extract(OutputPath, Debug);
                    break;

                // Portable Executable
                case PortableExecutable pex:
                    pex.Extract(OutputPath, Debug);
                    break;

                // Quantum
                case Quantum quantum:
                    quantum.Extract(OutputPath, Debug);
                    break;

                // RAR
                case RAR rar:
                    rar.Extract(OutputPath, Debug);
                    break;

                // SGA
                case SGA sga:
                    sga.Extract(OutputPath, Debug);
                    break;

                // Tape Archive
                case TapeArchive tar:
                    tar.Extract(OutputPath, Debug);
                    break;

                // VBSP
                case VBSP vbsp:
                    vbsp.Extract(OutputPath, Debug);
                    break;

                // VPK
                case VPK vpk:
                    vpk.Extract(OutputPath, Debug);
                    break;

                // WAD3
                case WAD3 wad:
                    wad.Extract(OutputPath, Debug);
                    break;

                // xz
                case XZ xz:
                    xz.Extract(OutputPath, Debug);
                    break;

                // XZP
                case XZP xzp:
                    xzp.Extract(OutputPath, Debug);
                    break;

                // Everything else
                default:
                    Console.WriteLine("Not a supported extractable file format, skipping...");
                    Console.WriteLine();
                    break;
            }
        }

        /// <summary>
        /// Validate the extraction path
        /// </summary>
        private bool ValidateExtractionPath()
        {
            // Null or empty output path
            if (string.IsNullOrEmpty(OutputPath))
            {
                Console.WriteLine("Output directory required for extraction!");
                Console.WriteLine();
                return false;
            }

            // Malformed output path or invalid location
            try
            {
                OutputPath = Path.GetFullPath(OutputPath);
                Directory.CreateDirectory(OutputPath);
            }
            catch
            {
                Console.WriteLine("Output directory could not be created!");
                Console.WriteLine();
                return false;
            }

            return true;
        }
    }
}
