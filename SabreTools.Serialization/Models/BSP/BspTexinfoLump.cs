namespace SabreTools.Data.Models.BSP
{
    /// <see href="https://developer.valvesoftware.com/wiki/BSP_(GoldSrc)"/> 
    public sealed class BspTexinfoLump : Lump
    {
        /// <summary>
        /// Texinfos
        /// </summary>
        public BspTexinfo[]? Texinfos { get; set; }
    }
}