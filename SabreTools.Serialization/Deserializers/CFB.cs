using System.IO;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Deserializers
{
    public class CFB :
        IByteDeserializer<Models.CFB.Binary>,
        IFileDeserializer<Models.CFB.Binary>
    {
        #region IByteDeserializer

        /// <inheritdoc cref="IByteDeserializer.Deserialize(byte[]?, int)"/>
        public static Models.CFB.Binary? DeserializeBytes(byte[]? data, int offset)
        {
            var deserializer = new CFB();
            return deserializer.Deserialize(data, offset);
        }

        /// <inheritdoc/>
        public Models.CFB.Binary? Deserialize(byte[]? data, int offset)
        {
            // If the data is invalid
            if (data == null)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Create a memory stream and parse that
            var dataStream = new MemoryStream(data, offset, data.Length - offset);
            return Streams.CFB.DeserializeStream(dataStream);
        }

        #endregion

        #region IFileDeserializer

        /// <inheritdoc cref="IFileDeserializer.Deserialize(string?)"/>
        public static Models.CFB.Binary? DeserializeFile(string? path)
        {
            var deserializer = new CFB();
            return deserializer.Deserialize(path);
        }

        /// <inheritdoc/>
        public Models.CFB.Binary? Deserialize(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return Streams.CFB.DeserializeStream(stream);
        }

        #endregion
    }
}