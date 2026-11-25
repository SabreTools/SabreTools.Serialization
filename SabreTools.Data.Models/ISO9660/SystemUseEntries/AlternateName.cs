namespace SabreTools.Data.Models.ISO9660.SystemUseEntries
{
    /// <summary>
    /// "NM" Alternate name, the POSIX name of the file, used instead of the ISO 9660 file identifier
    /// The name contents of several entries are concatenated until an entry without the CONTINUE flag
    /// </summary>
    /// <see href="https://people.freebsd.org/~emaste/rrip112.pdf"/>
    public sealed class AlternateName : BaseEntry
    {
        /// <summary>
        /// Flags of the entry
        /// </summary>
        public AlternateNameFlags Flags { get; set; }

        /// <summary>
        /// Name content, the alternate name or a part of it
        /// </summary>
        public byte[] NameContent { get; set; } = [];
    }
}
