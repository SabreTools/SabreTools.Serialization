namespace SabreTools.Data.Models.BSP
{
    /// <summary>
    /// Half-Life Level
    /// </summary>
    /// <see href="https://github.com/RavuAlHemio/hllib/blob/master/HLLib/VBSPFile.h"/>
    /// <see href="https://developer.valvesoftware.com/wiki/BSP_(Source)"/>
    public sealed class VbspFile
    {
        /// <summary>
        /// Header data
        /// </summary>
        public VbspHeader? Header { get; set; }

        #region Lumps

        /// <summary>
        /// LUMP_ENTITIES [0]
        /// </summary>
        public EntitiesLump? Entities { get; set; }

        /// <summary>
        /// LUMP_PLANES [1]
        /// </summary>
        public PlanesLump? PlanesLump { get; set; }

        /// <summary>
        /// LUMP_TEXDATA [2]
        /// </summary>
        public TexdataLump? TexdataLump { get; set; }

        /// <summary>
        /// LUMP_VERTEXES [3]
        /// </summary>
        public VerticesLump? VerticesLump { get; set; }

        /// <summary>
        /// LUMP_VISIBILITY [4]
        /// </summary>
        public VisibilityLump? VisibilityLump { get; set; }

        /// <summary>
        /// LUMP_NODES [5]
        /// </summary>
        public VbspNodesLump? NodesLump { get; set; }

        /// <summary>
        /// LUMP_TEXINFO [6]
        /// </summary>
        public VbspTexinfoLump? TexinfoLump { get; set; }

        /// <summary>
        /// LUMP_FACES [7]
        /// </summary>
        public VbspFacesLump? FacesLump { get; set; }

        /// <summary>
        /// LUMP_LIGHTING [8]
        /// </summary>
        public LightmapLump? LightmapLump { get; set; }

        /// <summary>
        /// LUMP_OCCLUSION [9]
        /// </summary>s
        public OcclusionLump? OcclusionLump { get; set; }

        /// <summary>
        /// LUMP_LEAVES [10]
        /// </summary>
        public VbspLeavesLump? LeavesLump { get; set; }

        /// <summary>
        /// LUMP_FACEIDS [11]
        /// </summary>
        public MarksurfacesLump? MarksurfacesLump { get; set; }

        /// <summary>
        /// LUMP_EDGES [12]
        /// </summary>
        public EdgesLump? EdgesLump { get; set; }

        /// <summary>
        /// LUMP_SURFEDGES [13]
        /// </summary>
        public SurfedgesLump? SurfedgesLump { get; set; }

        /// <summary>
        /// LUMP_MODELS [14]
        /// </summary>
        public VbspModelsLump? ModelsLump { get; set; }

        /// <summary>
        /// LUMP_WORLDLIGHTS [15]
        /// </summary>
        public WorldLightsLump? LDRWorldLightsLump { get; set; }

        /// <summary>
        /// LUMP_LEAFFACES [16]
        /// </summary>
        public LeafFacesLump? LeafFacesLump { get; set; }

        /// <summary>
        /// LUMP_LEAFBRUSHES [17]
        /// </summary>
        public LeafBrushesLump? LeafBrushesLump { get; set; }

        /// <summary>
        /// LUMP_BRUSHES [18]
        /// </summary>
        public BrushesLump? BrushesLump { get; set; }

        /// <summary>
        /// LUMP_BRUSHSIDES [19]
        /// </summary>
        public BrushsidesLump? BrushsidesLump { get; set; }

        /// <summary>
        /// LUMP_AREAS [20]
        /// </summary>
        /// TODO: Find definition and implement
        // public AreasLump? AreasLump { get; set; }

        /// <summary>
        /// LUMP_AREAPORTALS [21]
        /// </summary>
        /// TODO: Find definition and implement
        // public AreaPortalsLump? AreaPortalsLump { get; set; }

        /// <summary>
        /// LUMP_PORTALS / LUMP_UNUSED0 / LUMP_PROPCOLLISION [22]
        /// </summary>
        /// TODO: Find definition and implement
        // public PortalsLump? PortalsLump { get; set; }

        /// <summary>
        /// LUMP_CLUSTERS / LUMP_UNUSED1 / LUMP_PROPHULLS [23]
        /// </summary>
        /// TODO: Find definition and implement
        // public ClustersLump? ClustersLump { get; set; }

        /// <summary>
        /// LUMP_PORTALVERTS / LUMP_UNUSED2 / LUMP_FAKEENTITIES / LUMP_PROPHULLVERTS [24]
        /// </summary>
        /// TODO: Find definition and implement
        // public PortalVertsLump? PortalVertsLump { get; set; }

        /// <summary>
        /// LUMP_CLUSTERPORTALS / LUMP_UNUSED3 / LUMP_PROPTRIS [25]
        /// </summary>
        /// TODO: Find definition and implement
        // public ClusterPortalsLump? ClusterPortalsLump { get; set; }

        /// <summary>
        /// LUMP_DISPINFO [26]
        /// </summary>
        public DispInfosLump? DispInfosLump { get; set; }

        /// <summary>
        /// LUMP_ORIGINALFACES [27]
        /// </summary>
        public VbspFacesLump? OriginalFacesLump { get; set; }

        /// <summary>
        /// LUMP_PHYSDISP [28]
        /// </summary>
        /// TODO: Find definition and implement
        // public PhysDispLump? PhysDispLump { get; set; }

        /// <summary>
        /// LUMP_PHYSCOLLIDE [29]
        /// </summary>
        public PhysCollideLump? PhysCollideLump { get; set; }

        /// <summary>
        /// LUMP_VERTNORMALS [30]
        /// </summary>
        /// TODO: Find definition and implement
        // public VertNormalsLump? VertNormalsLump { get; set; }

        /// <summary>
        /// LUMP_VERTNORMALINDICES [31]
        /// </summary>
        /// TODO: Find definition and implement
        // public VertNormalIndicesLump? VertNormalIndicesLump { get; set; }

        /// <summary>
        /// LUMP_DISP_LIGHTMAP_ALPHAS [32]
        /// </summary>
        /// TODO: Find definition and implement
        // public DispLightmapAlphasLump? DispLightmapAlphasLump { get; set; }

        /// <summary>
        /// LUMP_DISP_VERTS [33]
        /// </summary>
        public DispVertsLump? DispVertsLump { get; set; }

        /// <summary>
        /// LUMP_DISP_LIGHTMAP_SAMPLE_POSITIONS [34]
        /// </summary>
        /// TODO: Find definition and implement
        // public DispLightmapSamplePositions? DispLightmapSamplePositions { get; set; }

        /// <summary>
        /// LUMP_GAME_LUMP [35]
        /// </summary>
        public GameLump? GameLump { get; set; }

        /// <summary>
        /// LUMP_LEAFWATERDATA [36]
        /// </summary>
        /// TODO: Find definition and implement
        // public LeafWaterDataLump? LeafWaterDataLump { get; set; }

        /// <summary>
        /// LUMP_PRIMITIVES [37]
        /// </summary>
        /// TODO: Find definition and implement
        // public PrimitivesLump? PrimitivesLump { get; set; }

        /// <summary>
        /// LUMP_PRIMVERTS [38]
        /// </summary>
        /// TODO: Find definition and implement
        // public PrimVertsLump? PrimVertsLump { get; set; }

        /// <summary>
        /// LUMP_PRIMINDICES [39]
        /// </summary>
        /// TODO: Find definition and implement
        // public PrimIndicesLump? PrimIndicesLump { get; set; }

        /// <summary>
        /// LUMP_PAKFILE [40]
        /// </summary>
        public PakfileLump? PakfileLump { get; set; }

        /// <summary>
        /// LUMP_CLIPPORTALVERTS [41]
        /// </summary>
        /// TODO: Find definition and implement
        // public ClipPortalVertsLump? ClipPortalVertsLump { get; set; }

        /// <summary>
        /// LUMP_CUBEMAPS [42]
        /// </summary>
        public CubemapsLump? CubemapsLump { get; set; }

        /// <summary>
        /// LUMP_TEXDATA_STRING_DATA [43]
        /// </summary>
        public TexdataStringData? TexdataStringData { get; set; }

        /// <summary>
        /// LUMP_TEXDATA_STRING_TABLE [44]
        /// </summary>
        public TexdataStringTable? TexdataStringTable { get; set; }

        /// <summary>
        /// LUMP_OVERLAYS [45]
        /// </summary>
        public OverlaysLump? OverlaysLump { get; set; }

        /// <summary>
        /// LUMP_LEAFMINDISTTOWATER [46]
        /// </summary>
        /// TODO: Find definition and implement
        // public LeafMindIsToWaterLump? LeafMindIsToWaterLump { get; set; }

        /// <summary>
        /// LUMP_FACE_MACRO_TEXTURE_INFO [47]
        /// </summary>
        /// TODO: Find definition and implement
        // public FaceMacroTextureInfoLump? FaceMacroTextureInfoLump { get; set; }

        /// <summary>
        /// LUMP_DISP_TRIS [48]
        /// </summary>
        public DispTrisLump? DispTrisLump { get; set; }

        /// <summary>
        /// LUMP_PHYSCOLLIDESURFACE / LUMP_PROP_BLOB [49]
        /// </summary>
        /// TODO: Find definition and implement
        // public PhysCollideSurfaceLump? PhysCollideSurfaceLump { get; set; }

        /// <summary>
        /// LUMP_WATEROVERLAYS [50]
        /// </summary>
        /// TODO: Find definition and implement
        // public WaterOverlaysLump? WaterOverlaysLump { get; set; }

        /// <summary>
        /// LUMP_LIGHTMAPPAGES / LUMP_LEAF_AMBIENT_INDEX_HDR [51]
        /// </summary>
        public AmbientIndexLump? HDRAmbientIndexLump { get; set; }

        /// <summary>
        /// LUMP_LIGHTMAPPAGEINFOS / LUMP_LEAF_AMBIENT_INDEX [52]
        /// </summary>
        public AmbientIndexLump? LDRAmbientIndexLump { get; set; }

        /// <summary>
        /// LUMP_LIGHTING_HDR [53]
        /// </summary>
        /// TODO: Find definition and implement
        // public LightingHdrLump? LightingHdrLump { get; set; }

        /// <summary>
        /// LUMP_WORLDLIGHTS_HDR [54]
        /// </summary>
        public WorldLightsLump? HDRWorldLightsLump { get; set; }

        /// <summary>
        /// LUMP_LEAF_AMBIENT_LIGHTING_HDR [55]
        /// </summary>
        public AmbientLightingLump? HDRAmbientLightingLump { get; set; }

        /// <summary>
        /// LUMP_LEAF_AMBIENT_LIGHTING [56]
        /// </summary>
        public AmbientLightingLump? LDRAmbientLightingLump { get; set; }

        /// <summary>
        /// LUMP_XZIPPAKFILE [57]
        /// </summary>
        /// TODO: Find definition and implement
        // public XzipPakFileLump? XzipPakFileLump { get; set; }

        /// <summary>
        /// LUMP_FACES_HDR [58]
        /// </summary>
        /// TODO: Find definition and implement
        // public FacesHdrLump? FacesHdrLump { get; set; }

        /// <summary>
        /// LUMP_MAP_FLAGS [59]
        /// </summary>
        /// TODO: Find definition and implement
        // public MapFlagsLump? MapFlagsLump { get; set; }

        /// <summary>
        /// LUMP_OVERLAY_FADES [60]
        /// </summary>
        /// TODO: Find definition and implement
        // public OverlayFadesLump? OverlayFadesLump { get; set; }

        /// <summary>
        /// LUMP_OVERLAY_SYSTEM_LEVELS [61]
        /// </summary>
        /// TODO: Find definition and implement
        // public OverlaySystemLevelsLump? OverlaySystemLevelsLump { get; set; }

        /// <summary>
        /// LUMP_PHYSLEVEL [62]
        /// </summary>
        /// TODO: Find definition and implement
        // public PhysLevelLump? PhysLevelLump { get; set; }

        /// <summary>
        /// LUMP_DISP_MULTIBLEND [63]
        /// </summary>
        /// TODO: Find definition and implement
        // public DispMultiBlendLump? DispMultiBlendLump { get; set; }

        #endregion
    }
}