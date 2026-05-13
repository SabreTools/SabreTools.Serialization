namespace SabreTools.Data.Models.WIA
{
    /// <summary>
    /// Describes a contiguous range of sectors within a Wii partition.
    /// Part of a <see cref="PartitionEntry"/>. Size: 0x10 bytes.
    /// </summary>
    public sealed class PartitionDataEntry
    {
        /// <summary>
        /// Zero-based index of the first sector covered by this range
        /// </summary>
        /// <remarks>Big-endian</remarks>
        public uint FirstSector { get; set; }

        /// <summary>
        /// Number of sectors covered by this range
        /// </summary>
        /// <remarks>Big-endian</remarks>
        public uint NumberOfSectors { get; set; }

        /// <summary>
        /// Index into the group-entry array of the first group for this range
        /// </summary>
        /// <remarks>Big-endian</remarks>
        public uint GroupIndex { get; set; }

        /// <summary>
        /// Number of groups covering this range
        /// </summary>
        /// <remarks>Big-endian</remarks>
        public uint NumberOfGroups { get; set; }
    }
}
