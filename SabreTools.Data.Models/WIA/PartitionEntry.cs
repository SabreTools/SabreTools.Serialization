namespace SabreTools.Data.Models.WIA
{
    /// <summary>
    /// Describes a contiguous range of sectors within a Wii partition.
    /// Part of a <see cref="PartitionEntry"/>. Size: 0x10 bytes.
    /// </summary>
    public sealed class PartitionDataEntry
    {
        /// <summary>Zero-based index of the first sector covered by this range</summary>
        public uint FirstSector { get; set; }

        /// <summary>Number of sectors covered by this range</summary>
        public uint NumberOfSectors { get; set; }

        /// <summary>Index into the group-entry array of the first group for this range</summary>
        public uint GroupIndex { get; set; }

        /// <summary>Number of groups covering this range</summary>
        public uint NumberOfGroups { get; set; }
    }

    /// <summary>
    /// Describes a single Wii partition: its AES title key and two sector ranges.
    /// Size: 0x30 bytes.
    /// </summary>
    public sealed class PartitionEntry
    {
        /// <summary>
        /// Decrypted AES-128 partition title key
        /// </summary>
        /// <remarks>16 bytes</remarks>
        public byte[] PartitionKey { get; set; } = new byte[16];

        /// <summary>First sector range for this partition (typically encrypted data)</summary>
        public PartitionDataEntry DataEntry0 { get; set; } = new();

        /// <summary>Second sector range for this partition (typically decrypted/raw data)</summary>
        public PartitionDataEntry DataEntry1 { get; set; } = new();
    }
}
