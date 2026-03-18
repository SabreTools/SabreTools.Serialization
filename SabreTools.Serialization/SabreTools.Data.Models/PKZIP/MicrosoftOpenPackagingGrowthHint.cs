namespace SabreTools.Data.Models.PKZIP
{
    /// <remarks>Header ID = 0xa220</remarks>
    /// <see href="https://pkware.cachefly.net/webdocs/casestudies/APPNOTE.TXT"/>
    public class MicrosoftOpenPackagingGrowthHint : ExtensibleDataField
    {
        /// <summary>
        /// Verification signature (A028)
        /// </summary>
        public ushort Sig { get; set; }

        /// <summary>
        /// Initial padding value
        /// </summary>
        public ushort PadVal { get; set; }

        /// <summary>
        /// Filled with NULL characters
        /// </summary>
        public byte[] Padding { get; set; } = [];
    }
}
