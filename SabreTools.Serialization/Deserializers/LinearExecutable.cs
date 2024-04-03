using System.IO;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Deserializers
{
    public class LinearExecutable :
        IByteDeserializer<Models.LinearExecutable.Executable>,
        IFileDeserializer<Models.LinearExecutable.Executable>
    {
        #region IByteDeserializer

        /// <inheritdoc cref="IByteDeserializer.Deserialize(byte[]?, int)"/>
        public static Models.LinearExecutable.Executable? DeserializeBytes(byte[]? data, int offset)
        {
            var deserializer = new LinearExecutable();
            return deserializer.Deserialize(data, offset);
        }

        /// <inheritdoc/>
        public Models.LinearExecutable.Executable? Deserialize(byte[]? data, int offset)
        {
            // If the data is invalid
            if (data == null)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Create a memory stream and parse that
            var dataStream = new MemoryStream(data, offset, data.Length - offset);
            return Streams.LinearExecutable.DeserializeStream(dataStream);
        }

        #endregion

        #region IFileDeserializer

        /// <inheritdoc cref="IFileDeserializer.Deserialize(string?)"/>
        public static Models.LinearExecutable.Executable? DeserializeFile(string? path)
        {
            var deserializer = new LinearExecutable();
            return deserializer.Deserialize(path);
        }

        /// <inheritdoc/>
        public Models.LinearExecutable.Executable? Deserialize(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return Streams.LinearExecutable.DeserializeStream(stream);
        }

        #endregion
    }
}