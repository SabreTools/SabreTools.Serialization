namespace SabreTools.Data.Models.ISO9660.SystemUseEntries
{
    /// <summary>
    /// Component record of a "SL" entry, one component of the path of a symbolic link or a part of it
    /// The contents of several records are concatenated until a record without the CONTINUE flag
    /// </summary>
    /// <see href="https://people.freebsd.org/~emaste/rrip112.pdf"/>
    public sealed class SymbolicLinkComponent
    {
        /// <summary>
        /// Flags of the component record
        /// </summary>
        public ComponentFlags Flags { get; set; }

        /// <summary>
        /// Length of the component content in bytes
        /// </summary>
        public byte ComponentLength { get; set; }

        /// <summary>
        /// Component content, empty if the component is the current, parent or root directory
        /// </summary>
        public byte[] Content { get; set; } = [];
    }
}
