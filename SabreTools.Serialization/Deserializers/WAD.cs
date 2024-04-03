using System.IO;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Deserializers
{
    public class WAD :
        IByteDeserializer<Models.WAD.File>,
        IFileDeserializer<Models.WAD.File>
    {
        #region IByteDeserializer

        /// <inheritdoc cref="IByteDeserializer.Deserialize(byte[]?, int)"/>
        public static Models.WAD.File? DeserializeBytes(byte[]? data, int offset)
        {
            var deserializer = new WAD();
            return deserializer.Deserialize(data, offset);
        }

        /// <inheritdoc/>
        public Models.WAD.File? Deserialize(byte[]? data, int offset)
        {
            // If the data is invalid
            if (data == null)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Create a memory stream and parse that
            var dataStream = new MemoryStream(data, offset, data.Length - offset);
            return Streams.WAD.DeserializeStream(dataStream);
        }

        #endregion

        #region IFileDeserializer

        /// <inheritdoc cref="IFileDeserializer.Deserialize(string?)"/>
        public static Models.WAD.File? DeserializeFile(string? path)
        {
            var deserializer = new WAD();
            return deserializer.Deserialize(path);
        }

        /// <inheritdoc/>
        public Models.WAD.File? Deserialize(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return Streams.WAD.DeserializeStream(stream);
        }

        #endregion
    }
}