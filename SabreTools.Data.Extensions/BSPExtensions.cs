using SabreTools.Data.Models.BSP;

namespace SabreTools.Data.Extensions
{
    public static class BSPExtensions
    {
        /// <summary>
        /// Convert a <see cref="BspLumpType"/> value to string
        /// </summary>
        public static string FromBspLumpType(this BspLumpType type)
        {
            return type switch
            {
                BspLumpType.LUMP_ENTITIES => " - LUMP_ENTITIES",
                BspLumpType.LUMP_PLANES => " - LUMP_PLANES",
                BspLumpType.LUMP_TEXTURES => " - LUMP_TEXTURES",
                BspLumpType.LUMP_VERTICES => " - LUMP_VERTICES",
                BspLumpType.LUMP_VISIBILITY => " - LUMP_VISIBILITY",
                BspLumpType.LUMP_NODES => " - LUMP_NODES",
                BspLumpType.LUMP_TEXINFO => " - LUMP_TEXINFO",
                BspLumpType.LUMP_FACES => " - LUMP_FACES",
                BspLumpType.LUMP_LIGHTING => " - LUMP_LIGHTING",
                BspLumpType.LUMP_CLIPNODES => " - LUMP_CLIPNODES",
                BspLumpType.LUMP_LEAVES => " - LUMP_LEAVES",
                BspLumpType.LUMP_MARKSURFACES => " - LUMP_MARKSURFACES",
                BspLumpType.LUMP_EDGES => " - LUMP_EDGES",
                BspLumpType.LUMP_SURFEDGES => " - LUMP_SURFEDGES",
                BspLumpType.LUMP_MODELS => " - LUMP_MODELS",
                _ => string.Empty,
            };
        }
    }
}
