using System.IO;

namespace SabreTools.Serialization.Bytes
{
    public partial class NCF : IByteSerializer<Models.NCF.File>
    {
        /// <inheritdoc/>
#if NET48
        public Models.NCF.File Deserialize(byte[] data, int offset)
#else
        public Models.NCF.File? Deserialize(byte[]? data, int offset)
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
            return new Streams.NCF().Deserialize(dataStream);
        }
    }
}