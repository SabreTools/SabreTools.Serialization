using System.IO;
using System.Text;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    /// <summary>
    /// Base class for other JSON serializers
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public partial class JsonFile<T> : IFileSerializer<T>
    {
        /// <inheritdoc/>
        public virtual bool SerializeImpl(T? obj, string? path)
            => SerializeImpl(obj, path, new UTF8Encoding(false));

        /// <summary>
        /// Serialize a <typeparamref name="T"/> into a file
        /// </summary>
        /// <typeparam name="T">Type of object to serialize from</typeparam>
        /// <param name="obj">Data to serialize</param>
        /// <param name="path">Path to the file to serialize to</param>
        /// <param name="encoding">Encoding to parse text as</param>
        /// <returns>True on successful serialization, false otherwise</returns>
        public bool SerializeImpl(T? obj, string? path, Encoding encoding)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            using var stream = new Streams.JsonFile<T>().Serialize(obj, encoding);
            if (stream == null)
                return false;

            using var fs = File.OpenWrite(path);
            stream.CopyTo(fs);

            return true;
        }
    }
}
