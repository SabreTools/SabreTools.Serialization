using System;
using System.IO;
using SabreTools.IO.Compression.Deflate;

namespace SabreTools.Serialization.Wrappers
{
    public partial class BFPK : IExtractable
    {
        /// <inheritdoc/>
        public bool Extract(string outputDirectory, bool includeDebug)
        {
            // If we have no files
            if (Files == null || Files.Length == 0)
                return false;

            // Loop through and extract all files to the output
            bool allExtracted = true;
            for (int i = 0; i < Files.Length; i++)
            {
                allExtracted &= ExtractFile(i, outputDirectory, includeDebug);
            }

            return allExtracted;
        }

        /// <summary>
        /// Extract a file from the BFPK to an output directory by index
        /// </summary>
        /// <param name="index">File index to extract</param>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>True if the file extracted, false otherwise</returns>
        public bool ExtractFile(int index, string outputDirectory, bool includeDebug)
        {
            // If we have no files
            if (Files == null || Files.Length == 0)
                return false;

            // If we have an invalid index
            if (index < 0 || index >= Files.Length)
                return false;

            // Get the file information
            var file = Files[index];
            if (file == null)
                return false;

            // Get the read index and length
            int offset = file.Offset + 4;
            int compressedSize = file.CompressedSize;

            // Some files can lack the length prefix
            if (compressedSize > Length)
            {
                offset -= 4;
                compressedSize = file.UncompressedSize;
            }

            // If we have an invalid output directory
            if (string.IsNullOrEmpty(outputDirectory))
                return false;

            // Ensure directory separators are consistent
            string filename = file.Name.Length == 0 ? $"file{index}" : file.Name;
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

                // Read the data block
                var data = ReadRangeFromSource(offset, compressedSize);
                if (data.Length == 0)
                    return false;

                // If we have uncompressed data
                if (compressedSize == file.UncompressedSize)
                {
                    fs.Write(data, 0, compressedSize);
                    fs.Flush();
                }
                else
                {
                    using var ms = new MemoryStream(data);
                    using var zs = new ZlibStream(ms, CompressionMode.Decompress);
                    zs.CopyTo(fs);
                    fs.Flush();
                }
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
