namespace SabreTools.Data.Models.BSP
{
    /// <see href="https://developer.valvesoftware.com/wiki/BSP_(GoldSrc)"/>
    public sealed class ClipnodesLump : Lump
    {
        /// <summary>
        /// Clipnodes
        /// </summary>
        public Clipnode[] Clipnodes { get; set; } = [];
    }
}
