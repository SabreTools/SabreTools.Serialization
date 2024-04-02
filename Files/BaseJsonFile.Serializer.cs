using System.IO;
using System.Text;

namespace SabreTools.Serialization.Files
{
    /// <summary>
    /// Base class for other JSON serializers
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public partial class BaseJsonFile<T>
    {
        /// <summary>
        /// Serializes the defined type to an JSON file
        /// </summary>
        /// <param name="obj">Data to serialize</param>
        /// <param name="path">Path to the file to serialize to</param>
        /// <returns>True on successful serialization, false otherwise</returns>
        public bool Serialize(T? obj, string? path, Encoding encoding)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            using var stream = new Streams.BaseJsonFile<T>().Serialize(obj, encoding);
            if (stream == null)
                return false;

            using var fs = File.OpenWrite(path);
            stream.CopyTo(fs);

            return true;
        }
    }
}
