namespace SabreTools.Data.Models.BSP
{
    /// <summary>
    /// The leafface lump (Lump 16) is an array of unsigned shorts which are
    /// used to map from faces referenced in the leaf structure to indices in
    /// the face array.
    /// </summary>
    /// <see href="https://developer.valvesoftware.com/wiki/BSP_(Source)"/>
    public sealed class LeafFacesLump : Lump
    {
        /// <summary>
        /// Map
        /// </summary>
        public ushort[] Map { get; set; }
    }
}
