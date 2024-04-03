using System.IO;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Bytes
{
    public partial class BFPK : IByteSerializer<Models.BFPK.Archive>
    {
        /// <inheritdoc cref="IByteSerializer.Deserialize(byte[]?, int)"/>
        public static Models.BFPK.Archive? Deserialize(byte[]? data, int offset)
        {
            var obj = new BFPK();
            return obj.DeserializeImpl(data, offset);
        }

        /// <inheritdoc/>
        public Models.BFPK.Archive? DeserializeImpl(byte[]? data, int offset)
        {
            // If the data is invalid
            if (data == null)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Create a memory stream and parse that
            var dataStream = new MemoryStream(data, offset, data.Length - offset);
            return new Streams.BFPK().Deserialize(dataStream);
        }
    }
}