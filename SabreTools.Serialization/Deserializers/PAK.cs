using System.IO;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Deserializers
{
    public class PAK :
        IByteDeserializer<Models.PAK.File>,
        IFileDeserializer<Models.PAK.File>
    {
        #region IByteDeserializer

        /// <inheritdoc cref="IByteDeserializer.Deserialize(byte[]?, int)"/>
        public static Models.PAK.File? DeserializeBytes(byte[]? data, int offset)
        {
            var deserializer = new PAK();
            return deserializer.Deserialize(data, offset);
        }

        /// <inheritdoc/>
        public Models.PAK.File? Deserialize(byte[]? data, int offset)
        {
            // If the data is invalid
            if (data == null)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Create a memory stream and parse that
            var dataStream = new MemoryStream(data, offset, data.Length - offset);
            return Streams.PAK.DeserializeStream(dataStream);
        }

        #endregion

        #region IFileDeserializer

        /// <inheritdoc cref="IFileDeserializer.Deserialize(string?)"/>
        public static Models.PAK.File? DeserializeFile(string? path)
        {
            var deserializer = new PAK();
            return deserializer.Deserialize(path);
        }

        /// <inheritdoc/>
        public Models.PAK.File? Deserialize(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return Streams.PAK.DeserializeStream(stream);
        }

        #endregion
    }
}