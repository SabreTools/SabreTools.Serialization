using System.IO;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Bytes
{
    public partial class AACS : IByteSerializer<Models.AACS.MediaKeyBlock>
    {
        /// <inheritdoc cref="IByteSerializer.Deserialize(byte[]?, int)"/>
        public static Models.AACS.MediaKeyBlock? Deserialize(byte[]? data, int offset)
        {
            var obj = new AACS();
            return obj.DeserializeImpl(data, offset);
        }

        /// <inheritdoc/>
        public Models.AACS.MediaKeyBlock? DeserializeImpl(byte[]? data, int offset)
        {
            // If the data is invalid
            if (data == null)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Create a memory stream and parse that
            var dataStream = new MemoryStream(data, offset, data.Length - offset);
            return new Streams.AACS().Deserialize(dataStream);
        }
    }
}