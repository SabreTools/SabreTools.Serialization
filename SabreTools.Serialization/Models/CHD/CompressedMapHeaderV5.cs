using System.Runtime.InteropServices;

namespace SabreTools.Data.Models.CHD
{
    /// <see href="https://github.com/mamedev/mame/blob/master/src/lib/util/chd.h"/> 
    [StructLayout(LayoutKind.Sequential)]
    public sealed class CompressedMapHeaderV5
    {
        /// <summary>
        /// Length of compressed map
        /// </summary>
        public uint Length;

        /// <summary>
        /// Offset of first block
        /// </summary>
        /// <remarks>Actually UInt48</remarks>
        public ulong DataStart;

        /// <summary>
        /// CRC-16 of the map
        /// </summary>
        public ushort CRC;

        /// <summary>
        /// Bits used to encode complength
        /// </summary>
        public byte LengthBits;

        /// <summary>
        /// Bits used to encode self-refs
        /// </summary>
        public byte HunkBits;

        /// <summary>
        /// Bits used to encode parent unit refs
        /// </summary>
        public byte ParentUnitBits;

        /// <summary>
        /// Future use
        /// </summary>
        public byte Reserved;
    }
}
