using System;
using System.IO;

namespace ExtractionTool
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
        public bool Debug { get; set; }

        /// <summary>
        /// Output path for archive extraction
        /// </summary>
        public string OutputPath { get; set; } = string.Empty;

        #endregion

        /// <summary>
        /// Validate the extraction path
        /// </summary>
        public bool ValidateExtractionPath()
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