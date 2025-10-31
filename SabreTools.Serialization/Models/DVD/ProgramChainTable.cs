namespace SabreTools.Data.Models.DVD
{
    /// <see href="https://dvd.sourceforge.net/dvdinfo/ifo_vmg.html"/>
    public sealed class ProgramChainTable
    {
        /// <summary>
        /// Number of Program Chains
        /// </summary>
        public ushort NumberOfProgramChains { get; set; }

        /// <summary>
        /// Reserved
        /// </summary>
        public ushort Reserved { get; set; }

        /// <summary>
        /// End address (last byte of last PGC in this LU)
        /// relative to VMGM_LU
        /// </summary>
        public uint EndAddress { get; set; }

        /// <summary>
        /// Program Chains
        /// </summary>
        /// <remarks>NumberOfProgramChains entries</remarks>
        public ProgramChainTableEntry[] Entries { get; set; }
    }
}
