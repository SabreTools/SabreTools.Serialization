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

            // Create a new Half-Life 2 Level to fill
            var file = new VbspFile();

            #region Header

            // Try to parse the header
            var header = data.ReadType<VbspHeader>();
            if (header?.Signature != SignatureString)
                return null;
            if (Array.IndexOf([17, 18, 19, 20, 21, 22, 23, 25, 27, 29, 0x00040014], header.Version) > -1)
                return null;
            if (header.Lumps == null || header.Lumps.Length != VBSP_HEADER_LUMPS)
                return null;

            // Set the package header
            file.Header = header;

            #endregion

            #region Lumps

            for (int l = 0; l < VBSP_HEADER_LUMPS; l++)
            {
                // Get the next lump entry
                var lumpEntry = header.Lumps[l];
                if (lumpEntry == null)
                    continue;
                if (lumpEntry.Offset == 0 || lumpEntry.Length == 0)
                    continue;

                // Seek to the lump offset
                data.Seek(lumpEntry.Offset, SeekOrigin.Begin);

                // Read according to the lump type
                switch ((LumpType)l)
                {
                    case LumpType.LUMP_ENTITIES:
                        file.Entities = ParseEntitiesLump(data, lumpEntry.Offset, lumpEntry.Length);
                        break;
                    case LumpType.LUMP_PLANES:
                        file.PlanesLump = ParsePlanesLump(data, lumpEntry.Offset, lumpEntry.Length);
                        break;
                    case LumpType.LUMP_TEXTURES:
                        file.TexdataLump = ParseTexdataLump(data, lumpEntry.Offset, lumpEntry.Length);
                        break;
                    case LumpType.LUMP_VERTICES:
                        file.VerticesLump = ParseVerticesLump(data, lumpEntry.Offset, lumpEntry.Length);
                        break;
                    case LumpType.LUMP_VISIBILITY:
                        file.VisibilityLump = ParseVisibilityLump(data, lumpEntry.Offset, lumpEntry.Length);
                        break;
                    case LumpType.LUMP_NODES:
                        file.NodesLump = ParseNodesLump(data, lumpEntry.Offset, lumpEntry.Length);
                        break;
                    case LumpType.LUMP_TEXINFO:
                        file.TexinfoLump = ParseTexinfoLump(data, lumpEntry.Offset, lumpEntry.Length);
                        break;
                    case LumpType.LUMP_FACES:
                        file.FacesLump = ParseFacesLump(data, lumpEntry.Offset, lumpEntry.Length);
                        break;
                    case LumpType.LUMP_LIGHTING:
                        file.LightmapLump = ParseLightmapLump(data, lumpEntry.Offset, lumpEntry.Length);
                        break;
                    case LumpType.LUMP_CLIPNODES:
                        file.OcclusionLump = ParseOcclusionLump(data, lumpEntry.Offset, lumpEntry.Length);
                        break;
                    case LumpType.LUMP_LEAVES:
                        file.LeavesLump = ParseLeavesLump(data, lumpEntry.Version, lumpEntry.Offset, lumpEntry.Length);
                        break;
                    case LumpType.LUMP_MARKSURFACES:
                        file.MarksurfacesLump = ParseMarksurfacesLump(data, lumpEntry.Offset, lumpEntry.Length);
                        break;
                    case LumpType.LUMP_EDGES:
                        file.EdgesLump = ParseEdgesLump(data, lumpEntry.Offset, lumpEntry.Length);
                        break;
                    case LumpType.LUMP_SURFEDGES:
                        file.SurfedgesLump = ParseSurfedgesLump(data, lumpEntry.Offset, lumpEntry.Length);
                        break;
                    case LumpType.LUMP_MODELS:
                        file.ModelsLump = ParseModelsLump(data, lumpEntry.Offset, lumpEntry.Length);
                        break;
                    case LumpType.LUMP_WORLDLIGHTS:
                        file.LDRWorldLightsLump = ParseWorldLightsLump(data, lumpEntry.Offset, lumpEntry.Length);
                        break;
                    case LumpType.LUMP_LEAFFACES:
                        file.LeafFacesLump = ParseLeafFacesLump(data, lumpEntry.Offset, lumpEntry.Length);
                        break;
                    case LumpType.LUMP_LEAFBRUSHES:
                        file.LeafBrushesLump = ParseLeafBrushesLump(data, lumpEntry.Offset, lumpEntry.Length);
                        break;
                    case LumpType.LUMP_BRUSHES:
                        file.BrushesLump = ParseBrushesLump(data, lumpEntry.Offset, lumpEntry.Length);
                        break;
                    case LumpType.LUMP_BRUSHSIDES:
                        file.BrushsidesLump = ParseBrushsidesLump(data, lumpEntry.Offset, lumpEntry.Length);
                        break;
                    case LumpType.LUMP_AREAS:
                        // TODO: Support LUMP_AREAS [20] when in Models
                        break;
                    case LumpType.LUMP_AREAPORTALS:
                        // TODO: Support LUMP_AREAPORTALS [21] when in Models
                        break;
                    case LumpType.LUMP_PORTALS:
                        // TODO: Support LUMP_PORTALS / LUMP_UNUSED0 / LUMP_PROPCOLLISION [22] when in Models
                        break;
                    case LumpType.LUMP_CLUSTERS:
                        // TODO: Support LUMP_CLUSTERS / LUMP_UNUSED1 / LUMP_PROPHULLS [23] when in Models
                        break;
                    case LumpType.LUMP_PORTALVERTS:
                        // TODO: Support LUMP_PORTALVERTS / LUMP_UNUSED2 / LUMP_FAKEENTITIES / LUMP_PROPHULLVERTS [24] when in Models
                        break;
                    case LumpType.LUMP_CLUSTERPORTALS:
                        // TODO: Support LUMP_CLUSTERPORTALS / LUMP_UNUSED3 / LUMP_PROPTRIS [25] when in Models
                        break;
                    case LumpType.LUMP_DISPINFO:
                        file.DispInfosLump = ParseDispInfosLump(data, lumpEntry.Offset, lumpEntry.Length);
                        break;
                    case LumpType.LUMP_ORIGINALFACES:
                        file.OriginalFacesLump = ParseFacesLump(data, lumpEntry.Offset, lumpEntry.Length);
                        break;
                    case LumpType.LUMP_PHYSDISP:
                        // TODO: Support LUMP_PHYSDISP [28] when in Models
                        break;
                    case LumpType.LUMP_PHYSCOLLIDE:
                        file.PhysCollideLump = ParsePhysCollideLump(data, lumpEntry.Offset, lumpEntry.Length);
                        break;
                    case LumpType.LUMP_VERTNORMALS:
                        // TODO: Support LUMP_VERTNORMALS [30] when in Models
                        break;
                    case LumpType.LUMP_VERTNORMALINDICES:
                        // TODO: Support LUMP_VERTNORMALINDICES [31] when in Models
                        break;
                    case LumpType.LUMP_DISP_LIGHTMAP_ALPHAS:
                        // TODO: Support LUMP_DISP_LIGHTMAP_ALPHAS [32] when in Models
                        break;
                    case LumpType.LUMP_DISP_VERTS:
                        file.DispVertsLump = ParseDispVertsLump(data, lumpEntry.Offset, lumpEntry.Length);
                        break;
                    case LumpType.LUMP_DISP_LIGHTMAP_SAMPLE_POSITIONS:
                        // TODO: Support LUMP_DISP_LIGHTMAP_SAMPLE_POSITIONS [34] when in Models
                        break;
                    case LumpType.LUMP_GAME_LUMP:
                        file.GameLump = ParseGameLump(data, lumpEntry.Offset, lumpEntry.Length);
                        break;
                    case LumpType.LUMP_LEAFWATERDATA:
                        // TODO: Support LUMP_LEAFWATERDATA [36] when in Models
                        break;
                    case LumpType.LUMP_PRIMITIVES:
                        // TODO: Support LUMP_PRIMITIVES [37] when in Models
                        break;
                    case LumpType.LUMP_PRIMVERTS:
                        // TODO: Support LUMP_PRIMVERTS [38] when in Models
                        break;
                    case LumpType.LUMP_PRIMINDICES:
                        // TODO: Support LUMP_PRIMINDICES [39] when in Models
                        break;
                    case LumpType.LUMP_PAKFILE:
                        file.PakfileLump = ParsePakfileLump(data, lumpEntry.Offset, lumpEntry.Length);
                        break;
                    case LumpType.LUMP_CLIPPORTALVERTS:
                        // TODO: Support LUMP_CLIPPORTALVERTS [41] when in Models
                        break;
                    case LumpType.LUMP_CUBEMAPS:
                        file.CubemapsLump = ParseCubemapsLump(data, lumpEntry.Offset, lumpEntry.Length);
                        break;
                    case LumpType.LUMP_TEXDATA_STRING_DATA:
                        file.TexdataStringData = ParseTexdataStringData(data, lumpEntry.Offset, lumpEntry.Length);
                        break;
                    case LumpType.LUMP_TEXDATA_STRING_TABLE:
                        file.TexdataStringTable = ParseTexdataStringTable(data, lumpEntry.Offset, lumpEntry.Length);
                        break;
                    case LumpType.LUMP_OVERLAYS:
                        file.OverlaysLump = ParseOverlaysLump(data, lumpEntry.Offset, lumpEntry.Length);
                        break;
                    case LumpType.LUMP_LEAFMINDISTTOWATER:
                        // TODO: Support LUMP_LEAFMINDISTTOWATER [46] when in Models
                        break;
                    case LumpType.LUMP_FACE_MACRO_TEXTURE_INFO:
                        // TODO: Support LUMP_FACE_MACRO_TEXTURE_INFO [47] when in Models
                        break;
                    case LumpType.LUMP_DISP_TRIS:
                        file.DispTrisLump = ParseDispTrisLump(data, lumpEntry.Offset, lumpEntry.Length);
                        break;
                    case LumpType.LUMP_PHYSCOLLIDESURFACE:
                        // TODO: Support LUMP_PHYSCOLLIDESURFACE / LUMP_PROP_BLOB [49] when in Models
                        break;
                    case LumpType.LUMP_WATEROVERLAYS:
                        // TODO: Support LUMP_WATEROVERLAYS [50] when in Models
                        break;
                    case LumpType.LUMP_LIGHTMAPPAGES:
                        file.HDRAmbientIndexLump = ParseAmbientIndexLump(data, lumpEntry.Offset, lumpEntry.Length);
                        break;
                    case LumpType.LUMP_LIGHTMAPPAGEINFOS:
                        file.LDRAmbientIndexLump = ParseAmbientIndexLump(data, lumpEntry.Offset, lumpEntry.Length);
                        break;
                    case LumpType.LUMP_LIGHTING_HDR:
                        // TODO: Support LUMP_LIGHTING_HDR [53] when in Models
                        break;
                    case LumpType.LUMP_WORLDLIGHTS_HDR:
                        file.HDRWorldLightsLump = ParseWorldLightsLump(data, lumpEntry.Offset, lumpEntry.Length);
                        break;
                    case LumpType.LUMP_LEAF_AMBIENT_LIGHTING_HDR:
                        file.HDRAmbientLightingLump = ParseAmbientLightingLump(data, lumpEntry.Offset, lumpEntry.Length);
                        break;
                    case LumpType.LUMP_LEAF_AMBIENT_LIGHTING:
                        file.LDRAmbientLightingLump = ParseAmbientLightingLump(data, lumpEntry.Offset, lumpEntry.Length);
                        break;
                    case LumpType.LUMP_XZIPPAKFILE:
                        // TODO: Support LUMP_XZIPPAKFILE [57] when in Models
                        break;
                    case LumpType.LUMP_FACES_HDR:
                        // TODO: Support LUMP_FACES_HDR [58] when in Models
                        break;
                    case LumpType.LUMP_MAP_FLAGS:
                        // TODO: Support LUMP_MAP_FLAGS [59] when in Models
                        break;
                    case LumpType.LUMP_OVERLAY_FADES:
                        // TODO: Support LUMP_OVERLAY_FADES [60] when in Models
                        break;
                    case LumpType.LUMP_OVERLAY_SYSTEM_LEVELS:
                        // TODO: Support LUMP_OVERLAY_SYSTEM_LEVELS [61] when in Models
                        break;
                    case LumpType.LUMP_PHYSLEVEL:
                        // TODO: Support LUMP_PHYSLEVEL [62] when in Models
                        break;
                    case LumpType.LUMP_DISP_MULTIBLEND:
                        // TODO: Support LUMP_DISP_MULTIBLEND [63] when in Models
                        break;

                    default:
                        // Unsupported LumpType value, ignore
                        break;
                }
            }

            #endregion

            return file;
        }

        /// <summary>
        /// Parse a Stream into LUMP_ENTITIES
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled LUMP_ENTITIES on success, null on error</returns>
        private static EntitiesLump? ParseEntitiesLump(Stream data, int offset, int length)
        {
            var entities = new List<Entity>();

            // Read the entire lump as text
            byte[] lumpData = data.ReadBytes(length);
            string lumpText = Encoding.ASCII.GetString(lumpData);

            // Break the text by ending curly braces
            string[] lumpSections = lumpText.Split('}');
            Array.ForEach(lumpSections, s => s.Trim('{', '}'));

            // Loop through all sections
            for (int i = 0; i < lumpSections.Length; i++)
            {
                // Prepare an attributes list
                var attributes = new List<KeyValuePair<string, string>>();

                // Split the section by newlines
                string section = lumpSections[i];
                string[] lines = section.Split('\n');
                Array.ForEach(lines, l => l.Trim());

                // Convert each line into a key-value pair and add
                for (int j = 0; j < lines.Length; j++)
                {
                    // TODO: Split lines and add
                }

                // Create a new entity and add
                var entity = new Entity { Attributes = attributes };
                entities.Add(entity);
            }

            return new EntitiesLump { Entities = [.. entities] };
        }

        /// <summary>
        /// Parse a Stream into LUMP_PLANES
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled LUMP_PLANES on success, null on error</returns>
        private static PlanesLump? ParsePlanesLump(Stream data, int offset, int length)
        {
            var planes = new List<Plane>();
            while (data.Position < offset + length)
            {
                var plane = data.ReadType<Plane>();
                if (plane != null)
                    planes.Add(plane);
            }

            return new PlanesLump { Planes = [.. planes] };
        }

        /// <summary>
        /// Parse a Stream into LUMP_TEXDATA
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled LUMP_TEXDATA on success, null on error</returns>
        private static TexdataLump? ParseTexdataLump(Stream data, int offset, int length)
        {
            var texdatas = new List<Texdata>();
            while (data.Position < offset + length)
            {
                var texdata = data.ReadType<Texdata>();
                if (texdata != null)
                    texdatas.Add(texdata);
            }

            return new TexdataLump { Texdatas = [.. texdatas] };
        }

        /// <summary>
        /// Parse a Stream into LUMP_VERTEXES
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled LUMP_VERTEXES on success, null on error</returns>
        private static VerticesLump? ParseVerticesLump(Stream data, int offset, int length)
        {
            var vertices = new List<Vector3D>();
            while (data.Position < offset + length)
            {
                var vertex = data.ReadType<Vector3D>();
                if (vertex == null)
                    break;

                vertices.Add(vertex);
            }

            return new VerticesLump { Vertices = [.. vertices] };
        }

        /// <summary>
        /// Parse a Stream into LUMP_VISIBILITY
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled LUMP_VISIBILITY on success, null on error</returns>
        private static VisibilityLump? ParseVisibilityLump(Stream data, int offset, int length)
        {
            var lump = new VisibilityLump();

            lump.NumClusters = data.ReadInt32();
            lump.ByteOffsets = new int[lump.NumClusters][];
            for (int i = 0; i < lump.NumClusters; i++)
            {
                lump.ByteOffsets[i] = new int[2];
                for (int j = 0; j < 2; j++)
                {
                    lump.ByteOffsets[i][j] = data.ReadInt32();
                }
            }

            return lump;
        }

        /// <summary>
        /// Parse a Stream into LUMP_NODES
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled LUMP_NODES on success, null on error</returns>
        private static VbspNodesLump? ParseNodesLump(Stream data, int offset, int length)
        {
            var nodes = new List<VbspNode>();
            while (data.Position < offset + length)
            {
                var node = data.ReadType<VbspNode>();
                if (node != null)
                    nodes.Add(node);
            }

            return new VbspNodesLump { Nodes = [.. nodes] };
        }

        /// <summary>
        /// Parse a Stream into LUMP_TEXINFO
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled LUMP_TEXINFO on success, null on error</returns>
        private static VbspTexinfoLump? ParseTexinfoLump(Stream data, int offset, int length)
        {
            var texinfos = new List<VbspTexinfo>();
            while (data.Position < offset + length)
            {
                var texinfo = data.ReadType<VbspTexinfo>();
                if (texinfo != null)
                    texinfos.Add(texinfo);
            }

            return new VbspTexinfoLump { Texinfos = [.. texinfos] };
        }

        /// <summary>
        /// Parse a Stream into LUMP_FACES / LUMP_ORIGINALFACES
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled LUMP_FACES / LUMP_ORIGINALFACES on success, null on error</returns>
        private static VbspFacesLump? ParseFacesLump(Stream data, int offset, int length)
        {
            var faces = new List<VbspFace>();
            while (data.Position < offset + length)
            {
                var face = data.ReadType<VbspFace>();
                if (face != null)
                    faces.Add(face);
            }

            return new VbspFacesLump { Faces = [.. faces] };
        }

        /// <summary>
        /// Parse a Stream into LUMP_PHYSCOLLIDE
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled LUMP_PHYSCOLLIDE on success, null on error</returns>
        private static PhysCollideLump? ParsePhysCollideLump(Stream data, int offset, int length)
        {
            var models = new List<PhysModel>();
            while (data.Position < offset + length)
            {
                var model = ParsePhysModel(data);
                if (model != null)
                    models.Add(model);
            }

            return new PhysCollideLump { Models = [.. models] };
        }

        /// <summary>
        /// Parse a Stream into PhysModel
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled PhysModel on success, null on error</returns>
        private static PhysModel? ParsePhysModel(Stream data)
        {
            var model = new PhysModel();

            model.ModelIndex = data.ReadInt32();
            model.DataSize = data.ReadInt32();
            model.KeydataSize = data.ReadInt32();
            model.SolidCount = data.ReadInt32();
            model.Solids = new PhysSolid[model.SolidCount];
            for (int i = 0; i < model.Solids.Length; i++)
            {
                var solid = ParsePhysSolid(data);
                if (solid != null)
                    model.Solids[i] = solid;
            }

            return model;
        }

        /// <summary>
        /// Parse a Stream into PhysSolid
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled PhysSolid on success, null on error</returns>
        private static PhysSolid? ParsePhysSolid(Stream data)
        {
            var solid = new PhysSolid();

            solid.Size = data.ReadInt32();
            if (solid.Size > 0)
                solid.CollisionData = data.ReadBytes(solid.Size);

            return solid;
        }

        /// <summary>
        /// Parse a Stream into LUMP_LIGHTING
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled LUMP_LIGHTING on success, null on error</returns>
        private static LightmapLump? ParseLightmapLump(Stream data, int offset, int length)
        {
            var lump = new LightmapLump();
            lump.Lightmap = new byte[length / 3][];

            for (int i = 0; i < length / 3; i++)
            {
                lump.Lightmap[i] = data.ReadBytes(3);
            }

            return lump;
        }

        /// <summary>
        /// Parse a Stream into LUMP_OCCLUSION
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled LUMP_OCCLUSION on success, null on error</returns>
        private static OcclusionLump? ParseOcclusionLump(Stream data, int offset, int length)
        {
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
            lump.VertexIndicies = new int[lump.VertexIndexCount];
            for (int i = 0; i < lump.VertexIndexCount; i++)
            {
                lump.VertexIndicies[i] = data.ReadInt32();
            }

            return lump;
        }

        /// <summary>
        /// Parse a Stream into LUMP_LEAVES
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled LUMP_LEAVES on success, null on error</returns>
        private static VbspLeavesLump? ParseLeavesLump(Stream data, uint version, int offset, int length)
        {
            var leaves = new List<VbspLeaf>();
            while (data.Position < offset + length)
            {
                var leaf = ParseVbspLeaf(data, version);
                if (leaf != null)
                    leaves.Add(leaf);
            }

            return new VbspLeavesLump { Leaves = [.. leaves] };
        }

        /// <summary>
        /// Parse a Stream into VbspLeaf
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled VbspLeaf on success, null on error</returns>
        private static VbspLeaf? ParseVbspLeaf(Stream data, uint version)
        {
            var leaf = new VbspLeaf();

            leaf.Contents = (VbspContents)data.ReadUInt32();
            leaf.Cluster = data.ReadInt16();
            leaf.AreaFlags = data.ReadInt16();
            leaf.Mins = new short[3];
            for (int i = 0; i < leaf.Mins.Length; i++)
            {
                leaf.Mins[i] = data.ReadInt16();
            }
            leaf.Maxs = new short[3];
            for (int i = 0; i < leaf.Maxs.Length; i++)
            {
                leaf.Maxs[i] = data.ReadInt16();
            }
            leaf.FirstLeafFace = data.ReadUInt16();
            leaf.NumLeafFaces = data.ReadUInt16();
            leaf.FirstLeafBrush = data.ReadUInt16();
            leaf.NumLeafBrushes = data.ReadUInt16();
            leaf.LeafWaterDataID = data.ReadInt16();

            if (version == 1)
                leaf.AmbientLighting = data.ReadType<CompressedLightCube>();
            else
                leaf.Padding = data.ReadInt16();

            return leaf;
        }

        /// <summary>
        /// Parse a Stream into LUMP_FACEIDS
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled LUMP_FACEIDS on success, null on error</returns>
        private static MarksurfacesLump? ParseMarksurfacesLump(Stream data, int offset, int length)
        {
            var marksurfaces = new List<ushort>();
            while (data.Position < offset + length)
            {
                marksurfaces.Add(data.ReadUInt16());
            }

            return new MarksurfacesLump { Marksurfaces = [.. marksurfaces] };
        }

        /// <summary>
        /// Parse a Stream into LUMP_EDGES
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled LUMP_EDGES on success, null on error</returns>
        private static EdgesLump? ParseEdgesLump(Stream data, int offset, int length)
        {
            var edges = new List<Edge>();
            while (data.Position < offset + length)
            {
                var edge = data.ReadType<Edge>();
                if (edge != null)
                    edges.Add(edge);
            }

            return new EdgesLump { Edges = [.. edges] };
        }

        /// <summary>
        /// Parse a Stream into LUMP_SURFEDGES
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled LUMP_SURFEDGES on success, null on error</returns>
        private static SurfedgesLump? ParseSurfedgesLump(Stream data, int offset, int length)
        {
            var surfedges = new List<int>();
            while (data.Position < offset + length)
            {
                surfedges.Add(data.ReadInt32());
            }

            return new SurfedgesLump { Surfedges = [.. surfedges] };
        }

        /// <summary>
        /// Parse a Stream into LUMP_MODELS
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled LUMP_MODELS on success, null on error</returns>
        private static VbspModelsLump? ParseModelsLump(Stream data, int offset, int length)
        {
            var models = new List<VbspModel>();
            while (data.Position < offset + length)
            {
                var model = data.ReadType<VbspModel>();
                if (model != null)
                    models.Add(model);
            }

            return new VbspModelsLump { Models = [.. models] };
        }

        /// <summary>
        /// Parse a Stream into LUMP_WORLDLIGHTS / LUMP_WORLDLIGHTS_HDR
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled LUMP_WORLDLIGHTS / LUMP_WORLDLIGHTS_HDR on success, null on error</returns>
        private static WorldLightsLump? ParseWorldLightsLump(Stream data, int offset, int length)
        {
            var worldLights = new List<WorldLight>();
            while (data.Position < offset + length)
            {
                var worldLight = data.ReadType<WorldLight>();
                if (worldLight != null)
                    worldLights.Add(worldLight);
            }

            return new WorldLightsLump { WorldLights = [.. worldLights] };
        }

        /// <summary>
        /// Parse a Stream into LUMP_LEAFFACES
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled LUMP_LEAFFACES on success, null on error</returns>
        private static LeafFacesLump? ParseLeafFacesLump(Stream data, int offset, int length)
        {
            var map = new List<ushort>();
            while (data.Position < offset + length)
            {
                map.Add(data.ReadUInt16());
            }

            return new LeafFacesLump { Map = [.. map] };
        }

        /// <summary>
        /// Parse a Stream into LUMP_LEAFBRUSHES
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled LUMP_LEAFBRUSHES on success, null on error</returns>
        private static LeafBrushesLump? ParseLeafBrushesLump(Stream data, int offset, int length)
        {
            var map = new List<ushort>();
            while (data.Position < offset + length)
            {
                map.Add(data.ReadUInt16());
            }

            return new LeafBrushesLump { Map = [.. map] };
        }

        /// <summary>
        /// Parse a Stream into LUMP_BRUSHES
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled LUMP_BRUSHES on success, null on error</returns>
        private static BrushesLump? ParseBrushesLump(Stream data, int offset, int length)
        {
            var brushes = new List<Brush>();
            while (data.Position < offset + length)
            {
                var brush = data.ReadType<Brush>();
                if (brush != null)
                    brushes.Add(brush);
            }

            return new BrushesLump { Brushes = [.. brushes] };
        }

        /// <summary>
        /// Parse a Stream into LUMP_BRUSHSIDES
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled LUMP_BRUSHSIDES on success, null on error</returns>
        private static BrushsidesLump? ParseBrushsidesLump(Stream data, int offset, int length)
        {
            var brushsides = new List<Brushside>();
            while (data.Position < offset + length)
            {
                var brushside = data.ReadType<Brushside>();
                if (brushside != null)
                    brushsides.Add(brushside);
            }

            return new BrushsidesLump { Brushsides = [.. brushsides] };
        }

        /// <summary>
        /// Parse a Stream into LUMP_DISPINFO
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled LUMP_DISPINFO on success, null on error</returns>
        private static DispInfosLump? ParseDispInfosLump(Stream data, int offset, int length)
        {
            var dispInfos = new List<DispInfo>();
            while (data.Position < offset + length)
            {
                var dispInfo = data.ReadType<DispInfo>();
                if (dispInfo != null)
                    dispInfos.Add(dispInfo);
            }

            return new DispInfosLump { Infos = [.. dispInfos] };
        }

        /// <summary>
        /// Parse a Stream into LUMP_DISP_VERTS
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled LUMP_DISP_VERTS on success, null on error</returns>
        private static DispVertsLump? ParseDispVertsLump(Stream data, int offset, int length)
        {
            var verts = new List<DispVert>();
            while (data.Position < offset + length)
            {
                var vert = data.ReadType<DispVert>();
                if (vert != null)
                    verts.Add(vert);
            }

            return new DispVertsLump { Verts = [.. verts] };
        }

        /// <summary>
        /// Parse a Stream into LUMP_GAME_LUMP
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled LUMP_GAME_LUMP on success, null on error</returns>
        private static GameLump? ParseGameLump(Stream data, int offset, int length)
        {
            var lump = new GameLump();

            lump.LumpCount = data.ReadInt32();
            lump.Directories = new GameLumpDirectory[lump.LumpCount];
            for (int i = 0; i < lump.LumpCount; i++)
            {
                var dir = data.ReadType<GameLumpDirectory>();
                if (dir != null)
                    lump.Directories[i] = dir;
            }

            return lump;
        }

        /// <summary>
        /// Parse a Stream into LUMP_CUBEMAPS
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled LUMP_CUBEMAPS on success, null on error</returns>
        private static CubemapsLump? ParseCubemapsLump(Stream data, int offset, int length)
        {
            var cubemaps = new List<Cubemap>();
            while (data.Position < offset + length)
            {
                var cubemap = data.ReadType<Cubemap>();
                if (cubemap != null)
                    cubemaps.Add(cubemap);
            }

            return new CubemapsLump { Cubemaps = [.. cubemaps] };
        }

        /// <summary>
        /// Parse a Stream into LUMP_PAKFILE
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled LUMP_PAKFILE on success, null on error</returns>
        private static PakfileLump? ParsePakfileLump(Stream data, int offset, int length)
        {
            var lump = new PakfileLump();

            lump.Data = data.ReadBytes(length);

            return lump;
        }

        /// <summary>
        /// Parse a Stream into LUMP_TEXDATA_STRING_DATA
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled LUMP_TEXDATA_STRING_DATA on success, null on error</returns>
        private static TexdataStringData? ParseTexdataStringData(Stream data, int offset, int length)
        {
            var strings = new List<string>();
            while (data.Position < offset + length)
            {
                var str = data.ReadNullTerminatedAnsiString();
                if (str != null)
                    strings.Add(str);
            }

            return new TexdataStringData { Strings = [.. strings] };
        }

        /// <summary>
        /// Parse a Stream into LUMP_TEXDATA_STRING_TABLE
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled LUMP_TEXDATA_STRING_TABLE on success, null on error</returns>
        private static TexdataStringTable? ParseTexdataStringTable(Stream data, int offset, int length)
        {
            var offsets = new List<int>();
            while (data.Position < offset + length)
            {
                offsets.Add(data.ReadInt32());
            }

            return new TexdataStringTable { Offsets = [.. offsets] };
        }

        /// <summary>
        /// Parse a Stream into LUMP_OVERLAYS
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled LUMP_OVERLAYS on success, null on error</returns>
        private static OverlaysLump? ParseOverlaysLump(Stream data, int offset, int length)
        {
            var overlays = new List<Overlay>();
            while (data.Position < offset + length)
            {
                var overlay = data.ReadType<Overlay>();
                if (overlay != null)
                    overlays.Add(overlay);
            }

            return new OverlaysLump { Overlays = [.. overlays] };
        }

        /// <summary>
        /// Parse a Stream into LUMP_DISP_TRIS
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled LUMP_DISP_TRIS on success, null on error</returns>
        private static DispTrisLump? ParseDispTrisLump(Stream data, int offset, int length)
        {
            var tris = new List<DispTri>();
            while (data.Position < offset + length)
            {
                var tri = data.ReadType<DispTri>();
                if (tri != null)
                    tris.Add(tri);
            }

            return new DispTrisLump { Tris = [.. tris] };
        }

        /// <summary>
        /// Parse a Stream into LUMP_LIGHTMAPPAGES / LUMP_LEAF_AMBIENT_INDEX_HDR / LUMP_LIGHTMAPPAGEINFOS / LUMP_LEAF_AMBIENT_INDEX
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled LUMP_LIGHTMAPPAGES / LUMP_LEAF_AMBIENT_INDEX_HDR / LUMP_LIGHTMAPPAGEINFOS / LUMP_LEAF_AMBIENT_INDEX on success, null on error</returns>
        private static AmbientIndexLump? ParseAmbientIndexLump(Stream data, int offset, int length)
        {
            var indicies = new List<LeafAmbientIndex>();
            while (data.Position < offset + length)
            {
                var index = data.ReadType<LeafAmbientIndex>();
                if (index != null)
                    indicies.Add(index);
            }

            return new AmbientIndexLump { Indicies = [.. indicies] };
        }

        /// <summary>
        /// Parse a Stream into LUMP_LEAF_AMBIENT_LIGHTING_HDR / LUMP_LEAF_AMBIENT_LIGHTING
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled LUMP_LEAF_AMBIENT_LIGHTING_HDR / LUMP_LEAF_AMBIENT_LIGHTING on success, null on error</returns>
        private static AmbientLightingLump? ParseAmbientLightingLump(Stream data, int offset, int length)
        {
            var lightings = new List<LeafAmbientLighting>();
            while (data.Position < offset + length)
            {
                var lighting = data.ReadType<LeafAmbientLighting>();
                if (lighting != null)
                    lightings.Add(lighting);
            }

            return new AmbientLightingLump { Lightings = [.. lightings] };
        }
    }
}