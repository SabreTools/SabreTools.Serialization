using System.IO;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Bytes
{
    // TODO: Add multi-cabinet reading
    public partial class InstallShieldCabinet : IByteSerializer<Models.InstallShieldCabinet.Cabinet>
    {
        /// <inheritdoc cref="IByteSerializer.Deserialize(byte[]?, int)"/>
        public static Models.InstallShieldCabinet.Cabinet? Deserialize(byte[]? data, int offset)
        {
            var deserializer = new InstallShieldCabinet();
            return deserializer.DeserializeImpl(data, offset);
        }

        /// <inheritdoc/>
        public Models.InstallShieldCabinet.Cabinet? DeserializeImpl(byte[]? data, int offset)
        {
            // If the data is invalid
            if (data == null)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Create a memory stream and parse that
            var dataStream = new MemoryStream(data, offset, data.Length - offset);
            return new Streams.InstallShieldCabinet().Deserialize(dataStream);
        }
    }
}