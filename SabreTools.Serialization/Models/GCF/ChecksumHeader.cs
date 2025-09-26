using System.Runtime.InteropServices;

namespace SabreTools.Serialization.Models.GCF
{
    /// <see href="https://github.com/RavuAlHemio/hllib/blob/master/HLLib/GCFFile.h"/>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class ChecksumHeader
    {
        /// <summary>
        /// Always 0x00000001
        /// </summary>
        public uint Dummy0;

        /// <summary>
        /// Size of LPGCFCHECKSUMHEADER & LPGCFCHECKSUMMAPHEADER & in bytes.
        /// </summary>
        public uint ChecksumSize;
    }
}