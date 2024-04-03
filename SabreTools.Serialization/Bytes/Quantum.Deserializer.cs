using System.IO;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Bytes
{
    public partial class Quantum : IByteSerializer<Models.Quantum.Archive>
    {
        /// <inheritdoc cref="IByteSerializer.Deserialize(byte[]?, int)"/>
        public static Models.Quantum.Archive? Deserialize(byte[]? data, int offset)
        {
            var obj = new Quantum();
            return obj.DeserializeImpl(data, offset);
        }

        /// <inheritdoc/>
        public Models.Quantum.Archive? DeserializeImpl(byte[]? data, int offset)
        {
            // If the data is invalid
            if (data == null)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Create a memory stream and parse that
            var dataStream = new MemoryStream(data, offset, data.Length - offset);
            return new Streams.Quantum().Deserialize(dataStream);
        }
    }
}