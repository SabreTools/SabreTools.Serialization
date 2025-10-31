namespace SabreTools.Data.Models.BSP
{
    /// <summary>
    /// Half-Life Level
    /// </summary>
    /// <see href="https://github.com/RavuAlHemio/hllib/blob/master/HLLib/BSPFile.h"/>
    /// <see href="https://developer.valvesoftware.com/wiki/BSP_(GoldSrc)"/>
    public sealed class BspFile
    {
        /// <summary>
        /// Header data
        /// </summary>
        public BspHeader Header { get; set; } = new();

        #region Lumps

        /// <summary>
        /// LUMP_ENTITIES [0]
        /// </summary>
        public EntitiesLump Entities { get; set; } = new();

        /// <summary>
        /// LUMP_PLANES [1]
        /// </summary>
        public PlanesLump PlanesLump { get; set; } = new();

        /// <summary>
        /// LUMP_TEXTURES [2]
        /// </summary>
        public TextureLump TextureLump { get; set; } = new();

        /// <summary>
        /// LUMP_VERTICES [3]
        /// </summary>
        public VerticesLump VerticesLump { get; set; } = new();

        /// <summary>
        /// LUMP_VISIBILITY [4]
        /// </summary>
        public VisibilityLump VisibilityLump { get; set; } = new();

        /// <summary>
        /// LUMP_NODES [5]
        /// </summary>
        public BspNodesLump NodesLump { get; set; } = new();

        /// <summary>
        /// LUMP_TEXINFO [6]
        /// </summary>
        public BspTexinfoLump TexinfoLump { get; set; } = new();

        /// <summary>
        /// LUMP_FACES [7]
        /// </summary>
        public BspFacesLump FacesLump { get; set; } = new();

        /// <summary>
        /// LUMP_LIGHTING [8]
        /// </summary>
        public LightmapLump LightmapLump { get; set; } = new();

        /// <summary>
        /// LUMP_CLIPNODES [9]
        /// </summary>s
        public ClipnodesLump ClipnodesLump { get; set; } = new();

        /// <summary>
        /// LUMP_LEAVES [10]
        /// </summary>
        public BspLeavesLump LeavesLump { get; set; } = new();

        /// <summary>
        /// LUMP_MARKSURFACES [11]
        /// </summary>
        public MarksurfacesLump MarksurfacesLump { get; set; } = new();

        /// <summary>
        /// LUMP_EDGES [12]
        /// </summary>
        public EdgesLump EdgesLump { get; set; } = new();

        /// <summary>
        /// LUMP_SURFEDGES [13]
        /// </summary>
        public SurfedgesLump SurfedgesLump { get; set; } = new();

        /// <summary>
        /// LUMP_MODELS [14]
        /// </summary>
        public BspModelsLump ModelsLump { get; set; } = new();

        #endregion
    }
}
