using System.Runtime.InteropServices;

namespace SabreTools.Data.Models.BSP
{
    /// <summary>
    /// Unlike the simpler decals (infodecal entities), info_overlays
    /// are removed from the entity lump and stored separately in the
    /// Overlay lump (Lump 45). 
    /// 
    /// The FaceCountAndRenderOrder member is split into two parts;
    /// the lower 14 bits are the number of faces that the overlay
    /// appears on, with the top 2 bits being the render order of
    /// the overlay (for overlapping decals). The Ofaces array, which
    /// is 64 elements in size (OVERLAY_BSP_FACE_COUNT) are the indices
    /// into the face array indicating which map faces the overlay
    /// should be displayed on. The other elements set the texture,
    /// scale, and orientation of the overlay decal. There is no
    /// enforced limit on overlays inside the engine. VBSP enforces
    /// a limit of 512 (MAX_MAP_OVERLAYS, 1024 in Counter-Strike:
    /// Global Offensive), but custom compilers can circumvent this. 
    /// </summary>
    /// <see href="https://developer.valvesoftware.com/wiki/BSP_(Source)"/>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class Overlay
    {
        public int Id;

        public short TexInfo;

        public ushort FaceCountAndRenderOrder;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.OVERLAY_BSP_FACE_COUNT)]
        public int[]? Ofaces = new int[Constants.OVERLAY_BSP_FACE_COUNT];

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public float[]? U = new float[2];

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public float[]? V = new float[2];

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public Vector3D[]? UVPoints = new Vector3D[4];

        public Vector3D? Origin;

        public Vector3D? BasisNormal;
    }
}