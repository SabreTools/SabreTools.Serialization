using System;
using System.IO;
using SabreTools.Data.Models.AtariLynx;
using SabreTools.IO.Extensions;

namespace SabreTools.Serialization.Wrappers
{
    public partial class AtariLynxCart : IExtractable
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
                    fs.Write(Header.Magic);
                    fs.Flush();

                    // Bytes 4-5
                    fs.Write(Header.Bank0PageSize);
                    fs.Flush();

                    // Bytes 6-7
                    fs.Write(Header.Bank0PageSize);
                    fs.Flush();

                    // Bytes 8-9
                    fs.Write(Header.Version);
                    fs.Flush();

                    // Bytes 10-41
                    fs.Write(Header.CartName);
                    fs.Flush();

                    // Bytes 42-57
                    fs.Write(Header.Manufacturer);
                    fs.Flush();

                    // Byte 58
                    fs.Write((byte)Header.Rotation);
                    fs.Flush();

                    // Bytes 59-63
                    fs.Write(Header.Spare);
                    fs.Flush();

                    // Header extracted
                    success = true;
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
                    // Open the output file for writing
                    using var fs = File.Open(romPath, FileMode.Create, FileAccess.Write, FileShare.None);
                    fs.Write(Model.Data, 0, Model.Data.Length);
                    fs.Flush();

                    // ROM extracted
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
