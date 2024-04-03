using System.IO;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Deserializers
{
    public class PortableExecutable :
        IByteDeserializer<Models.PortableExecutable.Executable>,
        IFileDeserializer<Models.PortableExecutable.Executable>
    {
        #region IByteDeserializer

        /// <inheritdoc cref="IByteDeserializer.Deserialize(byte[]?, int)"/>
        public static Models.PortableExecutable.Executable? DeserializeBytes(byte[]? data, int offset)
        {
            var deserializer = new PortableExecutable();
            return deserializer.Deserialize(data, offset);
        }

        /// <inheritdoc/>
        public Models.PortableExecutable.Executable? Deserialize(byte[]? data, int offset)
        {
            // If the data is invalid
            if (data == null)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Create a memory stream and parse that
            var dataStream = new MemoryStream(data, offset, data.Length - offset);
            return Streams.PortableExecutable.DeserializeStream(dataStream);
        }

        #endregion

        #region IFileDeserializer

        /// <inheritdoc cref="IFileDeserializer.Deserialize(string?)"/>
        public static Models.PortableExecutable.Executable? DeserializeFile(string? path)
        {
            var deserializer = new PortableExecutable();
            return deserializer.Deserialize(path);
        }

        /// <inheritdoc/>
        public Models.PortableExecutable.Executable? Deserialize(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return Streams.PortableExecutable.DeserializeStream(stream);
        }

        #endregion
    }
}