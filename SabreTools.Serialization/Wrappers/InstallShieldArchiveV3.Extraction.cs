using System;
using System.IO;
using SabreTools.IO.Compression.Blast;

namespace SabreTools.Serialization.Wrappers
{
    /// <remarks>
    /// Reference (de)compressor: https://www.sac.sk/download/pack/icomp95.zip
    /// </remarks>
    /// <see href="https://github.com/wfr/unshieldv3"/>
    public partial class InstallShieldArchiveV3 : IExtractable
    {
        /// <inheritdoc/>
        public bool Extract(string outputDirectory, bool includeDebug)
        {
            // Get the file count
            int fileCount = Files.Length;
            if (fileCount == 0)
                return false;

            // Loop through and extract all files to the output
            bool allExtracted = true;
            for (int i = 0; i < fileCount; i++)
            {
                allExtracted &= ExtractFile(i, outputDirectory, includeDebug);
            }

            return allExtracted;
        }

        /// <summary>
        /// Extract a file from the ISAv3 to an output directory by index
        /// </summary>
        /// <param name="index">File index to extract</param>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>True if the file extracted, false otherwise</returns>
        public bool ExtractFile(int index, string outputDirectory, bool includeDebug)
        {
            // If the files index is invalid
            if (index < 0 || index >= FileCount)
                return false;

            // Get the file
            var file = Files[index];
            if (file == null)
                return false;

            // Create the filename
            var filename = file.Name;
            if (filename == null)
                return false;

            // Get the directory index
            int dirIndex = FileDirMap[index];
            if (dirIndex < 0 || dirIndex > DirCount)
                return false;

            // Get the directory name
            var dirName = Directories[dirIndex].Name;
            if (dirName != null)
                filename = Path.Combine(dirName, filename);

            // Get and adjust the file offset
            long fileOffset = file.Offset + DataStart;
            if (fileOffset < 0 || fileOffset >= Length)
                return false;

            // Get the file sizes
            long fileSize = file.CompressedSize;
            long outputFileSize = file.UncompressedSize;

            // Read the compressed data directly
            var compressedData = ReadRangeFromSource((int)fileOffset, (int)fileSize);
            if (compressedData.Length == 0)
                return false;

            // If the compressed and uncompressed sizes match
            byte[] data;
            if (fileSize == outputFileSize)
            {
                data = compressedData;
            }
            else
            {
                // Decompress the data
                var decomp = Decompressor.Create();
                using var outData = new MemoryStream();
                decomp.CopyTo(compressedData, outData);
                data = outData.ToArray();
            }

            // If we have an invalid output directory
            if (string.IsNullOrEmpty(outputDirectory))
                return false;

            // Ensure directory separators are consistent
            if (Path.DirectorySeparatorChar == '\\')
                filename = filename.Replace('/', '\\');
            else if (Path.DirectorySeparatorChar == '/')
                filename = filename.Replace('\\', '/');

            // Ensure the full output directory exists
            filename = Path.Combine(outputDirectory, filename);
            var directoryName = Path.GetDirectoryName(filename);
            if (directoryName != null && !System.IO.Directory.Exists(directoryName))
                System.IO.Directory.CreateDirectory(directoryName);

            // Try to write the data
            try
            {
                // Open the output file for writing
                using var fs = File.Open(filename, FileMode.Create, FileAccess.Write, FileShare.None);
                fs.Write(data, 0, data.Length);
                fs.Flush();
            }
            catch (Exception ex)
            {
                if (includeDebug) Console.Error.WriteLine(ex);
                return false;
            }

            return false;
        }
    }
}
