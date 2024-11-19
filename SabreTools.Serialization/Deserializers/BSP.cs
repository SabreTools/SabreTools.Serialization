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

            // Cache the current offset
            int initialOffset = (int)data.Position;

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

            // LUMP_TEXTURES [2]
            lumpEntry = header.Lumps[(int)LumpType.LUMP_TEXTURES];
            if (lumpEntry != null && lumpEntry.Offset > 0 && lumpEntry.Length > 0)
            {
                // Seek to the lump offset
                data.Seek(lumpEntry.Offset, SeekOrigin.Begin);

                // Read the header
                var lump = new TextureLump();
                lump.Header = ParseTextureHeader(data);

                // Read the lump data
                var textures = new List<MipTexture>();
                while (data.Position < lumpEntry.Offset + lumpEntry.Length)
                {
                    var texture = data.ReadType<MipTexture>();
                    if (texture != null)
                        textures.Add(texture);
                }

                lump.Textures = [.. textures];

                file.TextureLump = lump;
            }

            // LUMP_VERTICES [3]
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
                // TODO: Parse LUMP_VISIBILITY when added to model
            }

            // LUMP_NODES [5]
            lumpEntry = header.Lumps[(int)LumpType.LUMP_NODES];
            if (lumpEntry != null && lumpEntry.Offset > 0 && lumpEntry.Length > 0)
            {
                // Seek to the lump offset
                data.Seek(lumpEntry.Offset, SeekOrigin.Begin);

                // Read the lump data
                var nodes = new List<BspNode>();
                while (data.Position < lumpEntry.Offset + lumpEntry.Length)
                {
                    var node = data.ReadType<BspNode>();
                    if (node != null)
                        nodes.Add(node);
                }

                var lump = new BspNodesLump();
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
                var texinfos = new List<BspTexinfo>();
                while (data.Position < lumpEntry.Offset + lumpEntry.Length)
                {
                    var texinfo = data.ReadType<BspTexinfo>();
                    if (texinfo != null)
                        texinfos.Add(texinfo);
                }

                var lump = new BspTexinfoLump();
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
                var faces = new List<BspFace>();
                while (data.Position < lumpEntry.Offset + lumpEntry.Length)
                {
                    var face = data.ReadType<BspFace>();
                    if (face != null)
                        faces.Add(face);
                }

                var lump = new BspFacesLump();
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

            // LUMP_CLIPNODES [9]
            lumpEntry = header.Lumps[(int)LumpType.LUMP_CLIPNODES];
            if (lumpEntry != null && lumpEntry.Offset > 0 && lumpEntry.Length > 0)
            {
                // Seek to the lump offset
                data.Seek(lumpEntry.Offset, SeekOrigin.Begin);

                // Read the lump data
                var clipnodes = new List<Clipnode>();
                while (data.Position < lumpEntry.Offset + lumpEntry.Length)
                {
                    var clipnode = data.ReadType<Clipnode>();
                    if (clipnode != null)
                        clipnodes.Add(clipnode);
                }

                var lump = new ClipnodesLump();
                lump.Clipnodes = [.. clipnodes];

                file.ClipnodesLump = lump;
            }

            // LUMP_LEAVES [10]
            lumpEntry = header.Lumps[(int)LumpType.LUMP_LEAVES];
            if (lumpEntry != null && lumpEntry.Offset > 0 && lumpEntry.Length > 0)
            {
                // Seek to the lump offset
                data.Seek(lumpEntry.Offset, SeekOrigin.Begin);

                // Read the lump data
                var leaves = new List<BspLeaf>();
                while (data.Position < lumpEntry.Offset + lumpEntry.Length)
                {
                    var leaf = data.ReadType<BspLeaf>();
                    if (leaf != null)
                        leaves.Add(leaf);
                }

                var lump = new BspLeavesLump();
                lump.Leaves = [.. leaves];

                file.LeavesLump = lump;
            }

            // LUMP_MARKSURFACES [11]
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
                var models = new List<BspModel>();
                while (data.Position < lumpEntry.Offset + lumpEntry.Length)
                {
                    var model = data.ReadType<BspModel>();
                    if (model != null)
                        models.Add(model);
                }

                var lump = new BspModelsLump();
                lump.Models = [.. models];

                file.ModelsLump = lump;
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
    }
}