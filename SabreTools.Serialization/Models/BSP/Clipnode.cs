using System.Runtime.InteropServices;

namespace SabreTools.Serialization.Models.BSP
{
    /// <summary>
    /// This lump contains the so-called clipnodes, which build a second
    /// BSP tree used only for collision detection.
    /// 
    /// This structure is a reduced form of the BSPNODE struct from the
    /// nodes lump. Also the BSP tree built by the clipnodes is simpler
    /// than the one described by the BSPNODEs to accelerate collision calculations. 
    /// </summary>
    /// <see href="https://developer.valvesoftware.com/wiki/BSP_(GoldSrc)"/> 
    [StructLayout(LayoutKind.Sequential)]
    public sealed class Clipnode
    {
        /// <summary>
        /// Index into planes
        /// </summary>
        public int PlaneIndex;

        /// <summary>
        /// Negative numbers are contents
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public short[]? ChildrenIndices = new short[2];
    }
}