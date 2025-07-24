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
                        Print(builder, model.TexdataLump);
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
                        Print(builder, model.OcclusionLump);
                        break;
                    case LumpType.LUMP_LEAVES:
                        Print(builder, model.LeavesLump, lump.Version);
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
                    case LumpType.LUMP_WORLDLIGHTS:
                        Print(builder, model.LDRWorldLightsLump);
                        break;
                    case LumpType.LUMP_LEAFFACES:
                        Print(builder, model.LeafFacesLump);
                        break;
                    case LumpType.LUMP_LEAFBRUSHES:
                        Print(builder, model.LeafBrushesLump);
                        break;
                    case LumpType.LUMP_BRUSHES:
                        Print(builder, model.BrushesLump);
                        break;
                    case LumpType.LUMP_BRUSHSIDES:
                        Print(builder, model.BrushsidesLump);
                        break;
                    case LumpType.LUMP_AREAS:
                        // TODO: Support LUMP_AREAS [20] when in Models
                        builder.AppendLine("    Data not parsed...");
                        break;
                    case LumpType.LUMP_AREAPORTALS:
                        // TODO: Support LUMP_AREAPORTALS [21] when in Models
                        builder.AppendLine("    Data not parsed...");
                        break;
                    case LumpType.LUMP_PORTALS:
                        // TODO: Support LUMP_PORTALS / LUMP_UNUSED0 / LUMP_PROPCOLLISION [22] when in Models
                        builder.AppendLine("    Data not parsed...");
                        break;
                    case LumpType.LUMP_CLUSTERS:
                        // TODO: Support LUMP_CLUSTERS / LUMP_UNUSED1 / LUMP_PROPHULLS [23] when in Models
                        builder.AppendLine("    Data not parsed...");
                        break;
                    case LumpType.LUMP_PORTALVERTS:
                        // TODO: Support LUMP_PORTALVERTS / LUMP_UNUSED2 / LUMP_FAKEENTITIES / LUMP_PROPHULLVERTS [24] when in Models
                        builder.AppendLine("    Data not parsed...");
                        break;
                    case LumpType.LUMP_CLUSTERPORTALS:
                        // TODO: Support LUMP_CLUSTERPORTALS / LUMP_UNUSED3 / LUMP_PROPTRIS [25] when in Models
                        builder.AppendLine("    Data not parsed...");
                        break;
                    case LumpType.LUMP_DISPINFO:
                        Print(builder, model.DispInfosLump);
                        break;
                    case LumpType.LUMP_ORIGINALFACES:
                        Print(builder, model.OriginalFacesLump);
                        break;
                    case LumpType.LUMP_PHYSDISP:
                        // TODO: Support LUMP_PHYSDISP [28] when in Models
                        builder.AppendLine("    Data not parsed...");
                        break;
                    case LumpType.LUMP_PHYSCOLLIDE:
                        Print(builder, model.PhysCollideLump);
                        break;
                    case LumpType.LUMP_VERTNORMALS:
                        // TODO: Support LUMP_VERTNORMALS [30] when in Models
                        builder.AppendLine("    Data not parsed...");
                        break;
                    case LumpType.LUMP_VERTNORMALINDICES:
                        // TODO: Support LUMP_VERTNORMALINDICES [31] when in Models
                        builder.AppendLine("    Data not parsed...");
                        break;
                    case LumpType.LUMP_DISP_LIGHTMAP_ALPHAS:
                        // TODO: Support LUMP_DISP_LIGHTMAP_ALPHAS [32] when in Models
                        builder.AppendLine("    Data not parsed...");
                        break;
                    case LumpType.LUMP_DISP_VERTS:
                        Print(builder, model.DispVertsLump);
                        break;
                    case LumpType.LUMP_DISP_LIGHTMAP_SAMPLE_POSITIONS:
                        // TODO: Support LUMP_DISP_LIGHTMAP_SAMPLE_POSITIONS [34] when in Models
                        builder.AppendLine("    Data not parsed...");
                        break;
                    case LumpType.LUMP_GAME_LUMP:
                        Print(builder, model.GameLump);
                        break;
                    case LumpType.LUMP_LEAFWATERDATA:
                        // TODO: Support LUMP_LEAFWATERDATA [36] when in Models
                        builder.AppendLine("    Data not parsed...");
                        break;
                    case LumpType.LUMP_PRIMITIVES:
                        // TODO: Support LUMP_PRIMITIVES [37] when in Models
                        builder.AppendLine("    Data not parsed...");
                        break;
                    case LumpType.LUMP_PRIMVERTS:
                        // TODO: Support LUMP_PRIMVERTS [38] when in Models
                        builder.AppendLine("    Data not parsed...");
                        break;
                    case LumpType.LUMP_PRIMINDICES:
                        // TODO: Support LUMP_PRIMINDICES [39] when in Models
                        builder.AppendLine("    Data not parsed...");
                        break;
                    case LumpType.LUMP_PAKFILE:
                        Print(builder, model.PakfileLump);
                        break;
                    case LumpType.LUMP_CLIPPORTALVERTS:
                        // TODO: Support LUMP_CLIPPORTALVERTS [41] when in Models
                        builder.AppendLine("    Data not parsed...");
                        break;
                    case LumpType.LUMP_CUBEMAPS:
                        Print(builder, model.CubemapsLump);
                        break;
                    case LumpType.LUMP_TEXDATA_STRING_DATA:
                        Print(builder, model.TexdataStringData);
                        break;
                    case LumpType.LUMP_TEXDATA_STRING_TABLE:
                        Print(builder, model.TexdataStringTable);
                        break;
                    case LumpType.LUMP_OVERLAYS:
                        Print(builder, model.OverlaysLump);
                        break;
                    case LumpType.LUMP_LEAFMINDISTTOWATER:
                        // TODO: Support LUMP_LEAFMINDISTTOWATER [46] when in Models
                        builder.AppendLine("    Data not parsed...");
                        break;
                    case LumpType.LUMP_FACE_MACRO_TEXTURE_INFO:
                        // TODO: Support LUMP_FACE_MACRO_TEXTURE_INFO [47] when in Models
                        builder.AppendLine("    Data not parsed...");
                        break;
                    case LumpType.LUMP_DISP_TRIS:
                        Print(builder, model.DispTrisLump);
                        break;
                    case LumpType.LUMP_PHYSCOLLIDESURFACE:
                        // TODO: Support LUMP_PHYSCOLLIDESURFACE / LUMP_PROP_BLOB [49] when in Models
                        builder.AppendLine("    Data not parsed...");
                        break;
                    case LumpType.LUMP_WATEROVERLAYS:
                        // TODO: Support LUMP_WATEROVERLAYS [50] when in Models
                        builder.AppendLine("    Data not parsed...");
                        break;
                    case LumpType.LUMP_LIGHTMAPPAGES:
                        Print(builder, model.HDRAmbientIndexLump);
                        break;
                    case LumpType.LUMP_LIGHTMAPPAGEINFOS:
                        Print(builder, model.LDRAmbientIndexLump);
                        break;
                    case LumpType.LUMP_LIGHTING_HDR:
                        // TODO: Support LUMP_LIGHTING_HDR [53] when in Models
                        builder.AppendLine("    Data not parsed...");
                        break;
                    case LumpType.LUMP_WORLDLIGHTS_HDR:
                        Print(builder, model.HDRWorldLightsLump);
                        break;
                    case LumpType.LUMP_LEAF_AMBIENT_LIGHTING_HDR:
                        Print(builder, model.HDRAmbientLightingLump);
                        break;
                    case LumpType.LUMP_LEAF_AMBIENT_LIGHTING:
                        Print(builder, model.LDRAmbientLightingLump);
                        break;
                    case LumpType.LUMP_XZIPPAKFILE:
                        // TODO: Support LUMP_XZIPPAKFILE [57] when in Models
                        builder.AppendLine("    Data not parsed...");
                        break;
                    case LumpType.LUMP_FACES_HDR:
                        // TODO: Support LUMP_FACES_HDR [58] when in Models
                        builder.AppendLine("    Data not parsed...");
                        break;
                    case LumpType.LUMP_MAP_FLAGS:
                        // TODO: Support LUMP_MAP_FLAGS [59] when in Models
                        builder.AppendLine("    Data not parsed...");
                        break;
                    case LumpType.LUMP_OVERLAY_FADES:
                        // TODO: Support LUMP_OVERLAY_FADES [60] when in Models
                        builder.AppendLine("    Data not parsed...");
                        break;
                    case LumpType.LUMP_OVERLAY_SYSTEM_LEVELS:
                        // TODO: Support LUMP_OVERLAY_SYSTEM_LEVELS [61] when in Models
                        builder.AppendLine("    Data not parsed...");
                        break;
                    case LumpType.LUMP_PHYSLEVEL:
                        // TODO: Support LUMP_PHYSLEVEL [62] when in Models
                        builder.AppendLine("    Data not parsed...");
                        break;
                    case LumpType.LUMP_DISP_MULTIBLEND:
                        // TODO: Support LUMP_DISP_MULTIBLEND [63] when in Models
                        builder.AppendLine("    Data not parsed...");
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

        private static void Print(StringBuilder builder, EntitiesLump? lump)
        {
            if (lump?.Entities == null || lump.Entities.Length == 0)
            {
                builder.AppendLine("    No data");
                return;
            }

            for (int i = 0; i < lump.Entities.Length; i++)
            {
                // TODO: Implement entity printing
                var entity = lump.Entities[i];

                builder.AppendLine($"    Entity {i}: Not printed yet");
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

        private static void Print(StringBuilder builder, TexdataLump? lump)
        {
            if (lump?.Texdatas == null || lump.Texdatas.Length == 0)
            {
                builder.AppendLine("    No data");
                return;
            }

            for (int i = 0; i < lump.Texdatas.Length; i++)
            {
                var texdata = lump.Texdatas[i];

                builder.AppendLine($"    Texture {i}");
                builder.AppendLine($"      Reflectivity: ({texdata.Reflectivity?.X}, {texdata.Reflectivity?.Y}, {texdata.Reflectivity?.Z})");
                builder.AppendLine(texdata.NameStringTableID, "      Name string table ID");
                builder.AppendLine(texdata.Width, "      Width");
                builder.AppendLine(texdata.Height, "      Height");
                builder.AppendLine(texdata.ViewWidth, "      View width");
                builder.AppendLine(texdata.ViewHeight, "      View height");
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

        private static void Print(StringBuilder builder, VbspNodesLump? lump)
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
                builder.AppendLine(node.Area, "      Area");
                builder.AppendLine(node.Padding, "      Padding");
            }
        }

        private static void Print(StringBuilder builder, VbspTexinfoLump? lump)
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
                builder.AppendLine($"      Texture S-Vector: ({texinfo.TextureSVector?.X}, {texinfo.TextureSVector?.Y}, {texinfo.TextureSVector?.Z})");
                builder.AppendLine(texinfo.TextureSShift, "      Texture shift in S direction");
                builder.AppendLine($"      Texture T-Vector: ({texinfo.TextureTVector?.X}, {texinfo.TextureTVector?.Y}, {texinfo.TextureTVector?.Z})");
                builder.AppendLine(texinfo.TextureTShift, "      Texture shift in T direction");
                builder.AppendLine($"      Lightmap S-Vector: ({texinfo.LightmapSVector?.X}, {texinfo.LightmapSVector?.Y}, {texinfo.LightmapSVector?.Z})");
                builder.AppendLine(texinfo.TextureSShift, "      Lightmap shift in S direction");
                builder.AppendLine($"      Lightmap T-Vector: ({texinfo.LightmapTVector?.X}, {texinfo.LightmapTVector?.Y}, {texinfo.LightmapTVector?.Z})");
                builder.AppendLine(texinfo.TextureTShift, "      Lightmap shift in T direction");
                builder.AppendLine($"      Flags: {texinfo.Flags} (0x{texinfo.Flags:X})");
                builder.AppendLine(texinfo.TexData, "      Pointer to texdata");
            }
        }

        private static void Print(StringBuilder builder, VbspFacesLump? lump)
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

        private static void Print(StringBuilder builder, PhysCollideLump? lump)
        {
            if (lump?.Models == null || lump.Models.Length == 0)
            {
                builder.AppendLine("    No data");
                return;
            }

            for (int i = 0; i < lump.Models.Length; i++)
            {
                var model = lump.Models[i];
                builder.AppendLine($"    Model {i}");
                builder.AppendLine(model.ModelIndex, "      Model index");
                builder.AppendLine(model.DataSize, "      Data size");
                builder.AppendLine(model.KeydataSize, "      Keydata size");
                builder.AppendLine(model.SolidCount, "      Solid count");
                if (model.Solids == null || model.Solids.Length == 0)
                {
                    builder.AppendLine("      No solids");
                }
                else
                {
                    for (int j = 0; j < model.Solids.Length; j++)
                    {
                        var solid = model.Solids[j];
                        builder.AppendLine($"      Solid {j}");
                        builder.AppendLine(solid.Size, "        Size");
                        builder.AppendLine("        Collision data skipped...");
                    }
                }

                builder.AppendLine("      Keydata skipped...");
            }
        }

        private static void Print(StringBuilder builder, LightmapLump? lump)
        {
            if (lump?.Lightmap == null || lump.Lightmap.Length == 0)
                builder.AppendLine("    No data");
            else
                builder.AppendLine("    Lightmap data skipped...");
        }

        private static void Print(StringBuilder builder, OcclusionLump? lump)
        {
            if (lump == null)
            {
                builder.AppendLine("    No data");
                return;
            }

            builder.AppendLine(lump.Count, "    Count");
            if (lump.Data == null || lump.Data.Length == 0)
            {
                builder.AppendLine("    No occluder data");
            }
            else
            {
                for (int i = 0; i < lump.Data.Length; i++)
                {
                    var data = lump.Data[i];

                    builder.AppendLine($"    Occluder Data {i}");
                    builder.AppendLine(data.Flags, "      Flags");
                    builder.AppendLine(data.FirstPoly, "      First poly");
                    builder.AppendLine(data.PolyCount, "      Poly count");
                    builder.AppendLine($"      Mins: {data.Mins?.X}, {data.Mins?.Y}, {data.Mins?.Z}");
                    builder.AppendLine($"      Maxs: {data.Maxs?.X}, {data.Maxs?.Y}, {data.Maxs?.Z}");
                    builder.AppendLine(data.Area, "      Area");
                }
            }

            builder.AppendLine(lump.PolyDataCount, "    Polydata count");
            if (lump.PolyData == null || lump.PolyData.Length == 0)
            {
                builder.AppendLine("    No occluder polydata");
            }
            else
            {
                for (int i = 0; i < lump.PolyData.Length; i++)
                {
                    var polydata = lump.PolyData[i];

                    builder.AppendLine($"    Occluder Polydata {i}");
                    builder.AppendLine(polydata.FirstVertexIndex, "      First vertex index");
                    builder.AppendLine(polydata.VertexCount, "      Vertex count");
                    builder.AppendLine(polydata.PlanEnum, "      Plan enum");
                }
            }

            builder.AppendLine(lump.VertexIndexCount, "    Vertex index count");
            if (lump.VertexIndicies == null || lump.VertexIndicies.Length == 0)
            {
                builder.AppendLine("    No vertex indicies");
            }
            else
            {
                for (int i = 0; i < lump.VertexIndicies.Length; i++)
                {
                    builder.AppendLine($"    Vertex Index {i}: {lump.VertexIndicies[i]}");
                }
            }
        }

        private static void Print(StringBuilder builder, VbspLeavesLump? lump, uint version)
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
                builder.AppendLine(leaf.Cluster, "      Cluster");
                builder.AppendLine(leaf.AreaFlags, "      AreaFlags");
                builder.AppendLine(leaf.Mins, "      Mins");
                builder.AppendLine(leaf.Maxs, "      Maxs");
                builder.AppendLine(leaf.FirstLeafFace, "      First leaf face");
                builder.AppendLine(leaf.NumLeafFaces, "      Leaf faces count");
                builder.AppendLine(leaf.FirstLeafBrush, "      First leaf brush");
                builder.AppendLine(leaf.NumLeafBrushes, "      Leaf brushes count");
                builder.AppendLine(leaf.LeafWaterDataID, "      Leaf water data ID");
                if (version == 0)
                {
                    if (leaf.AmbientLighting?.Colors == null || leaf.AmbientLighting.Colors.Length == 0)
                    {
                        builder.AppendLine("      No ambient lighting colors");
                    }
                    else
                    {
                        for (int j = 0; j < leaf.AmbientLighting.Colors.Length; j++)
                        {
                            var color = leaf.AmbientLighting.Colors[j];
                            builder.AppendLine($"      Ambient Lighting Color {j}");
                            builder.AppendLine(color.Red, "        Red");
                            builder.AppendLine(color.Green, "        Green");
                            builder.AppendLine(color.Blue, "        Blue");
                            builder.AppendLine(color.Exponent, "        Exponent");
                        }
                    }
                }
                else
                {
                    builder.AppendLine(leaf.Padding, "      Padding");
                }
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

        private static void Print(StringBuilder builder, VbspModelsLump? lump)
        {
            if (lump?.Models == null || lump.Models.Length == 0)
            {
                builder.AppendLine("    No data");
                return;
            }

            for (int i = 0; i < lump.Models.Length; i++)
            {
                var model = lump.Models[i];

                builder.AppendLine($"    Model {i}");
                builder.AppendLine($"      Mins: ({model.Mins?.X}, {model.Mins?.Y}, {model.Mins?.Z})");
                builder.AppendLine($"      Maxs: ({model.Maxs?.X}, {model.Maxs?.Y}, {model.Maxs?.Z})");
                builder.AppendLine($"      Origin vector: ({model.OriginVector?.X}, {model.OriginVector?.Y}, {model.OriginVector?.Z})");
                builder.AppendLine(model.HeadNode, "      Headnode index");
                builder.AppendLine(model.FirstFaceIndex, "      First face index");
                builder.AppendLine(model.FacesCount, "      Faces count");
            }
        }

        private static void Print(StringBuilder builder, WorldLightsLump? lump)
        {
            if (lump?.WorldLights == null || lump.WorldLights.Length == 0)
            {
                builder.AppendLine("    No data");
                return;
            }

            for (int j = 0; j < lump.WorldLights.Length; j++)
            {
                var worldlight = lump.WorldLights[j];

                builder.AppendLine($"    World Light {j}");
                builder.AppendLine($"      Origin: ({worldlight.Origin?.X}, {worldlight.Origin?.Y}, {worldlight.Origin?.Z})");
                builder.AppendLine($"      Intensity: ({worldlight.Intensity?.X}, {worldlight.Intensity?.Y}, {worldlight.Intensity?.Z})");
                builder.AppendLine($"      Normal: ({worldlight.Normal?.X}, {worldlight.Normal?.Y}, {worldlight.Normal?.Z})");
                builder.AppendLine(worldlight.Cluster, "      Cluster");
                builder.AppendLine($"      Emit type: {worldlight.EmitType} (0x{worldlight.EmitType:X})");
                builder.AppendLine(worldlight.Style, "      Style");
                builder.AppendLine(worldlight.StopDot, "      Start of penumbra");
                builder.AppendLine(worldlight.StopDot2, "      End of penumbra");
                builder.AppendLine(worldlight.Exponent, "      Exponent");
                builder.AppendLine(worldlight.Radius, "      Radius");
                builder.AppendLine(worldlight.ConstantAttn, "      Constant attn.");
                builder.AppendLine(worldlight.LinearAttn, "      Linear attn.");
                builder.AppendLine(worldlight.QuadraticAttn, "      Quadratic attn.");
                builder.AppendLine(worldlight.Flags, "      Flags");
                builder.AppendLine(worldlight.Texinfo, "      Texinfo");
                builder.AppendLine(worldlight.Owner, "      Owner");
            }
        }

        private static void Print(StringBuilder builder, LeafFacesLump? lump)
        {
            if (lump?.Map == null || lump.Map.Length == 0)
            {
                builder.AppendLine("    No data");
                return;
            }

            for (int i = 0; i < lump.Map.Length; i++)
            {
                var entry = lump.Map[i];
                builder.AppendLine($"    Map entry {i}: {entry}");
            }
        }

        private static void Print(StringBuilder builder, LeafBrushesLump? lump)
        {
            if (lump?.Map == null || lump.Map.Length == 0)
            {
                builder.AppendLine("    No data");
                return;
            }

            for (int i = 0; i < lump.Map.Length; i++)
            {
                var entry = lump.Map[i];
                builder.AppendLine($"    Map entry {i}: {entry}");
            }
        }

        private static void Print(StringBuilder builder, BrushesLump? lump)
        {
            if (lump?.Brushes == null || lump.Brushes.Length == 0)
            {
                builder.AppendLine("    No data");
                return;
            }

            for (int i = 0; i < lump.Brushes.Length; i++)
            {
                var brush = lump.Brushes[i];

                builder.AppendLine($"    Brush {i}");
                builder.AppendLine(brush.FirstSide, "      First brushside");
                builder.AppendLine(brush.NumSides, "      Number of brushsides");
                builder.AppendLine($"      Contents: {brush.Contents} (0x{brush.Contents:X})");
            }
        }

        private static void Print(StringBuilder builder, BrushsidesLump? lump)
        {
            if (lump?.Brushsides == null || lump.Brushsides.Length == 0)
            {
                builder.AppendLine("    No data");
                return;
            }

            for (int i = 0; i < lump.Brushsides.Length; i++)
            {
                var brushside = lump.Brushsides[i];

                builder.AppendLine($"    Brushside {i}");
                builder.AppendLine(brushside.PlaneNum, "      Plane number");
                builder.AppendLine(brushside.TextureInfo, "      Texture info");
                builder.AppendLine(brushside.DisplacementInfo, "      Displacement info");
                builder.AppendLine(brushside.Bevel, "      Bevel");
            }
        }

        private static void Print(StringBuilder builder, DispInfosLump? lump)
        {
            if (lump?.Infos == null || lump.Infos.Length == 0)
            {
                builder.AppendLine("    No data");
                return;
            }

            for (int i = 0; i < lump.Infos.Length; i++)
            {
                var info = lump.Infos[i];

                builder.AppendLine($"    Disp Info {i}");
                builder.AppendLine($"      Start position: ({info.StartPosition?.X}, {info.StartPosition?.Y}, {info.StartPosition?.Z})");
                builder.AppendLine(info.DispVertStart, "      Index into disp verts");
                builder.AppendLine(info.DispTriStart, "      Index into disp tris");
                builder.AppendLine(info.Power, "      Power");
                builder.AppendLine(info.MinTess, "      Minimum tesselation");
                builder.AppendLine(info.SmoothingAngle, "      Smoothing angle");
                builder.AppendLine(info.Contents, "      Contents");
                builder.AppendLine(info.MapFace, "      Map face");
                builder.AppendLine(info.LightmapAlphaStart, "      Lightmap alpha start");
                builder.AppendLine(info.LightmapSamplePositionStart, "      Lightmap sample position start");
                builder.AppendLine($"      Edge Neighbors:");
                if (info.EdgeNeighbors == null || info.EdgeNeighbors.Length == 0)
                {
                    builder.AppendLine("      No edge neighbors");
                }
                else
                {
                    for (int j = 0; j < info.EdgeNeighbors.Length; j++)
                    {
                        var edgeNeighbor = info.EdgeNeighbors[j];
                        builder.AppendLine($"        Edge Neighbor {j}");
                        if (edgeNeighbor.SubNeighbors == null || edgeNeighbor.SubNeighbors.Length == 0)
                        {
                            builder.AppendLine("          No subneighbors");
                        }
                        else
                        {
                            for (int k = 0; k < edgeNeighbor.SubNeighbors.Length; k++)
                            {
                                var subNeighbor = edgeNeighbor.SubNeighbors[k];
                                builder.AppendLine($"          Subneighbor {k}");
                                builder.AppendLine(subNeighbor.NeighborIndex, "            Neighbor index");
                                builder.AppendLine(subNeighbor.NeighborOrientation, "            Neighbor orientation");
                                builder.AppendLine(subNeighbor.Span, "            Span");
                                builder.AppendLine(subNeighbor.NeighborSpan, "            Neighbor span");
                            }
                        }
                    }
                }

                builder.AppendLine($"      Corner Neighbors:");
                if (info.CornerNeighbors == null || info.CornerNeighbors.Length == 0)
                {
                    builder.AppendLine("      No corner neighbors");
                }
                else
                {
                    for (int j = 0; j < info.CornerNeighbors.Length; j++)
                    {
                        var cornerNeighbor = info.CornerNeighbors[j];
                        builder.AppendLine($"        Corner Neighbor {j}");
                        builder.AppendLine(cornerNeighbor.Neighbors, "          Neighbors");
                        builder.AppendLine(cornerNeighbor.NeighborCount, "          Neighbor count");
                    }
                }

                builder.AppendLine(info.AllowedVerts, "      Allowed verts");
            }
        }

        private static void Print(StringBuilder builder, DispVertsLump? lump)
        {
            if (lump?.Verts == null || lump.Verts.Length == 0)
            {
                builder.AppendLine("    No data");
                return;
            }

            for (int i = 0; i < lump.Verts.Length; i++)
            {
                var vert = lump.Verts[i];

                builder.AppendLine($"    Disp Vert {i}");
                builder.AppendLine($"      Vec: ({vert.Vec?.X}, {vert.Vec?.Y}, {vert.Vec?.Z})");
                builder.AppendLine(vert.Dist, "      Dist");
                builder.AppendLine(vert.Alpha, "      Alpha");
            }
        }

        private static void Print(StringBuilder builder, GameLump? lump)
        {
            if (lump == null)
            {
                builder.AppendLine("    No data");
                return;
            }

            builder.AppendLine(lump.LumpCount, "    Lump count");
            if (lump.Directories == null || lump.Directories.Length == 0)
            {
                builder.AppendLine("    No directories");
                return;
            }

            for (int i = 0; i < lump.Directories.Length; i++)
            {
                var dir = lump.Directories[i];

                builder.AppendLine($"    Game Lump Directory {i}");
                builder.AppendLine(dir.Id, "      Id");
                builder.AppendLine(dir.Flags, "      Flags");
                builder.AppendLine(dir.Version, "      Version");
                builder.AppendLine(dir.FileOffset, "      File offset");
                builder.AppendLine(dir.FileLength, "      File length");
            }
        }

        private static void Print(StringBuilder builder, PakfileLump? lump)
        {
            if (lump?.Data == null || lump.Data.Length == 0)
            {
                builder.AppendLine("    No data");
                return;
            }

            builder.AppendLine("    Data skipped...");
        }

        private static void Print(StringBuilder builder, CubemapsLump? lump)
        {
            if (lump?.Cubemaps == null || lump.Cubemaps.Length == 0)
            {
                builder.AppendLine("    No data");
                return;
            }

            for (int i = 0; i < lump.Cubemaps.Length; i++)
            {
                var cubemap = lump.Cubemaps[i];

                builder.AppendLine($"    Cubemap {i}");
                builder.AppendLine(cubemap.Origin, "      Origin");
                builder.AppendLine(cubemap.Size, "      Size");
            }
        }

        private static void Print(StringBuilder builder, TexdataStringData? lump)
        {
            if (lump?.Strings == null || lump.Strings.Length == 0)
            {
                builder.AppendLine("    No data");
                return;
            }

            for (int i = 0; i < lump.Strings.Length; i++)
            {
                var str = lump.Strings[i];
                builder.AppendLine($"    String {i}: {str}");
            }
        }

        private static void Print(StringBuilder builder, TexdataStringTable? lump)
        {
            if (lump?.Offsets == null || lump.Offsets.Length == 0)
            {
                builder.AppendLine("    No data");
                return;
            }

            for (int i = 0; i < lump.Offsets.Length; i++)
            {
                var offset = lump.Offsets[i];
                builder.AppendLine($"    Offset {i}: {offset}");
            }
        }

        private static void Print(StringBuilder builder, OverlaysLump? lump)
        {
            if (lump?.Overlays == null || lump.Overlays.Length == 0)
            {
                builder.AppendLine("    No data");
                return;
            }

            for (int i = 0; i < lump.Overlays.Length; i++)
            {
                var overlay = lump.Overlays[i];

                builder.AppendLine($"    Overlay {i}");
                builder.AppendLine(overlay.Id, "      Id");
                builder.AppendLine(overlay.TexInfo, "      Texinfo");
                builder.AppendLine(overlay.FaceCountAndRenderOrder, "      Face count and render order");
                builder.AppendLine(overlay.Ofaces, "      Ofaces");
                builder.AppendLine(overlay.U, "      U");
                builder.AppendLine(overlay.V, "      V");
                if (overlay.UVPoints == null || overlay.UVPoints.Length == 0)
                {
                    builder.AppendLine("      No UV points");
                }
                else
                {
                    for (int j = 0; j < overlay.UVPoints.Length; j++)
                    {
                        var point = overlay.UVPoints[j];
                        builder.AppendLine($"      UV Point {j}: ({point.X}, {point.Y}, {point.Z})");
                    }
                }

                builder.AppendLine($"      Origin: ({overlay.Origin?.X}, {overlay.Origin?.Y}, {overlay.Origin?.Z})");
                builder.AppendLine($"      Basis normal: ({overlay.BasisNormal?.X}, {overlay.BasisNormal?.Y}, {overlay.BasisNormal?.Z})");
            }
        }

        private static void Print(StringBuilder builder, DispTrisLump? lump)
        {
            if (lump?.Tris == null || lump.Tris.Length == 0)
            {
                builder.AppendLine("    No data");
                return;
            }

            for (int i = 0; i < lump.Tris.Length; i++)
            {
                var tri = lump.Tris[i];

                builder.AppendLine($"    Disp Tri {i}");
                builder.AppendLine($"      Tags: {tri.Tags} (0x{tri.Tags:X})");
            }
        }

        private static void Print(StringBuilder builder, AmbientIndexLump? lump)
        {
            if (lump?.Indicies == null || lump.Indicies.Length == 0)
            {
                builder.AppendLine("    No data");
                return;
            }

            for (int i = 0; i < lump.Indicies.Length; i++)
            {
                var index = lump.Indicies[i];

                builder.AppendLine($"    Index {i}");
                builder.AppendLine(index.AmbientSampleCount, "      Ambient sample count");
                builder.AppendLine(index.FirstAmbientSample, "      First ambient sample");
            }
        }

        private static void Print(StringBuilder builder, AmbientLightingLump? lump)
        {
            if (lump?.Lightings == null || lump.Lightings.Length == 0)
            {
                builder.AppendLine("    No data");
                return;
            }

            for (int i = 0; i < lump.Lightings.Length; i++)
            {
                var lighting = lump.Lightings[i];

                builder.AppendLine($"    Lighting {i}");
                builder.AppendLine("      Colors array skipped...");
                builder.AppendLine(lighting.X, "      X");
                builder.AppendLine(lighting.Y, "      Y");
                builder.AppendLine(lighting.Z, "      Z");
                builder.AppendLine(lighting.Pad, "      Pad");
            }
        }
    }
}
