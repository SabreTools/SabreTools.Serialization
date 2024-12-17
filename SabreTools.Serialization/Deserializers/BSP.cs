using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SabreTools.IO.Extensions;
using SabreTools.Models.BSP;
using static SabreTools.Models.BSP.Constants;

namespace SabreTools.Serialization.Deserializers
{
    public class BSP : BaseBinaryDeserializer<BspFile>
    {
        /// <inheritdoc/>
        public override BspFile? Deserialize(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                // Create a new Half-Life Level to fill
                var file = new BspFile();

                #region Header

                // Try to parse the header
                var header = ParseBspHeader(data);
                if (header.Version < 29 || header.Version > 30)
                    return null;

                // Set the level header
                file.Header = header;

                #endregion

                #region Lumps

                for (int l = 0; l < BSP_HEADER_LUMPS; l++)
                {
                    // Get the next lump entry
                    var lumpEntry = header.Lumps![l];
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
                            file.TextureLump = ParseTextureLump(data, lumpEntry.Offset, lumpEntry.Length);
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
                            file.ClipnodesLump = ParseClipnodesLump(data, lumpEntry.Offset, lumpEntry.Length);
                            break;
                        case LumpType.LUMP_LEAVES:
                            file.LeavesLump = ParseLeavesLump(data, lumpEntry.Offset, lumpEntry.Length);
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
                        default:
                            // Unsupported LumpType value, ignore
                            break;
                    }
                }

                #endregion

                return file;
            }
            catch
            {
                // Ignore the actual error
                return null;
            }
        }

        /// <summary>
        /// Parse a Stream into BspFace
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled BspFace on success, null on error</returns>
        public static BspFace ParseBspFace(Stream data)
        {
            var obj = new BspFace();

            obj.PlaneIndex = data.ReadUInt16LittleEndian();
            obj.PlaneSideCount = data.ReadUInt16LittleEndian();
            obj.FirstEdgeIndex = data.ReadUInt32LittleEndian();
            obj.NumberOfEdges = data.ReadUInt16LittleEndian();
            obj.TextureInfoIndex = data.ReadUInt16LittleEndian();
            obj.LightingStyles = data.ReadBytes(4);
            obj.LightmapOffset = data.ReadInt32LittleEndian();

            return obj;
        }

        /// <summary>
        /// Parse a Stream into BspHeader
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled BspHeader on success, null on error</returns>
        public static BspHeader ParseBspHeader(Stream data)
        {
            var obj = new BspHeader();

            obj.Version = data.ReadInt32LittleEndian();
            obj.Lumps = new BspLumpEntry[BSP_HEADER_LUMPS];
            for (int i = 0; i < BSP_HEADER_LUMPS; i++)
            {
                obj.Lumps[i] = ParseBspLumpEntry(data);
            }

            return obj;
        }

        /// <summary>
        /// Parse a Stream into BspLeaf
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled BspLeaf on success, null on error</returns>
        public static BspLeaf ParseBspLeaf(Stream data)
        {
            var obj = new BspLeaf();

            obj.Contents = (BspContents)data.ReadInt32LittleEndian();
            obj.VisOffset = data.ReadInt32LittleEndian();
            obj.Mins = new short[3];
            for (int i = 0; i < 3; i++)
            {
                obj.Mins[i] = data.ReadInt16LittleEndian();
            }
            obj.Maxs = new short[3];
            for (int i = 0; i < 3; i++)
            {
                obj.Maxs[i] = data.ReadInt16LittleEndian();
            }
            obj.FirstMarkSurfaceIndex = data.ReadUInt16LittleEndian();
            obj.MarkSurfacesCount = data.ReadUInt16LittleEndian();
            obj.AmbientLevels = data.ReadBytes(4);

            return obj;
        }

        /// <summary>
        /// Parse a Stream into BspLumpEntry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled BspLumpEntry on success, null on error</returns>
        public static BspLumpEntry ParseBspLumpEntry(Stream data)
        {
            var obj = new BspLumpEntry();

            obj.Offset = data.ReadInt32LittleEndian();
            obj.Length = data.ReadInt32LittleEndian();

            return obj;
        }

        /// <summary>
        /// Parse a Stream into BspModel
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled BspModel on success, null on error</returns>
        public static BspModel ParseBspModel(Stream data)
        {
            var obj = new BspModel();

            obj.Mins = ParseVector3D(data);
            obj.Maxs = ParseVector3D(data);
            obj.OriginVector = ParseVector3D(data);
            obj.HeadnodesIndex = new int[MAX_MAP_HULLS];
            for (int i = 0; i < MAX_MAP_HULLS; i++)
            {
                obj.HeadnodesIndex[i] = data.ReadInt32LittleEndian();
            }
            obj.VisLeafsCount = data.ReadInt32LittleEndian();
            obj.FirstFaceIndex = data.ReadInt32LittleEndian();
            obj.FacesCount = data.ReadInt32LittleEndian();

            return obj;
        }

        /// <summary>
        /// Parse a Stream into BspNode
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled BspNode on success, null on error</returns>
        public static BspNode ParseBspNode(Stream data)
        {
            var obj = new BspNode();

            obj.PlaneIndex = data.ReadUInt32LittleEndian();
            obj.Children = new ushort[2];
            for (int i = 0; i < 2; i++)
            {
                obj.Children[i] = data.ReadUInt16LittleEndian();
            }
            obj.Mins = new ushort[3];
            for (int i = 0; i < 3; i++)
            {
                obj.Mins[i] = data.ReadUInt16LittleEndian();
            }
            obj.Maxs = new ushort[3];
            for (int i = 0; i < 3; i++)
            {
                obj.Maxs[i] = data.ReadUInt16LittleEndian();
            }
            obj.FirstFace = data.ReadUInt16LittleEndian();
            obj.FaceCount = data.ReadUInt16LittleEndian();

            return obj;
        }

        /// <summary>
        /// Parse a Stream into BspTexinfo
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled BspTexinfo on success, null on error</returns>
        public static BspTexinfo ParseBspTexinfo(Stream data)
        {
            var obj = new BspTexinfo();

            obj.SVector = ParseVector3D(data);
            obj.TextureSShift = data.ReadSingle();
            obj.TVector = ParseVector3D(data);
            obj.TextureTShift = data.ReadSingle();
            obj.MiptexIndex = data.ReadUInt32LittleEndian();
            obj.Flags = (TextureFlag)data.ReadUInt32LittleEndian();

            return obj;
        }

        /// <summary>
        /// Parse a Stream into Clipnode
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Clipnode on success, null on error</returns>
        public static Clipnode ParseClipnode(Stream data)
        {
            var obj = new Clipnode();

            obj.PlaneIndex = data.ReadInt32LittleEndian();
            obj.ChildrenIndices = new short[2];
            for (int i = 0; i < 2; i++)
            {
                obj.ChildrenIndices[i] = data.ReadInt16LittleEndian();
            }

            return obj;
        }

        /// <summary>
        /// Parse a Stream into Edge
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Edge on success, null on error</returns>
        public static Edge ParseEdge(Stream data)
        {
            var obj = new Edge();

            obj.VertexIndices = new ushort[2];
            for (int i = 0; i < 2; i++)
            {
                obj.VertexIndices[i] = data.ReadUInt16LittleEndian();
            }

            return obj;
        }

        /// <summary>
        /// Parse a Stream into MipTexture
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled MipTexture on success, null on error</returns>
        public static MipTexture ParseMipTexture(Stream data)
        {
            var obj = new MipTexture();

            byte[] name = data.ReadBytes(MAXTEXTURENAME);
            obj.Name = Encoding.ASCII.GetString(name).TrimEnd('\0');
            obj.Width = data.ReadUInt32LittleEndian();
            obj.Height = data.ReadUInt32LittleEndian();
            obj.Offsets = new uint[MIPLEVELS];
            for (int i = 0; i < MIPLEVELS; i++)
            {
                obj.Offsets[i] = data.ReadUInt32LittleEndian();
            }

            return obj;
        }

        /// <summary>
        /// Parse a Stream into Plane
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Plane on success, null on error</returns>
        public static Plane ParsePlane(Stream data)
        {
            var obj = new Plane();

            obj.NormalVector = ParseVector3D(data);
            obj.Distance = data.ReadSingle();
            obj.PlaneType = (PlaneType)data.ReadInt32LittleEndian();

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a TextureHeader
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled TextureHeader on success, null on error</returns>
        public static TextureHeader ParseTextureHeader(Stream data)
        {
            var obj = new TextureHeader();

            obj.MipTextureCount = data.ReadUInt32LittleEndian();
            obj.Offsets = new int[obj.MipTextureCount];
            for (int i = 0; i < obj.Offsets.Length; i++)
            {
                obj.Offsets[i] = data.ReadInt16LittleEndian();
            }

            return obj;
        }

        /// <summary>
        /// Parse a Stream into Vector3D
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Vector3D on success, null on error</returns>
        public static Vector3D ParseVector3D(Stream data)
        {
            var obj = new Vector3D();

            obj.X = data.ReadSingle();
            obj.Y = data.ReadSingle();
            obj.Z = data.ReadSingle();

            return obj;
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
                var plane = ParsePlane(data);
                planes.Add(plane);
            }

            return new PlanesLump { Planes = [.. planes] };
        }

        /// <summary>
        /// Parse a Stream into LUMP_TEXTURES
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled LUMP_TEXTURES on success, null on error</returns>
        private static TextureLump? ParseTextureLump(Stream data, int offset, int length)
        {
            var lump = new TextureLump();

            lump.Header = ParseTextureHeader(data);
            var textures = new List<MipTexture>();
            while (data.Position < offset + length)
            {
                var texture = ParseMipTexture(data);
                textures.Add(texture);
            }

            lump.Textures = [.. textures];
            return lump;
        }

        /// <summary>
        /// Parse a Stream into LUMP_VERTICES
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled LUMP_VERTICES on success, null on error</returns>
        private static VerticesLump? ParseVerticesLump(Stream data, int offset, int length)
        {
            var vertices = new List<Vector3D>();
            while (data.Position < offset + length)
            {
                var vertex = ParseVector3D(data);
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

            // BSP29 has an incompatible value here
            int bytesNeeded = lump.NumClusters * 8;
            if (bytesNeeded > length)
                return null;

            lump.ByteOffsets = new int[lump.NumClusters][];
            for (int i = 0; i < lump.NumClusters; i++)
            {
                lump.ByteOffsets[i] = new int[2];
                for (int j = 0; j < 2; j++)
                {
                    lump.ByteOffsets[i][j] = data.ReadInt32LittleEndian();
                }
            }

            return lump;
        }

        /// <summary>
        /// Parse a Stream into LUMP_NODES
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled LUMP_NODES on success, null on error</returns>
        private static BspNodesLump? ParseNodesLump(Stream data, int offset, int length)
        {
            var nodes = new List<BspNode>();
            while (data.Position < offset + length)
            {
                var node = ParseBspNode(data);
                nodes.Add(node);
            }

            return new BspNodesLump { Nodes = [.. nodes] };
        }

        /// <summary>
        /// Parse a Stream into LUMP_TEXINFO
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled LUMP_TEXINFO on success, null on error</returns>
        private static BspTexinfoLump? ParseTexinfoLump(Stream data, int offset, int length)
        {
            var texinfos = new List<BspTexinfo>();
            while (data.Position < offset + length)
            {
                var texinfo = ParseBspTexinfo(data);
                texinfos.Add(texinfo);
            }

            return new BspTexinfoLump { Texinfos = [.. texinfos] };
        }

        /// <summary>
        /// Parse a Stream into LUMP_FACES
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled LUMP_FACES on success, null on error</returns>
        private static BspFacesLump? ParseFacesLump(Stream data, int offset, int length)
        {
            var faces = new List<BspFace>();
            while (data.Position < offset + length)
            {
                var face = ParseBspFace(data);
                faces.Add(face);
            }

            return new BspFacesLump { Faces = [.. faces] };
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
        /// Parse a Stream into LUMP_CLIPNODES
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled LUMP_CLIPNODES on success, null on error</returns>
        private static ClipnodesLump? ParseClipnodesLump(Stream data, int offset, int length)
        {
            var clipnodes = new List<Clipnode>();
            while (data.Position < offset + length)
            {
                var clipnode = ParseClipnode(data);
                clipnodes.Add(clipnode);
            }

            return new ClipnodesLump { Clipnodes = [.. clipnodes] };
        }

        /// <summary>
        /// Parse a Stream into LUMP_LEAVES
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled LUMP_LEAVES on success, null on error</returns>
        private static BspLeavesLump? ParseLeavesLump(Stream data, int offset, int length)
        {
            var leaves = new List<BspLeaf>();
            while (data.Position < offset + length)
            {
                var leaf = ParseBspLeaf(data);
                leaves.Add(leaf);
            }

            return new BspLeavesLump { Leaves = [.. leaves] };
        }

        /// <summary>
        /// Parse a Stream into LUMP_MARKSURFACES
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled LUMP_MARKSURFACES on success, null on error</returns>
        private static MarksurfacesLump? ParseMarksurfacesLump(Stream data, int offset, int length)
        {
            var marksurfaces = new List<ushort>();
            while (data.Position < offset + length)
            {
                marksurfaces.Add(data.ReadUInt16LittleEndian());
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
                var edge = ParseEdge(data);
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
                surfedges.Add(data.ReadInt32LittleEndian());
            }

            return new SurfedgesLump { Surfedges = [.. surfedges] };
        }

        /// <summary>
        /// Parse a Stream into LUMP_MODELS
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled LUMP_MODELS on success, null on error</returns>
        private static BspModelsLump? ParseModelsLump(Stream data, int offset, int length)
        {
            var models = new List<BspModel>();
            while (data.Position < offset + length)
            {
                var model = ParseBspModel(data);
                models.Add(model);
            }

            return new BspModelsLump { Models = [.. models] };
        }
    }
}