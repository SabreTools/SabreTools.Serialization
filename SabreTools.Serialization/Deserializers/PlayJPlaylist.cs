using System.IO;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Deserializers
{
    public class PlayJPlaylist :
        IByteDeserializer<Models.PlayJ.Playlist>,
        IFileDeserializer<Models.PlayJ.Playlist>
    {
        #region IByteDeserializer

        /// <inheritdoc cref="IByteDeserializer.Deserialize(byte[]?, int)"/>
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

        #endregion

        #region IFileDeserializer

        /// <inheritdoc cref="IFileDeserializer.Deserialize(string?)"/>
        public static Models.PlayJ.Playlist? DeserializeFile(string? path)
        {
            var deserializer = new PlayJPlaylist();
            return deserializer.Deserialize(path);
        }

        /// <inheritdoc/>
        public Models.PlayJ.Playlist? Deserialize(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return Streams.PlayJPlaylist.DeserializeStream(stream);
        }

        #endregion
    }
}