using System.IO;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Deserializers
{
    public class GCF :
        IByteDeserializer<Models.GCF.File>,
        IFileDeserializer<Models.GCF.File>
    {
        #region IByteDeserializer

        /// <inheritdoc cref="IByteDeserializer.Deserialize(byte[]?, int)"/>
        public static Models.GCF.File? DeserializeBytes(byte[]? data, int offset)
        {
            var deserializer = new GCF();
            return deserializer.Deserialize(data, offset);
        }

        /// <inheritdoc/>
        public Models.GCF.File? Deserialize(byte[]? data, int offset)
        {
            // If the data is invalid
            if (data == null)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Create a memory stream and parse that
            var dataStream = new MemoryStream(data, offset, data.Length - offset);
            return Streams.GCF.DeserializeStream(dataStream);
        }

        #endregion

        #region IFileDeserializer

        /// <inheritdoc cref="IFileDeserializer.Deserialize(string?)"/>
        public static Models.GCF.File? DeserializeFile(string? path)
        {
            var deserializer = new GCF();
            return deserializer.Deserialize(path);
        }

        /// <inheritdoc/>
        public Models.GCF.File? Deserialize(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return Streams.GCF.DeserializeStream(stream);
        }

        #endregion
    }
}