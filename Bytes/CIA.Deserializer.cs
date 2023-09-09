using System.IO;

namespace SabreTools.Serialization.Bytes
{
    public partial class CIA : IByteSerializer<Models.N3DS.CIA>
    {
        /// <inheritdoc/>
#if NET48
        public Models.N3DS.CIA Deserialize(byte[] data, int offset)
#else
        public Models.N3DS.CIA? Deserialize(byte[]? data, int offset)
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
            return new Streams.CIA().Deserialize(dataStream);
        }
    }
}