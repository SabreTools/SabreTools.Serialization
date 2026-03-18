namespace SabreTools.Data.Models.PFF
{
    /// <summary>
    /// PFF segment identifier
    /// </summary>
    /// <see href="https://devilsclaws.net/download/file-pff-new-bz2"/>
    public sealed class Segment
    {
        /// <summary>
        /// Deleted flag
        /// </summary>
        public uint Deleted { get; set; }

        /// <summary>
        /// File location
        /// </summary>
        public uint FileLocation { get; set; }

        /// <summary>
        /// File size
        /// </summary>
        public uint FileSize { get; set; }

        /// <summary>
        /// Packed date
        /// </summary>
        public uint PackedDate { get; set; }

        /// <summary>
        /// File name
        /// </summary>
        public string? FileName { get; set; }

        /// <summary>
        /// Modified date
        /// </summary>
        /// <remarks>Only for versions 3 and 4</remarks>
        public uint ModifiedDate { get; set; }

        /// <summary>
        /// Compression level
        /// </summary>
        /// <remarks>Only for version 4</remarks>
        public uint CompressionLevel { get; set; }
    }
}
