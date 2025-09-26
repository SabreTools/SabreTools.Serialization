using System.Runtime.InteropServices;

namespace SabreTools.Serialization.Models.VPK
{
    /// <see href="https://github.com/RavuAlHemio/hllib/blob/master/HLLib/VPKFile.h"/>
    /// <see href="https://developer.valvesoftware.com/wiki/VPK_(file_format)"/>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class Header
    {
        /// <summary>
        /// Always 0x55aa1234.
        /// </summary>
        public uint Signature;

        public uint Version;

        /// <summary>
        /// The size, in bytes, of the directory tree
        /// </summary>
        public uint TreeSize;
    }
}
