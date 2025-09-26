namespace SabreTools.Data.Models.PlayJ
{
    /// <summary>
    /// PlayJ audio header / CDS entry header
    /// </summary>
    /// <remarks>V1 and V2 variants exist</remarks>
    public abstract class AudioHeader
    {
        /// <summary>
        /// Signature (0x4B539DFF)
        /// </summary>
        public uint Signature { get; set; }

        /// <summary>
        /// Version
        /// </summary>
        public uint Version { get; set; }

        // Header-specific data goes here

        /// <summary>
        /// Length of the track name
        /// </summary>
        public ushort TrackLength { get; set; }

        /// <summary>
        /// Track name (not null-terminated)
        /// </summary>
        public string? Track { get; set; }

        /// <summary>
        /// Length of the artist name
        /// </summary>
        public ushort ArtistLength { get; set; }

        /// <summary>
        /// Artist name (not null-terminated)
        /// </summary>
        public string? Artist { get; set; }

        /// <summary>
        /// Length of the album name
        /// </summary>
        public ushort AlbumLength { get; set; }

        /// <summary>
        /// Album name (not null-terminated)
        /// </summary>
        public string? Album { get; set; }

        /// <summary>
        /// Length of the writer name
        /// </summary>
        public ushort WriterLength { get; set; }

        /// <summary>
        /// Writer name (not null-terminated)
        /// </summary>
        public string? Writer { get; set; }

        /// <summary>
        /// Length of the publisher name
        /// </summary>
        public ushort PublisherLength { get; set; }

        /// <summary>
        /// Publisher name (not null-terminated)
        /// </summary>
        public string? Publisher { get; set; }

        /// <summary>
        /// Length of the label name
        /// </summary>
        public ushort LabelLength { get; set; }

        /// <summary>
        /// Label name (not null-terminated)
        /// </summary>
        public string? Label { get; set; }

        /// <summary>
        /// Length of the comments
        /// </summary>
        /// <remarks>Optional field only in some samples</remarks>
        public ushort CommentsLength { get; set; }

        /// <summary>
        /// Comments (not null-terminated)
        /// </summary>
        /// <remarks>Optional field only in some samples</remarks>
        public string? Comments { get; set; }
    }
}