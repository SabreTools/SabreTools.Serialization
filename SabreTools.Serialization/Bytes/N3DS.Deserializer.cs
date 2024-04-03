using System.IO;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Bytes
{
    public partial class N3DS : IByteSerializer<Models.N3DS.Cart>
    {
        /// <inheritdoc cref="IByteSerializer.DeserializeImpl(byte[]?, int)"/>
        public static Models.N3DS.Cart? Deserialize(byte[]? data, int offset)
        {
            var deserializer = new N3DS();
            return deserializer.DeserializeImpl(data, offset);
        }

        /// <inheritdoc/>
        public Models.N3DS.Cart? DeserializeImpl(byte[]? data, int offset)
        {
            // If the data is invalid
            if (data == null)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Create a memory stream and parse that
            var dataStream = new MemoryStream(data, offset, data.Length - offset);
            return Streams.N3DS.Deserialize(dataStream);
        }
    }
}