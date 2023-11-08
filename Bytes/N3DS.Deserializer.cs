using System.IO;
using SabreTools.Models.N3DS;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Bytes
{
    public partial class N3DS : IByteSerializer<Cart>
    {
        /// <inheritdoc/>
        public Cart? Deserialize(byte[]? data, int offset)
        {
            // If the data is invalid
            if (data == null)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Create a memory stream and parse that
            MemoryStream dataStream = new MemoryStream(data, offset, data.Length - offset);
            return new Streams.N3DS().Deserialize(dataStream);
        }
    }
}