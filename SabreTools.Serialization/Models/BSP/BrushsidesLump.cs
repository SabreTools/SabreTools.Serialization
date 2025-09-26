namespace SabreTools.Data.Models.BSP
{
    /// <see href="https://developer.valvesoftware.com/wiki/BSP_(Source)"/>
    public sealed class BrushsidesLump : Lump
    {
        /// <summary>
        /// Brushsides
        /// </summary>
        public Brushside[]? Brushsides { get; set; }
    }
}