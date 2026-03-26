using SabreTools.Data.Models.BSP;

namespace SabreTools.Data.Extensions
{
    public static class VBSPExtensions
    {
        /// <summary>
        /// Convert a <see cref="VbspLumpType"/> value to string
        /// </summary>
        public static string FromVbspLumpType(this VbspLumpType type)
        {
            return type switch
            {
                VbspLumpType.LUMP_ENTITIES => " - LUMP_ENTITIES",
                VbspLumpType.LUMP_PLANES => " - LUMP_PLANES",
                VbspLumpType.LUMP_TEXTURES => " - LUMP_TEXDATA",
                VbspLumpType.LUMP_VERTICES => " - LUMP_VERTEXES",
                VbspLumpType.LUMP_VISIBILITY => " - LUMP_VISIBILITY",
                VbspLumpType.LUMP_NODES => " - LUMP_NODES",
                VbspLumpType.LUMP_TEXINFO => " - LUMP_TEXINFO",
                VbspLumpType.LUMP_FACES => " - LUMP_FACES",
                VbspLumpType.LUMP_LIGHTING => " - LUMP_LIGHTING",
                VbspLumpType.LUMP_CLIPNODES => " - LUMP_OCCLUSION",
                VbspLumpType.LUMP_LEAVES => " - LUMP_LEAVES",
                VbspLumpType.LUMP_MARKSURFACES => " - LUMP_FACEIDS",
                VbspLumpType.LUMP_EDGES => " - LUMP_EDGES",
                VbspLumpType.LUMP_SURFEDGES => " - LUMP_SURFEDGES",
                VbspLumpType.LUMP_MODELS => " - LUMP_MODELS",
                VbspLumpType.LUMP_WORLDLIGHTS => " - LUMP_WORLDLIGHTS",
                VbspLumpType.LUMP_LEAFFACES => " - LUMP_LEAFFACES",
                VbspLumpType.LUMP_LEAFBRUSHES => " - LUMP_LEAFBRUSHES",
                VbspLumpType.LUMP_BRUSHES => " - LUMP_BRUSHES",
                VbspLumpType.LUMP_BRUSHSIDES => " - LUMP_BRUSHSIDES",
                VbspLumpType.LUMP_AREAS => " - LUMP_AREAS",
                VbspLumpType.LUMP_AREAPORTALS => " - LUMP_AREAPORTALS",
                VbspLumpType.LUMP_PORTALS => " - LUMP_PORTALS / LUMP_UNUSED0 / LUMP_PROPCOLLISION",
                VbspLumpType.LUMP_CLUSTERS => " - LUMP_CLUSTERS / LUMP_UNUSED1 / LUMP_PROPHULLS",
                VbspLumpType.LUMP_PORTALVERTS => " - LUMP_PORTALVERTS / LUMP_UNUSED2 / LUMP_FAKEENTITIES / LUMP_PROPHULLVERTS",
                VbspLumpType.LUMP_CLUSTERPORTALS => " - LUMP_CLUSTERPORTALS / LUMP_UNUSED3 / LUMP_PROPTRIS",
                VbspLumpType.LUMP_DISPINFO => " - LUMP_DISPINFO",
                VbspLumpType.LUMP_ORIGINALFACES => " - LUMP_ORIGINALFACES",
                VbspLumpType.LUMP_PHYSDISP => " - LUMP_PHYSDISP",
                VbspLumpType.LUMP_PHYSCOLLIDE => " - LUMP_PHYSCOLLIDE",
                VbspLumpType.LUMP_VERTNORMALS => " - LUMP_VERTNORMALS",
                VbspLumpType.LUMP_VERTNORMALINDICES => " - LUMP_VERTNORMALINDICES",
                VbspLumpType.LUMP_DISP_LIGHTMAP_ALPHAS => " - LUMP_DISP_LIGHTMAP_ALPHAS",
                VbspLumpType.LUMP_DISP_VERTS => " - LUMP_DISP_VERTS",
                VbspLumpType.LUMP_DISP_LIGHTMAP_SAMPLE_POSITIONS => " - LUMP_DISP_LIGHTMAP_SAMPLE_POSITIONS",
                VbspLumpType.LUMP_GAME_LUMP => " - LUMP_GAME_LUMP",
                VbspLumpType.LUMP_LEAFWATERDATA => " - LUMP_LEAFWATERDATA",
                VbspLumpType.LUMP_PRIMITIVES => " - LUMP_PRIMITIVES",
                VbspLumpType.LUMP_PRIMVERTS => " - LUMP_PRIMVERTS",
                VbspLumpType.LUMP_PRIMINDICES => " - LUMP_PRIMINDICES",
                VbspLumpType.LUMP_PAKFILE => " - LUMP_PAKFILE",
                VbspLumpType.LUMP_CLIPPORTALVERTS => " - LUMP_CLIPPORTALVERTS",
                VbspLumpType.LUMP_CUBEMAPS => " - LUMP_CUBEMAPS",
                VbspLumpType.LUMP_TEXDATA_STRING_DATA => " - LUMP_TEXDATA_STRING_DATA",
                VbspLumpType.LUMP_TEXDATA_STRING_TABLE => " - LUMP_TEXDATA_STRING_TABLE",
                VbspLumpType.LUMP_OVERLAYS => " - LUMP_OVERLAYS",
                VbspLumpType.LUMP_LEAFMINDISTTOWATER => " - LUMP_LEAFMINDISTTOWATER",
                VbspLumpType.LUMP_FACE_MACRO_TEXTURE_INFO => " - LUMP_FACE_MACRO_TEXTURE_INFO",
                VbspLumpType.LUMP_DISP_TRIS => " - LUMP_DISP_TRIS",
                VbspLumpType.LUMP_PHYSCOLLIDESURFACE => " - LUMP_PHYSCOLLIDESURFACE / LUMP_PROP_BLOB",
                VbspLumpType.LUMP_WATEROVERLAYS => " - LUMP_WATEROVERLAYS",
                VbspLumpType.LUMP_LIGHTMAPPAGES => " - LUMP_LIGHTMAPPAGES / LUMP_LEAF_AMBIENT_INDEX_HDR",
                VbspLumpType.LUMP_LIGHTMAPPAGEINFOS => " - LUMP_LIGHTMAPPAGEINFOS / LUMP_LEAF_AMBIENT_INDEX",
                VbspLumpType.LUMP_LIGHTING_HDR => " - LUMP_LIGHTING_HDR",
                VbspLumpType.LUMP_WORLDLIGHTS_HDR => " - LUMP_WORLDLIGHTS_HDR",
                VbspLumpType.LUMP_LEAF_AMBIENT_LIGHTING_HDR => " - LUMP_LEAF_AMBIENT_LIGHTING_HDR",
                VbspLumpType.LUMP_LEAF_AMBIENT_LIGHTING => " - LUMP_LEAF_AMBIENT_LIGHTING",
                VbspLumpType.LUMP_XZIPPAKFILE => " - LUMP_XZIPPAKFILE",
                VbspLumpType.LUMP_FACES_HDR => " - LUMP_FACES_HDR",
                VbspLumpType.LUMP_MAP_FLAGS => " - LUMP_MAP_FLAGS",
                VbspLumpType.LUMP_OVERLAY_FADES => " - LUMP_OVERLAY_FADES",
                VbspLumpType.LUMP_OVERLAY_SYSTEM_LEVELS => " - LUMP_OVERLAY_SYSTEM_LEVELS",
                VbspLumpType.LUMP_PHYSLEVEL => " - LUMP_PHYSLEVEL",
                VbspLumpType.LUMP_DISP_MULTIBLEND => " - LUMP_DISP_MULTIBLEND",
                _ => string.Empty,
            };
        }
    }
}
