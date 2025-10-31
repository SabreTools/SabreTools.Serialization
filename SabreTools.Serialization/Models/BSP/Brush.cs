using System.Runtime.InteropServices;

namespace SabreTools.Data.Models.BSP
{
    /// <summary>
    /// The brush lump (Lump 18) contains all brushes that were
    /// present in the original VMF file before compiling.
    /// Unlike faces, brushes are constructive solid geometry (CSG)
    /// defined by planes instead of edges and vertices. It is the
    /// presence of the brush and brushside lumps in Source BSP
    /// files that makes decompiling them a much easier job than
    /// for GoldSrc files, which lacked this info.
    /// </summary>
    /// <see href="https://developer.valvesoftware.com/wiki/BSP_(Source)"/>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class Brush
    {
        /// <summary>
        /// First brushside
        /// </summary>
        public int FirstSide;

        /// <summary>
        /// Number of brushsides
        /// </summary>
        public int NumSides;

        /// <summary>
        /// Contents flags
        /// </summary>
        public VbspContents Contents;
    }
}
