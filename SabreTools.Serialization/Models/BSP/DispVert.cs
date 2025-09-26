using System.Runtime.InteropServices;

namespace SabreTools.Data.Models.BSP
{
    /// <summary>
    /// The DispVerts lump (Lump 33) contains the vertex data of the displacements.
    /// vec is the normalized vector of the offset of each displacement vertex from
    /// its original (flat) position; dist is the distance the offset has taken
    /// place; and alpha is the alpha-blending of the texture at that vertex.
    /// 
    /// A displacement of power p references (2^p + 1)^2 dispverts in the array,
    /// starting from the DispVertStart index. 
    /// </summary>
    /// <see href="https://developer.valvesoftware.com/wiki/BSP_(Source)"/> 
    [StructLayout(LayoutKind.Sequential)]
    public sealed class DispVert
    {
        /// <summary>
        /// Vector field defining displacement volume.
        /// </summary>
        public Vector3D? Vec;

        /// <summary>
        /// Displacement distances.
        /// </summary>
        public float Dist;

        /// <summary>
        /// "Per vertex" alpha values.
        /// </summary>
        public float Alpha;
    }
}