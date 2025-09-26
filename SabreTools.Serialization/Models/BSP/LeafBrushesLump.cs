namespace SabreTools.Data.Models.BSP
{
    /// <summary>
    /// The leafbrush lump (Lump 17) is an array of unsigned shorts which are
    /// used to map from brushes referenced in the leaf structure to indices in
    /// the brush array.
    /// </summary>
    /// <see href="https://developer.valvesoftware.com/wiki/BSP_(Source)"/>
    public sealed class LeafBrushesLump : Lump
    {
        /// <summary>
        /// Map
        /// </summary>
        public ushort[]? Map { get; set; }
    }
}