using System.IO;
using SabreTools.IO.Extensions;
using SabreTools.Models.PlayJ;

namespace SabreTools.Serialization.Deserializers
{
    public class PlayJPlaylist : BaseBinaryDeserializer<Playlist>
    {
        /// <inheritdoc/>
        public override Playlist? Deserialize(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            // If the offset is out of bounds
            if (data.Position < 0 || data.Position >= data.Length)
                return null;

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
                    continue;

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
        private static PlaylistHeader? ParsePlaylistHeader(Stream data)
        {
            var playlistHeader = new PlaylistHeader();

            playlistHeader.TrackCount = data.ReadUInt32();
            if (playlistHeader.TrackCount > int.MaxValue)
                return null;

            playlistHeader.Data = data.ReadBytes(52);

            return playlistHeader;
        }
    }
}