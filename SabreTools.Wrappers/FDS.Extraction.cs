using System;
using System.IO;
using SabreTools.Numerics.Extensions;

namespace SabreTools.Wrappers
{
    public partial class FDS : IExtractable
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
                    // Open the output file for writing
                    using var fs = File.Open(headerPath, FileMode.Create, FileAccess.Write, FileShare.None);

                    // Bytes 0-3
                    fs.Write(Header.IdentificationString);
                    fs.Flush();

                    // Byte 4
                    fs.Write(Header.DiskSides);
                    fs.Flush();

                    // Byte 5-15
                    fs.Write(Header.Padding);
                    fs.Flush();

                    // Header extracted
                    success = true;
                }
                catch (Exception ex)
                {
                    if (includeDebug) Console.Error.WriteLine(ex);
                }
            }

            // Disk data
            // TODO: Convert to QD format?
            if (Model.Data.Length > 0)
            {
                string diskPath = $"{basePath}.unh";
                if (includeDebug) Console.WriteLine($"Attempting to extract disk data to {diskPath}");

                // Try to write the data
                try
                {
                    // Open the output file for writing
                    using var fs = File.Open(diskPath, FileMode.Create, FileAccess.Write, FileShare.None);
                    fs.Write(Model.Data, 0, Model.Data.Length);
                    fs.Flush();

                    // PRG-ROM extracted
                    success = true;
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
