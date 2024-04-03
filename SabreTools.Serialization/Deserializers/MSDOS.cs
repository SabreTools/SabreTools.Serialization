using System.IO;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Deserializers
{
    public class MSDOS :
        IByteDeserializer<Models.MSDOS.Executable>,
        IFileDeserializer<Models.MSDOS.Executable>
    {
        #region IByteDeserializer

        /// <inheritdoc cref="IByteDeserializer.Deserialize(byte[]?, int)"/>
        public static Models.MSDOS.Executable? DeserializeBytes(byte[]? data, int offset)
        {
            var deserializer = new MSDOS();
            return deserializer.Deserialize(data, offset);
        }

        /// <inheritdoc/>
        public Models.MSDOS.Executable? Deserialize(byte[]? data, int offset)
        {
            // If the data is invalid
            if (data == null)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Create a memory stream and parse that
            var dataStream = new MemoryStream(data, offset, data.Length - offset);
            return Streams.MSDOS.DeserializeStream(dataStream);
        }

        #endregion

        #region IFileDeserializer

        /// <inheritdoc cref="IFileDeserializer.Deserialize(string?)"/>
        public static Models.MSDOS.Executable? DeserializeFile(string? path)
        {
            var deserializer = new MSDOS();
            return deserializer.Deserialize(path);
        }

        /// <inheritdoc/>
        public Models.MSDOS.Executable? Deserialize(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return Streams.MSDOS.DeserializeStream(stream);
        }

        #endregion
    }
}