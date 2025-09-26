namespace SabreTools.Serialization.Models.BSP
{
    /// <see href="https://developer.valvesoftware.com/wiki/BSP_(Source)"/>
    public sealed class BrushesLump : Lump
    {
        /// <summary>
        /// Brushes
        /// </summary>
        public Brush[]? Brushes { get; set; }
    }
}