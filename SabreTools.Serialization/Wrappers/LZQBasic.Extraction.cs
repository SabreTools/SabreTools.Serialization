using System;
using System.IO;
using SabreTools.IO.Compression.SZDD;

namespace SabreTools.Serialization.Wrappers
{
    public partial class LZQBasic : IExtractable
    {
        /// <inheritdoc/>
        public bool Extract(string outputDirectory, bool includeDebug)
        {
            // Get the length of the compressed data
            long compressedSize = Length - 12;
            if (compressedSize < 12)
                return false;

            // Read in the data as an array
            byte[]? contents = ReadRangeFromSource(12, (int)compressedSize);
            if (contents.Length == 0)
                return false;

            // Get the decompressor
            var decompressor = Decompressor.CreateQBasic(contents);
            if (decompressor == null)
                return false;

            // If we have an invalid output directory
            if (string.IsNullOrEmpty(outputDirectory))
                return false;

            // Ensure directory separators are consistent
            string filename = "tempfile.bin";
            if (Path.DirectorySeparatorChar == '\\')
                filename = filename.Replace('/', '\\');
            else if (Path.DirectorySeparatorChar == '/')
                filename = filename.Replace('\\', '/');

            // Ensure the full output directory exists
            filename = Path.Combine(outputDirectory, filename);
            var directoryName = Path.GetDirectoryName(filename);
            if (directoryName != null && !Directory.Exists(directoryName))
                Directory.CreateDirectory(directoryName);

            // Try to write the data
            try
            {
                // Open the output file for writing
                using var fs = File.Open(filename, FileMode.Create, FileAccess.Write, FileShare.None);
                decompressor.CopyTo(fs);
                fs.Flush();
            }
            catch (Exception ex)
            {
                if (includeDebug) Console.Error.WriteLine(ex);
                return false;
            }

            return true;
        }
    }
}
