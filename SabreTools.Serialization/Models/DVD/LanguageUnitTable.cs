namespace SabreTools.Serialization.Models.DVD
{
    /// <see href="https://dvd.sourceforge.net/dvdinfo/ifo_vmg.html"/>
    public sealed class LanguageUnitTable
    {
        /// <summary>
        /// Number of Language Units
        /// </summary>
        public ushort NumberOfLanguageUnits { get; set; }

        /// <summary>
        /// Reserved
        /// </summary>
        public ushort Reserved { get; set; }

        /// <summary>
        /// End address (last byte of last PGC in last LU)
        /// relative to VMGM_PGCI_UT
        /// </summary>
        public uint EndAddress { get; set; }

        /// <summary>
        /// Language Units
        /// </summary>
        /// <remarks>NumberOfVOBIDs entries</remarks>
        public LanguageUnitTableEntry[]? Entries { get; set; }

        /// <summary>
        /// Program Chains
        /// </summary>
        /// <remarks>NumberOfVOBIDs entries</remarks>
        public ProgramChainTable[]? ProgramChains { get; set; }
    }
}