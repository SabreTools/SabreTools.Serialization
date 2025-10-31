namespace SabreTools.Data.Models.BSP
{
    /// <see href="https://developer.valvesoftware.com/wiki/BSP_(GoldSrc)"/>
    public sealed class TextureLump : Lump
    {
        /// <summary>
        /// Texture header data
        /// </summary>
        public TextureHeader Header { get; set; }

        /// <summary>
        /// Textures
        /// </summary>
        public MipTexture[] Textures { get; set; }
    }
}
