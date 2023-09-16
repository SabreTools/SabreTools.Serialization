using System.IO;
using SabreTools.Models.PFF;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Bytes
{
    public partial class PFF : IByteSerializer<Archive>
    {
        /// <inheritdoc/>
#if NET48
        public Archive Deserialize(byte[] data, int offset)
#else
        public Archive? Deserialize(byte[]? data, int offset)
#endif
        {
            // If the data is invalid
            if (data == null)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Create a memory stream and parse that
            MemoryStream dataStream = new MemoryStream(data, offset, data.Length - offset);
            return new Streams.PFF().Deserialize(dataStream);
        }
    }
}