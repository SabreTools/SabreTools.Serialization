using System.IO;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    /// <summary>
    /// Base class for other XML serializers
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public partial class XmlFile<T> : IFileSerializer<T>
    {
        /// <inheritdoc/>
        public bool Serialize(T? obj, string? path)
            => Serialize(obj, path, null, null, null, null);

        /// <summary>
        /// Serializes the defined type to an XML file
        /// </summary>
        /// <param name="obj">Data to serialize</param>
        /// <param name="path">Path to the file to serialize to</param>
        /// <param name="name">Optional DOCTYPE name</param>
        /// <param name="pubid">Optional DOCTYPE pubid</param>
        /// <param name="sysid">Optional DOCTYPE sysid</param>
        /// <param name="subset">Optional DOCTYPE name</param>
        /// <returns>True on successful serialization, false otherwise</returns>
        public bool Serialize(T? obj, string? path, string? name = null, string? pubid = null, string? sysid = null, string? subset = null)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            using var stream = new Streams.XmlFile<T>().Serialize(obj, name, pubid, sysid, subset);
            if (stream == null)
                return false;

            using var fs = File.OpenWrite(path);
            stream.CopyTo(fs);

            return true;
        }
    }
}