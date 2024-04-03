using System.IO;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Deserializers
{
    public class BFPK :
        IByteDeserializer<Models.BFPK.Archive>,
        IFileDeserializer<Models.BFPK.Archive>
    {
        #region IByteDeserializer

        /// <inheritdoc cref="IByteDeserializer.Deserialize(byte[]?, int)"/>
        public static Models.BFPK.Archive? DeserializeBytes(byte[]? data, int offset)
        {
            var deserializer = new BFPK();
            return deserializer.Deserialize(data, offset);
        }

        /// <inheritdoc/>
        public Models.BFPK.Archive? Deserialize(byte[]? data, int offset)
        {
            // If the data is invalid
            if (data == null)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Create a memory stream and parse that
            var dataStream = new MemoryStream(data, offset, data.Length - offset);
            return Streams.BFPK.DeserializeStream(dataStream);
        }

        #endregion

        #region IFileDeserializer

        /// <inheritdoc cref="IFileDeserializer.Deserialize(string?)"/>
        public static Models.BFPK.Archive? DeserializeFile(string? path)
        {
            var deserializer = new BFPK();
            return deserializer.Deserialize(path);
        }

        /// <inheritdoc/>
        public Models.BFPK.Archive? Deserialize(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return Streams.BFPK.DeserializeStream(stream);
        }

        #endregion
    }
}