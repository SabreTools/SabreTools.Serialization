using System.IO;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Bytes
{
    public partial class IRD : IByteDeserializer<Models.IRD.File>
    {
        /// <inheritdoc cref="IByteSerializer.Deserialize(byte[]?, int)"/>
        public static Models.IRD.File? DeserializeBytes(byte[]? data, int offset)
        {
            var deserializer = new IRD();
            return deserializer.Deserialize(data, offset);
        }

        /// <inheritdoc/>
        public Models.IRD.File? Deserialize(byte[]? data, int offset)
        {
            // If the data is invalid
            if (data == null)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Create a memory stream and parse that
            var dataStream = new MemoryStream(data, offset, data.Length - offset);
            return Streams.IRD.DeserializeStream(dataStream);
        }
    }
}