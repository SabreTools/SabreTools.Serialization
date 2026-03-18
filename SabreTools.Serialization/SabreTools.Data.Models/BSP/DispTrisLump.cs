namespace SabreTools.Data.Models.BSP
{
    /// <see href="https://developer.valvesoftware.com/wiki/BSP_(Source)"/>
    public sealed class DispTrisLump : Lump
    {
        /// <summary>
        /// Tris
        /// </summary>
        public DispTri[] Tris { get; set; } = [];
    }
}
