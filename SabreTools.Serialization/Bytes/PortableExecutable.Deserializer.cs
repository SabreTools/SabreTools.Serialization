using System.IO;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Bytes
{
    public partial class PortableExecutable : IByteSerializer<Models.PortableExecutable.Executable>
    {
        /// <inheritdoc cref="IByteSerializer.Deserialize(byte[]?, int)"/>
        public static Models.PortableExecutable.Executable? Deserialize(byte[]? data, int offset)
        {
            var obj = new PortableExecutable();
            return obj.DeserializeImpl(data, offset);
        }

        /// <inheritdoc/>
        public Models.PortableExecutable.Executable? DeserializeImpl(byte[]? data, int offset)
        {
            // If the data is invalid
            if (data == null)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Create a memory stream and parse that
            var dataStream = new MemoryStream(data, offset, data.Length - offset);
            return new Streams.PortableExecutable().Deserialize(dataStream);
        }
    }
}