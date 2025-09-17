using System;
using System.IO;
using SabreTools.Hashing;
using SabreTools.IO.Compression.Deflate;
using SabreTools.Matching;
using SabreTools.Models.SecuROM;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Wrappers
{
    public partial class SecuROMMatroschkaPackage : IExtractable
    {
        /// <inheritdoc/>
        public bool Extract(string outputDirectory, bool includeDebug)
        {
            // Extract the header-defined files
            bool extracted = ExtractHeaderDefinedFiles(outputDirectory, includeDebug);
            if (!extracted)
            {
                if (includeDebug) Console.Error.WriteLine("Could not extract header-defined files");
                return false;
            }

            return true;
        }

        // Currently unaware of any NE samples. That said, as they wouldn't have a .WISE section, it's unclear how such
        // samples could be identified.

        /// <summary>
        /// Extract the predefined, static files defined in the header
        /// </summary>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>True if the files extracted successfully, false otherwise</returns>
        private bool ExtractHeaderDefinedFiles(string outputDirectory, bool includeDebug)
        {
            lock (_dataSourceLock)
            {
                if (Entries == null)
                    return false;
                
                bool successful = true;

                //Extract entries
                for (int i = 0; i < Entries.Length; i++)
                {
                    MatroshkaEntry entry = Entries[i];
                    if (FileDataArray == null)
                    {
                        return false;
                    }

                    var fileData = FileDataArray[i];
                    

                    // Extract file
                    if (ExtractFile(entry, fileData, outputDirectory, includeDebug) !=
                        ExtractionStatus.GOOD)
                        successful = false;
                }
                
                return successful;
            }
        }

        /// <summary>
        /// Attempt to extract a file defined by a filename
        /// </summary>
        /// <param name="entry">Entry being extracted</param>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>Extraction status representing the final state</returns>
        /// <remarks>Assumes that the current stream position is the end of where the data lives</remarks>
        private ExtractionStatus ExtractFile(MatroshkaEntry entry,
            byte[] fileData,
            string outputDirectory,
            bool includeDebug)
        {
            if (entry.Path == null)
                return ExtractionStatus.INVALID;
            
            if (fileData == null)
                return ExtractionStatus.INVALID;

            string filename = System.Text.Encoding.ASCII.GetString(entry.Path);
            // Ensure directory separators are consistent
            if (Path.DirectorySeparatorChar == '\\')
                filename = filename.Replace('/', '\\');
            else if (Path.DirectorySeparatorChar == '/')
                filename = filename.Replace('\\', '/');

            if (includeDebug) Console.WriteLine($"Attempting to extract {filename}");

            // Extract the file
            ExtractionStatus status;
            status = CheckBytes(entry, fileData, includeDebug);

            // If the extracted data is invalid
            if (status != ExtractionStatus.GOOD)
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
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns></returns>
        private ExtractionStatus CheckBytes(MatroshkaEntry entry, byte[] fileData, bool includeDebug)
        {
            // Debug output
            if (includeDebug) Console.WriteLine($"Offset: {entry.Offset:X8}, Expected Size: {entry.Size}");

            if (entry.MD5 == null)
            {
                return ExtractionStatus.INVALID;
            }
            
#if NET5_0_OR_GREATER
            string expectedMD5 = Convert.ToHexString(entry.MD5).ToUpper(); // TODO: is ToUpper right?
#else
                string expectedMD5 = BitConverter.ToString(entry.MD5).Replace("-",""); // TODO: endianness?
#endif
            
            // Debug output
            if (includeDebug) Console.WriteLine($"Expected MD5: {expectedMD5}");

            if (fileData == null)
            {
                return ExtractionStatus.INVALID;
            }

            byte[]? hashBytes = HashTool.GetByteArrayHashArray(fileData, HashType.MD5);
            if (hashBytes != null)
            {
#if NET5_0_OR_GREATER
                string actualMD5 = Convert.ToHexString(hashBytes).ToUpper(); // TODO: is ToUpper right?
#else
                string actualMD5 = BitConverter.ToString(hashBytes).Replace("-",""); // TODO: endianness?
#endif

                // Debug output
                if (includeDebug) Console.WriteLine($"Actual MD5: {actualMD5}");
                
                if (!hashBytes.EqualsExactly(entry.MD5))
                {
                    if (includeDebug) Console.Error.WriteLine("Mismatched MD5 values!");
                    return ExtractionStatus.INVALID;
                }
            }
            
            return ExtractionStatus.GOOD;
        }
    }
}