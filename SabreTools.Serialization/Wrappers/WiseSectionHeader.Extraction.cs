using System;
using System.IO;
using SabreTools.Hashing;
using SabreTools.IO.Compression.Deflate;
using SabreTools.IO.Extensions;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Wrappers
{
    public partial class WiseSectionHeader : IExtractable
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
                // Seek to the compressed data offset
                _dataSource.Seek(CompressedDataOffset, SeekOrigin.Begin);
                bool successful = true;

                // Extract first executable, if it exists
                if (ExtractFile("FirstExecutable.exe", outputDirectory, FirstExecutableFileEntryLength, includeDebug) != ExtractionStatus.GOOD)
                    successful = false;

                // Extract second executable, if it exists
                // If there's a size provided for the second executable but no size for the first executable, the size of
                // the second executable appears to be some unrelated value that's larger than the second executable
                // actually is. Currently unable to extract properly in these cases, as no header value in such installers
                // seems to actually correspond to the real size of the second executable.
                if (ExtractFile("SecondExecutable.exe", outputDirectory, SecondExecutableFileEntryLength, includeDebug) != ExtractionStatus.GOOD)
                    successful = false;

                // Extract third executable, if it exists
                if (ExtractFile("ThirdExecutable.exe", outputDirectory, ThirdExecutableFileEntryLength, includeDebug) != ExtractionStatus.GOOD)
                    successful = false;

                // Extract main MSI file
                if (ExtractFile("ExtractedMsi.msi", outputDirectory, MsiFileEntryLength, includeDebug) != ExtractionStatus.GOOD)
                {
                    // Fallback- seek to the position that's the length of the MSI file entry from the end, then try and
                    // extract from there.
                    _dataSource.Seek(-MsiFileEntryLength + 1, SeekOrigin.End);
                    if (ExtractFile("ExtractedMsi.msi", outputDirectory, MsiFileEntryLength, includeDebug) != ExtractionStatus.GOOD)
                        return false; // The fallback also failed.
                }

                return successful;
            }
        }

        /// <summary>
        /// Attempt to extract a file defined by a filename
        /// </summary>
        /// <param name="filename">Output filename, null to auto-generate</param>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <param name="entrySize">Expected size of the file plus crc32</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>Extraction status representing the final state</returns>
        /// <remarks>Assumes that the current stream position is the end of where the data lives</remarks>
        private ExtractionStatus ExtractFile(string filename,
            string outputDirectory,
            uint entrySize,
            bool includeDebug)
        {
            if (includeDebug) Console.WriteLine($"Attempting to extract {filename}");

            // Extract the file
            var destination = new MemoryStream();
            ExtractionStatus status;
            if (!(Version != null && Version[1] == 0x01))
            {
                status = ExtractStreamWithChecksum(destination, entrySize, includeDebug);
            }
            else // hack for Codesited5.exe , very early and very strange.
            {
                status = ExtractStreamWithoutChecksum(destination, entrySize, includeDebug);
            }

            // If the extracted data is invalid
            if (status != ExtractionStatus.GOOD || destination == null)
                return status;

            // Ensure the full output directory exists
            filename = Path.Combine(outputDirectory, filename);
            var directoryName = Path.GetDirectoryName(filename);
            if (directoryName != null && !Directory.Exists(directoryName))
                Directory.CreateDirectory(directoryName);

            // Write the output file
            File.WriteAllBytes(filename, destination.ToArray());
            return status;
        }

        /// <summary>
        /// Extract source data with a trailing CRC-32 checksum
        /// </summary>
        /// <param name="destination">Stream where the file data will be written</param>
        /// <param name="entrySize">Expected size of the file plus crc32</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns></returns>
        private ExtractionStatus ExtractStreamWithChecksum(Stream destination, uint entrySize, bool includeDebug)
        {
            // Debug output
            if (includeDebug) Console.WriteLine($"Offset: {_dataSource.Position:X8}, Expected Read: {entrySize}, Expected Write:{entrySize - 4}"); // clamp to zero

            // Check the validity of the inputs
            if (entrySize == 0)
            {
                if (includeDebug) Console.Error.WriteLine("Not attempting to extract, expected to read 0 bytes");
                return ExtractionStatus.GOOD; // If size is 0, then it shouldn't be extracted
            }
            else if (entrySize > (_dataSource.Length - _dataSource.Position))
            {
                if (includeDebug) Console.Error.WriteLine($"Not attempting to extract, expected to read {entrySize} bytes but only {_dataSource.Position} bytes remain");
                return ExtractionStatus.INVALID;
            }

            // Extract the file
            try
            {
                byte[] actual = _dataSource.ReadBytes((int)entrySize - 4);
                uint expectedCrc32 = _dataSource.ReadUInt32();

                // Debug output
                if (includeDebug) Console.WriteLine($"Expected CRC-32: {expectedCrc32:X8}");

                byte[]? hashBytes = HashTool.GetByteArrayHashArray(actual, HashType.CRC32);
                if (hashBytes != null)
                {
                    uint actualCrc32 = BitConverter.ToUInt32(hashBytes, 0);

                    // Debug output
                    if (includeDebug) Console.WriteLine($"Actual CRC-32: {actualCrc32:X8}");

                    if (expectedCrc32 != actualCrc32)
                    {
                        if (includeDebug) Console.Error.WriteLine("Mismatched CRC-32 values!");
                        return ExtractionStatus.BAD_CRC;
                    }
                }

                destination.Write(actual, 0, actual.Length);
                return ExtractionStatus.GOOD;
            }
            catch
            {
                if (includeDebug) Console.Error.WriteLine("Could not extract");
                return ExtractionStatus.FAIL;
            }
        }

        /// <summary>
        /// Extract source data without a trailing CRC-32 checksum
        /// </summary>
        /// <param name="destination">Stream where the file data will be written</param>
        /// <param name="entrySize">Expected size of the file</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns></returns>
        private ExtractionStatus ExtractStreamWithoutChecksum(Stream destination, uint entrySize, bool includeDebug)
        {
            // Debug output
            if (includeDebug) Console.WriteLine($"Offset: {_dataSource.Position:X8}, Expected Read: {entrySize}, Expected Write:{entrySize - 4}");

            // Check the validity of the inputs
            if (entrySize == 0)
            {
                if (includeDebug) Console.Error.WriteLine("Not attempting to extract, expected to read 0 bytes");
                return ExtractionStatus.GOOD; // If size is 0, then it shouldn't be extracted
            }
            else if (entrySize > (_dataSource.Length - _dataSource.Position))
            {
                if (includeDebug) Console.Error.WriteLine($"Not attempting to extract, expected to read {entrySize} bytes but only {_dataSource.Position} bytes remain");
                return ExtractionStatus.INVALID;
            }

            // Extract the file
            try
            {
                byte[] actual = _dataSource.ReadBytes((int)entrySize);

                // Debug output
                if (includeDebug) Console.WriteLine("No CRC-32!");

                destination.Write(actual, 0, actual.Length);
                return ExtractionStatus.GOOD;
            }
            catch
            {
                if (includeDebug) Console.Error.WriteLine("Could not extract");
                return ExtractionStatus.FAIL;
            }
        }
    }
}
