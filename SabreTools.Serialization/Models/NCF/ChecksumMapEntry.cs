using System.Runtime.InteropServices;

namespace SabreTools.Serialization.Models.NCF
{
    /// <see href="https://github.com/RavuAlHemio/hllib/blob/master/HLLib/NCFFile.h"/>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class ChecksumMapEntry
    {
        /// <summary>
        /// Number of checksums.
        /// </summary>
        public uint ChecksumCount;

        /// <summary>
        /// Index of first checksum.
        /// </summary>
        public uint FirstChecksumIndex;
    }
}
