using System;
using System.IO;
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
            // Extract the packaged files
            var extracted = ExtractPackagedFiles(outputDirectory, includeDebug);
            if (!extracted)
            {
                if (includeDebug) Console.Error.WriteLine("Could not extract packaged files");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Extract the packaged files.
        /// </summary>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>True if the files extracted successfully, false otherwise</returns>
        private bool ExtractPackagedFiles(string outputDirectory, bool includeDebug)
        {
                if (Entries == null)
                    return false;
                
                var successful = true;

                // Extract entries
                for (var i = 0; i < Entries.Length; i++)
                {
                    var entry = Entries[i];
                    
                    // Extract file
                    if (!ExtractFile(entry, outputDirectory, includeDebug))
                        successful = false;
                }
                
                return successful;
        }

        /// <summary>
        /// Attempt to extract a file
        /// </summary>
        /// <param name="entry">Matroschka file entry being extracted</param>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>Boolean representing true on success or false on failure</returns>
        /// <remarks>Assumes that the current stream position is the end of where the data lives</remarks>
        private bool ExtractFile(MatroshkaEntry entry, string outputDirectory, bool includeDebug)
        {
            if (entry.Path == null)
                return false;

            var filename = System.Text.Encoding.ASCII.GetString(entry.Path).TrimEnd('\0');

            // Ensure directory separators are consistent
            if (Path.DirectorySeparatorChar == '\\')
                filename = filename.Replace('/', '\\');
            else if (Path.DirectorySeparatorChar == '/')
                filename = filename.Replace('\\', '/');

            if (includeDebug) Console.WriteLine($"Attempting to extract {filename}");

            // Extract the file
            var fileData = ReadRangeFromSource(entry.Offset, (int)entry.Size); // TODO: safety? validation? anything?
            var status = CheckBytes(entry, fileData, includeDebug);

            if (!status)
                return false;

            // Ensure the full output directory exists
            filename = Path.Combine(outputDirectory, filename);
            var directoryName = Path.GetDirectoryName(filename);
            if (directoryName != null && !Directory.Exists(directoryName))
                Directory.CreateDirectory(directoryName);

            // Write the output file
            File.WriteAllBytes(filename, fileData);
            return status;
        }

        /// <summary>
        /// Check bytes to be extracted against MD5 checksum.
        /// </summary>
        /// <param name="entry">Entry being extracted</param>
        /// <param name="fileData">File data being extracted</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns></returns>
        private bool CheckBytes(MatroshkaEntry entry, byte[] fileData, bool includeDebug)
        {
            // Debug output
            if (includeDebug) Console.WriteLine($"Offset: {entry.Offset:X8}, Expected Size: {entry.Size}");

            string expectedMd5 = BitConverter.ToString(entry.MD5!);
            expectedMd5 = expectedMd5.ToLowerInvariant().Replace("-", string.Empty);

            // Debug output
            if (includeDebug) Console.WriteLine($"Expected MD5: {expectedMd5}");

            if (fileData == null)
                return false;

            var hashBytes = HashTool.GetByteArrayHashArray(fileData, HashType.MD5);

            string actualMd5 = BitConverter.ToString(hashBytes!);
            actualMd5 = actualMd5.ToLowerInvariant().Replace("-", string.Empty);
            
            // Debug output
            if (includeDebug) Console.WriteLine($"Actual MD5: {actualMd5}");
                
            if (hashBytes == null || actualMd5 != expectedMd5)
            {
                var filename = System.Text.Encoding.ASCII.GetString(entry.Path!).TrimEnd('\0');
                Console.Error.WriteLine($"MD5 checksum failure for file {filename})");
                return false;
            }


            return true;
        }
    }
}