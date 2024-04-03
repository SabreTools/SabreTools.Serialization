using System.IO;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Deserializers
{
    public partial class PlayJPlaylist : IByteDeserializer<Models.PlayJ.Playlist>
    {
        /// <inheritdoc cref="IByteSerializer.Deserialize(byte[]?, int)"/>
        public static Models.PlayJ.Playlist? DeserializeBytes(byte[]? data, int offset)
        {
            var deserializer = new PlayJPlaylist();
            return deserializer.Deserialize(data, offset);
        }

        /// <inheritdoc/>
        public Models.PlayJ.Playlist? Deserialize(byte[]? data, int offset)
        {
            // If the data is invalid
            if (data == null)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Create a memory stream and parse that
            var dataStream = new MemoryStream(data, offset, data.Length - offset);
            return Streams.PlayJPlaylist.DeserializeStream(dataStream);
        }
    }
}