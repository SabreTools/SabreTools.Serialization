namespace SabreTools.Data.Models.BSP
{
    /// <summary>
    /// This is one of the largest lumps in the BSP file. The lightmap
    /// lump stores all lightmaps used in the entire map. The lightmaps
    /// are arrays of triples of bytes (3 channel color, RGB) and stored
    /// continuously.
    /// </summary>
    /// <see href="https://developer.valvesoftware.com/wiki/BSP_(GoldSrc)"/>
    public sealed class LightmapLump : Lump
    {
        /// <summary>
        /// Lightmap RGB values
        /// </summary>
        /// <remarks>Array of 3-byte values</remarks>
        public byte[][] Lightmap { get; set; }
    }
}
