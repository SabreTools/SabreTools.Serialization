using System.Runtime.InteropServices;

namespace SabreTools.Data.Models.NCF
{
    /// <see href="https://github.com/RavuAlHemio/hllib/blob/master/HLLib/NCFFile.h"/>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class ChecksumHeader
    {
        /// <summary>
        /// Always 0x00000001
        /// </summary>
        public uint Dummy0;

        /// <summary>
        /// Size of LPNCFCHECKSUMHEADER & LPNCFCHECKSUMMAPHEADER & in bytes.
        /// </summary>
        public uint ChecksumSize;
    }
}
