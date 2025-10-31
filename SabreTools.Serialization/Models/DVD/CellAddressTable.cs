namespace SabreTools.Data.Models.DVD
{
    /// <see href="https://dvd.sourceforge.net/dvdinfo/ifo.html"/>
    public sealed class CellAddressTable
    {
        /// <summary>
        /// Number of VOB IDs
        /// </summary>
        public ushort NumberOfVOBIDs { get; set; }

        /// <summary>
        /// Reserved
        /// </summary>
        public ushort Reserved { get; set; }

        /// <summary>
        /// End address (last byte of last entry)
        /// </summary>
        public uint EndAddress { get; set; }

        /// <summary>
        /// 12-byte entries
        /// </summary>
        /// <remarks>NumberOfVOBIDs entries</remarks>
        public CellAddressTableEntry[] Entries { get; set; }
    }
}
