using System.IO;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Deserializers
{
    public class Nitro :
        IByteDeserializer<Models.Nitro.Cart>,
        IFileDeserializer<Models.Nitro.Cart>
    {
        #region IByteDeserializer

        /// <inheritdoc cref="IByteDeserializer.Deserialize(byte[]?, int)"/>
        public static Models.Nitro.Cart? DeserializeBytes(byte[]? data, int offset)
        {
            var deserializer = new Nitro();
            return deserializer.Deserialize(data, offset);
        }

        /// <inheritdoc/>
        public Models.Nitro.Cart? Deserialize(byte[]? data, int offset)
        {
            // If the data is invalid
            if (data == null)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Create a memory stream and parse that
            var dataStream = new MemoryStream(data, offset, data.Length - offset);
            return Streams.Nitro.DeserializeStream(dataStream);
        }

        #endregion

        #region IFileDeserializer

        /// <inheritdoc cref="IFileDeserializer.Deserialize(string?)"/>
        public static Models.Nitro.Cart? DeserializeFile(string? path)
        {
            var deserializer = new Nitro();
            return deserializer.Deserialize(path);
        }

        /// <inheritdoc/>
        public Models.Nitro.Cart? Deserialize(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return Streams.Nitro.DeserializeStream(stream);
        }

        #endregion
    }
}