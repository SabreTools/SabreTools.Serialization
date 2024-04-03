using System.IO;
using System.Xml;
using System.Xml.Schema;
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
        /// <inheritdoc/>
        public T? DeserializeImpl(Stream? data)
        {
            // If the stream is null
            if (data == null)
                return default;

            // Setup the serializer and the reader
            var serializer = new XmlSerializer(typeof(T));
            var settings = new XmlReaderSettings
            {
                CheckCharacters = false,
#if NET40_OR_GREATER || NETCOREAPP
                DtdProcessing = DtdProcessing.Ignore,
#endif
                ValidationFlags = XmlSchemaValidationFlags.None,
                ValidationType = ValidationType.None,
            };
            var streamReader = new StreamReader(data);
            var xmlReader = XmlReader.Create(streamReader, settings);

            // Perform the deserialization and return
            return (T?)serializer.Deserialize(xmlReader);
        }
    }
}