using System.IO;

namespace SabreTools.Serialization.Deserializers
{
    internal class PathProcessor
    {
        /// <summary>
        /// Opens a path as a stream in a safe manner, decompressing if needed
        /// </summary>
        /// <param name="path">Path to open as a stream</param>
        /// <returns>Stream representing the file, null on error</returns>
        public static Stream? OpenStream(string? path)
        {
            try
            {
                // If we don't have a file
                if (string.IsNullOrEmpty(path) || !File.Exists(path))
                    return null;

                // Open the file for deserialization
                return File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            }
            catch
            {
                // TODO: Handle logging the exception
                return null;
            }
        }
    }
}
