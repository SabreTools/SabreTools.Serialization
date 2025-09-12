using System;
using System.IO;
using System.Text;
using SabreTools.IO.Extensions;
using SabreTools.Models.CFB;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Wrappers
{
    public partial class CFB : IExtractable
    {
        /// <inheritdoc/>
        public bool Extract(string outputDirectory, bool includeDebug)
        {
            // If we have no files
            if (DirectoryEntries == null || DirectoryEntries.Length == 0)
                return false;

            // Loop through and extract all directory entries to the output
            bool allExtracted = true;
            for (int i = 0; i < DirectoryEntries.Length; i++)
            {
                allExtracted &= ExtractEntry(i, outputDirectory, includeDebug);
            }

            return allExtracted;
        }

        /// <summary>
        /// Extract a file from the CFB to an output directory by index
        /// </summary>
        /// <param name="index">Entry index to extract</param>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>True if the file extracted, false otherwise</returns>
        public bool ExtractEntry(int index, string outputDirectory, bool includeDebug)
        {
            // If we have no entries
            if (DirectoryEntries == null || DirectoryEntries.Length == 0)
                return false;

            // If we have an invalid index
            if (index < 0 || index >= DirectoryEntries.Length)
                return false;

            // Get the entry information
            var entry = DirectoryEntries[index];
            if (entry == null)
                return false;

            // Only try to extract stream objects
            if (entry.ObjectType != ObjectType.StreamObject)
                return true;

            // Get the entry data
            byte[]? data = GetDirectoryEntryData(entry);
            if (data == null)
                return false;

            // If we have an invalid output directory
            if (string.IsNullOrEmpty(outputDirectory))
                return false;

            // Ensure the output filename is trimmed
            string filename = entry.Name ?? $"entry{index}";
            byte[] nameBytes = Encoding.UTF8.GetBytes(filename);
            if (nameBytes[0] == 0xe4 && nameBytes[1] == 0xa1 && nameBytes[2] == 0x80)
                filename = Encoding.UTF8.GetString(nameBytes, 3, nameBytes.Length - 3);

            foreach (char c in Path.GetInvalidFileNameChars())
            {
                filename = filename.Replace(c, '_');
            }

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
                using FileStream fs = File.OpenWrite(filename);
                fs.Write(data);
                fs.Flush();
            }
            catch (Exception ex)
            {
                if (includeDebug) Console.Error.WriteLine(ex);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Read the entry data for a single directory entry, if possible
        /// </summary>
        /// <param name="entry">Entry to try to retrieve data for</param>
        /// <returns>Byte array representing the entry data on success, null otherwise</returns>
        private byte[]? GetDirectoryEntryData(DirectoryEntry entry)
        {
            // If the CFB is invalid
            if (Header == null)
                return null;

            // Only try to extract stream objects
            if (entry.ObjectType != ObjectType.StreamObject)
                return null;

            // Determine which FAT is being used
            bool miniFat = entry.StreamSize < Header.MiniStreamCutoffSize;

            // Get the chain data
            var chain = miniFat
                ? GetMiniFATSectorChainData((SectorNumber)entry.StartingSectorLocation)
                : GetFATSectorChainData((SectorNumber)entry.StartingSectorLocation);
            if (chain == null)
                return null;

            // Return only the proper amount of data
            byte[] data = new byte[entry.StreamSize];
            Array.Copy(chain, 0, data, 0, (int)Math.Min(chain.Length, (long)entry.StreamSize));
            return data;
        }
    }
}
