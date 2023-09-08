using System;
using System.IO;
using System.IO.Compression;

namespace SabreTools.Serialization
{
    internal class PathProcessor
    {
        /// <summary>
        /// Opens a path as a stream in a safe manner, decompressing if needed
        /// </summary>
        /// <param name="path">Path to open as a stream</param>
        /// <returns>Stream representing the file, null on error</returns>
#if NET48
        public static Stream OpenStream(string path)
#else
        public static Stream? OpenStream(string path)
#endif
        {
            try
            {
                // If we don't have a file
                if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
                    return null;

                // Open the file for deserialization
                var stream = File.OpenRead(path);

                // Get the extension to determine if additional handling is needed
                string ext = Path.GetExtension(path).TrimStart('.');

                // Determine what we do based on the extension
                if (string.Equals(ext, "gz", StringComparison.OrdinalIgnoreCase))
                {
                    return new GZipStream(stream, CompressionMode.Decompress);
                }
                else if (string.Equals(ext, "zip", StringComparison.OrdinalIgnoreCase))
                {
                    // TODO: Support zip-compressed files
                    return null;
                }
                else
                {
                    return stream;
                }
            }
            catch
            {
                // TODO: Handle logging the exception
                return null;
            }
        }
    }
}