using System.IO;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Deserializers
{
    public class PFF :
        IByteDeserializer<Models.PFF.Archive>,
        IFileDeserializer<Models.PFF.Archive>
    {
        #region IByteDeserializer

        /// <inheritdoc cref="IByteDeserializer.Deserialize(byte[]?, int)"/>
        public static Models.PFF.Archive? DeserializeBytes(byte[]? data, int offset)
        {
            var deserializer = new PFF();
            return deserializer.Deserialize(data, offset);
        }

        /// <inheritdoc/>
        public Models.PFF.Archive? Deserialize(byte[]? data, int offset)
        {
            // If the data is invalid
            if (data == null)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Create a memory stream and parse that
            var dataStream = new MemoryStream(data, offset, data.Length - offset);
            return Streams.PFF.DeserializeStream(dataStream);
        }

        #endregion

        #region IFileDeserializer

        /// <inheritdoc cref="IFileDeserializer.Deserialize(string?)"/>
        public static Models.PFF.Archive? DeserializeFile(string? path)
        {
            var deserializer = new PFF();
            return deserializer.Deserialize(path);
        }

        /// <inheritdoc/>
        public Models.PFF.Archive? Deserialize(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return Streams.PFF.DeserializeStream(stream);
        }

        #endregion
    }
}