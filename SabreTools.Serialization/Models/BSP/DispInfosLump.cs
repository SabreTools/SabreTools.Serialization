namespace SabreTools.Data.Models.BSP
{
    /// <see href="https://developer.valvesoftware.com/wiki/BSP_(Source)"/>
    public sealed class DispInfosLump : Lump
    {
        /// <summary>
        /// Infos
        /// </summary>
        public DispInfo[] Infos { get; set; }
    }
}
