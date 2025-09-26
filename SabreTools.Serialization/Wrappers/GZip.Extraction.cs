using System;
using System.IO;
using SabreTools.IO.Compression.Deflate;
using SabreTools.Serialization.Interfaces;
using SabreTools.Serialization.Models.GZIP;

namespace SabreTools.Serialization.Wrappers
{
    public partial class GZip : IExtractable
    {
        /// <inheritdoc/>
        public bool Extract(string outputDirectory, bool includeDebug)
        {
            // Ensure there is data to extract
            if (Header == null || DataOffset < 0)
            {
                if (includeDebug) Console.Error.WriteLine("Invalid archive detected, skipping...");
                return false;
            }

            // Ensure that DEFLATE is being used
            if (Header.CompressionMethod != CompressionMethod.Deflate)
            {
                if (includeDebug) Console.Error.WriteLine($"Invalid compression method {Header.CompressionMethod} detected, only DEFLATE is supported. Skipping...");
                return false;
            }

            try
            {
                // Seek to the start of the compressed data
                long offset = _dataSource.Seek(DataOffset, SeekOrigin.Begin);
                if (offset != DataOffset)
                {
                    if (includeDebug) Console.Error.WriteLine($"Could not seek to compressed data at {DataOffset}");
                    return false;
                }

                // Ensure directory separators are consistent
                string filename = Header.OriginalFileName
                    ?? (Filename != null ? Path.GetFileName(Filename).Replace(".gz", string.Empty) : null)
                    ?? $"extracted_file";

                if (Path.DirectorySeparatorChar == '\\')
                    filename = filename.Replace('/', '\\');
                else if (Path.DirectorySeparatorChar == '/')
                    filename = filename.Replace('\\', '/');

                // Ensure the full output directory exists
                filename = Path.Combine(outputDirectory, filename);
                var directoryName = Path.GetDirectoryName(filename);
                if (directoryName != null && !Directory.Exists(directoryName))
                    Directory.CreateDirectory(directoryName);

                // Open the source as a DEFLATE stream
                var deflateStream = new DeflateStream(_dataSource, CompressionMode.Decompress, leaveOpen: true);

                // Write the file
                using var fs = File.Open(filename, FileMode.Create, FileAccess.Write, FileShare.None);
                deflateStream.CopyTo(fs);
                fs.Flush();

                return true;
            }
            catch (Exception ex)
            {
                if (includeDebug) Console.Error.WriteLine(ex);
                return false;
            }
        }
    }
}
