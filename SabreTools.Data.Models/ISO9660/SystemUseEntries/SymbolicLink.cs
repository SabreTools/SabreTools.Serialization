namespace SabreTools.Data.Models.ISO9660.SystemUseEntries
{
    /// <summary>
    /// "SL" Symbolic link, the content of a symbolic link as component records
    /// The component records of several entries are concatenated until an entry without the CONTINUE flag
    /// </summary>
    /// <see href="https://people.freebsd.org/~emaste/rrip112.pdf"/>
    public sealed class SymbolicLink : BaseEntry
    {
        /// <summary>
        /// Flags of the entry
        /// </summary>
        public SymbolicLinkFlags Flags { get; set; }

        /// <summary>
        /// Component records of the entry
        /// </summary>
        public SymbolicLinkComponent[] Components { get; set; } = [];
    }
}
