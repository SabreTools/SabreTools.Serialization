using System.Runtime.InteropServices;

namespace SabreTools.Data.Models.BSP
{
    /// <summary>
    /// The structure is 176 bytes long. The startPosition element is the
    /// coordinates of the first corner of the displacement. DispVertStart
    /// and DispTriStart are indices into the DispVerts and DispTris lumps.
    /// The power entry gives the number of subdivisions in the displacement
    /// surface - allowed values are 2, 3 and 4, and these correspond to 4,
    /// 8 and 16 subdivisions on each side of the displacement surface.
    /// The structure also references any neighbouring displacements on the
    /// sides or the corners of this displacement through the EdgeNeighbors
    /// and CornerNeighbors members. There are complex rules governing the
    /// order that these neighbour displacements are given; see the comments
    /// in bspfile.h for more. The MapFace value is an index into the face
    /// array and is face that was turned into a displacement surface.
    /// This face is used to set the texture and overall physical location
    /// and boundaries of the displacement.
    /// </summary>
    /// <see href="https://developer.valvesoftware.com/wiki/BSP_(Source)"/>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class DispInfo
    {
        /// <summary>
        /// Start position used for orientation
        /// </summary>
        public Vector3D StartPosition = new();

        /// <summary>
        /// Index into LUMP_DISP_VERTS.
        /// </summary>
        public int DispVertStart;

        /// <summary>
        /// Index into LUMP_DISP_TRIS.
        /// </summary>
        public int DispTriStart;

        /// <summary>
        /// Power - indicates size of surface (2^power 1)
        /// </summary>
        public int Power;

        /// <summary>
        /// Minimum tesselation allowed
        /// </summary>
        public int MinTess;

        /// <summary>
        /// Lighting smoothing angle
        /// </summary>
        public float SmoothingAngle;

        /// <summary>
        /// Surface contents
        /// </summary>
        public int Contents;

        /// <summary>
        /// Which map face this displacement comes from.
        /// </summary>
        public ushort MapFace;

        /// <summary>
        /// Index into ddisplightmapalpha.
        /// </summary>
        public int LightmapAlphaStart;

        /// <summary>
        /// Index into LUMP_DISP_LIGHTMAP_SAMPLE_POSITIONS.
        /// </summary>
        public int LightmapSamplePositionStart;

        /// <summary>
        /// Indexed by NEIGHBOREDGE_ defines.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public CDispNeighbor[] EdgeNeighbors = new CDispNeighbor[4];

        /// <summary>
        /// Indexed by CORNER_ defines.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public CDispCornerNeighbors[] CornerNeighbors = new CDispCornerNeighbors[4];

        /// <summary>
        /// Active verticies
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public uint[] AllowedVerts = new uint[10];
    }
}
