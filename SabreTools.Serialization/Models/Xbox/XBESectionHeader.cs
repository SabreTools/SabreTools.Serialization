namespace SabreTools.Data.Models.Xbox
{
    /// <summary>
    /// XBox Executable format section header
    /// </summary>
    /// <see href="https://www.caustik.com/cxbx/download/xbe.htm"/>
    /// <see cref="COFF.SectionHeader"/>
    public class XBESectionHeader
    {
        /// <summary>
        /// Various flags for this .XBE section.
        /// </summary>
        public XbeSectionFlags SectionFlags { get; set; }

        /// <summary>
        /// Address of memory to load this section at.
        /// </summary>
        public uint VirtualAddress { get; set; }

        /// <summary>
        /// Number of bytes in memory to fill with this section.
        /// </summary>
        public uint VirtualSize { get; set; }

        /// <summary>
        /// File address where this section resides in the .XBE file.
        /// </summary>
        public uint RawAddress { get; set; }

        /// <summary>
        /// Number of bytes of this section that exist in the .XBE file.
        /// </summary>
        public uint RawSize { get; set; }

        /// <summary>
        /// Address to the name for this section, after the .XBE is loaded into memory.
        /// </summary>
        public uint SectionNameAddress { get; set; }

        /// <summary>
        /// It is typically safe to set this to zero.
        /// </summary>
        public uint SectionNameReferenceCount { get; set; }

        /// <summary>
        /// It is typically safe to set this to point to a 2-byte WORD in memory with value zero.
        /// </summary>
        public uint HeadSharedPageReferenceCountAddress { get; set; }

        /// <summary>
        /// It is typically safe to set this to point to a 2-byte WORD in memory with value zero.
        /// </summary>
        public uint TailSharedPageReferenceCountAddress { get; set; }

        /// <summary>
        /// 20-byte digest for this section. For unsigned .XBE files, it is safe to set this to zeros.
        /// </summary>
        public byte[] SectionDigest { get; set; } = new byte[20];
    }
}
