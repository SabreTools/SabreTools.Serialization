namespace SabreTools.Data.Models.NintendoDisc
{
    /// <summary>
    /// A single entry in the Wii disc partition table.
    /// The table lives at 0x40000–0x4FFFF on the disc.
    /// </summary>
    /// <see href="https://wiibrew.org/wiki/Wii_disc#Partition_table"/>
    public sealed class WiiPartitionTableEntry
    {
        /// <summary>
        /// Absolute byte offset of the partition on the disc.
        /// Stored on-disc as <c>offset &gt;&gt; 2</c> (big-endian u32).
        /// </summary>
        public long Offset { get; set; }

        /// <summary>
        /// Partition type: 0 = DATA, 1 = UPDATE, 2 = CHANNEL, or an ASCII title ID.
        /// </summary>
        public uint Type { get; set; }
    }
}
