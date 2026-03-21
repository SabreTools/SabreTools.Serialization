using System.Runtime.InteropServices;

namespace SabreTools.Data.Models.BSP
{
    /// <summary>
    /// The doccluderdata_t structure contains flags and dimensions
    /// of the occluder, as well as the area where it remains.
    /// firstpoly is the first index into the doccluderpolydata_t
    /// with a total of polycount entries.
    /// </summary>
    /// <see href="https://developer.valvesoftware.com/wiki/BSP_(Source)"/>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class OccluderData
    {
        public int Flags;

        /// <summary>
        /// Index into doccluderpolys
        /// </summary>
        public int FirstPoly;

        /// <summary>
        /// Amount of polygons
        /// </summary>
        public int PolyCount;

        /// <summary>
        /// Minima of all vertices
        /// </summary>
        public Vector3D Mins = new();

        /// <summary>
        /// Maxima of all vertices
        /// </summary>
        public Vector3D Maxs = new();

        /// <remarks>Since v1</remarks>
        public int Area;
    }
}
