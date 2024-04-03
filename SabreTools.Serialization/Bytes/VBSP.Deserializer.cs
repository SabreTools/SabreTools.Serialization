using System.IO;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Bytes
{
    public partial class VBSP : IByteSerializer<Models.VBSP.File>
    {
        /// <inheritdoc cref="IByteSerializer.DeserializeImpl(byte[]?, int)"/>
        public static Models.VBSP.File? Deserialize(byte[]? data, int offset)
        {
            var deserializer = new VBSP();
            return deserializer.DeserializeImpl(data, offset);
        }

        /// <inheritdoc/>
        public Models.VBSP.File? DeserializeImpl(byte[]? data, int offset)
        {
            // If the data is invalid
            if (data == null)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Create a memory stream and parse that
            var dataStream = new MemoryStream(data, offset, data.Length - offset);
            return Streams.VBSP.Deserialize(dataStream);
        }
    }
}