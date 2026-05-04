namespace SabreTools.Data.Models.WIA
{
    /// <summary>
    /// RVZ group entry — extends <see cref="WiaGroupEntry"/> with a packed-data size field.
    /// Size: 0x0C bytes.
    /// </summary>
    public sealed class RvzGroupEntry
    {
        /// <summary>
        /// Actual byte offset of this group's data within the RVZ file.
        /// (On disk this value is stored as <c>offset &gt;&gt; 2</c>.)
        /// </summary>
        public ulong DataOffset { get; set; }

        /// <summary>
        /// Total size of this group's data (compressed + any RVZ-pack section) in bytes
        /// </summary>
        public uint DataSize { get; set; }

        /// <summary>
        /// Size of the RVZ-packed (junk-stripped) portion within this group's data.
        /// 0 means no RVZ packing was applied.
        /// </summary>
        public uint RvzPackedSize { get; set; }
    }
}
