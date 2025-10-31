using System.Runtime.InteropServices;

namespace SabreTools.Data.Models.CHD
{
    /// <see href="https://github.com/mamedev/mame/blob/master/src/lib/util/chd.h"/>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class MapV3
    {
        /// <summary>
        /// Starting offset within the file
        /// </summary>
        public ulong StartingOffset;

        /// <summary>
        /// 32-bit CRC of the uncompressed data
        /// </summary>
        public uint CRC32;

        /// <summary>
        /// Lower 16 bits of length
        /// </summary>
        public ushort LengthLo;

        /// <summary>
        /// Upper 8 bits of length
        /// </summary>
        public byte LengthHi;

        /// <summary>
        /// Flags, indicating compression info
        /// </summary>
        public byte Flags;
    }
}
