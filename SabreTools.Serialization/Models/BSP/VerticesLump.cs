namespace SabreTools.Data.Models.BSP
{
    /// <summary>
    /// This lump simply consists of all vertices of the BSP tree.
    /// They are stored as a primitve array of triples of floats.
    /// </summary>
    /// <see href="https://developer.valvesoftware.com/wiki/BSP_(GoldSrc)"/>
    /// <see href="https://developer.valvesoftware.com/wiki/BSP_(Source)"/>
    public sealed class VerticesLump : Lump
    {
        /// <summary>
        /// Vertices
        /// </summary>
        public Vector3D[] Vertices { get; set; }
    }
}
