namespace SabreTools.Data.Models.NintendoDisc
{
    public class WiiPartitionGroup
    {
        /// <summary>
        /// Number of entries in the group
        /// </summary>
        /// <remarks>Big-endian</remarks>
        public uint Count { get; set; }

        /// <summary>
        /// Offset to the start of the table
        /// </summary>
        /// <remarks>Big-endian, requires left bit shift of 2 to get the real value</remarks>
        public uint Offset { get; set; }

        /// <summary>
        /// Entries for the group, stored at <see cref="Offset"/>
        /// </summary>
        /// <remarks>Number of entries determined by <see cref="Count"/></remarks>
        public WiiPartitionTableEntry[] Entries { get; set; } = [];
    }
}
