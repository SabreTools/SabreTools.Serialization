namespace SabreTools.Data.Models.BSP
{
    /// <see href="https://developer.valvesoftware.com/wiki/BSP_(Source)"/> 
    public sealed class VbspTexinfoLump : Lump
    {
        /// <summary>
        /// Texinfos
        /// </summary>
        public VbspTexinfo[]? Texinfos { get; set; }
    }
}