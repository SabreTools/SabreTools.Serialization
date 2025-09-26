using System.IO;

namespace SabreTools.Serialization.Wrappers
{
    public class PlayJPlaylist : WrapperBase<SabreTools.Models.PlayJ.Playlist>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "PlayJ Playlist";

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public PlayJPlaylist(SabreTools.Models.PlayJ.Playlist model, byte[] data) : base(model, data) { }

        /// <inheritdoc/>
        public PlayJPlaylist(SabreTools.Models.PlayJ.Playlist model, byte[] data, int offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public PlayJPlaylist(SabreTools.Models.PlayJ.Playlist model, byte[] data, int offset, int length) : base(model, data, offset, length) { }

        /// <inheritdoc/>
        public PlayJPlaylist(SabreTools.Models.PlayJ.Playlist model, Stream data) : base(model, data) { }

        /// <inheritdoc/>
        public PlayJPlaylist(SabreTools.Models.PlayJ.Playlist model, Stream data, long offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public PlayJPlaylist(SabreTools.Models.PlayJ.Playlist model, Stream data, long offset, long length) : base(model, data, offset, length) { }

        #endregion

        #region Static Constructors

        /// <summary>
        /// Create a PlayJ playlist from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the playlist</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A PlayJ playlist wrapper on success, null on failure</returns>
        public static PlayJPlaylist? Create(byte[]? data, int offset)
        {
            // If the data is invalid
            if (data == null || data.Length == 0)
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
            if (data == null || !data.CanRead)
                return null;

            try
            {
                // Cache the current offset
                long currentOffset = data.Position;

                var model = new Deserializers.PlayJPlaylist().Deserialize(data);
                if (model == null)
                    return null;

                return new PlayJPlaylist(model, data, currentOffset);
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}
