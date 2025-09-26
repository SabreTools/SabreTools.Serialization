using System;
using System.Collections.Generic;
using System.IO;
using SabreTools.IO.Compression.zlib;

namespace SabreTools.Serialization.Wrappers
{
    public partial class SGA : IExtractable
    {
        /// <inheritdoc/>
        public bool Extract(string outputDirectory, bool includeDebug)
        {
            // Get the file count
            int fileCount = FileCount;
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
        /// Extract a file from the SGA to an output directory by index
        /// </summary>
        /// <param name="index">File index to extract</param>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>True if the file extracted, false otherwise</returns>
        public bool ExtractFile(int index, string outputDirectory, bool includeDebug)
        {
            // Get the file count
            int fileCount = FileCount;
            if (fileCount == 0)
                return false;

            // If the files index is invalid
            if (index < 0 || index >= fileCount)
                return false;

            // Create the filename
            var filename = GetFileName(index);
            if (filename == null)
                return false;

            // Loop through and get all parent directories
            var parentNames = new List<string> { filename };

            // Get the parent directory
            string? folderName = GetParentName(index);
            if (folderName != null)
                parentNames.Add(folderName);

            // TODO: Should the section name/alias be used in the path as well?

            // Reverse and assemble the filename
            parentNames.Reverse();
#if NET20 || NET35
            filename = parentNames[0];
            for (int i = 1; i < parentNames.Count; i++)
            {
                filename = Path.Combine(filename, parentNames[i]);
            }
#else
            filename = Path.Combine([.. parentNames]);
#endif

            // Get and adjust the file offset
            long fileOffset = GetFileOffset(index);
            fileOffset += FileDataOffset;
            if (fileOffset < 0)
                return false;

            // Get the file sizes
            long fileSize = GetCompressedSize(index);
            long outputFileSize = GetUncompressedSize(index);

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
                // Inflate the data into the buffer
                var zstream = new ZLib.z_stream_s();
                data = new byte[outputFileSize];
                unsafe
                {
                    fixed (byte* payloadPtr = compressedData)
                    fixed (byte* dataPtr = data)
                    {
                        zstream.next_in = payloadPtr;
                        zstream.avail_in = (uint)compressedData.Length;
                        zstream.total_in = (uint)compressedData.Length;
                        zstream.next_out = dataPtr;
                        zstream.avail_out = (uint)data.Length;
                        zstream.total_out = 0;

                        ZLib.inflateInit_(zstream, ZLib.zlibVersion(), compressedData.Length);
                        int zret = ZLib.inflate(zstream, 1);
                        ZLib.inflateEnd(zstream);
                    }
                }
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
