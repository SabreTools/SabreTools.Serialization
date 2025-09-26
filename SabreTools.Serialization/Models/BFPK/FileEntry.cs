namespace SabreTools.Serialization.Models.BFPK
{
    /// <summary>
    /// File entry
    /// </summary>
    /// <see cref="https://forum.xentax.com/viewtopic.php?t=5102"/>
    public sealed class FileEntry
    {
        /// <summary>
        /// Name size
        /// </summary>
        public int NameSize { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Uncompressed size
        /// </summary>
        public int UncompressedSize { get; set; }

        /// <summary>
        /// Offset
        /// </summary>
        public int Offset { get; set; }

        /// <summary>
        /// Compressed size
        /// </summary>
        public int CompressedSize { get; set; }
    }
}
