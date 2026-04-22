namespace SabreTools.Data.Models.NintendoDisc
{
    /// <summary>
    /// Represents a parsed GameCube or Wii disc image
    /// </summary>
    public class Disc
    {
        /// <summary>
        /// Disc boot block header (first 0x440 bytes)
        /// </summary>
        public DiscHeader Header { get; set; } = new();

        /// <summary>
        /// Detected platform (GameCube or Wii)
        /// </summary>
        public Platform Platform { get; set; }

        /// <summary>
        /// Wii partition table entries (Wii discs only)
        /// </summary>
        public WiiPartitionTableEntry[]? PartitionTableEntries { get; set; }

        /// <summary>
        /// Wii region data at disc offset 0x4E000 (Wii discs only)
        /// </summary>
        public WiiRegionData? RegionData { get; set; }
    }
}
