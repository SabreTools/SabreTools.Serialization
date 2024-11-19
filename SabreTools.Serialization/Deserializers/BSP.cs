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
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            // If the offset is out of bounds
            if (data.Position < 0 || data.Position >= data.Length)
                return null;

            // Create a new Half-Life Level to fill
            var file = new BspFile();

            #region Header

            // Try to parse the header
            var header = ParseHeader(data);
            if (header?.Lumps == null)
                return null;

            // Set the level header
            file.Header = header;

            #endregion

            #region Lumps

            for (int l = 0; l < BSP_HEADER_LUMPS; l++)
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
                        file.TextureLump = ParseTextureLump(data, lumpEntry.Offset, lumpEntry.Length);
                        break;
                    case LumpType.LUMP_VERTICES:
                        file.VerticesLump = ParseVerticesLump(data, lumpEntry.Offset, lumpEntry.Length);
                        break;
                    case LumpType.LUMP_VISIBILITY:
                        // TODO: Assign when Models supports it
                        _ = ParseVisibilityLump(data, lumpEntry.Offset, lumpEntry.Length);
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

        /// <summary>
        /// Parse a Stream into a Half-Life Level header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Half-Life Level header on success, null on error</returns>
        /// <remarks>Only recognized versions are 29 and 30</remarks>
        private static BspHeader? ParseHeader(Stream data)
        {
            // TODO: Use marshalling here later
            var header = new BspHeader();

            header.Version = data.ReadInt32();
            if (header.Version < 29 || header.Version > 30)
                return null;

            header.Lumps = new BspLumpEntry[BSP_HEADER_LUMPS];
            for (int i = 0; i < BSP_HEADER_LUMPS; i++)
            {
                header.Lumps[i] = data.ReadType<BspLumpEntry>()!;
            }

            return header;
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
                var texture = data.ReadType<MipTexture>();
                if (texture != null)
                    textures.Add(texture);
            }

            lump.Textures = [.. textures];
            return lump;
        }

        /// <summary>
        /// Parse a Stream into a Half-Life Level texture header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Half-Life Level texture header on success, null on error</returns>
        private static TextureHeader ParseTextureHeader(Stream data)
        {
            // TODO: Use marshalling here instead of building
            var textureHeader = new TextureHeader();

            textureHeader.MipTextureCount = data.ReadUInt32();
            textureHeader.Offsets = new int[textureHeader.MipTextureCount];
            for (int i = 0; i < textureHeader.Offsets.Length; i++)
            {
                textureHeader.Offsets[i] = data.ReadInt32();
            }

            return textureHeader;
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
                vertices.Add(data.ReadType<Vector3D>());
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
            lump.ByteOffsets = new int[lump.NumClusters, 2];
            for (int i = 0; i < lump.NumClusters; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    lump.ByteOffsets[i, j] = data.ReadInt32();
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
                var node = data.ReadType<BspNode>();
                if (node != null)
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
                var texinfo = data.ReadType<BspTexinfo>();
                if (texinfo != null)
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
                var face = data.ReadType<BspFace>();
                if (face != null)
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
            lump.Lightmap = new byte[length / 3, 3];

            for (int i = 0; i < length / 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    lump.Lightmap[i, j] = data.ReadByteValue();
                }
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
                var clipnode = data.ReadType<Clipnode>();
                if (clipnode != null)
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
                var leaf = data.ReadType<BspLeaf>();
                if (leaf != null)
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
        private static BspModelsLump? ParseModelsLump(Stream data, int offset, int length)
        {
            var models = new List<BspModel>();
            while (data.Position < offset + length)
            {
                var model = data.ReadType<BspModel>();
                if (model != null)
                    models.Add(model);
            }

            return new BspModelsLump { Models = [.. models] };
        }
    }
}