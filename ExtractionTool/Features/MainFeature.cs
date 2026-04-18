using System;
using System.IO;
using SabreTools.CommandLine;
using SabreTools.CommandLine.Inputs;
using SabreTools.IO.Extensions;
using SabreTools.Numerics.Extensions;
using SabreTools.Wrappers;

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
            try
            {
                Console.WriteLine($"Attempting to extract all files from {file}");
                using Stream stream = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                // Read the first 32 bytes — needed to detect NintendoDisc magic at 0x18/0x1C
                byte[] magic = stream.PeekBytes(32);

                // Get the file type
                string extension = Path.GetExtension(file).TrimStart('.');
                WrapperType ft = WrapperFactory.GetFileType(magic ?? [], extension);

                // Print out the file format
                Console.WriteLine($"File format found: {ft}");

                // Setup the wrapper to extract
                var wrapper = WrapperFactory.CreateWrapper(ft, stream);

                // If we don't have a wrapper
                if (wrapper is null)
                {
                    Console.WriteLine($"Either {ft} is not supported or something went wrong during parsing!");
                    Console.WriteLine();
                    return;
                }

                // If the wrapper is not extractable
                if (wrapper is not IExtractable extractable)
                {
                    Console.WriteLine($"{ft} is not supported for extraction!");
                    Console.WriteLine();
                    return;
                }

                // Print the preamble
                Console.WriteLine($"Attempting to extract from '{wrapper.Description()}'");
                Console.WriteLine();

                // Attempt the extraction
                Directory.CreateDirectory(OutputPath);
                extractable.Extract(OutputPath, Debug);
            }
            catch (Exception ex)
            {
                Console.WriteLine(Debug ? ex : "[Exception opening file, please try again]");
                Console.WriteLine();
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
