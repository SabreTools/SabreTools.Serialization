using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace SabreTools.Serialization.Streams
{
    /// <summary>
    /// Base class for other XML serializers
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public partial class XmlFile<T> : IStreamSerializer<T>
    {
        /// <inheritdoc/>
#if NET48
        public T Deserialize(Stream data)
#else
        public T? Deserialize(Stream? data)
#endif
        {
            // If the stream is null
            if (data == null)
                return default;

            // Setup the serializer and the reader
            var serializer = new XmlSerializer(typeof(T));
            var settings = new XmlReaderSettings
            {
                CheckCharacters = false,
                DtdProcessing = DtdProcessing.Ignore,
                ValidationFlags = XmlSchemaValidationFlags.None,
                ValidationType = ValidationType.None,
            };
            var streamReader = new StreamReader(data);
            var xmlReader = XmlReader.Create(streamReader, settings);

            // Perform the deserialization and return
#if NET48
            return (T)serializer.Deserialize(xmlReader);
#else
            return (T?)serializer.Deserialize(xmlReader);
#endif
        }
    }
}