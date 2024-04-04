using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Deserializers
{
    /// <summary>
    /// Base class for other XML deserializers
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class XmlFile<T> :
        IFileDeserializer<T>,
        IStreamDeserializer<T>
    {
        #region IFileDeserializer

        /// <inheritdoc/>
        public T? Deserialize(string? path)
        {
            using var data = PathProcessor.OpenStream(path);
            return Deserialize(data);
        }

        #endregion

        #region IStreamDeserializer

        /// <inheritdoc/>
        public T? Deserialize(Stream? data)
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

        #endregion
    }
}