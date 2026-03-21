using System.Runtime.InteropServices;

namespace SabreTools.Data.Models.CHD
{
    /// <see href="https://github.com/mamedev/mame/blob/master/src/lib/util/chd.h"/>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class CompressedMapEntryV5
    {
        /// <summary>
        /// Compression type
        /// </summary>
        public byte Compression;

        /// <summary>
        /// Compressed length
        /// </summary>
        /// <remarks>Actually UInt24</remarks>
        public uint CompLength;

        /// <summary>
        /// Offset
        /// </summary>
        /// <remarks>Actually UInt48</remarks>
        public ulong Offset;

        /// <summary>
        /// CRC-16 of the data
        /// </summary>
        public ushort CRC;
    }
}
