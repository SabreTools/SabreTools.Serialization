namespace SabreTools.Data.Models.WIA
{
    /// <summary>
    /// Describes a region of non-partition data (e.g. disc header, partition table).
    /// Size: 0x18 bytes.
    /// </summary>
    public sealed class RawDataEntry
    {
        /// <summary>
        /// Byte offset of this region within the equivalent ISO image
        /// </summary>
        /// <remarks>Big-endian</remarks>
        public ulong DataOffset { get; set; }

        /// <summary>
        /// Size of this region in bytes
        /// </summary>
        /// <remarks>Big-endian</remarks>
        public ulong DataSize { get; set; }

        /// <summary>
        /// Index into the group-entry array of the first group for this region
        /// </summary>
        /// <remarks>Big-endian</remarks>
        public uint GroupIndex { get; set; }

        /// <summary>
        /// Number of groups covering this region
        /// </summary>
        /// <remarks>Big-endian</remarks>
        public uint NumberOfGroups { get; set; }
    }
}
