namespace SabreTools.Data.Models.BSP
{
    /// <see href="https://developer.valvesoftware.com/wiki/BSP_(Source)"/>
    public sealed class DispVertsLump : Lump
    {
        /// <summary>
        /// Verts
        /// </summary>
        public DispVert[]? Verts { get; set; }
    }
}