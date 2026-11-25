using SabreTools.Numerics;

namespace SabreTools.Data.Models.ISO9660.SystemUseEntries
{
    /// <summary>
    /// "SF" File data in sparse file format, the file data starts with a header block and index blocks
    /// </summary>
    /// <see href="https://people.freebsd.org/~emaste/rrip112.pdf"/>
    public sealed class SparseFile : BaseEntry
    {
        /// <summary>
        /// High order 32 bits of the 64-bit virtual file size
        /// </summary>
        public BothUInt32 VirtualFileSizeHigh { get; set; } = 0;

        /// <summary>
        /// Low order 32 bits of the 64-bit virtual file size
        /// </summary>
        public BothUInt32 VirtualFileSizeLow { get; set; } = 0;

        /// <summary>
        /// Depth of the first index block
        /// </summary>
        public byte TableDepth { get; set; }
    }
}
