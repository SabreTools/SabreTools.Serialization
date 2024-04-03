using System.IO;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Bytes
{
    // TODO: Add multi-cabinet reading
    public partial class MicrosoftCabinet : IByteSerializer<Models.MicrosoftCabinet.Cabinet>
    {
        /// <inheritdoc cref="IByteSerializer.DeserializeImpl(byte[]?, int)"/>
        public static Models.MicrosoftCabinet.Cabinet? Deserialize(byte[]? data, int offset)
        {
            var deserializer = new MicrosoftCabinet();
            return deserializer.DeserializeImpl(data, offset);
        }

        /// <inheritdoc/>
        public Models.MicrosoftCabinet.Cabinet? DeserializeImpl(byte[]? data, int offset)
        {
            // If the data is invalid
            if (data == null)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Create a memory stream and parse that
            var dataStream = new MemoryStream(data, offset, data.Length - offset);
            return Streams.MicrosoftCabinet.Deserialize(dataStream);
        }
    }
}