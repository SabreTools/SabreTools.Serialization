using System.Text;
using SabreTools.Models.BSP;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Printers
{
    public class VBSP : IPrinter<VbspFile>
    {
        /// <inheritdoc/>
        public void PrintInformation(StringBuilder builder, VbspFile model)
            => Print(builder, model);

        public static void Print(StringBuilder builder, VbspFile file)
        {
            builder.AppendLine("BSP Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine();

            Print(builder, file.Header);
            PrintLumps(builder, file);
        }

        private static void Print(StringBuilder builder, VbspHeader? header)
        {
            builder.AppendLine("  Header Information:");
            builder.AppendLine("  -------------------------");
            if (header == null)
            {
                builder.AppendLine("  No header");
                builder.AppendLine();
                return;
            }

            builder.AppendLine(header.Signature, "  Signature");
            builder.AppendLine(header.Version, "  Version");
            builder.AppendLine(header.MapRevision, "  Map revision");
            builder.AppendLine();
        }

        private static void PrintLumps(StringBuilder builder, VbspFile? model)
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
                        if (model.TexdataLump?.Texdatas == null || model.TexdataLump.Texdatas.Length == 0)
                        {
                            builder.AppendLine("    No data");
                        }
                        else
                        {
                            for (int j = 0; j < model.TexdataLump.Texdatas.Length; j++)
                            {
                                var texdata = model.TexdataLump.Texdatas[j];
                                builder.AppendLine($"    Texture {j}");
                                builder.AppendLine($"      Reflectivity: {texdata.Reflectivity.X}, {texdata.Reflectivity.Y}, {texdata.Reflectivity.Z}");
                                builder.AppendLine(texdata.NameStringTableID, "      Name string table ID");
                                builder.AppendLine(texdata.Width, "      Width");
                                builder.AppendLine(texdata.Height, "      Height");
                                builder.AppendLine(texdata.ViewWidth, "      View width");
                                builder.AppendLine(texdata.ViewHeight, "      View height");
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
                        if (model.VisibilityLump == null)
                        {
                            builder.AppendLine("    No data");
                        }
                        else
                        {
                            builder.AppendLine(model.VisibilityLump.NumClusters, "    Cluster count");
                            builder.AppendLine("    Byte offsets skipped...");
                        }
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
                                builder.AppendLine(node.Area, "      Area");
                                builder.AppendLine(node.Padding, "      Padding");
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
                                builder.AppendLine($"      Texture S-Vector: {texinfo.TextureSVector.X}, {texinfo.TextureSVector.Y}, {texinfo.TextureSVector.Z}");
                                builder.AppendLine(texinfo.TextureSShift, "      Texture shift in S direction");
                                builder.AppendLine($"      Texture T-Vector: {texinfo.TextureTVector.X}, {texinfo.TextureTVector.Y}, {texinfo.TextureTVector.Z}");
                                builder.AppendLine(texinfo.TextureTShift, "      Texture shift in T direction");
                                builder.AppendLine($"      Lightmap S-Vector: {texinfo.LightmapSVector.X}, {texinfo.LightmapSVector.Y}, {texinfo.LightmapSVector.Z}");
                                builder.AppendLine(texinfo.TextureSShift, "      Lightmap shift in S direction");
                                builder.AppendLine($"      Lightmap T-Vector: {texinfo.LightmapTVector.X}, {texinfo.LightmapTVector.Y}, {texinfo.LightmapTVector.Z}");
                                builder.AppendLine(texinfo.TextureTShift, "      Lightmap shift in T direction");
                                builder.AppendLine($"      Flags: {texinfo.Flags} (0x{texinfo.Flags:X})");
                                builder.AppendLine(texinfo.TexData, "      Pointer to texdata");
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
                                builder.AppendLine(face.PlaneNum, "      Plane number");
                                builder.AppendLine(face.Side, "      Side");
                                builder.AppendLine(face.OnNode, "      On node");
                                builder.AppendLine(face.FirstEdgeIndex, "      First surfedge index");
                                builder.AppendLine(face.NumberOfEdges, "      Surfedge count");
                                builder.AppendLine(face.TextureInfoIndex, "      Texture info index");
                                builder.AppendLine(face.DisplacementInfoIndex, "      Displacement info index");
                                builder.AppendLine(face.SurfaceFogVolumeID, "      Surface fog volume ID");
                                builder.AppendLine(face.Styles, "      Styles");
                                builder.AppendLine(face.LightmapOffset, "      Lightmap offset");
                                builder.AppendLine(face.Area, "      Area");
                                builder.AppendLine(face.LightmapTextureMinsInLuxels, "      Lightmap texture mins in Luxels");
                                builder.AppendLine(face.LightmapTextureSizeInLuxels, "      Lightmap texture size in Luxels");
                                builder.AppendLine(face.OrigFace, "      Original face index");
                                builder.AppendLine(face.PrimitiveCount, "      Primitive count");
                                builder.AppendLine(face.FirstPrimitiveID, "      First primitive ID");
                                builder.AppendLine(face.SmoothingGroups, "      Smoothing groups");
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
                        if (model.OcclusionLump == null)
                        {
                            builder.AppendLine("    No data");
                        }
                        else
                        {
                            builder.AppendLine(model.OcclusionLump.Count, "    Count");
                            if (model.OcclusionLump.Data == null || model.OcclusionLump.Data.Length == 0)
                            {
                                builder.AppendLine("    No occluder data");
                            }
                            else
                            {
                                for (int j = 0; j < model.OcclusionLump.Data.Length; j++)
                                {
                                    var data = model.OcclusionLump.Data[j];
                                    builder.AppendLine($"    Occluder Data {j}");
                                    builder.AppendLine(data.Flags, "      Flags");
                                    builder.AppendLine(data.FirstPoly, "      First poly");
                                    builder.AppendLine(data.PolyCount, "      Poly count");
                                    builder.AppendLine($"      Mins: {data.Mins.X}, {data.Mins.Y}, {data.Mins.Z}");
                                    builder.AppendLine($"      Maxs: {data.Maxs.X}, {data.Maxs.Y}, {data.Maxs.Z}");
                                    builder.AppendLine(data.Area, "      Area");
                                }
                            }
                            builder.AppendLine(model.OcclusionLump.PolyDataCount, "    Polydata count");
                            if (model.OcclusionLump.PolyData == null || model.OcclusionLump.PolyData.Length == 0)
                            {
                                builder.AppendLine("    No occluder polydata");
                            }
                            else
                            {
                                for (int j = 0; j < model.OcclusionLump.PolyData.Length; j++)
                                {
                                    var polydata = model.OcclusionLump.PolyData[j];
                                    builder.AppendLine($"    Occluder Polydata {j}");
                                    builder.AppendLine(polydata.FirstVertexIndex, "      First vertex index");
                                    builder.AppendLine(polydata.VertexCount, "      Vertex count");
                                    builder.AppendLine(polydata.PlanEnum, "      Plan enum");
                                }
                            }
                            builder.AppendLine(model.OcclusionLump.VertexIndexCount, "    Vertex index count");
                            if (model.OcclusionLump.VertexIndices == null || model.OcclusionLump.VertexIndices.Length == 0)
                            {
                                builder.AppendLine("    No vertex indicies");
                            }
                            else
                            {
                                for (int j = 0; j < model.OcclusionLump.VertexIndices.Length; j++)
                                {
                                    builder.AppendLine($"    Vertex Index {j}: {model.OcclusionLump.VertexIndices[j]}");
                                }
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
                                builder.AppendLine(leaf.Cluster, "      Cluster");
                                builder.AppendLine(leaf.AreaFlags, "      AreaFlags");
                                builder.AppendLine(leaf.Mins, "      Mins");
                                builder.AppendLine(leaf.Maxs, "      Maxs");
                                builder.AppendLine(leaf.FirstLeafFace, "      First leaf face");
                                builder.AppendLine(leaf.NumLeafFaces, "      Leaf faces count");
                                builder.AppendLine(leaf.FirstLeafBrush, "      First leaf brush");
                                builder.AppendLine(leaf.NumLeafBrushes, "      Leaf brushes count");
                                builder.AppendLine(leaf.LeafWaterDataID, "      Leaf water data ID");
                                if (lump.Version == 0)
                                {
                                    // TODO: Figure out how to print the colors array
                                }
                                else
                                {
                                    builder.AppendLine(leaf.Padding, "      Padding");
                                }
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
                                builder.AppendLine(bmodel.HeadNode, "      Headnode index");
                                builder.AppendLine(bmodel.FirstFaceIndex, "      First face index");
                                builder.AppendLine(bmodel.FacesCount, "      Faces count");
                            }
                        }
                        break;
                    
                    // TODO: Implement remaining printed lump types
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
                LumpType.LUMP_TEXTURES => " - LUMP_TEXDATA",
                LumpType.LUMP_VERTICES => " - LUMP_VERTEXES",
                LumpType.LUMP_VISIBILITY => " - LUMP_VISIBILITY",
                LumpType.LUMP_NODES => " - LUMP_NODES",
                LumpType.LUMP_TEXINFO => " - LUMP_TEXINFO",
                LumpType.LUMP_FACES => " - LUMP_FACES",
                LumpType.LUMP_LIGHTING => " - LUMP_LIGHTING",
                LumpType.LUMP_CLIPNODES => " - LUMP_OCCLUSION",
                LumpType.LUMP_LEAVES => " - LUMP_LEAVES",
                LumpType.LUMP_MARKSURFACES => " - LUMP_FACEIDS",
                LumpType.LUMP_EDGES => " - LUMP_EDGES",
                LumpType.LUMP_SURFEDGES => " - LUMP_SURFEDGES",
                LumpType.LUMP_MODELS => " - LUMP_MODELS",
                LumpType.LUMP_WORLDLIGHTS => " - LUMP_WORLDLIGHTS",
                LumpType.LUMP_LEAFFACES => " - LUMP_LEAFFACES",
                LumpType.LUMP_LEAFBRUSHES => " - LUMP_LEAFBRUSHES",
                LumpType.LUMP_BRUSHES => " - LUMP_BRUSHES",
                LumpType.LUMP_BRUSHSIDES => " - LUMP_BRUSHSIDES",
                LumpType.LUMP_AREAS => " - LUMP_AREAS",
                LumpType.LUMP_AREAPORTALS => " - LUMP_AREAPORTALS",
                LumpType.LUMP_PORTALS => " - LUMP_PORTALS / LUMP_UNUSED0 / LUMP_PROPCOLLISION",
                LumpType.LUMP_CLUSTERS => " - LUMP_CLUSTERS / LUMP_UNUSED1 / LUMP_PROPHULLS",
                LumpType.LUMP_PORTALVERTS => " - LUMP_PORTALVERTS / LUMP_UNUSED2 / LUMP_FAKEENTITIES / LUMP_PROPHULLVERTS",
                LumpType.LUMP_CLUSTERPORTALS => " - LUMP_CLUSTERPORTALS / LUMP_UNUSED3 / LUMP_PROPTRIS",
                LumpType.LUMP_DISPINFO => " - LUMP_DISPINFO",
                LumpType.LUMP_ORIGINALFACES => " - LUMP_ORIGINALFACES",
                LumpType.LUMP_PHYSDISP => " - LUMP_PHYSDISP",
                LumpType.LUMP_PHYSCOLLIDE => " - LUMP_PHYSCOLLIDE",
                LumpType.LUMP_VERTNORMALS => " - LUMP_VERTNORMALS",
                LumpType.LUMP_VERTNORMALINDICES => " - LUMP_VERTNORMALINDICES",
                LumpType.LUMP_DISP_LIGHTMAP_ALPHAS => " - LUMP_DISP_LIGHTMAP_ALPHAS",
                LumpType.LUMP_DISP_VERTS => " - LUMP_DISP_VERTS",
                LumpType.LUMP_DISP_LIGHTMAP_SAMPLE_POSITIONS => " - LUMP_DISP_LIGHTMAP_SAMPLE_POSITIONS",
                LumpType.LUMP_GAME_LUMP => " - LUMP_GAME_LUMP",
                LumpType.LUMP_LEAFWATERDATA => " - LUMP_LEAFWATERDATA",
                LumpType.LUMP_PRIMITIVES => " - LUMP_PRIMITIVES",
                LumpType.LUMP_PRIMVERTS => " - LUMP_PRIMVERTS",
                LumpType.LUMP_PRIMINDICES => " - LUMP_PRIMINDICES",
                LumpType.LUMP_PAKFILE => " - LUMP_PAKFILE",
                LumpType.LUMP_CLIPPORTALVERTS => " - LUMP_CLIPPORTALVERTS",
                LumpType.LUMP_CUBEMAPS => " - LUMP_CUBEMAPS",
                LumpType.LUMP_TEXDATA_STRING_DATA => " - LUMP_TEXDATA_STRING_DATA",
                LumpType.LUMP_TEXDATA_STRING_TABLE => " - LUMP_TEXDATA_STRING_TABLE",
                LumpType.LUMP_OVERLAYS => " - LUMP_OVERLAYS",
                LumpType.LUMP_LEAFMINDISTTOWATER => " - LUMP_LEAFMINDISTTOWATER",
                LumpType.LUMP_FACE_MACRO_TEXTURE_INFO => " - LUMP_FACE_MACRO_TEXTURE_INFO",
                LumpType.LUMP_DISP_TRIS => " - LUMP_DISP_TRIS",
                LumpType.LUMP_PHYSCOLLIDESURFACE => " - LUMP_PHYSCOLLIDESURFACE / LUMP_PROP_BLOB",
                LumpType.LUMP_WATEROVERLAYS => " - LUMP_WATEROVERLAYS",
                LumpType.LUMP_LIGHTMAPPAGES => " - LUMP_LIGHTMAPPAGES / LUMP_LEAF_AMBIENT_INDEX_HDR",
                LumpType.LUMP_LIGHTMAPPAGEINFOS => " - LUMP_LIGHTMAPPAGEINFOS / LUMP_LEAF_AMBIENT_INDEX",
                LumpType.LUMP_LIGHTING_HDR => " - LUMP_LIGHTING_HDR",
                LumpType.LUMP_WORLDLIGHTS_HDR => " - LUMP_WORLDLIGHTS_HDR",
                LumpType.LUMP_LEAF_AMBIENT_LIGHTING_HDR => " - LUMP_LEAF_AMBIENT_LIGHTING_HDR",
                LumpType.LUMP_LEAF_AMBIENT_LIGHTING => " - LUMP_LEAF_AMBIENT_LIGHTING",
                LumpType.LUMP_XZIPPAKFILE => " - LUMP_XZIPPAKFILE",
                LumpType.LUMP_FACES_HDR => " - LUMP_FACES_HDR",
                LumpType.LUMP_MAP_FLAGS => " - LUMP_MAP_FLAGS",
                LumpType.LUMP_OVERLAY_FADES => " - LUMP_OVERLAY_FADES",
                LumpType.LUMP_OVERLAY_SYSTEM_LEVELS => " - LUMP_OVERLAY_SYSTEM_LEVELS",
                LumpType.LUMP_PHYSLEVEL => " - LUMP_PHYSLEVEL",
                LumpType.LUMP_DISP_MULTIBLEND => " - LUMP_DISP_MULTIBLEND",
                _ => string.Empty,
            };
        }
    }
}