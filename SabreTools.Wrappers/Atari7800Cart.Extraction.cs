using System;
using System.IO;

namespace SabreTools.Wrappers
{
    public partial class Atari7800Cart : IExtractable
    {
        /// <inheritdoc/>
        public bool Extract(string outputDirectory, bool includeDebug)
        {
            // Get the base path
            string baseFilename = Filename is null
                ? Guid.NewGuid().ToString()
                : Path.GetFileNameWithoutExtension(Filename);
            string basePath = Path.Combine(outputDirectory, baseFilename);

            // Check if any data was extracted successfully
            bool success = false;

            // Header data
            if (Header is not null)
            {
                string headerPath = $"{basePath}.hdr";
                if (includeDebug) Console.WriteLine($"Attempting to extract header data to {headerPath}");

                // Try to write the data
                try
                {
                    using var fs = File.Open(headerPath, FileMode.Create, FileAccess.Write, FileShare.None);
                    success |= WriteHeader(fs, includeDebug);
                }
                catch (Exception ex)
                {
                    if (includeDebug) Console.Error.WriteLine(ex);
                }
            }

            // ROM data
            if (Model.Data.Length > 0)
            {
                string romPath = $"{basePath}.bin";
                if (includeDebug) Console.WriteLine($"Attempting to extract ROM data to {romPath}");

                // Try to write the data
                try
                {
                    using var fs = File.Open(romPath, FileMode.Create, FileAccess.Write, FileShare.None);
                    success |= WriteRom(fs, includeDebug);
                }
                catch (Exception ex)
                {
                    if (includeDebug) Console.Error.WriteLine(ex);
                }
            }

            return success;
        }
    }
}
