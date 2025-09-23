using System.Text;
using SabreTools.Models.BSP;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Printers
{
    public class BSP : IPrinter<BspFile>
    {
        /// <inheritdoc/>
        public void PrintInformation(StringBuilder builder, BspFile model)
            => Print(builder, model);

        public static void Print(StringBuilder builder, BspFile file)
        {
            builder.AppendLine("BSP Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine();

            Print(builder, file.Header);
            PrintLumps(builder, file);
        }

        private static void Print(StringBuilder builder, BspHeader? header)
        {
            builder.AppendLine("  Header Information:");
            builder.AppendLine("  -------------------------");
            if (header == null)
            {
                builder.AppendLine("  No header");
                builder.AppendLine();
                return;
            }

            builder.AppendLine(header.Version, "  Version");
            builder.AppendLine();
        }

        private static void PrintLumps(StringBuilder builder, BspFile? model)
        {
            builder.AppendLine("  Lumps Information:");
            builder.AppendLine("  -------------------------");
            if (model?.Header?.Lumps == null || model.Header.Lumps.Length == 0)
            {
                builder.AppendLine("  No lumps");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < model.Header.Lumps.Length; i++)
            {
                var lump = model.Header.Lumps[i];
                string specialLumpName = GetLumpName(i);

                builder.AppendLine($"  Lump {i}{specialLumpName}");
                builder.AppendLine(lump.Offset, "    Offset");
                builder.AppendLine(lump.Length, "    Length");

                switch ((LumpType)i)
                {
                    case LumpType.LUMP_ENTITIES:
                        Print(builder, model.Entities);
                        break;
                    case LumpType.LUMP_PLANES:
                        Print(builder, model.PlanesLump);
                        break;
                    case LumpType.LUMP_TEXTURES:
                        Print(builder, model.TextureLump);
                        break;
                    case LumpType.LUMP_VERTICES:
                        Print(builder, model.VerticesLump);
                        break;
                    case LumpType.LUMP_VISIBILITY:
                        Print(builder, model.VisibilityLump);
                        break;
                    case LumpType.LUMP_NODES:
                        Print(builder, model.NodesLump);
                        break;
                    case LumpType.LUMP_TEXINFO:
                        Print(builder, model.TexinfoLump);
                        break;
                    case LumpType.LUMP_FACES:
                        Print(builder, model.FacesLump);
                        break;
                    case LumpType.LUMP_LIGHTING:
                        Print(builder, model.LightmapLump);
                        break;
                    case LumpType.LUMP_CLIPNODES:
                        Print(builder, model.ClipnodesLump);
                        break;
                    case LumpType.LUMP_LEAVES:
                        Print(builder, model.LeavesLump);
                        break;
                    case LumpType.LUMP_MARKSURFACES:
                        Print(builder, model.MarksurfacesLump);
                        break;
                    case LumpType.LUMP_EDGES:
                        Print(builder, model.EdgesLump);
                        break;
                    case LumpType.LUMP_SURFEDGES:
                        Print(builder, model.SurfedgesLump);
                        break;
                    case LumpType.LUMP_MODELS:
                        Print(builder, model.ModelsLump);
                        break;
                    default:
                        builder.AppendLine($"    Unsupported lump type: {(LumpType)i} (0x{i:X4})");
                        break;
                }
            }

            builder.AppendLine();
        }

        private static string GetLumpName(int i)
        {
            return (LumpType)i switch
            {
                LumpType.LUMP_ENTITIES => " - LUMP_ENTITIES",
                LumpType.LUMP_PLANES => " - LUMP_PLANES",
                LumpType.LUMP_TEXTURES => " - LUMP_TEXTURES",
                LumpType.LUMP_VERTICES => " - LUMP_VERTICES",
                LumpType.LUMP_VISIBILITY => " - LUMP_VISIBILITY",
                LumpType.LUMP_NODES => " - LUMP_NODES",
                LumpType.LUMP_TEXINFO => " - LUMP_TEXINFO",
                LumpType.LUMP_FACES => " - LUMP_FACES",
                LumpType.LUMP_LIGHTING => " - LUMP_LIGHTING",
                LumpType.LUMP_CLIPNODES => " - LUMP_CLIPNODES",
                LumpType.LUMP_LEAVES => " - LUMP_LEAVES",
                LumpType.LUMP_MARKSURFACES => " - LUMP_MARKSURFACES",
                LumpType.LUMP_EDGES => " - LUMP_EDGES",
                LumpType.LUMP_SURFEDGES => " - LUMP_SURFEDGES",
                LumpType.LUMP_MODELS => " - LUMP_MODELS",
                _ => string.Empty,
            };
        }

        private static void Print(StringBuilder builder, EntitiesLump? lump)
        {
            if (lump?.Entities == null || lump.Entities.Length == 0)
            {
                builder.AppendLine("    No data");
                return;
            }

            for (int i = 0; i < lump.Entities.Length; i++)
            {
                var entity = lump.Entities[i];

                builder.AppendLine($"    Entity {i}:");
                if (entity.Attributes == null || entity.Attributes.Count == 0)
                {
                    builder.AppendLine("      No attributes");
                    continue;
                }

                for (int j = 0; j < entity.Attributes.Count; j++)
                {
                    var kvp = entity.Attributes[j];

                    builder.AppendLine($"      Attribute {j}: {kvp.Key}={kvp.Value}");
                }
            }
        }

        private static void Print(StringBuilder builder, PlanesLump? lump)
        {
            if (lump?.Planes == null || lump.Planes.Length == 0)
            {
                builder.AppendLine("    No data");
                return;
            }

            for (int i = 0; i < lump.Planes.Length; i++)
            {
                var plane = lump.Planes[i];

                builder.AppendLine($"    Plane {i}");
                builder.AppendLine($"      Normal vector: ({plane.NormalVector?.X}, {plane.NormalVector?.Y}, {plane.NormalVector?.Z})");
                builder.AppendLine(plane.Distance, "      Distance");
                builder.AppendLine($"      Plane type: {plane.PlaneType} (0x{plane.PlaneType:X})");
            }
        }

        private static void Print(StringBuilder builder, TextureLump? lump)
        {
            if (lump == null)
            {
                builder.AppendLine("    No data");
                return;
            }

            if (lump?.Header == null)
            {
                builder.AppendLine("    No texture header");
            }
            else
            {
                builder.AppendLine("    Texture Header:");
                builder.AppendLine(lump.Header.MipTextureCount, "      MipTexture count");
                builder.AppendLine(lump.Header.Offsets, "      Offsets");
            }

            builder.AppendLine("    Textures:");
            if (lump?.Textures == null || lump.Textures.Length == 0)
            {
                builder.AppendLine("    No texture data");
                return;
            }

            for (int i = 0; i < lump.Textures.Length; i++)
            {
                var texture = lump.Textures[i];

                builder.AppendLine($"      Texture {i}");
                builder.AppendLine(texture.Name, "        Name");
                builder.AppendLine(texture.Width, "        Width");
                builder.AppendLine(texture.Height, "        Height");
                builder.AppendLine(texture.Offsets, "        Offsets");
            }
        }

        private static void Print(StringBuilder builder, VerticesLump? lump)
        {
            if (lump?.Vertices == null || lump.Vertices.Length == 0)
            {
                builder.AppendLine("    No data");
                return;
            }

            for (int i = 0; i < lump.Vertices.Length; i++)
            {
                var vertex = lump.Vertices[i];
                builder.AppendLine($"    Vertex {i}: ({vertex.X}, {vertex.Y}, {vertex.Z})");
            }
        }

        private static void Print(StringBuilder builder, VisibilityLump? lump)
        {
            if (lump == null)
            {
                builder.AppendLine("    No data");
                return;
            }

            builder.AppendLine(lump.NumClusters, "    Cluster count");
            builder.AppendLine("    Byte offsets skipped...");
        }

        private static void Print(StringBuilder builder, BspNodesLump? lump)
        {
            if (lump?.Nodes == null || lump.Nodes.Length == 0)
            {
                builder.AppendLine("    No data");
                return;
            }

            for (int i = 0; i < lump.Nodes.Length; i++)
            {
                var node = lump.Nodes[i];

                builder.AppendLine($"    Node {i}");
                builder.AppendLine(node.Children, "      Children");
                builder.AppendLine(node.Mins, "      Mins");
                builder.AppendLine(node.Maxs, "      Maxs");
                builder.AppendLine(node.FirstFace, "      First face index");
                builder.AppendLine(node.FaceCount, "      Count of faces");
            }
        }

        private static void Print(StringBuilder builder, BspTexinfoLump? lump)
        {
            if (lump?.Texinfos == null || lump.Texinfos.Length == 0)
            {
                builder.AppendLine("    No data");
                return;
            }

            for (int i = 0; i < lump.Texinfos.Length; i++)
            {
                var texinfo = lump.Texinfos[i];

                builder.AppendLine($"    Texinfo {i}");
                builder.AppendLine($"      S-Vector: ({texinfo.SVector?.X}, {texinfo.SVector?.Y}, {texinfo.SVector?.Z})");
                builder.AppendLine(texinfo.TextureSShift, "      Texture shift in S direction");
                builder.AppendLine($"      T-Vector: ({texinfo.TVector?.X}, {texinfo.TVector?.Y}, {texinfo.TVector?.Z})");
                builder.AppendLine(texinfo.TextureTShift, "      Texture shift in T direction");
                builder.AppendLine(texinfo.MiptexIndex, "      Miptex index");
                builder.AppendLine($"      Flags: {texinfo.Flags} (0x{texinfo.Flags:X})");
            }
        }

        private static void Print(StringBuilder builder, BspFacesLump? lump)
        {
            if (lump?.Faces == null || lump.Faces.Length == 0)
            {
                builder.AppendLine("    No data");
                return;
            }

            for (int i = 0; i < lump.Faces.Length; i++)
            {
                var face = lump.Faces[i];

                builder.AppendLine($"    Face {i}");
                builder.AppendLine(face.PlaneIndex, "      Plane index");
                builder.AppendLine(face.PlaneSideCount, "      Plane side count");
                builder.AppendLine(face.FirstEdgeIndex, "      First surfedge index");
                builder.AppendLine(face.NumberOfEdges, "      Surfedge count");
                builder.AppendLine(face.TextureInfoIndex, "      Texture info index");
                builder.AppendLine(face.LightingStyles, "      Lighting styles");
                builder.AppendLine(face.LightmapOffset, "      Lightmap offset");
            }
        }

        private static void Print(StringBuilder builder, LightmapLump? lump)
        {
            if (lump?.Lightmap == null || lump.Lightmap.Length == 0)
                builder.AppendLine("    No data");
            else
                builder.AppendLine("    Lightmap data skipped...");
        }

        private static void Print(StringBuilder builder, ClipnodesLump? lump)
        {
            if (lump?.Clipnodes == null || lump.Clipnodes.Length == 0)
            {
                builder.AppendLine("    No data");
                return;
            }

            for (int i = 0; i < lump.Clipnodes.Length; i++)
            {
                var clipnode = lump.Clipnodes[i];

                builder.AppendLine($"    Clipnode {i}");
                builder.AppendLine(clipnode.PlaneIndex, "      Plane index");
                builder.AppendLine(clipnode.ChildrenIndices, "      Children indices");
            }
        }

        private static void Print(StringBuilder builder, BspLeavesLump? lump)
        {
            if (lump?.Leaves == null || lump.Leaves.Length == 0)
            {
                builder.AppendLine("    No data");
                return;
            }

            for (int i = 0; i < lump.Leaves.Length; i++)
            {
                var leaf = lump.Leaves[i];

                builder.AppendLine($"    Leaf {i}");
                builder.AppendLine($"      Contents: {leaf.Contents} (0x{leaf.Contents:X})");
                builder.AppendLine(leaf.VisOffset, "      Visibility offset");
                builder.AppendLine(leaf.Mins, "      Mins");
                builder.AppendLine(leaf.Maxs, "      Maxs");
                builder.AppendLine(leaf.FirstMarkSurfaceIndex, "      First marksurface index");
                builder.AppendLine(leaf.MarkSurfacesCount, "      Marksurfaces count");
                builder.AppendLine(leaf.AmbientLevels, "      Ambient sound levels");
            }
        }

        private static void Print(StringBuilder builder, MarksurfacesLump? lump)
        {
            if (lump?.Marksurfaces == null || lump.Marksurfaces.Length == 0)
            {
                builder.AppendLine("    No data");
                return;
            }

            for (int i = 0; i < lump.Marksurfaces.Length; i++)
            {
                var marksurface = lump.Marksurfaces[i];
                builder.AppendLine(marksurface, $"    Marksurface {i}");
            }
        }

        private static void Print(StringBuilder builder, EdgesLump? lump)
        {
            if (lump?.Edges == null || lump.Edges.Length == 0)
            {
                builder.AppendLine("    No data");
                return;
            }

            for (int i = 0; i < lump.Edges.Length; i++)
            {
                var edge = lump.Edges[i];
                builder.AppendLine($"    Edge {i}");
                builder.AppendLine(edge.VertexIndices, "      Vertex indices");
            }
        }

        private static void Print(StringBuilder builder, SurfedgesLump? lump)
        {
            if (lump?.Surfedges == null || lump.Surfedges.Length == 0)
            {
                builder.AppendLine("    No data");
                return;
            }

            for (int i = 0; i < lump.Surfedges.Length; i++)
            {
                var surfedge = lump.Surfedges[i];
                builder.AppendLine(surfedge, $"    Surfedge {i}");
            }
        }

        private static void Print(StringBuilder builder, BspModelsLump? lump)
        {
            if (lump?.Models == null || lump.Models.Length == 0)
            {
                builder.AppendLine("    No data");
                return;
            }

            for (int i = 0; i < lump.Models.Length; i++)
            {
                var bmodel = lump.Models[i];

                builder.AppendLine($"    Model {i}");
                builder.AppendLine($"      Mins: {bmodel.Mins?.X}, {bmodel.Mins?.Y}, {bmodel.Mins?.Z}");
                builder.AppendLine($"      Maxs: {bmodel.Maxs?.X}, {bmodel.Maxs?.Y}, {bmodel.Maxs?.Z}");
                builder.AppendLine($"      Origin vector: {bmodel.OriginVector?.X}, {bmodel.OriginVector?.Y}, {bmodel.OriginVector?.Z}");
                builder.AppendLine(bmodel.HeadnodesIndex, "      Headnodes index");
                builder.AppendLine(bmodel.VisLeafsCount, "      ??? (VisLeafsCount)");
                builder.AppendLine(bmodel.FirstFaceIndex, "      First face index");
                builder.AppendLine(bmodel.FacesCount, "      Faces count");
            }
        }
    }
}
