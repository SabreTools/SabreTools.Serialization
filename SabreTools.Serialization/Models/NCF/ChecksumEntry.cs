using System.Runtime.InteropServices;

namespace SabreTools.Data.Models.NCF
{
    /// <see href="https://github.com/RavuAlHemio/hllib/blob/master/HLLib/NCFFile.h"/>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class ChecksumEntry
    {
        /// <summary>
        /// Checksum.
        /// </summary>
        public uint Checksum;
    }
}
