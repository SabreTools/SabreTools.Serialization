using System.Runtime.InteropServices;

namespace SabreTools.Data.Models.BSP
{
    /// <summary>
    /// The leaves lump contains the leaves of the BSP tree.
    ///
    /// The first entry of this struct is the type of the content
    /// of this leaf. It can be one of the predefined values, found
    /// in the compiler source codes, and is litte relevant for the
    /// actual rendering process. All the more important is the
    /// next integer containing the offset into the vis lump. It
    /// defines the start of the raw PVS data for this leaf. If this
    /// value equals -1, no VIS lists are available for this leaf,
    /// usually if the map has been built without the VIS compiler.
    /// The next two 16bit integer triples span the bounding box of
    /// this leaf. Furthermore, the struct contains an index pointing
    /// into the array of marksurfaces loaded from the marksufaces
    /// lump as well as the number of consecutive marksurfaces belonging
    /// to this leaf. The marksurfaces are looped through during the
    /// rendering process and point to the actual faces. The final 4 bytes
    /// specify the volume of ambient sounds in Quake, but are unused in
    /// GoldSrc.
    /// </summary>
    /// <see href="https://developer.valvesoftware.com/wiki/BSP_(Source)"/>
    public sealed class VbspLeaf
    {
        /// <summary>
        /// Contents enumeration
        /// </summary>
        public VbspContents Contents;

        /// <summary>
        /// Cluster this leaf is in
        /// </summary>
        public short Cluster;

        /// <summary>
        /// Area this leaf is in and flags
        /// </summary>
        /// <remarks>area:9, flags:7</remarks>
        public short AreaFlags;

        /// <summary>
        /// For frustum culling
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public short[] Mins = new short[3];

        /// <summary>
        /// For frustum culling
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public short[] Maxs = new short[3];

        /// <summary>
        /// Index into leaffaces
        /// </summary>
        public ushort FirstLeafFace;

        /// <summary>
        /// Count of leaffaces
        /// </summary>
        public ushort NumLeafFaces;

        /// <summary>
        /// Index into leafbrushes
        /// </summary>
        public ushort FirstLeafBrush;

        /// <summary>
        /// Count of leafbrushes
        /// </summary>
        public ushort NumLeafBrushes;

        /// <summary>
        /// -1 for not in water
        /// </summary>
        public short LeafWaterDataID;

        #region Lump Version 0

        /// <summary>
        /// Precaculated light info for entities.
        /// </summary>
        public CompressedLightCube AmbientLighting = new();

        #endregion

        #region Lump Version 1+

        /// <summary>
        /// Padding to 4-byte boundary
        /// </summary>
        public short Padding;

        #endregion
    }
}
