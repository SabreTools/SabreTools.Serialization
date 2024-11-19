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
                if (lump == null)
                {
                    builder.AppendLine("    [NULL]");
                    continue;
                }

                builder.AppendLine(lump.Offset, "    Offset");
                builder.AppendLine(lump.Length, "    Length");
                switch ((LumpType)i)
                {
                    case LumpType.LUMP_ENTITIES:
                        if (model.Entities?.Entities == null || model.Entities.Entities.Length == 0)
                        {
                            builder.AppendLine("    No data");
                        }
                        else
                        {
                            for (int j = 0; j < model.Entities.Entities.Length; j++)
                            {
                                // TODO: Implement entity printing
                                var entity = model.Entities.Entities[j];
                                builder.AppendLine($"    Entity {j}");
                                builder.AppendLine("      Entity data is not parsed properly");
                            }
                        }
                        break;
                    case LumpType.LUMP_PLANES:
                        if (model.PlanesLump?.Planes == null || model.PlanesLump.Planes.Length == 0)
                        {
                            builder.AppendLine("    No data");
                        }
                        else
                        {
                            for (int j = 0; j < model.PlanesLump.Planes.Length; j++)
                            {
                                var plane = model.PlanesLump.Planes[j];
                                builder.AppendLine($"    Plane {j}");
                                builder.AppendLine($"      Normal vector: {plane.NormalVector.X}, {plane.NormalVector.Y}, {plane.NormalVector.Z}");
                                builder.AppendLine(plane.Distance, "      Distance");
                                builder.AppendLine($"      Plane type: {plane.PlaneType} (0x{plane.PlaneType:X})");
                            }
                        }
                        break;
                    case LumpType.LUMP_TEXTURES:
                        if (model.TextureLump?.Textures == null || model.TextureLump.Textures.Length == 0)
                        {
                            builder.AppendLine("    No data");
                        }
                        else
                        {
                            var header = model.TextureLump.Header;
                            if (header == null)
                            {
                                builder.AppendLine("    No texture header");
                            }
                            else
                            {
                                builder.AppendLine("    Texture Header:");
                                builder.AppendLine(header.MipTextureCount, "      MipTexture count");
                                builder.AppendLine(header.Offsets, "      Offsets");
                            }

                            builder.AppendLine("    Textures:");
                            for (int j = 0; j < model.TextureLump.Textures.Length; j++)
                            {
                                var texture = model.TextureLump.Textures[j];
                                builder.AppendLine($"      Texture {j}");
                                builder.AppendLine(texture.Name, "        Name");
                                builder.AppendLine(texture.Width, "        Width");
                                builder.AppendLine(texture.Height, "        Height");
                                builder.AppendLine(texture.Offsets, "        Offsets");
                            }
                        }
                        break;
                    case LumpType.LUMP_VERTICES:
                        if (model.VerticesLump?.Vertices == null || model.VerticesLump.Vertices.Length == 0)
                        {
                            builder.AppendLine("    No data");
                        }
                        else
                        {
                            for (int j = 0; j < model.VerticesLump.Vertices.Length; j++)
                            {
                                var vertex = model.VerticesLump.Vertices[j];
                                builder.AppendLine($"    Vertex {j}: {vertex.X}, {vertex.Y}, {vertex.Z}");
                            }
                        }
                        break;
                    case LumpType.LUMP_VISIBILITY:
                        // TODO: Implement when added to Models
                        // if (model.VisibilityLump == null)
                        // {
                        //     builder.AppendLine("    No data");
                        // }
                        // else
                        // {
                        //     builder.AppendLine(model.VisibilityLump.NumClusters, "    Cluster count");
                        //     builder.AppendLine(model.VisibilityLump.ByteOffsets, "    Byte offsets");
                        // }
                        break;
                    case LumpType.LUMP_NODES:
                        if (model.NodesLump?.Nodes == null || model.NodesLump.Nodes.Length == 0)
                        {
                            builder.AppendLine("    No data");
                        }
                        else
                        {
                            for (int j = 0; j < model.NodesLump.Nodes.Length; j++)
                            {
                                var node = model.NodesLump.Nodes[j];
                                builder.AppendLine($"    Node {j}");
                                builder.AppendLine(node.Children, "      Children");
                                builder.AppendLine(node.Mins, "      Mins");
                                builder.AppendLine(node.Maxs, "      Maxs");
                                builder.AppendLine(node.FirstFace, "      First face index");
                                builder.AppendLine(node.FaceCount, "      Count of faces");
                            }
                        }
                        break;
                    case LumpType.LUMP_TEXINFO:
                        if (model.TexinfoLump?.Texinfos == null || model.TexinfoLump.Texinfos.Length == 0)
                        {
                            builder.AppendLine("    No data");
                        }
                        else
                        {
                            for (int j = 0; j < model.TexinfoLump.Texinfos.Length; j++)
                            {
                                var texinfo = model.TexinfoLump.Texinfos[j];
                                builder.AppendLine($"    Texinfo {j}");
                                builder.AppendLine($"      S-Vector: {texinfo.SVector.X}, {texinfo.SVector.Y}, {texinfo.SVector.Z}");
                                builder.AppendLine(texinfo.TextureSShift, "      Texture shift in S direction");
                                builder.AppendLine($"      T-Vector: {texinfo.TVector.X}, {texinfo.TVector.Y}, {texinfo.TVector.Z}");
                                builder.AppendLine(texinfo.TextureTShift, "      Texture shift in T direction");
                                builder.AppendLine(texinfo.MiptexIndex, "      Miptex index");
                                builder.AppendLine($"      Flags: {texinfo.Flags} (0x{texinfo.Flags:X})");
                            }
                        }
                        break;
                    case LumpType.LUMP_FACES:
                        if (model.FacesLump?.Faces == null || model.FacesLump.Faces.Length == 0)
                        {
                            builder.AppendLine("    No data");
                        }
                        else
                        {
                            for (int j = 0; j < model.FacesLump.Faces.Length; j++)
                            {
                                var face = model.FacesLump.Faces[j];
                                builder.AppendLine($"    Face {j}");
                                builder.AppendLine(face.PlaneIndex, "      Plane index");
                                builder.AppendLine(face.PlaneSideCount, "      Plane side count");
                                builder.AppendLine(face.FirstEdgeIndex, "      First surfedge index");
                                builder.AppendLine(face.NumberOfEdges, "      Surfedge count");
                                builder.AppendLine(face.TextureInfoIndex, "      Texture info index");
                                builder.AppendLine(face.LightingStyles, "      Lighting styles");
                                builder.AppendLine(face.LightmapOffset, "      Lightmap offset");
                            }
                        }
                        break;
                    case LumpType.LUMP_LIGHTING:
                        if (model.LightmapLump?.Lightmap == null || model.LightmapLump.Lightmap.Length == 0)
                            builder.AppendLine("    No data");
                        else
                            builder.AppendLine("    Lightmap data skipped...");
                        break;
                    case LumpType.LUMP_CLIPNODES:
                        if (model.ClipnodesLump?.Clipnodes == null || model.ClipnodesLump.Clipnodes.Length == 0)
                        {
                            builder.AppendLine("    No data");
                        }
                        else
                        {
                            for (int j = 0; j < model.ClipnodesLump.Clipnodes.Length; j++)
                            {
                                var clipnode = model.ClipnodesLump.Clipnodes[j];
                                builder.AppendLine($"    Clipnode {j}");
                                builder.AppendLine(clipnode.PlaneIndex, "      Plane index");
                                builder.AppendLine(clipnode.ChildrenIndices, "      Children indices");
                            }
                        }
                        break;
                    case LumpType.LUMP_LEAVES:
                        if (model.LeavesLump?.Leaves == null || model.LeavesLump.Leaves.Length == 0)
                        {
                            builder.AppendLine("    No data");
                        }
                        else
                        {
                            for (int j = 0; j < model.LeavesLump.Leaves.Length; j++)
                            {
                                var leaf = model.LeavesLump.Leaves[j];
                                builder.AppendLine($"    Leaf {j}");
                                builder.AppendLine($"      Contents: {leaf.Contents} (0x{leaf.Contents:X})");
                                builder.AppendLine(leaf.VisOffset, "      Visibility offset");
                                builder.AppendLine(leaf.Mins, "      Mins");
                                builder.AppendLine(leaf.Maxs, "      Maxs");
                                builder.AppendLine(leaf.FirstMarkSurfaceIndex, "      First marksurface index");
                                builder.AppendLine(leaf.MarkSurfacesCount, "      Marksurfaces count");
                                builder.AppendLine(leaf.AmbientLevels, "      Ambient sound levels");
                            }
                        }
                        break;
                    case LumpType.LUMP_MARKSURFACES:
                        if (model.MarksurfacesLump?.Marksurfaces == null || model.MarksurfacesLump.Marksurfaces.Length == 0)
                        {
                            builder.AppendLine("    No data");
                        }
                        else
                        {
                            for (int j = 0; j < model.MarksurfacesLump.Marksurfaces.Length; j++)
                            {
                                var marksurface = model.MarksurfacesLump.Marksurfaces[j];
                                builder.AppendLine($"    Marksurface {j}: {marksurface} (0x{marksurface:X4})");
                            }
                        }
                        break;
                    case LumpType.LUMP_EDGES:
                        if (model.EdgesLump?.Edges == null || model.EdgesLump.Edges.Length == 0)
                        {
                            builder.AppendLine("    No data");
                        }
                        else
                        {
                            for (int j = 0; j < model.EdgesLump.Edges.Length; j++)
                            {
                                var edge = model.EdgesLump.Edges[j];
                                builder.AppendLine($"    Edge {j}");
                                builder.AppendLine(edge.VertexIndices, "      Vertex indices");
                            }
                        }
                        break;
                    case LumpType.LUMP_SURFEDGES:
                        if (model.SurfedgesLump?.Surfedges == null || model.SurfedgesLump.Surfedges.Length == 0)
                        {
                            builder.AppendLine("    No data");
                        }
                        else
                        {
                            for (int j = 0; j < model.SurfedgesLump.Surfedges.Length; j++)
                            {
                                var surfedge = model.SurfedgesLump.Surfedges[j];
                                builder.AppendLine($"    Surfedge {j}: {surfedge} (0x{surfedge:X4})");
                            }
                        }
                        break;
                    case LumpType.LUMP_MODELS:
                        if (model.ModelsLump?.Models == null || model.ModelsLump.Models.Length == 0)
                        {
                            builder.AppendLine("    No data");
                        }
                        else
                        {
                            for (int j = 0; j < model.ModelsLump.Models.Length; j++)
                            {
                                var bmodel = model.ModelsLump.Models[j];
                                builder.AppendLine($"    Model {j}");
                                builder.AppendLine($"      Mins: {bmodel.Mins.X}, {bmodel.Mins.Y}, {bmodel.Mins.Z}");
                                builder.AppendLine($"      Maxs: {bmodel.Maxs.X}, {bmodel.Maxs.Y}, {bmodel.Maxs.Z}");
                                builder.AppendLine($"      Origin vector: {bmodel.OriginVector.X}, {bmodel.OriginVector.Y}, {bmodel.OriginVector.Z}");
                                builder.AppendLine(bmodel.HeadnodesIndex, "      Headnodes index");
                                builder.AppendLine(bmodel.VisLeafsCount, "      ??? (VisLeafsCount)");
                                builder.AppendLine(bmodel.FirstFaceIndex, "      First face index");
                                builder.AppendLine(bmodel.FacesCount, "      Faces count");
                            }
                        }
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
    }
}