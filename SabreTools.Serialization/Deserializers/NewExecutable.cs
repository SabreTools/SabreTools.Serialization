using System.IO;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Deserializers
{
    public class NewExecutable :
        IByteDeserializer<Models.NewExecutable.Executable>,
        IFileDeserializer<Models.NewExecutable.Executable>
    {
        #region IByteDeserializer

        /// <inheritdoc cref="IByteDeserializer.Deserialize(byte[]?, int)"/>
        public static Models.NewExecutable.Executable? DeserializeBytes(byte[]? data, int offset)
        {
            var deserializer = new NewExecutable();
            return deserializer.Deserialize(data, offset);
        }

        /// <inheritdoc/>
        public Models.NewExecutable.Executable? Deserialize(byte[]? data, int offset)
        {
            // If the data is invalid
            if (data == null)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Create a memory stream and parse that
            var dataStream = new MemoryStream(data, offset, data.Length - offset);
            return Streams.NewExecutable.DeserializeStream(dataStream);
        }

        #endregion

        #region IFileDeserializer

        /// <inheritdoc cref="IFileDeserializer.Deserialize(string?)"/>
        public static Models.NewExecutable.Executable? DeserializeFile(string? path)
        {
            var deserializer = new NewExecutable();
            return deserializer.Deserialize(path);
        }

        /// <inheritdoc/>
        public Models.NewExecutable.Executable? Deserialize(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return Streams.NewExecutable.DeserializeStream(stream);
        }

        #endregion
    }
}