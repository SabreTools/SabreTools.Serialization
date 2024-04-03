using System.IO;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Deserializers
{
    public class N3DS :
        IByteDeserializer<Models.N3DS.Cart>,
        IFileDeserializer<Models.N3DS.Cart>
    {
        #region IByteDeserializer

        /// <inheritdoc cref="IByteDeserializer.Deserialize(byte[]?, int)"/>
        public static Models.N3DS.Cart? DeserializeBytes(byte[]? data, int offset)
        {
            var deserializer = new N3DS();
            return deserializer.Deserialize(data, offset);
        }

        /// <inheritdoc/>
        public Models.N3DS.Cart? Deserialize(byte[]? data, int offset)
        {
            // If the data is invalid
            if (data == null)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Create a memory stream and parse that
            var dataStream = new MemoryStream(data, offset, data.Length - offset);
            return Streams.N3DS.DeserializeStream(dataStream);
        }

        #endregion

        #region IFileDeserializer

        /// <inheritdoc cref="IFileDeserializer.Deserialize(string?)"/>
        public static Models.N3DS.Cart? DeserializeFile(string? path)
        {
            var deserializer = new N3DS();
            return deserializer.Deserialize(path);
        }

        /// <inheritdoc/>
        public Models.N3DS.Cart? Deserialize(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return Streams.N3DS.DeserializeStream(stream);
        }

        #endregion
    }
}