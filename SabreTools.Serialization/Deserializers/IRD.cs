using System.IO;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Deserializers
{
    public class IRD :
        IByteDeserializer<Models.IRD.File>,
        IFileDeserializer<Models.IRD.File>
    {
        #region IByteDeserializer

        /// <inheritdoc cref="IByteDeserializer.Deserialize(byte[]?, int)"/>
        public static Models.IRD.File? DeserializeBytes(byte[]? data, int offset)
        {
            var deserializer = new IRD();
            return deserializer.Deserialize(data, offset);
        }

        /// <inheritdoc/>
        public Models.IRD.File? Deserialize(byte[]? data, int offset)
        {
            // If the data is invalid
            if (data == null)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Create a memory stream and parse that
            var dataStream = new MemoryStream(data, offset, data.Length - offset);
            return Streams.IRD.DeserializeStream(dataStream);
        }

        #endregion

        #region IFileDeserializer

        /// <inheritdoc cref="IFileDeserializer.Deserialize(string?)"/>
        public static Models.IRD.File? DeserializeFile(string? path)
        {
            var deserializer = new IRD();
            return deserializer.Deserialize(path);
        }

        /// <inheritdoc/>
        public Models.IRD.File? Deserialize(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return Streams.IRD.DeserializeStream(stream);
        }

        #endregion
    }
}