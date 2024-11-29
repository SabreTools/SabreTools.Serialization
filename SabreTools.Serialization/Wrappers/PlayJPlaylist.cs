using System.IO;

namespace SabreTools.Serialization.Wrappers
{
    public class PlayJPlaylist : WrapperBase<Models.PlayJ.Playlist>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "PlayJ Playlist";

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public PlayJPlaylist(Models.PlayJ.Playlist? model, byte[]? data, int offset)
            : base(model, data, offset)
        {
            // All logic is handled by the base class
        }

        /// <inheritdoc/>
        public PlayJPlaylist(Models.PlayJ.Playlist? model, Stream? data)
            : base(model, data)
        {
            // All logic is handled by the base class
        }

        /// <summary>
        /// Create a PlayJ playlist from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the playlist</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A PlayJ playlist wrapper on success, null on failure</returns>
        public static PlayJPlaylist? Create(byte[]? data, int offset)
        {
            // If the data is invalid
            if (data == null)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Create a memory stream and use that
            var dataStream = new MemoryStream(data, offset, data.Length - offset);
            return Create(dataStream);
        }

        /// <summary>
        /// Create a PlayJ playlist from a Stream
        /// </summary>
        /// <param name="data">Stream representing the playlist</param>
        /// <returns>A PlayJ playlist wrapper on success, null on failure</returns>
        public static PlayJPlaylist? Create(Stream? data)
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            try
            {
                var playlist = Deserializers.PlayJPlaylist.DeserializeStream(data);
                if (playlist == null)
                    return null;

                return new PlayJPlaylist(playlist, data);
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}