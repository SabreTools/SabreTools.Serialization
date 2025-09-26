namespace SabreTools.Serialization.Models.WAD3
{
    /// <see href="https://twhl.info/wiki/page/Specification:_WAD3"/>
    public sealed class QpicImage : FileEntry
    {
        /// <summary>
        /// Dimensions of the texture (must be divisible by 16)
        /// </summary>
        public uint Width;

        /// <summary>
        /// Dimensions of the texture (must be divisible by 16)
        /// </summary>
        public uint Height;

        /// <summary>
        /// Raw image data. Each byte points to an index in the palette
        /// </summary>
        /// <remarks>[height][width]</remarks>
        public byte[][]? Data { get; set; }

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
