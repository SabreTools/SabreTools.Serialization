using System.Runtime.InteropServices;

namespace SabreTools.Data.Models.WAD3
{
    /// <see href="https://twhl.info/wiki/page/Specification:_WAD3"/>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public sealed class Font : FileEntry
    {
        /// <summary>
        /// Dimensions of the texture (must be divisible by 16)
        /// </summary>
        public uint Width;

        /// <summary>
        /// Dimensions of the texture (must be divisible by 16)
        /// </summary>
        public uint Height;

        public uint RowCount;

        public uint RowHeight;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        public CharInfo[] FontInfo = new CharInfo[256];

        /// <remarks>[width][height]</remarks>
        public byte[][]? Data;

        /// <summary>
        /// Number of colors used in the palette (always 256 for GoldSrc)
        /// </summary>
        public ushort ColorsUsed;

        /// <summary>
        /// The color palette for the mipmaps (always 256 * 3 = 768 for GoldSrc)
        /// </summary>
        /// <remarks>[ColorsUsed][3]</remarks>
        public byte[][] Palette { get; set; } = [];
    }
}
