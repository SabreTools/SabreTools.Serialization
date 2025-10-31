namespace SabreTools.Data.Models.BSP
{
    /// <summary>
    /// The marksurfaces lump is a simple array of short integers.
    ///
    /// This lump is a simple table for redirecting the marksurfaces
    /// indexes in the leafs to the actial face indexes. A leaf inserts
    /// it's marksurface indexes into this array and gets the associated
    /// faces contained within this leaf.
    /// </summary>
    /// <see href="https://developer.valvesoftware.com/wiki/BSP_(GoldSrc)"/>
    public sealed class MarksurfacesLump : Lump
    {
        /// <summary>
        /// Marksurfaces
        /// </summary>
        public ushort[] Marksurfaces { get; set; }
    }
}
