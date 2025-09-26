using System.Runtime.InteropServices;

namespace SabreTools.Serialization.Models.BSP
{
    /// <summary>
    /// Occluder polygons are stored in the doccluderpolydata_t
    /// structure and contain the firstvertexindex field, which
    /// is the first index into the vertex array of the occluder,
    /// which are again indices for the vertex array of the vertex
    /// lump (Lump 3). The total number of vertex indices is
    /// stored in vertexcount.
    /// </summary>
    /// <see href="https://developer.valvesoftware.com/wiki/BSP_(Source)"/>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class OccluderPolyData
    {
        /// <summary>
        /// Index into doccludervertindices
        /// </summary>
        public int FirstVertexIndex;

        /// <summary>
        /// Amount of vertex indices
        /// </summary>
        public int VertexCount;

        public int PlanEnum;
    }
}