using System.IO;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Deserializers
{
    // TODO: Add multi-cabinet reading
    public partial class InstallShieldCabinet : IByteDeserializer<Models.InstallShieldCabinet.Cabinet>
    {
        /// <inheritdoc cref="IByteSerializer.Deserialize(byte[]?, int)"/>
        public static Models.InstallShieldCabinet.Cabinet? DeserializeBytes(byte[]? data, int offset)
        {
            var deserializer = new InstallShieldCabinet();
            return deserializer.Deserialize(data, offset);
        }

        /// <inheritdoc/>
        public Models.InstallShieldCabinet.Cabinet? Deserialize(byte[]? data, int offset)
        {
            // If the data is invalid
            if (data == null)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Create a memory stream and parse that
            var dataStream = new MemoryStream(data, offset, data.Length - offset);
            return Streams.InstallShieldCabinet.DeserializeStream(dataStream);
        }
    }
}