namespace SabreTools.Data.Models.SpoonInstaller
{
    /// <summary>
    /// Structure similar to the end of central directory
    /// header in PKZIP files
    /// </summary>
    public class Footer
    {
        /// <summary>
        /// Unknown
        /// </summary>
        /// <remarks>
        /// Theorized to be a checksum?
        ///
        /// Observed values:
        /// - 81 83 DA 2F (802849665)
        /// </remarks>
        public uint Unknown0 { get; set; }

        /// <summary>
        /// Unknown
        /// </summary>
        /// <remarks>
        /// Observed values:
        /// - 6A 00 00 00 (106)
        /// </remarks>
        public uint Unknown1 { get; set; }

        /// <summary>
        /// Offset of the start of the file table
        /// </summary>
        public uint TablePointer { get; set; }

        /// <summary>
        /// Number of entries that preceed the footer
        /// </summary>
        public uint EntryCount { get; set; }

        /// <summary>
        /// Unknown
        /// </summary>
        /// <remarks>
        /// Could also be part of the signature?
        ///
        /// Observed values:
        /// - 83 52 34 03 (53760643)
        /// </remarks>
        public uint Unknown2 { get; set; }

        /// <summary>
        /// Unknown, signature?
        /// </summary>
        /// <remarks>
        /// Observed values:
        /// - 43 F3 FF 0E (251654979)
        /// </remarks>
        public uint Unknown3 { get; set; }
    }
}
