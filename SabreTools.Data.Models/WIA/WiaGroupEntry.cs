namespace SabreTools.Data.Models.WIA
{
    /// <summary>
    /// WIA group entry pointing to compressed data for one group. Size: 0x08 bytes.
    /// DataOffset is stored on-disk as the actual byte offset shifted right by 2 (i.e. &gt;&gt;2).
    /// </summary>
    public sealed class WiaGroupEntry
    {
        /// <summary>
        /// Actual byte offset of this group's data within the WIA file.
        /// (On disk this value is stored as <c>offset &gt;&gt; 2</c>.)
        /// </summary>
        public ulong DataOffset { get; set; }

        /// <summary>
        /// Compressed size of this group's data in bytes (0 means group contains only zeroes)
        /// </summary>
        public uint DataSize { get; set; }
    }
}
