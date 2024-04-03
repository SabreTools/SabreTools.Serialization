using System.IO;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Bytes
{
    public partial class NCF : IByteDeserializer<Models.NCF.File>
    {
        /// <inheritdoc cref="IByteSerializer.Deserialize(byte[]?, int)"/>
        public static Models.NCF.File? DeserializeBytes(byte[]? data, int offset)
        {
            var deserializer = new NCF();
            return deserializer.Deserialize(data, offset);
        }

        /// <inheritdoc/>
        public Models.NCF.File? Deserialize(byte[]? data, int offset)
        {
            // If the data is invalid
            if (data == null)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Create a memory stream and parse that
            var dataStream = new MemoryStream(data, offset, data.Length - offset);
            return Streams.NCF.DeserializeStream(dataStream);
        }
    }
}