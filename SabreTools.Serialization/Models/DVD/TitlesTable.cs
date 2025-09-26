namespace SabreTools.Serialization.Models.DVD
{
    /// <see href="https://dvd.sourceforge.net/dvdinfo/ifo_vmg.html"/>
    public sealed class TitlesTable
    {
        /// <summary>
        /// Number of titles
        /// </summary>
        public ushort NumberOfTitles { get; set; }

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
        /// <remarks>NumberOfTitles entries</remarks>
        public TitlesTableEntry[]? Entries { get; set; }
    }
}