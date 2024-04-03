using System.IO;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Deserializers
{
    public class Quantum :
        IByteDeserializer<Models.Quantum.Archive>,
        IFileDeserializer<Models.Quantum.Archive>
    {
        #region IByteDeserializer

        /// <inheritdoc cref="IByteDeserializer.Deserialize(byte[]?, int)"/>
        public static Models.Quantum.Archive? DeserializeBytes(byte[]? data, int offset)
        {
            var deserializer = new Quantum();
            return deserializer.Deserialize(data, offset);
        }

        /// <inheritdoc/>
        public Models.Quantum.Archive? Deserialize(byte[]? data, int offset)
        {
            // If the data is invalid
            if (data == null)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Create a memory stream and parse that
            var dataStream = new MemoryStream(data, offset, data.Length - offset);
            return Streams.Quantum.DeserializeStream(dataStream);
        }

        #endregion

        #region IFileDeserializer

        /// <inheritdoc cref="IFileDeserializer.Deserialize(string?)"/>
        public static Models.Quantum.Archive? DeserializeFile(string? path)
        {
            var deserializer = new Quantum();
            return deserializer.Deserialize(path);
        }

        /// <inheritdoc/>
        public Models.Quantum.Archive? Deserialize(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return Streams.Quantum.DeserializeStream(stream);
        }

        #endregion
    }
}