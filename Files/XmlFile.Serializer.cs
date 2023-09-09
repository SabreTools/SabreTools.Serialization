using System.IO;

namespace SabreTools.Serialization.Files
{
    /// <summary>
    /// Base class for other XML serializers
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public partial class XmlFile<T> : IFileSerializer<T>
    {
        /// <inheritdoc/>
#if NET48
        public bool Serialize(T obj, string path)
#else
        public bool Serialize(T? obj, string? path)
#endif
            => Serialize(obj, path, null, null, null, null);

        /// <summary>
        /// Serializes the defined type to an XML file
        /// </summary>
        /// <param name="obj">Data to serialize</param>
        /// <param name="path">Path to the file to serialize to</param>
        /// <param name="obj">Data to serialize</param>
        /// <param name="name">Optional DOCTYPE name</param>
        /// <param name="pubid">Optional DOCTYPE pubid</param>
        /// <param name="sysid">Optional DOCTYPE sysid</param>
        /// <param name="subset">Optional DOCTYPE name</param>
        /// <returns>True on successful serialization, false otherwise</returns>
#if NET48
        public bool Serialize(T obj, string path, string name = null, string pubid = null, string sysid = null, string subset = null)
#else
        public bool Serialize(T? obj, string? path, string? name = null, string? pubid = null, string? sysid = null, string? subset = null)
#endif
        {
            using (var stream = new Streams.XmlFile<T>().Serialize(obj, name, pubid, sysid, subset))
            {
                if (stream == null)
                    return false;

                using (var fs = File.OpenWrite(path))
                {
                    stream.CopyTo(fs);
                }

                return true;
            }
        }
    }
}