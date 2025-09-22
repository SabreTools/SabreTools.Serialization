using System;
using System.IO;
using SabreTools.IO.Compression.SZDD;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Wrappers
{
    public partial class LZKWAJ : IExtractable
    {
        /// <inheritdoc/>
        public bool Extract(string outputDirectory, bool includeDebug)
        {
            // Get the length of the compressed data
            long compressedSize = Length - DataOffset;
            if (compressedSize < DataOffset)
                return false;

            // Read in the data as an array
            byte[]? contents = ReadRangeFromSource(DataOffset, (int)compressedSize);
            if (contents.Length == 0)
                return false;

            // Get the decompressor
            var decompressor = Decompressor.CreateKWAJ(contents, CompressionType);
            if (decompressor == null)
                return false;

            // If we have an invalid output directory
            if (string.IsNullOrEmpty(outputDirectory))
                return false;

            // Create the full output path
            string filename = FileName ?? "tempfile";
            if (FileExtension != null)
                filename += $".{FileExtension}";

            // Ensure directory separators are consistent
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
