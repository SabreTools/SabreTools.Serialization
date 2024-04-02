using System.IO;
using SabreTools.Models.MicrosoftCabinet;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Bytes
{
    // TODO: Add multi-cabinet reading
    public partial class MicrosoftCabinet : IByteSerializer<Cabinet>
    {
        /// <inheritdoc/>
        public Cabinet? Deserialize(byte[]? data, int offset)
        {
            // If the data is invalid
            if (data == null)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Create a memory stream and parse that
            MemoryStream dataStream = new MemoryStream(data, offset, data.Length - offset);
            return new Streams.MicrosoftCabinet().Deserialize(dataStream);
        }
    }
}