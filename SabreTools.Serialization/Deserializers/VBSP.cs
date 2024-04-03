using System.IO;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Deserializers
{
    public class VBSP :
        IByteDeserializer<Models.VBSP.File>,
        IFileDeserializer<Models.VBSP.File>
    {
        #region IByteDeserializer

        /// <inheritdoc cref="IByteDeserializer.Deserialize(byte[]?, int)"/>
        public static Models.VBSP.File? DeserializeBytes(byte[]? data, int offset)
        {
            var deserializer = new VBSP();
            return deserializer.Deserialize(data, offset);
        }

        /// <inheritdoc/>
        public Models.VBSP.File? Deserialize(byte[]? data, int offset)
        {
            // If the data is invalid
            if (data == null)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Create a memory stream and parse that
            var dataStream = new MemoryStream(data, offset, data.Length - offset);
            return Streams.VBSP.DeserializeStream(dataStream);
        }

        #endregion

        #region IFileDeserializer

        /// <inheritdoc cref="IFileDeserializer.Deserialize(string?)"/>
        public static Models.VBSP.File? DeserializeFile(string? path)
        {
            var deserializer = new VBSP();
            return deserializer.Deserialize(path);
        }

        /// <inheritdoc/>
        public Models.VBSP.File? Deserialize(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return Streams.VBSP.DeserializeStream(stream);
        }

        #endregion
    }
}