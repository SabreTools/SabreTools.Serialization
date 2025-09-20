using System;
using System.IO;
using System.Text;
using SabreTools.Hashing;
using SabreTools.Models.SecuROM;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Wrappers
{
    public partial class SecuROMMatroschkaPackage : IExtractable
    {
        /// <inheritdoc/>
        public bool Extract(string outputDirectory, bool includeDebug)
        {
            // If we have no entries
            if (Entries == null || Entries.Length == 0)
                return false;

            // Loop through and extract all files to the output
            bool allExtracted = true;
            for (var i = 0; i < Entries.Length; i++)
            {
                allExtracted &= ExtractFile(i, outputDirectory, includeDebug);
            }

            return allExtracted;
        }

        /// <summary>
        /// Extract a file from the package to an output directory by index
        /// </summary>
        /// <param name="index">File index to extract</param>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>True if the file extracted, false otherwise</returns>
        public bool ExtractFile(int index, string outputDirectory, bool includeDebug)
        {
            // If we have no entries
            if (Entries == null || Entries.Length == 0)
                return false;

            // If the entry index is invalid
            if (index < 0 || index >= Entries.Length)
                return false;

            // Get the entry
            var entry = Entries[index];
            if (entry.Path == null)
                return false;

            // Ensure directory separators are consistent
            string filename = Encoding.ASCII.GetString(entry.Path).TrimEnd('\0');
            if (Path.DirectorySeparatorChar == '\\')
                filename = filename.Replace('/', '\\');
            else if (Path.DirectorySeparatorChar == '/')
                filename = filename.Replace('\\', '/');

            if (includeDebug) Console.WriteLine($"Attempting to extract {filename}");

            // Read the file
            var data = ReadFile(entry, includeDebug);
            if (data == null)
                return false;

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
                fs.Write(data, 0, data.Length);
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
        /// Read file and check bytes to be extracted against MD5 checksum
        /// </summary>
        /// <param name="entry">Entry being extracted</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>Byte array of the file data if successful, null otherwise</returns>
        private byte[]? ReadFile(MatroshkaEntry entry, bool includeDebug)
        {
            // Skip if the entry is incomplete
            if (entry.Path == null || entry.MD5 == null)
                return null;

            // Cache the expected MD5
            string expectedMd5 = BitConverter.ToString(entry.MD5);
            expectedMd5 = expectedMd5.ToLowerInvariant().Replace("-", string.Empty);

            // Debug output
            if (includeDebug) Console.WriteLine($"Offset: {entry.Offset:X8}, Expected Size: {entry.Size}, Expected MD5: {expectedMd5}");

            // Attempt to read from the offset
            var fileData = ReadRangeFromSource(entry.Offset, (int)entry.Size);
            if (fileData.Length == 0)
            {
                if (includeDebug) Console.Error.WriteLine($"Could not read {entry.Size} bytes from {entry.Offset:X8}");
                return null;
            }

            // Get the actual MD5 of the data
            string actualMd5 = HashTool.GetByteArrayHash(fileData, HashType.MD5) ?? string.Empty;

            // Debug output
            if (includeDebug) Console.WriteLine($"Actual MD5: {actualMd5}");

            // Do not return on a hash mismatch
            if (actualMd5 != expectedMd5)
            {
                string filename = Encoding.ASCII.GetString(entry.Path).TrimEnd('\0');
                if (includeDebug) Console.Error.WriteLine($"MD5 checksum failure for file {filename})");
                return null;
            }

            return fileData;
        }
    }
}