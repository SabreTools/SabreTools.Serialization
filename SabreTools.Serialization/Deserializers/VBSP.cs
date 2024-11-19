using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SabreTools.IO.Extensions;
using SabreTools.Models.BSP;
using static SabreTools.Models.BSP.Constants;

namespace SabreTools.Serialization.Deserializers
{
    public class VBSP : BaseBinaryDeserializer<VbspFile>
    {
        /// <inheritdoc/>
        public override VbspFile? Deserialize(Stream? data)
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            // If the offset is out of bounds
            if (data.Position < 0 || data.Position >= data.Length)
                return null;

            // Cache the current offset
            long initialOffset = data.Position;

            // Create a new Half-Life 2 Level to fill
            var file = new VbspFile();

            #region Header

            // Try to parse the header
            var header = ParseHeader(data);
            if (header?.Lumps == null)
                return null;

            // Set the package header
            file.Header = header;

            #endregion

            #region Lumps

            // LUMP_ENTITIES [0]
            var lumpEntry = header.Lumps[(int)LumpType.LUMP_ENTITIES];
            if (lumpEntry != null && lumpEntry.Offset > 0 && lumpEntry.Length > 0)
            {
                // Seek to the lump offset
                data.Seek(lumpEntry.Offset, SeekOrigin.Begin);

                // Read the lump data
                var entities = new List<Entity>();
                while (data.Position < lumpEntry.Offset + lumpEntry.Length)
                {
                    // TODO: Read this into sets of key-value pairs
                    var sb = new StringBuilder();
                    char c = '\0';
                    do
                    {
                        c = (char)data.ReadByteValue();
                        sb.Append(c);
                    } while (c != '}');

                    var entity = new Entity();
                    entity.Attributes = new List<KeyValuePair<string, string>>
                    {
                        new("REPLACE", sb.ToString()),
                    };
                    entities.Add(entity);
                }

                var lump = new EntitiesLump();
                lump.Entities = [.. entities];

                file.Entities = lump;
            }

            // LUMP_PLANES [1]
            lumpEntry = header.Lumps[(int)LumpType.LUMP_PLANES];
            if (lumpEntry != null && lumpEntry.Offset > 0 && lumpEntry.Length > 0)
            {
                // Seek to the lump offset
                data.Seek(lumpEntry.Offset, SeekOrigin.Begin);

                // Read the lump data
                var planes = new List<Plane>();
                while (data.Position < lumpEntry.Offset + lumpEntry.Length)
                {
                    var plane = data.ReadType<Plane>();
                    if (plane != null)
                        planes.Add(plane);
                }

                var lump = new PlanesLump();
                lump.Planes = [.. planes];

                file.PlanesLump = lump;
            }

            // LUMP_TEXDATA [2]
            lumpEntry = header.Lumps[(int)LumpType.LUMP_TEXTURES];
            if (lumpEntry != null && lumpEntry.Offset > 0 && lumpEntry.Length > 0)
            {
                // Seek to the lump offset
                data.Seek(lumpEntry.Offset, SeekOrigin.Begin);

                // Read the lump data
                var texdatas = new List<Texdata>();
                while (data.Position < lumpEntry.Offset + lumpEntry.Length)
                {
                    var texdata = data.ReadType<Texdata>();
                    if (texdata != null)
                        texdatas.Add(texdata);
                }

                var lump = new TexdataLump();
                lump.Texdatas = [.. texdatas];

                file.TexdataLump = lump;
            }

            // LUMP_VERTEXES [3]
            lumpEntry = header.Lumps[(int)LumpType.LUMP_VERTICES];
            if (lumpEntry != null && lumpEntry.Offset > 0 && lumpEntry.Length > 0)
            {
                // Seek to the lump offset
                data.Seek(lumpEntry.Offset, SeekOrigin.Begin);

                // Read the lump data
                var vertices = new List<Vector3D>();
                while (data.Position < lumpEntry.Offset + lumpEntry.Length)
                {
                    vertices.Add(data.ReadType<Vector3D>());
                }

                var lump = new VerticesLump();
                lump.Vertices = [.. vertices];

                file.VerticesLump = lump;
            }

            // LUMP_VISIBILITY [4]
            lumpEntry = header.Lumps[(int)LumpType.LUMP_VISIBILITY];
            if (lumpEntry != null && lumpEntry.Offset > 0 && lumpEntry.Length > 0)
            {
                // Seek to the lump offset
                data.Seek(lumpEntry.Offset, SeekOrigin.Begin);

                // Read the lump data
                var lump = new VisibilityLump();

                lump.NumClusters = data.ReadInt32();
                lump.ByteOffsets = new int[lump.NumClusters, 2];
                for (int i = 0; i < lump.NumClusters; i++)
                for (int j = 0; j < 2; j++)
                {
                    lump.ByteOffsets[i, j] = data.ReadByteValue();
                }

                file.VisibilityLump = lump;
            }

            // LUMP_NODES [5]
            lumpEntry = header.Lumps[(int)LumpType.LUMP_NODES];
            if (lumpEntry != null && lumpEntry.Offset > 0 && lumpEntry.Length > 0)
            {
                // Seek to the lump offset
                data.Seek(lumpEntry.Offset, SeekOrigin.Begin);

                // Read the lump data
                var nodes = new List<VbspNode>();
                while (data.Position < lumpEntry.Offset + lumpEntry.Length)
                {
                    var node = data.ReadType<VbspNode>();
                    if (node != null)
                        nodes.Add(node);
                }

                var lump = new VbspNodesLump();
                lump.Nodes = [.. nodes];

                file.NodesLump = lump;
            }

            // LUMP_TEXINFO [6]
            lumpEntry = header.Lumps[(int)LumpType.LUMP_TEXINFO];
            if (lumpEntry != null && lumpEntry.Offset > 0 && lumpEntry.Length > 0)
            {
                // Seek to the lump offset
                data.Seek(lumpEntry.Offset, SeekOrigin.Begin);

                // Read the lump data
                var texinfos = new List<VbspTexinfo>();
                while (data.Position < lumpEntry.Offset + lumpEntry.Length)
                {
                    var texinfo = data.ReadType<VbspTexinfo>();
                    if (texinfo != null)
                        texinfos.Add(texinfo);
                }

                var lump = new VbspTexinfoLump();
                lump.Texinfos = [.. texinfos];

                file.TexinfoLump = lump;
            }

            // LUMP_FACES [7]
            lumpEntry = header.Lumps[(int)LumpType.LUMP_FACES];
            if (lumpEntry != null && lumpEntry.Offset > 0 && lumpEntry.Length > 0)
            {
                // Seek to the lump offset
                data.Seek(lumpEntry.Offset, SeekOrigin.Begin);

                // Read the lump data
                var faces = new List<VbspFace>();
                while (data.Position < lumpEntry.Offset + lumpEntry.Length)
                {
                    var face = data.ReadType<VbspFace>();
                    if (face != null)
                        faces.Add(face);
                }

                var lump = new VbspFacesLump();
                lump.Faces = [.. faces];

                file.FacesLump = lump;
            }

            // LUMP_LIGHTING [8]
            lumpEntry = header.Lumps[(int)LumpType.LUMP_LIGHTING];
            if (lumpEntry != null && lumpEntry.Offset > 0 && lumpEntry.Length > 0)
            {
                // Seek to the lump offset
                data.Seek(lumpEntry.Offset, SeekOrigin.Begin);

                // Read the lump data
                var lump = new LightmapLump();
                lump.Lightmap = new byte[lumpEntry.Length / 3, 3];

                for (int i = 0; i < lumpEntry.Length / 3; i++)
                for (int j = 0; j < 3; j++)
                {
                    lump.Lightmap[i, j] = data.ReadByteValue();
                }

                file.LightmapLump = lump;
            }

            // LUMP_OCCLUSION [9]
            lumpEntry = header.Lumps[(int)LumpType.LUMP_CLIPNODES];
            if (lumpEntry != null && lumpEntry.Offset > 0 && lumpEntry.Length > 0)
            {
                // Seek to the lump offset
                data.Seek(lumpEntry.Offset, SeekOrigin.Begin);

                // Read the lump data
                var lump = new OcclusionLump();

                lump.Count = data.ReadInt32();
                lump.Data = new OccluderData[lump.Count];
                for (int i = 0; i < lump.Count; i++)
                {
                    var occluderData = data.ReadType<OccluderData>();
                    if (occluderData != null)
                        lump.Data[i] = occluderData;
                }
                lump.PolyDataCount = data.ReadInt32();
                lump.PolyData = new OccluderPolyData[lump.Count];
                for (int i = 0; i < lump.Count; i++)
                {
                    var polyData = data.ReadType<OccluderPolyData>();
                    if (polyData != null)
                        lump.PolyData[i] = polyData;
                }
                lump.VertexIndexCount = data.ReadInt32();
                lump.VertexIndices = new int[lump.VertexIndexCount];
                for (int i = 0; i < lump.VertexIndexCount; i++)
                {
                    lump.VertexIndices[i] = data.ReadInt32();
                }

                file.OcclusionLump = lump;
            }

            // LUMP_LEAVES [10]
            lumpEntry = header.Lumps[(int)LumpType.LUMP_LEAVES];
            if (lumpEntry != null && lumpEntry.Offset > 0 && lumpEntry.Length > 0)
            {
                // Seek to the lump offset
                data.Seek(lumpEntry.Offset, SeekOrigin.Begin);

                // Read the lump data
                var leaves = new List<VbspLeaf>();
                while (data.Position < lumpEntry.Offset + lumpEntry.Length)
                {
                    // TODO: Fix parsing between V0 and V1+
                    var leaf = data.ReadType<VbspLeaf>();
                    if (leaf != null)
                        leaves.Add(leaf);
                }

                var lump = new VbspLeavesLump();
                lump.Leaves = [.. leaves];

                file.LeavesLump = lump;
            }

            // LUMP_FACEIDS [11]
            lumpEntry = header.Lumps[(int)LumpType.LUMP_MARKSURFACES];
            if (lumpEntry != null && lumpEntry.Offset > 0 && lumpEntry.Length > 0)
            {
                // Seek to the lump offset
                data.Seek(lumpEntry.Offset, SeekOrigin.Begin);

                // Read the lump data
                var marksurfaces = new List<ushort>();
                while (data.Position < lumpEntry.Offset + lumpEntry.Length)
                {
                    marksurfaces.Add(data.ReadUInt16());
                }

                var lump = new MarksurfacesLump();
                lump.Marksurfaces = [.. marksurfaces];

                file.MarksurfacesLump = lump;
            }

            // LUMP_EDGES [12]
            lumpEntry = header.Lumps[(int)LumpType.LUMP_EDGES];
            if (lumpEntry != null && lumpEntry.Offset > 0 && lumpEntry.Length > 0)
            {
                // Seek to the lump offset
                data.Seek(lumpEntry.Offset, SeekOrigin.Begin);

                // Read the lump data
                var edges = new List<Edge>();
                while (data.Position < lumpEntry.Offset + lumpEntry.Length)
                {
                    var edge = data.ReadType<Edge>();
                    if (edge != null)
                        edges.Add(edge);
                }

                var lump = new EdgesLump();
                lump.Edges = [.. edges];

                file.EdgesLump = lump;
            }

            // LUMP_SURFEDGES [13]
            lumpEntry = header.Lumps[(int)LumpType.LUMP_SURFEDGES];
            if (lumpEntry != null && lumpEntry.Offset > 0 && lumpEntry.Length > 0)
            {
                // Seek to the lump offset
                data.Seek(lumpEntry.Offset, SeekOrigin.Begin);

                // Read the lump data
                var surfedges = new List<int>();
                while (data.Position < lumpEntry.Offset + lumpEntry.Length)
                {
                    surfedges.Add(data.ReadInt32());
                }

                var lump = new SurfedgesLump();
                lump.Surfedges = [.. surfedges];

                file.SurfedgesLump = lump;
            }

            // LUMP_MODELS [14]
            lumpEntry = header.Lumps[(int)LumpType.LUMP_MODELS];
            if (lumpEntry != null && lumpEntry.Offset > 0 && lumpEntry.Length > 0)
            {
                // Seek to the lump offset
                data.Seek(lumpEntry.Offset, SeekOrigin.Begin);

                // Read the lump data
                var models = new List<VbspModel>();
                while (data.Position < lumpEntry.Offset + lumpEntry.Length)
                {
                    var model = data.ReadType<VbspModel>();
                    if (model != null)
                        models.Add(model);
                }

                var lump = new VbspModelsLump();
                lump.Models = [.. models];

                file.ModelsLump = lump;
            }

            // LUMP_WORLDLIGHTS [15]
            lumpEntry = header.Lumps[(int)LumpType.LUMP_WORLDLIGHTS];
            if (lumpEntry != null && lumpEntry.Offset > 0 && lumpEntry.Length > 0)
            {
                // Seek to the lump offset
                data.Seek(lumpEntry.Offset, SeekOrigin.Begin);

                // Read the lump data
                var worldLights = new List<WorldLight>();
                while (data.Position < lumpEntry.Offset + lumpEntry.Length)
                {
                    var worldLight = data.ReadType<WorldLight>();
                    if (worldLight != null)
                        worldLights.Add(worldLight);
                }

                var lump = new WorldLightsLump();
                lump.WorldLights = [.. worldLights];

                file.LDRWorldLightsLump = lump;
            }

            // LUMP_LEAFFACES [16]
            lumpEntry = header.Lumps[(int)LumpType.LUMP_LEAFFACES];
            if (lumpEntry != null && lumpEntry.Offset > 0 && lumpEntry.Length > 0)
            {
                // Seek to the lump offset
                data.Seek(lumpEntry.Offset, SeekOrigin.Begin);

                // Read the lump data
                var map = new List<ushort>();
                while (data.Position < lumpEntry.Offset + lumpEntry.Length)
                {
                    map.Add(data.ReadUInt16());
                }

                var lump = new LeafFacesLump();
                lump.Map = [.. map];

                file.LeafFacesLump = lump;
            }

            // LUMP_LEAFBRUSHES [17]
            lumpEntry = header.Lumps[(int)LumpType.LUMP_LEAFBRUSHES];
            if (lumpEntry != null && lumpEntry.Offset > 0 && lumpEntry.Length > 0)
            {
                // Seek to the lump offset
                data.Seek(lumpEntry.Offset, SeekOrigin.Begin);

                // Read the lump data
                var map = new List<ushort>();
                while (data.Position < lumpEntry.Offset + lumpEntry.Length)
                {
                    map.Add(data.ReadUInt16());
                }

                var lump = new LeafBrushesLump();
                lump.Map = [.. map];

                file.LeafBrushesLump = lump;
            }

            // LUMP_BRUSHES [18]
            lumpEntry = header.Lumps[(int)LumpType.LUMP_BRUSHES];
            if (lumpEntry != null && lumpEntry.Offset > 0 && lumpEntry.Length > 0)
            {
                // Seek to the lump offset
                data.Seek(lumpEntry.Offset, SeekOrigin.Begin);

                // Read the lump data
                var brushes = new List<Brush>();
                while (data.Position < lumpEntry.Offset + lumpEntry.Length)
                {
                    var brush = data.ReadType<Brush>();
                    if (brush != null)
                        brushes.Add(brush);
                }

                var lump = new BrushesLump();
                lump.Brushes = [.. brushes];

                file.BrushesLump = lump;
            }

            // LUMP_BRUSHSIDES [19]
            lumpEntry = header.Lumps[(int)LumpType.LUMP_BRUSHSIDES];
            if (lumpEntry != null && lumpEntry.Offset > 0 && lumpEntry.Length > 0)
            {
                // Seek to the lump offset
                data.Seek(lumpEntry.Offset, SeekOrigin.Begin);

                // Read the lump data
                var brushsides = new List<Brushside>();
                while (data.Position < lumpEntry.Offset + lumpEntry.Length)
                {
                    var brushside = data.ReadType<Brushside>();
                    if (brushside != null)
                        brushsides.Add(brushside);
                }

                var lump = new BrushsidesLump();
                lump.Brushsides = [.. brushsides];

                file.BrushsidesLump = lump;
            }

            // TODO: Support LUMP_AREAS [20] when in Models
            // TODO: Support LUMP_AREAPORTALS [21] when in Models
            // TODO: Support LUMP_PORTALS / LUMP_UNUSED0 / LUMP_PROPCOLLISION [22] when in Models
            // TODO: Support LUMP_CLUSTERS / LUMP_UNUSED1 / LUMP_PROPHULLS [23] when in Models
            // TODO: Support LUMP_PORTALVERTS / LUMP_UNUSED2 / LUMP_FAKEENTITIES / LUMP_PROPHULLVERTS [24] when in Models
            // TODO: Support LUMP_CLUSTERPORTALS / LUMP_UNUSED3 / LUMP_PROPTRIS [25] when in Models

            // LUMP_DISPINFO [26]
            lumpEntry = header.Lumps[(int)LumpType.LUMP_DISPINFO];
            if (lumpEntry != null && lumpEntry.Offset > 0 && lumpEntry.Length > 0)
            {
                // Seek to the lump offset
                data.Seek(lumpEntry.Offset, SeekOrigin.Begin);

                // Read the lump data
                var dispInfos = new List<DispInfo>();
                while (data.Position < lumpEntry.Offset + lumpEntry.Length)
                {
                    var dispInfo = data.ReadType<DispInfo>();
                    if (dispInfo != null)
                        dispInfos.Add(dispInfo);
                }

                var lump = new DispInfosLump();
                lump.Infos = [.. dispInfos];

                file.DispInfoLump = lump;
            }

            // LUMP_ORIGINALFACES [27]
            lumpEntry = header.Lumps[(int)LumpType.LUMP_ORIGINALFACES];
            if (lumpEntry != null && lumpEntry.Offset > 0 && lumpEntry.Length > 0)
            {
                // Seek to the lump offset
                data.Seek(lumpEntry.Offset, SeekOrigin.Begin);

                // Read the lump data
                var faces = new List<VbspFace>();
                while (data.Position < lumpEntry.Offset + lumpEntry.Length)
                {
                    var face = data.ReadType<VbspFace>();
                    if (face != null)
                        faces.Add(face);
                }

                var lump = new VbspFacesLump();
                lump.Faces = [.. faces];

                file.OriginalFacesLump = lump;
            }

            // TODO: Support LUMP_PHYSDISP [28] when in Models
            // TODO: Support LUMP_PHYSCOLLIDE [29] when in Models
            // TODO: Support LUMP_VERTNORMALS [30] when in Models
            // TODO: Support LUMP_VERTNORMALINDICES [31] when in Models
            // TODO: Support LUMP_DISP_LIGHTMAP_ALPHAS [32] when in Models

            // LUMP_DISP_VERTS [33]
            lumpEntry = header.Lumps[(int)LumpType.LUMP_DISP_VERTS];
            if (lumpEntry != null && lumpEntry.Offset > 0 && lumpEntry.Length > 0)
            {
                // Seek to the lump offset
                data.Seek(lumpEntry.Offset, SeekOrigin.Begin);

                // Read the lump data
                var verts = new List<DispVert>();
                while (data.Position < lumpEntry.Offset + lumpEntry.Length)
                {
                    var vert = data.ReadType<DispVert>();
                    if (vert != null)
                        verts.Add(vert);
                }

                var lump = new DispVertsLump();
                lump.Verts = [.. verts];

                file.DispVertLump = lump;
            }

            // TODO: Support LUMP_DISP_LIGHTMAP_SAMPLE_POSITIONS [34] when in Models
            
            // LUMP_GAME_LUMP [35]
            lumpEntry = header.Lumps[(int)LumpType.LUMP_GAME_LUMP];
            if (lumpEntry != null && lumpEntry.Offset > 0 && lumpEntry.Length > 0)
            {
                // Seek to the lump offset
                data.Seek(lumpEntry.Offset, SeekOrigin.Begin);

                // Read the lump data
                var lump = new GameLump();

                lump.LumpCount = data.ReadInt32();
                lump.Directories = new GameLumpDirectory[lump.LumpCount];
                for (int i = 0; i < lump.LumpCount; i++)
                {
                    var dir = data.ReadType<GameLumpDirectory>();
                    if (dir != null)
                        lump.Directories[i] = dir;
                }

                file.GameLump = lump;
            }

            // TODO: Support LUMP_LEAFWATERDATA [36] when in Models
            // TODO: Support LUMP_PRIMITIVES [37] when in Models
            // TODO: Support LUMP_PRIMVERTS [38] when in Models
            // TODO: Support LUMP_PRIMINDICES [39] when in Models
            // TODO: Support LUMP_PAKFILE [40] when in Models
            // TODO: Support LUMP_CLIPPORTALVERTS [41] when in Models
            
            // LUMP_CUBEMAPS [42]
            lumpEntry = header.Lumps[(int)LumpType.LUMP_CUBEMAPS];
            if (lumpEntry != null && lumpEntry.Offset > 0 && lumpEntry.Length > 0)
            {
                // Seek to the lump offset
                data.Seek(lumpEntry.Offset, SeekOrigin.Begin);

                // Read the lump data
                var cubemaps = new List<Cubemap>();
                while (data.Position < lumpEntry.Offset + lumpEntry.Length)
                {
                    var cubemap = data.ReadType<Cubemap>();
                    if (cubemap != null)
                        cubemaps.Add(cubemap);
                }

                var lump = new CubemapsLump();
                lump.Cubemaps = [.. cubemaps];

                file.CubemapLump = lump;
            }

            // LUMP_TEXDATA_STRING_DATA [43]
            lumpEntry = header.Lumps[(int)LumpType.LUMP_TEXDATA_STRING_DATA];
            if (lumpEntry != null && lumpEntry.Offset > 0 && lumpEntry.Length > 0)
            {
                // Seek to the lump offset
                data.Seek(lumpEntry.Offset, SeekOrigin.Begin);

                // Read the lump data
                var strings = new List<string>();
                while (data.Position < lumpEntry.Offset + lumpEntry.Length)
                {
                    var str = data.ReadNullTerminatedAnsiString();
                    if (str != null)
                        strings.Add(str);
                }

                var lump = new TexdataStringData();
                lump.Strings = [.. strings];

                file.TexdataStringData = lump;
            }

            // LUMP_TEXDATA_STRING_TABLE [44]
            lumpEntry = header.Lumps[(int)LumpType.LUMP_TEXDATA_STRING_TABLE];
            if (lumpEntry != null && lumpEntry.Offset > 0 && lumpEntry.Length > 0)
            {
                // Seek to the lump offset
                data.Seek(lumpEntry.Offset, SeekOrigin.Begin);

                // Read the lump data
                var offsets = new List<int>();
                while (data.Position < lumpEntry.Offset + lumpEntry.Length)
                {
                    offsets.Add(data.ReadInt32());
                }

                var lump = new TexdataStringTable();
                lump.Offsets = [.. offsets];

                file.TexdataStringTable = lump;
            }

            // LUMP_OVERLAYS [45]
            lumpEntry = header.Lumps[(int)LumpType.LUMP_OVERLAYS];
            if (lumpEntry != null && lumpEntry.Offset > 0 && lumpEntry.Length > 0)
            {
                // Seek to the lump offset
                data.Seek(lumpEntry.Offset, SeekOrigin.Begin);

                // Read the lump data
                var overlays = new List<Overlay>();
                while (data.Position < lumpEntry.Offset + lumpEntry.Length)
                {
                    var overlay = data.ReadType<Overlay>();
                    if (overlay != null)
                        overlays.Add(overlay);
                }

                var lump = new OverlaysLump();
                lump.Overlays = [.. overlays];

                file.OverlaysLump = lump;
            }

            // TODO: Support LUMP_LEAFMINDISTTOWATER [46] when in Models
            // TODO: Support LUMP_FACE_MACRO_TEXTURE_INFO [47] when in Models

            // LUMP_DISP_TRIS [48]
            lumpEntry = header.Lumps[(int)LumpType.LUMP_DISP_TRIS];
            if (lumpEntry != null && lumpEntry.Offset > 0 && lumpEntry.Length > 0)
            {
                // Seek to the lump offset
                data.Seek(lumpEntry.Offset, SeekOrigin.Begin);

                // Read the lump data
                var tris = new List<DispTri>();
                while (data.Position < lumpEntry.Offset + lumpEntry.Length)
                {
                    var tri = data.ReadType<DispTri>();
                    if (tri != null)
                        tris.Add(tri);
                }

                var lump = new DispTrisLump();
                lump.Tris = [.. tris];

                file.DispTrisLump = lump;
            }

            // TODO: Support LUMP_PHYSCOLLIDESURFACE / LUMP_PROP_BLOB [49] when in Models
            // TODO: Support LUMP_WATEROVERLAYS [50] when in Models

            // LUMP_LIGHTMAPPAGES / LUMP_LEAF_AMBIENT_INDEX_HDR [51]
            lumpEntry = header.Lumps[(int)LumpType.LUMP_LIGHTMAPPAGES];
            if (lumpEntry != null && lumpEntry.Offset > 0 && lumpEntry.Length > 0)
            {
                // Seek to the lump offset
                data.Seek(lumpEntry.Offset, SeekOrigin.Begin);

                // Read the lump data
                var indicies = new List<LeafAmbientIndex>();
                while (data.Position < lumpEntry.Offset + lumpEntry.Length)
                {
                    var index = data.ReadType<LeafAmbientIndex>();
                    if (index != null)
                        indicies.Add(index);
                }

                var lump = new AmbientIndexLump();
                lump.Indicies = [.. indicies];

                file.HDRAmbientIndexLump = lump;
            }

            // LUMP_LIGHTMAPPAGEINFOS / LUMP_LEAF_AMBIENT_INDEX [52]
            lumpEntry = header.Lumps[(int)LumpType.LUMP_LIGHTMAPPAGEINFOS];
            if (lumpEntry != null && lumpEntry.Offset > 0 && lumpEntry.Length > 0)
            {
                // Seek to the lump offset
                data.Seek(lumpEntry.Offset, SeekOrigin.Begin);

                // Read the lump data
                var indicies = new List<LeafAmbientIndex>();
                while (data.Position < lumpEntry.Offset + lumpEntry.Length)
                {
                    var index = data.ReadType<LeafAmbientIndex>();
                    if (index != null)
                        indicies.Add(index);
                }

                var lump = new AmbientIndexLump();
                lump.Indicies = [.. indicies];

                file.LDRAmbientIndexLump = lump;
            }

            // TODO: Support LUMP_LIGHTING_HDR [53] when in Models

            // LUMP_WORLDLIGHTS_HDR [54]
            lumpEntry = header.Lumps[(int)LumpType.LUMP_WORLDLIGHTS_HDR];
            if (lumpEntry != null && lumpEntry.Offset > 0 && lumpEntry.Length > 0)
            {
                // Seek to the lump offset
                data.Seek(lumpEntry.Offset, SeekOrigin.Begin);

                // Read the lump data
                var worldLights = new List<WorldLight>();
                while (data.Position < lumpEntry.Offset + lumpEntry.Length)
                {
                    var worldLight = data.ReadType<WorldLight>();
                    if (worldLight != null)
                        worldLights.Add(worldLight);
                }

                var lump = new WorldLightsLump();
                lump.WorldLights = [.. worldLights];

                file.WorldLightsLump = lump;
            }

            // LUMP_LEAF_AMBIENT_LIGHTING_HDR [55]
            lumpEntry = header.Lumps[(int)LumpType.LUMP_LEAF_AMBIENT_LIGHTING_HDR];
            if (lumpEntry != null && lumpEntry.Offset > 0 && lumpEntry.Length > 0)
            {
                // Seek to the lump offset
                data.Seek(lumpEntry.Offset, SeekOrigin.Begin);

                // Read the lump data
                var lightings = new List<LeafAmbientLighting>();
                while (data.Position < lumpEntry.Offset + lumpEntry.Length)
                {
                    var lighting = data.ReadType<LeafAmbientLighting>();
                    if (lighting != null)
                        lightings.Add(lighting);
                }

                var lump = new AmbientLightingLump();
                lump.Lightings = [.. lightings];

                file.HDRAmbientLightingLump = lump;
            }

            // LUMP_LEAF_AMBIENT_LIGHTING [56]
            lumpEntry = header.Lumps[(int)LumpType.LUMP_LEAF_AMBIENT_LIGHTING];
            if (lumpEntry != null && lumpEntry.Offset > 0 && lumpEntry.Length > 0)
            {
                // Seek to the lump offset
                data.Seek(lumpEntry.Offset, SeekOrigin.Begin);

                // Read the lump data
                var lightings = new List<LeafAmbientLighting>();
                while (data.Position < lumpEntry.Offset + lumpEntry.Length)
                {
                    var lighting = data.ReadType<LeafAmbientLighting>();
                    if (lighting != null)
                        lightings.Add(lighting);
                }

                var lump = new AmbientLightingLump();
                lump.Lightings = [.. lightings];

                file.LDRAmbientLightingLump = lump;
            }

            // TODO: Support LUMP_XZIPPAKFILE [57] when in Models
            // TODO: Support LUMP_FACES_HDR [58] when in Models
            // TODO: Support LUMP_MAP_FLAGS [59] when in Models
            // TODO: Support LUMP_OVERLAY_FADES [60] when in Models
            // TODO: Support LUMP_OVERLAY_SYSTEM_LEVELS [61] when in Models
            // TODO: Support LUMP_PHYSLEVEL [62] when in Models
            // TODO: Support LUMP_DISP_MULTIBLEND [63] when in Models

            #endregion

            return file;
        }

        /// <summary>
        /// Parse a Stream into a Half-Life 2 Level header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Half-Life 2 Level header on success, null on error</returns>
        private static VbspHeader? ParseHeader(Stream data)
        {
            var header = data.ReadType<VbspHeader>();

            if (header?.Signature != SignatureString)
                return null;
            if (Array.IndexOf([17, 18, 19, 20, 21, 22, 23, 25, 27, 29, 0x00040014], header.Version) > -1)
                return null;

            return header;
        }
    }
}