using System.Runtime.InteropServices;

namespace SabreTools.Serialization.Models.BSP
{
    /// <summary>
    /// The texinfo lump contains informations about how textures are
    /// applied to surfaces. The lump itself is an array of binary data
    /// structures.
    /// 
    /// This struct is mainly responsible for the calculation of the texture
    /// coordinates (vS, fSShift, vT, fTShift). This values determine the
    /// position of the texture on the surface. The iMiptex integer refers
    /// to the textures in the texture lump and would be the index in an
    /// array of BSPMITEX structs. Finally, there are 4 Bytes used for flags.
    /// Only one flag is used by the vanilla engine, being 0x1 for disabling
    /// lightmaps and subdivision for the surface (used by sky and liquids).
    /// </summary>
    /// <see href="https://developer.valvesoftware.com/wiki/BSP_(Source)"/>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class VbspTexinfo
    {
        /// <summary>
        /// Texture S-vector
        /// </summary>
        public Vector3D? TextureSVector;

        /// <summary>
        /// Texture shift in the S direction
        /// </summary>
        public float TextureSShift;

        /// <summary>
        /// Texture T-vector
        /// </summary>
        public Vector3D? TextureTVector;

        /// <summary>
        /// Texture shift in the T direction
        /// </summary>
        public float TextureTShift;

        /// <summary>
        /// Lightmap S-vector
        /// </summary>
        public Vector3D? LightmapSVector;

        /// <summary>
        /// Lightmap shift in the S direction
        /// </summary>
        public float LightmapSShift;

        /// <summary>
        /// Lightmap T-vector
        /// </summary>
        public Vector3D? LightmapTVector;

        /// <summary>
        /// Lightmap shift in the T direction
        /// </summary>
        public float LightmapTShift;

        /// <summary>
        /// Texture flags
        /// </summary>
        public TextureFlag Flags;

        /// <summary>
        /// Pointer to texture name, size, etc.
        /// </summary>
        public int TexData;
    }
}