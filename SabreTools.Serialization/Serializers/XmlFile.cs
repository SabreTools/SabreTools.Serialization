using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Serialization.Serializers
{
    /// <summary>
    /// Base class for other XML serializers
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class XmlFile<T> : BaseBinarySerializer<T>
    {
        #region IByteSerializer

        /// <inheritdoc/>
        public override byte[]? SerializeArray(T? obj)
            => SerializeArray(obj, null, null, null, null);

        /// <summary>
        /// Serializes the defined type to a byte array
        /// </summary>
        /// <param name="obj">Data to serialize</param>
        /// <param name="name">Optional DOCTYPE name</param>
        /// <param name="pubid">Optional DOCTYPE pubid</param>
        /// <param name="sysid">Optional DOCTYPE sysid</param>
        /// <param name="subset">Optional DOCTYPE name</param>
        /// <returns>Byte array containing serialized data on success, null otherwise</returns>
        public byte[]? SerializeArray(T? obj, string? name = null, string? pubid = null, string? sysid = null, string? subset = null)
        {
            using var stream = Serialize(obj, name, pubid, sysid, subset);
            if (stream == null)
                return null;

            byte[] bytes = new byte[stream.Length];
            int read = stream.Read(bytes, 0, bytes.Length);
            return bytes;
        }

        #endregion

        #region IFileSerializer

        /// <inheritdoc/>
        public override bool Serialize(T? obj, string? path)
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

            using var stream = Serialize(obj, name, pubid, sysid, subset);
            if (stream == null)
                return false;

            using var fs = File.OpenWrite(path);
            stream.CopyTo(fs);

            return true;
        }

        #endregion

        #region IStreamSerializer

        /// <inheritdoc/>
        public override Stream? Serialize(T? obj)
            => Serialize(obj, null, null, null, null);

        /// <summary>
        /// Serializes the defined type to a stream
        /// </summary>
        /// <param name="obj">Data to serialize</param>
        /// <param name="name">Optional DOCTYPE name</param>
        /// <param name="pubid">Optional DOCTYPE pubid</param>
        /// <param name="sysid">Optional DOCTYPE sysid</param>
        /// <param name="subset">Optional DOCTYPE name</param>
        /// <returns>Stream containing serialized data on success, null otherwise</returns>
        public Stream? Serialize(T? obj, string? name = null, string? pubid = null, string? sysid = null, string? subset = null)
        {
            // If the object is null
            if (obj == null)
                return null;

            // Setup the serializer and the writer
            var serializer = new XmlSerializer(typeof(T));
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add("", "");

            var settings = new XmlWriterSettings
            {
                CheckCharacters = false,
                Encoding = Encoding.UTF8,
                Indent = true,
                IndentChars = "\t",
#if NET40_OR_GREATER || NETCOREAPP || NETSTANDARD2_0_OR_GREATER
                NamespaceHandling = NamespaceHandling.OmitDuplicates,
#endif
                NewLineChars = "\n",
            };
            var stream = new MemoryStream();
            var streamWriter = new StreamWriter(stream);
            var xmlWriter = XmlWriter.Create(streamWriter, settings);

            // Write the doctype if provided
            if (!string.IsNullOrEmpty(name))
                xmlWriter.WriteDocType(name, pubid, sysid, subset);

            // Perform the deserialization and return
            serializer.Serialize(xmlWriter, obj, namespaces);
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }

        #endregion
    }
}
