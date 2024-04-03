using System.IO;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Bytes
{
    public partial class PlayJPlaylist : IByteSerializer<Models.PlayJ.Playlist>
    {
        /// <inheritdoc cref="IByteSerializer.Deserialize(byte[]?, int)"/>
        public static Models.PlayJ.Playlist? Deserialize(byte[]? data, int offset)
        {
            var deserializer = new PlayJPlaylist();
            return deserializer.DeserializeImpl(data, offset);
        }

        /// <inheritdoc/>
        public Models.PlayJ.Playlist? DeserializeImpl(byte[]? data, int offset)
        {
            // If the data is invalid
            if (data == null)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Create a memory stream and parse that
            var dataStream = new MemoryStream(data, offset, data.Length - offset);
            return new Streams.PlayJPlaylist().Deserialize(dataStream);
        }
    }
}