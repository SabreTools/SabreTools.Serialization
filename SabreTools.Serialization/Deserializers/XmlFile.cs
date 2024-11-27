using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace SabreTools.Serialization.Deserializers
{
    /// <summary>
    /// Base class for other XML deserializers
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class XmlFile<T> : BaseBinaryDeserializer<T>
    {
        /// <inheritdoc/>
        public override T? Deserialize(Stream? data)
        {
            // If the stream is invalid
            if (data == null || !data.CanRead)
                return default;

            try
            {
                // If the stream length and offset are invalid
                if (data.Length == 0 || data.Position < 0 || data.Position >= data.Length)
                    return default;
            }
            catch
            {
                // Ignore errors in getting position for compressed streams
            }

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
            try
            {
                return (T?)serializer.Deserialize(xmlReader);
            }
            catch
            {
                // Absorb all exceptions
                return default;
            }
        }
    }
}