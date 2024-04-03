using System.IO;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Bytes
{
    public partial class MSDOS : IByteSerializer<Models.MSDOS.Executable>
    {
        /// <inheritdoc cref="IByteSerializer.Deserialize(byte[]?, int)"/>
        public static Models.MSDOS.Executable? DeserializeBytes(byte[]? data, int offset)
        {
            var deserializer = new MSDOS();
            return deserializer.Deserialize(data, offset);
        }

        /// <inheritdoc/>
        public Models.MSDOS.Executable? Deserialize(byte[]? data, int offset)
        {
            // If the data is invalid
            if (data == null)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Create a memory stream and parse that
            var dataStream = new MemoryStream(data, offset, data.Length - offset);
            return Streams.MSDOS.DeserializeStream(dataStream);
        }
    }
}