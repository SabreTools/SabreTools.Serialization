namespace SabreTools.Serialization.Models.BSP
{
    /// <see href="https://developer.valvesoftware.com/wiki/BSP_(GoldSrc)"/> 
    /// <see href="https://developer.valvesoftware.com/wiki/BSP_(Source)"/> 
    public static class Constants
    {
        #region Header

        /// <summary>
        /// Number of lumps in a BSP
        /// </summary>
        public const int BSP_HEADER_LUMPS = 15;

        /// <summary>
        /// Number of lumps in a VBSP
        /// </summary>
        public const int VBSP_HEADER_LUMPS = 64;

        #endregion

        #region Lump

        public const int MAX_MAP_HULLS = 4;

        public const int MAX_MAP_MODELS = 400;
        public const int MAX_MAP_BRUSHES = 4096;
        public const int MAX_MAP_ENTITIES = 1024;
        public const int MAX_MAP_ENTSTRING = (128 * 1024);

        public const int MAX_MAP_PLANES = 32767;
        public const int MAX_MAP_NODES = 32767;
        public const int MAX_MAP_CLIPNODES = 32767;
        public const int MAX_MAP_LEAFS = 8192;
        public const int MAX_MAP_VERTS = 65535;
        public const int MAX_MAP_FACES = 65535;
        public const int MAX_MAP_MARKSURFACES = 65535;
        public const int MAX_MAP_TEXINFO = 8192;
        public const int MAX_MAP_EDGES = 256000;
        public const int MAX_MAP_SURFEDGES = 512000;
        public const int MAX_MAP_TEXTURES = 512;
        public const int MAX_MAP_MIPTEX = 0x200000;
        public const int MAX_MAP_LIGHTING = 0x200000;
        public const int MAX_MAP_VISIBILITY = 0x200000;

        public const int MAX_MAP_PORTALS = 65536;

        #endregion
    
        #region Entities

        public const int MAX_KEY = 32;

        public const int MAX_VALUE = 1024;

        #endregion

        #region Textures

        public const int MAXTEXTURENAME = 16;

        public const int MIPLEVELS = 4;

        #endregion
    
        #region VBSP

        public static readonly byte[] SignatureBytes = [0x56, 0x42, 0x53, 0x50];

        public const string SignatureString = "VBSP";

        public const uint SignatureUInt32 = 0x50534256;

        #endregion

        #region LZMA

        public static readonly byte[] LzmaHeaderBytes = [0x4C, 0x5A, 0x4D, 0x41];

        public const string LzmaHeaderString = "LZMA";

        public const uint LzmaHeaderUInt32 = 0x414D5A4C;
        
        #endregion
    
        #region Overlay

        public const int OVERLAY_BSP_FACE_COUNT = 64;

        #endregion
    
        #region Worldlights

        /// <summary>
        /// This says that the light was put into the per-leaf ambient cubes.
        /// </summary>
        public const int DWL_FLAGS_INAMBIENTCUBE = 0x0001;

        #endregion
    }
}