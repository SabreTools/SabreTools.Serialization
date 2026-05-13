namespace SabreTools.Data.Models.NintendoDisc
{
    /// <summary>
    /// A single entry in the Wii disc partition table.
    /// The table lives at 0x40000-0x4FFFF on the disc.
    /// </summary>
    /// <see href="https://wiibrew.org/wiki/Wii_disc#Partition_table"/>
    public sealed class WiiPartitionTableEntry
    {
        /// <summary>
        /// Absolute byte offset of the partition on the disc
        /// </summary>
        /// <remarks>Big-endian, requires left bit shift of 2 to get the real value</remarks>
        public long Offset { get; set; }

        /// <summary>
        /// Partition type: 0 = DATA, 1 = UPDATE, 2 = CHANNEL, or an ASCII title ID.
        /// </summary>
        public uint Type { get; set; }
    }
}
