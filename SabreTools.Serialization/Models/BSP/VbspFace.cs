using System.Runtime.InteropServices;

namespace SabreTools.Serialization.Models.BSP
{
    /// <summary>
    /// The face lump (Lump 7) contains the major geometry of the map,
    /// used by the game engine to render the viewpoint of the player.
    /// The face lump contains faces after they have undergone the BSP
    /// splitting process; they therefore do not directly correspond to
    /// the faces of brushes created in Hammer. Faces are always flat,
    /// convex polygons, though they can contain edges that are co-linear.
    /// </summary>
    /// <see href="https://developer.valvesoftware.com/wiki/BSP_(Source)"/> 
    [StructLayout(LayoutKind.Sequential)]
    public sealed class VbspFace
    {
        /// <summary>
        /// The plane number
        /// </summary>
        public ushort PlaneNum;

        /// <summary>
        /// Faces opposite to the node's plane direction
        /// </summary>
        public byte Side;

        /// <summary>
        ///  of on node, 0 if in leaf
        /// </summary>
        public byte OnNode;

        /// <summary>
        /// Index of the first surfedge
        /// </summary>
        public int FirstEdgeIndex;

        /// <summary>
        /// Number of consecutive surfedges
        /// </summary>
        public short NumberOfEdges;

        /// <summary>
        /// Index of the texture info structure
        /// </summary>
        public short TextureInfoIndex;

        /// <summary>
        /// Index of the displacement info structure
        /// </summary>
        public short DisplacementInfoIndex;

        /// <summary>
        /// ?
        /// </summary>
        public short SurfaceFogVolumeID;

        /// <summary>
        /// Switchable lighting info
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[]? Styles = new byte[4];

        /// <summary>
        /// Offset into lightmap lump
        /// </summary>
        public int LightmapOffset;

        /// <summary>
        /// Face area in units^2
        /// </summary>
        public float Area;

        /// <summary>
        /// Texture lighting info
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public int[]? LightmapTextureMinsInLuxels = new int[2];

        /// <summary>
        /// Texture lighting info
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public int[]? LightmapTextureSizeInLuxels = new int[2];

        /// <summary>
        /// Original face this was split from
        /// </summary>
        public int OrigFace;

        /// <summary>
        /// Primitives
        /// </summary>
        public ushort PrimitiveCount;

        /// <summary>
        /// First primitive ID
        /// </summary>
        public ushort FirstPrimitiveID;

        /// <summary>
        /// Lightmap smoothing group
        /// </summary>
        public uint SmoothingGroups;
    }
}