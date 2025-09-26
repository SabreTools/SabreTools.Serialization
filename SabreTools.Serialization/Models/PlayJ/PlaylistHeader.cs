namespace SabreTools.Data.Models.PlayJ
{
    /// <summary>
    /// PlayJ playlist header
    /// </summary>
    public sealed class PlaylistHeader
    {
        /// <summary>
        /// Number of tracks contained within the playlist
        /// </summary>
        public uint TrackCount { get; set; }

        /// <summary>
        /// 52 bytes of unknown data
        /// </summary>
        public byte[]? Data { get; set; }
    }
}