using System.Runtime.InteropServices;

namespace SabreTools.Data.Models.BSP
{
    /// <see href="https://github.com/RavuAlHemio/hllib/blob/master/HLLib/VBSPFile.h"/>
    /// <see href="https://developer.valvesoftware.com/wiki/BSP_(Source)"/>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public sealed class VbspLumpEntry : BspLumpEntry
    {
        /// <summary>
        /// Lump format version
        /// </summary>
        public uint Version;

        /// <summary>
        /// Lump ident code
        /// </summary>
        /// <remarks>Default to 0, 0, 0, 0</remarks>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] FourCC = new byte[4];
    }
}
