namespace SabreTools.Data.Models.BSP
{
    /// <see href="https://developer.valvesoftware.com/wiki/BSP_(Source)"/>
    public sealed class OverlaysLump : Lump
    {
        /// <summary>
        /// Overlays
        /// </summary>
        public Overlay[] Overlays { get; set; }
    }
}
