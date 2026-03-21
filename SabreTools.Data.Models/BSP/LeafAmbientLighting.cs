using System.Runtime.InteropServices;

namespace SabreTools.Data.Models.BSP
{
    /// <summary>
    /// The ambient lighting lumps (Lumps 55 and 56) are present in
    /// BSP version 20 and later. Lump 55 is used for HDR lighting,
    /// and Lump 56 is used for LDR lighting. These lumps are used to
    /// store the volumetric ambient lighting information in each leaf
    /// (i.e. lighting information for entities such as NPCs, the
    /// viewmodel, and non-static props). Prior to version 20, this
    /// data was stored in the leaf lump (Lump 10), in the dleaf_t
    /// structure, with far less precision than this newer lump allows.
    /// </summary>
    /// <see href="https://developer.valvesoftware.com/wiki/BSP_(Source)"/>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class LeafAmbientLighting
    {
        public CompressedLightCube Cube = new();

        /// <summary>
        /// Fixed point fraction of leaf bounds
        /// </summary>
        public byte X;

        /// <summary>
        /// Fixed point fraction of leaf bounds
        /// </summary>
        public byte Y;

        /// <summary>
        /// Fixed point fraction of leaf bounds
        /// </summary>
        public byte Z;

        /// <summary>
        /// Unused
        /// </summary>
        public byte Pad;
    }
}
