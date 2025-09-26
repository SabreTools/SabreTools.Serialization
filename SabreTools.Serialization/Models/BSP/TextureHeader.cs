namespace SabreTools.Serialization.Models.BSP
{
    /// <see href="https://github.com/RavuAlHemio/hllib/blob/master/HLLib/BSPFile.h"/>
    /// <see href="https://developer.valvesoftware.com/wiki/BSP_(GoldSrc)"/> 
    public sealed class TextureHeader
    {
        /// <summary>
        /// Number of BSPMIPTEX structures
        /// </summary>
        public uint MipTextureCount { get; set; }

        /// <summary>
        /// Offsets
        /// </summary>
        /// <remarks><see cref="MipTextureCount"> entries</remarks>
        public int[]? Offsets { get; set; }
    }
}