using System.IO;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Deserializers
{
    public class AACS :
        IByteDeserializer<Models.AACS.MediaKeyBlock>,
        IFileDeserializer<Models.AACS.MediaKeyBlock>
    {
        #region IByteDeserializer

        /// <inheritdoc cref="IByteDeserializer.Deserialize(byte[]?, int)"/>
        public static Models.AACS.MediaKeyBlock? DeserializeBytes(byte[]? data, int offset)
        {
            var deserializer = new AACS();
            return deserializer.Deserialize(data, offset);
        }

        /// <inheritdoc/>
        public Models.AACS.MediaKeyBlock? Deserialize(byte[]? data, int offset)
        {
            // If the data is invalid
            if (data == null)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Create a memory stream and parse that
            var dataStream = new MemoryStream(data, offset, data.Length - offset);
            return Streams.AACS.DeserializeStream(dataStream);
        }

        #endregion

        #region IFileDeserializer

        /// <inheritdoc cref="IFileDeserializer.Deserialize(string?)"/>
        public static Models.AACS.MediaKeyBlock? DeserializeFile(string? path)
        {
            var deserializer = new AACS();
            return deserializer.Deserialize(path);
        }

        /// <inheritdoc/>
        public Models.AACS.MediaKeyBlock? Deserialize(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return Streams.AACS.DeserializeStream(stream);
        }

        #endregion
    }
}