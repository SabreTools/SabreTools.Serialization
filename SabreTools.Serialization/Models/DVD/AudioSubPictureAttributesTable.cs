namespace SabreTools.Data.Models.DVD
{
    /// <see href="https://dvd.sourceforge.net/dvdinfo/ifo_vmg.html"/>
    public sealed class AudioSubPictureAttributesTable
    {
        /// <summary>
        /// Number of title sets
        /// </summary>
        public ushort NumberOfTitleSets { get; set; }

        /// <summary>
        /// Reserved
        /// </summary>
        public ushort Reserved { get; set; }

        /// <summary>
        /// End address (last byte of last VTS_ATRT)
        /// </summary>
        public uint EndAddress { get; set; }

        /// <summary>
        /// Offset to VTS_ATRT n
        /// </summary>
        /// <remarks>NumberOfTitleSets entries</remarks>
        public uint[] Offsets { get; set; }

        /// <summary>
        /// Entries
        /// </summary>
        public AudioSubPictureAttributesTableEntry[] Entries { get; set; }
    }
}
