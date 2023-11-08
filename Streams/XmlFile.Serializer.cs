using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Streams
{
    /// <summary>
    /// Base class for other XML serializers
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public partial class XmlFile<T> : IStreamSerializer<T>
    {
        /// <summary>
        /// Serializes the defined type to a stream
        /// </summary>
        /// <param name="obj">Data to serialize</param>
        /// <returns>Stream containing serialized data on success, null otherwise</returns>
        public Stream? Serialize(T? obj)
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

            // Setup the serializer and the reader
            var serializer = new XmlSerializer(typeof(T));
            var settings = new XmlWriterSettings
            {
                CheckCharacters = false,
                Encoding = Encoding.UTF8,
                Indent = true,
                IndentChars = "\t",
                NewLineChars = "\n",
            };
            var stream = new MemoryStream();
            var streamWriter = new StreamWriter(stream);
            var xmlWriter = XmlWriter.Create(streamWriter, settings);

            // Write the doctype if provided
            if (!string.IsNullOrWhiteSpace(name))
                xmlWriter.WriteDocType(name, pubid, sysid, subset);

            // Perform the deserialization and return
            serializer.Serialize(xmlWriter, obj);
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }
    }
}