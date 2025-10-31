using System.Runtime.InteropServices;

namespace SabreTools.Data.Models.WAD3
{
    /// <see href="https://twhl.info/wiki/page/Specification:_WAD3"/>
    public sealed class MipTex : FileEntry
    {
        /// <summary>
        /// Null-terminated texture name
        /// </summary>
        /// <remarks>16 bytes</remarks>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public string Name = string.Empty;

        /// <summary>
        /// Dimensions of the texture (must be divisible by 16)
        /// </summary>
        public uint Width;

        /// <summary>
        /// Dimensions of the texture (must be divisible by 16)
        /// </summary>
        public uint Height;

        /// <summary>
        /// Offset from start of this struct to each mipmap level's image
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public uint[] MipOffsets = new uint[4];

        /// <summary>
        /// One MipMap for each mipmap level
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public MipMap[] MipImages = new MipMap[4];

        /// <summary>
        /// Number of colors used in the palette (always 256 for GoldSrc)
        /// </summary>
        public ushort ColorsUsed;

        /// <summary>
        /// The color palette for the mipmaps (always 256 * 3 = 768 for GoldSrc)
        /// </summary>
        /// <remarks>[ColorsUsed][3]</remarks>
        public byte[][]? Palette { get; set; }
    }
}
