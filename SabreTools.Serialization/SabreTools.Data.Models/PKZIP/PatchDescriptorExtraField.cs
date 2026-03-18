namespace SabreTools.Data.Models.PKZIP
{
    /// <summary>
    /// The following is the layout of the Patch Descriptor
    /// "extra" block.
    ///
    /// Patch support is provided by PKPatchMaker(tm) technology
    /// and is covered under U.S. Patents and Patents Pending. The use or
    /// implementation in a product of certain technological aspects set
    /// forth in the current APPNOTE, including those with regard to
    /// strong encryption or patching requires a license from PKWARE.
    /// Refer to the section in this document entitled "Incorporating
    /// PKWARE Proprietary Technology into Your Product" for more
    /// information.
    /// </summary>
    /// <remarks>Header ID = 0x000F</remarks>
    /// <see href="https://pkware.cachefly.net/webdocs/casestudies/APPNOTE.TXT"/>
    public class PatchDescriptorExtraField : ExtensibleDataField
    {
        /// <summary>
        /// Version of the descriptor
        /// </summary>
        public ushort Version { get; set; }

        /// <summary>
        /// Actions and reactions
        /// </summary>
        public ActionsReactions Flags { get; set; }

        /// <summary>
        /// Size of the file about to be patched
        /// </summary>
        public uint OldSize { get; set; }

        /// <summary>
        /// 32-bit CRC of the file to be patched
        /// </summary>
        public uint OldCRC { get; set; }

        /// <summary>
        /// Size of the resulting file
        /// </summary>
        public uint NewSize { get; set; }

        /// <summary>
        /// 32-bit CRC of the resulting file
        /// </summary>
        public uint NewCRC { get; set; }
    }
}
