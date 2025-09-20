using System;
using System.Collections.Generic;
using System.IO;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Wrappers
{
    public partial class GCF : IExtractable
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
        /// Extract a file from the GCF to an output directory by index
        /// </summary>
        /// <param name="index">File index to extract</param>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>True if the file extracted, false otherwise</returns>
        public bool ExtractFile(int index, string outputDirectory, bool includeDebug)
        {
            // If we have no files
            if (Files == null || Files.Length == 0 || DataBlockOffsets == null)
                return false;

            // If the files index is invalid
            if (index < 0 || index >= Files.Length)
                return false;

            // Get the file
            var file = Files[index];
            if (file?.BlockEntries == null || file.Size == 0)
                return false;

            // If the file is encrypted -- TODO: Revisit later
            if (file.Encrypted)
                return false;

            // Get all data block offsets needed for extraction
            var dataBlockOffsets = new List<long>();
            for (int i = 0; i < file.BlockEntries.Length; i++)
            {
                var blockEntry = file.BlockEntries[i];

                uint dataBlockIndex = blockEntry.FirstDataBlockIndex;
                long blockEntrySize = blockEntry.FileDataSize;
                while (blockEntrySize > 0)
                {
                    long dataBlockOffset = DataBlockOffsets[dataBlockIndex++];
                    dataBlockOffsets.Add(dataBlockOffset);
                    blockEntrySize -= BlockSize;
                }
            }

            // If we have an invalid output directory
            if (string.IsNullOrEmpty(outputDirectory))
                return false;

            // Ensure directory separators are consistent
            string filename = file.Path ?? $"file{index}";
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
                using Stream fs = File.OpenWrite(filename);

                // Now read the data sequentially and write out while we have data left
                long fileSize = file.Size;
                for (int i = 0; i < dataBlockOffsets.Count; i++)
                {
                    int readSize = (int)Math.Min(BlockSize, fileSize);
                    var data = ReadRangeFromSource((int)dataBlockOffsets[i], readSize);
                    if (data.Length == 0)
                        return false;

                    fs.Write(data, 0, data.Length);
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
