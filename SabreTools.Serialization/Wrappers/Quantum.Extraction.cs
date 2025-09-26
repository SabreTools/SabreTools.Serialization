using System;

namespace SabreTools.Serialization.Wrappers
{
    public partial class Quantum : IExtractable
    {
        /// <inheritdoc/>
        public bool Extract(string outputDirectory, bool includeDebug)
        {
            // If we have no files
            if (FileList == null || FileList.Length == 0)
                return false;

            // Loop through and extract all files to the output
            bool allExtracted = true;
            for (int i = 0; i < FileList.Length; i++)
            {
                allExtracted &= ExtractFile(i, outputDirectory, includeDebug);
            }

            return allExtracted;
        }

        /// <summary>
        /// Extract a file from the Quantum archive to an output directory by index
        /// </summary>
        /// <param name="index">File index to extract</param>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>True if the file extracted, false otherwise</returns>
        public bool ExtractFile(int index, string outputDirectory, bool includeDebug)
        {
            // If we have no files
            if (Header == null || FileCount == 0 || FileList == null || FileList.Length == 0)
                return false;

            // If we have an invalid index
            if (index < 0 || index >= FileList.Length)
                return false;

            // Get the file information
            var fileDescriptor = FileList[index];

            // Read the entire compressed data
            int compressedDataOffset = (int)CompressedDataOffset;
            long compressedDataLength = Length - compressedDataOffset;
            var compressedData = ReadRangeFromSource(compressedDataOffset, (int)compressedDataLength);

            // Print a debug reminder
            if (includeDebug) Console.WriteLine("Quantum archive extraction is unsupported");

            // TODO: Figure out decompression
            // - Single-file archives seem to work
            // - Single-file archives with files that span a window boundary seem to work
            // - The first files in each archive seem to work
            return false;

            // // Setup the decompression state
            // State state = new State();
            // Decompressor.InitState(state, TableSize, CompressionFlags);

            // // Decompress the entire array
            // int decompressedDataLength = (int)FileList.Sum(fd => fd.ExpandedFileSize);
            // byte[] decompressedData = new byte[decompressedDataLength];
            // Decompressor.Decompress(state, compressedData.Length, compressedData, decompressedData.Length, decompressedData);

            // // Read the data
            // int offset = (int)FileList.Take(index).Sum(fd => fd.ExpandedFileSize);
            // byte[] data = new byte[fileDescriptor.ExpandedFileSize];
            // Array.Copy(decompressedData, offset, data, 0, data.Length);

            // // Loop through all files before the current
            // for (int i = 0; i < index; i++)
            // {
            //     // Decompress the next block of data
            //     byte[] tempData = new byte[FileList[i].ExpandedFileSize];
            //     int lastRead = Decompressor.Decompress(state, compressedData.Length, compressedData, tempData.Length, tempData);
            //     compressedData = new ReadOnlySpan<byte>(compressedData, (lastRead), compressedData.Length - (lastRead)).ToArray();
            // }

            // // Read the data
            // byte[] data = new byte[fileDescriptor.ExpandedFileSize];
            // _ = Decompressor.Decompress(state, compressedData.Length, compressedData, data.Length, data);

            // // Create the filename
            // string filename = fileDescriptor.FileName;

            // // If we have an invalid output directory
            // if (string.IsNullOrEmpty(outputDirectory))
            //     return false;

            // // Create the full output path
            // filename = Path.Combine(outputDirectory, filename);

            // // Ensure the output directory is created
            // Directory.CreateDirectory(Path.GetDirectoryName(filename));

            // // Try to write the data
            // try
            // {
            //     // Open the output file for writing
            //     using var fs = File.Open(filename, FileMode.Create, FileAccess.Write, FileShare.None);
            //     fs.Write(data, 0, data.Length);
            //     fs.Flush();
            // }
            // catch
            // {
            //     return false;
            // }

            // return true;
        }
    }
}
