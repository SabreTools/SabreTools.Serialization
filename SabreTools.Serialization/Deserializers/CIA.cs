using System.IO;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Deserializers
{
    public partial class CIA : IByteDeserializer<Models.N3DS.CIA>
    {
        /// <inheritdoc cref="IByteSerializer.Deserialize(byte[]?, int)"/>
        public static Models.N3DS.CIA? DeserializeBytes(byte[]? data, int offset)
        {
            var deserializer = new CIA();
            return deserializer.Deserialize(data, offset);
        }

        /// <inheritdoc/>
        public Models.N3DS.CIA? Deserialize(byte[]? data, int offset)
        {
            // If the data is invalid
            if (data == null)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Create a memory stream and parse that
            var dataStream = new MemoryStream(data, offset, data.Length - offset);
            return Streams.CIA.DeserializeStream(dataStream);
        }
    }
}