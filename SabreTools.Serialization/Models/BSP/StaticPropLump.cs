using System.Runtime.InteropServices;

namespace SabreTools.Data.Models.BSP
{
    /// <summary>
    /// The coordinates of the prop are given by the Origin member; its
    /// orientation (pitch, yaw, roll) is given by the Angles entry, which
    /// is a 3-float vector. The PropType element is an index into the
    /// dictionary of prop model names, given above. The other elements
    /// correspond to the location of the prop in the BSP structure of the
    /// map, its lighting, and other entity properties as set in Hammer.
    /// The other elements (ForcedFadeScale, etc.) are only present in the
    /// static prop structure if the gamelump's specified version is high enough
    /// (see dgamelump_t.version); both version 4 and version 5 static prop
    /// gamelumps are used in official Half-Life 2 maps. Version 6 has been
    /// encountered in Team Fortress 2; Version 7 is used in some Left 4 Dead
    /// maps, and a modified version 7 is present in Zeno Clash maps. Version 8
    /// is used predominantly in Left 4 Dead, and version 9 in Left 4 Dead 2.
    /// The new version 10 appears in Tactical Intervention. Version 11 is used in
    /// Counter-Strike: Global Offensive since the addition of uniform prop scaling
    /// (before this it was version 10). After version 7, DX level options were
    /// removed. In version 11 XBox 360 flags were removed.
    /// 
    /// Version 7* is used by games built on Source 2013 Multiplayer
    /// ( Team Fortress 2, Counter-Strike: Source, etc.) and may come across as
    /// either version 7 or 10. Specifically, Team Fortress 2 has referred to it
    /// as version 7 in the past but now refers to it as version 10 even though
    /// they are identical. This version's structure is based on version 6 but
    /// rearranged such that Flags is an int and at the bottom, above two new
    /// entries. These new entries (LightmapResX and LightmapResY) control the
    /// width and height of the prop's lightmap image and are specific to this
    /// version. 
    /// </summary>
    /// <see href="https://developer.valvesoftware.com/wiki/BSP_(Source)"/>
    public sealed class StaticPropLump
    {
        #region v4

        /// <summary>
        /// Origin
        /// </summary>
        public Vector3D? Origin;

        /// <summary>
        /// Orientation (pitch yaw roll)
        /// </summary>
        public QAngle? Angles;

        #endregion

        #region v4

        /// <summary>
        /// Index into model name dictionary
        /// </summary>
        public ushort PropType;

        /// <summary>
        /// Index into leaf array
        /// </summary>
        public ushort FirstLeaf;

        public ushort LeafCount;

        /// <summary>
        /// Solidity type
        /// </summary>
        public byte Solid;

        #endregion

        #region Every version except v7*

        [MarshalAs(UnmanagedType.U1)]
        public StaticPropFlags FlagsV4;

        #endregion

        #region v4 still

        /// <summary>
        /// Model skin numbers
        /// </summary>
        public int Skin;

        public float FadeMinDist;

        public float FadeMaxDist;

        /// <summary>
        /// For lighting
        /// </summary>
        public Vector3D? LightingOrigin;

        #endregion

        #region Since v5

        /// <summary>
        /// Fade distance scale
        /// </summary>
        public float ForcedFadeScale;

        #endregion

        #region v6, v7, and v7* only

        /// <summary>
        /// Minimum DirectX version to be visible
        /// </summary>
        public ushort MinDXLevel;

        /// <summary>
        /// Maximum DirectX version to be visible
        /// </summary>
        public ushort MaxDXLevel;

        #endregion

        #region v7* only
        
        [MarshalAs(UnmanagedType.U4)]
        public byte FlagsV7;

        /// <summary>
        /// Lightmap image width
        /// </summary>
        public ushort LightmapResX;

        /// <summary>
        /// Lightmap image height
        /// </summary>
        public ushort LightmapResY;

        #endregion

        #region Since v8

        public byte MinCPULevel;

        public byte MaxCPULevel;

        public byte MinGPULevel;

        public byte MaxGPULevel;

        #endregion

        #region Since v7

        /// <summary>
        /// Per instance color and alpha modulation
        /// </summary>
        public ColorRGBExp32? DiffuseModulation;

        #endregion

        #region v9 and v10 only

        /// <summary>
        /// If true, don't show on XBox 360 (4-bytes long)
        /// </summary>
        public bool DisableX360;

        #endregion

        #region Since v10

        /// <summary>
        /// Further bitflags.
        /// </summary>
        public StaticPropFlagsEx FlagsEx;

        #endregion

        #region Since v11

        /// <summary>
        /// Prop scale
        /// </summary>
        public float UniformScale;

        #endregion
    }
}