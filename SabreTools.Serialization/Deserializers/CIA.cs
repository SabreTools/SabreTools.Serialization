using System.IO;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Deserializers
{
    public class CIA :
        IByteDeserializer<Models.N3DS.CIA>,
        IFileDeserializer<Models.N3DS.CIA>
    {
        #region IByteDeserializer

        /// <inheritdoc cref="IByteDeserializer.Deserialize(byte[]?, int)"/>
        public static Models.N3DS.CIA? DeserializeBytes(byte[]? data, int offset)
        {
            var deserializer = new CIA();
            return deserializer.Deserialize(data, offset);
        }

        /// <inheritdoc/>
        public Models.N3DS.CIA? Deserialize(byte[]? data, int offset)
        {
            // If the data is invalid
            if (data == null)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Create a memory stream and parse that
            var dataStream = new MemoryStream(data, offset, data.Length - offset);
            return Streams.CIA.DeserializeStream(dataStream);
        }

        #endregion

        #region IFileDeserializer

        /// <inheritdoc cref="IFileDeserializer.Deserialize(string?)"/>
        public static Models.N3DS.CIA? DeserializeFile(string? path)
        {
            var deserializer = new CIA();
            return deserializer.Deserialize(path);
        }

        /// <inheritdoc/>
        public Models.N3DS.CIA? Deserialize(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return Streams.CIA.DeserializeStream(stream);
        }

        #endregion
    }
}