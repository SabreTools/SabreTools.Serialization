using System.IO;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Deserializers
{
    public partial class PFF : IByteDeserializer<Models.PFF.Archive>
    {
        /// <inheritdoc cref="IByteSerializer.Deserialize(byte[]?, int)"/>
        public static Models.PFF.Archive? DeserializeBytes(byte[]? data, int offset)
        {
            var deserializer = new PFF();
            return deserializer.Deserialize(data, offset);
        }

        /// <inheritdoc/>
        public Models.PFF.Archive? Deserialize(byte[]? data, int offset)
        {
            // If the data is invalid
            if (data == null)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Create a memory stream and parse that
            var dataStream = new MemoryStream(data, offset, data.Length - offset);
            return Streams.PFF.DeserializeStream(dataStream);
        }
    }
}