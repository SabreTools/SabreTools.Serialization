using System.IO;
using SabreTools.IO;
using SabreTools.Models.PlayJ;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Deserializers
{
    public class PlayJPlaylist :
        IByteDeserializer<Playlist>,
        IFileDeserializer<Playlist>,
        IStreamDeserializer<Playlist>
    {
        #region IByteDeserializer

        /// <inheritdoc cref="IByteDeserializer.Deserialize(byte[]?, int)"/>
        public static Playlist? DeserializeBytes(byte[]? data, int offset)
        {
            var deserializer = new PlayJPlaylist();
            return deserializer.Deserialize(data, offset);
        }

        /// <inheritdoc/>
        public Playlist? Deserialize(byte[]? data, int offset)
        {
            // If the data is invalid
            if (data == null)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Create a memory stream and parse that
            var dataStream = new MemoryStream(data, offset, data.Length - offset);
            return DeserializeStream(dataStream);
        }

        #endregion

        #region IFileDeserializer

        /// <inheritdoc cref="IFileDeserializer.Deserialize(string?)"/>
        public static Playlist? DeserializeFile(string? path)
        {
            var deserializer = new PlayJPlaylist();
            return deserializer.Deserialize(path);
        }

        /// <inheritdoc/>
        public Playlist? Deserialize(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return DeserializeStream(stream);
        }

        #endregion

        #region IStreamDeserializer

        /// <inheritdoc cref="IStreamDeserializer.Deserialize(Stream?)"/>
        public static Playlist? DeserializeStream(Stream? data)
        {
            var deserializer = new PlayJPlaylist();
            return deserializer.Deserialize(data);
        }
        
        /// <inheritdoc/>
        public Playlist? Deserialize(Stream? data)
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            // If the offset is out of bounds
            if (data.Position < 0 || data.Position >= data.Length)
                return null;

            // Cache the current offset
            int initialOffset = (int)data.Position;

            // Create a new playlist to fill
            var playlist = new Playlist();

            #region Playlist Header

            // Try to parse the playlist header
            var playlistHeader = ParsePlaylistHeader(data);
            if (playlistHeader == null)
                return null;

            // Set the playlist header
            playlist.Header = playlistHeader;

            #endregion

            #region Audio Files

            // Create the audio files array
            playlist.AudioFiles = new AudioFile[playlistHeader.TrackCount];

            // Try to parse the audio files
            for (int i = 0; i < playlist.AudioFiles.Length; i++)
            {
                long currentOffset = data.Position;
                var entryHeader = PlayJAudio.DeserializeStream(data, currentOffset);
                if (entryHeader == null)
                    return null;

                playlist.AudioFiles[i] = entryHeader;
            }

            #endregion

            return playlist;
        }

        /// <summary>
        /// Parse a Stream into a playlist header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled playlist header on success, null on error</returns>
        private static PlaylistHeader ParsePlaylistHeader(Stream data)
        {
            // TODO: Use marshalling here instead of building
            PlaylistHeader playlistHeader = new PlaylistHeader();

            playlistHeader.TrackCount = data.ReadUInt32();
            playlistHeader.Data = data.ReadBytes(52);

            return playlistHeader;
        }

        #endregion
    }
}