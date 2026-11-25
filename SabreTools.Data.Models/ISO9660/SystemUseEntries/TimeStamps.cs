namespace SabreTools.Data.Models.ISO9660.SystemUseEntries
{
    /// <summary>
    /// "TF" Time stamp(s) for a file, the time stamps given by the flags, in the order of the flag bits
    /// </summary>
    /// <see href="https://people.freebsd.org/~emaste/rrip112.pdf"/>
    public sealed class TimeStamps : BaseEntry
    {
        /// <summary>
        /// Flags of the entry, the recorded time stamps and their format
        /// </summary>
        public TimeStampFlags Flags { get; set; }

        /// <summary>
        /// Time stamps in the 7-byte format of a directory record, recorded if LONG_FORM is not set
        /// </summary>
        public DirectoryRecordDateTime[] ShortFormTimeStamps { get; set; } = [];

        /// <summary>
        /// Time stamps in the 17-byte format of a volume descriptor, recorded if LONG_FORM is set
        /// </summary>
        public DecDateTime[] LongFormTimeStamps { get; set; } = [];
    }
}
