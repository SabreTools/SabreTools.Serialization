using System.Runtime.InteropServices;

namespace SabreTools.Data.Models.BSP
{
    /// <summary>
    /// The edges delimit the face and further refer to the vertices of the
    /// face. Each edge is pointing to the start and end vertex of the edge. 
    /// </summary>
    /// <see href="https://developer.valvesoftware.com/wiki/BSP_(GoldSrc)"/> 
    /// <see href="https://developer.valvesoftware.com/wiki/BSP_(Source)"/>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class Edge
    {
        /// <summary>
        /// Indices into vertex array
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public ushort[]? VertexIndices = new ushort[2];
    }
}