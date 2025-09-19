using System;
using System.IO;
using SabreTools.Hashing;
using SabreTools.Matching;
using SabreTools.Models.SecuROM;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Wrappers
{
    public partial class SecuROMMatroschkaPackage : IExtractable
    {
        /// <inheritdoc/>
        // TODO: I don't really know how to make use of this since I need to pass in fileDataArray, but I need it here for
        // TODO: IExtractable. I assume you'll probably not approve of fileDataArray to begin with and just want me to use
        // TODO: offsets, so let me know if that's indeed the case.
        public bool Extract(string outputDirectory, bool includeDebug)
        {
            // Extract the packaged files
            var extracted = ExtractPackagedFiles(outputDirectory, includeDebug, null);
            if (!extracted)
            {
                if (includeDebug) Console.Error.WriteLine("Could not extract header-defined files");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Extract the predefined, static files defined in the header
        /// </summary>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <param name="fileDataArray">File data array being extracted</param>

        /// <returns>True if the files extracted successfully, false otherwise</returns>
        public bool ExtractPackagedFiles(string outputDirectory, bool includeDebug, byte[][]? fileDataArray)
        {
                if (Entries == null)
                    return false;
                
                var successful = true;

                //Extract entries
                for (var i = 0; i < Entries.Length; i++)
                {
                    var entry = Entries[i];
                    if (fileDataArray == null)
                        return false;

                    var fileData = fileDataArray[i];
                    

                    // Extract file
                    if (!ExtractFile(entry, fileData, outputDirectory, includeDebug))
                        successful = false;
                }
                
                return successful;
        }

        /// <summary>
        /// Attempt to extract a file
        /// </summary>
        /// <param name="entry">Matroschka file entry being extracted</param>
        /// <param name="fileData">File data being extracted</param>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>Boolean representing true on success or false on failure</returns>
        /// <remarks>Assumes that the current stream position is the end of where the data lives</remarks>
        private bool ExtractFile(MatroshkaEntry entry,
            byte[] fileData,
            string outputDirectory,
            bool includeDebug)
        {
            if (entry.Path == null)
                return false;
            
            if (fileData == null)
                return false;

            var filename = System.Text.Encoding.ASCII.GetString(entry.Path).TrimEnd('\0');
            // Ensure directory separators are consistent
            if (Path.DirectorySeparatorChar == '\\')
                filename = filename.Replace('/', '\\');
            else if (Path.DirectorySeparatorChar == '/')
                filename = filename.Replace('\\', '/');

            if (includeDebug) Console.WriteLine($"Attempting to extract {filename}");

            // Extract the file
            bool status;
            status = CheckBytes(entry, fileData, includeDebug);

            // If the extracted data is invalid
            if (!status)
                return status;

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

            if (entry.MD5 == null)
                return false;
            
#if NET5_0_OR_GREATER
            var expectedMD5 = Convert.ToHexString(entry.MD5).ToUpper(); // TODO: is ToUpper right?
#else
                var expectedMD5 = BitConverter.ToString(entry.MD5).Replace("-",""); // TODO: endianness?
#endif
            
            // Debug output
            if (includeDebug) Console.WriteLine($"Expected MD5: {expectedMD5}");

            if (fileData == null)
                return false;

            var hashBytes = HashTool.GetByteArrayHashArray(fileData, HashType.MD5);
            if (hashBytes != null)
            {
#if NET5_0_OR_GREATER
                var actualMD5 = Convert.ToHexString(hashBytes).ToUpper(); // TODO: is ToUpper right?
#else
                var actualMD5 = BitConverter.ToString(hashBytes).Replace("-",""); // TODO: endianness?
#endif

                // Debug output
                if (includeDebug) Console.WriteLine($"Actual MD5: {actualMD5}");
                
                if (!hashBytes.EqualsExactly(entry.MD5))
                {
                    if (includeDebug) Console.Error.WriteLine("Mismatched MD5 values!");
                    return false;
                }
            }
            
            return true;
        }
    }
}