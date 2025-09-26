using System.Runtime.InteropServices;

namespace SabreTools.Serialization.Models.BSP
{
    /// <summary>
    /// Each of this structs describes a texture. The name of the
    /// texture is a string and may be 16 characters long (including
    /// the null-character at the end, char equals a 8bit signed
    /// integer). The name of the texture is needed, if the texture
    /// has to be found and loaded from an external WAD file.
    /// Furthermore, the struct contains the width and height of
    /// the texture. The 4 offsets at the end can either be zero,
    /// if the texture is stored in an external WAD file, or point
    /// to the beginnings of the binary texture data within the
    /// texture lump relative to the beginning of it's BSPMIPTEX struct.
    /// </summary>
    /// <see href="https://github.com/RavuAlHemio/hllib/blob/master/HLLib/BSPFile.h"/>
    /// <see href="https://developer.valvesoftware.com/wiki/BSP_(GoldSrc)"/> 
    [StructLayout(LayoutKind.Sequential)]
    public sealed class MipTexture
    {
        /// <summary>
        /// Name of texture
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Constants.MAXTEXTURENAME)]
        public string? Name;
    
        /// <summary>
        /// Extends of the texture
        /// </summary>
        public uint Width;

        /// <summary>
        /// Extends of the texture
        /// </summary>
        public uint Height;

        /// <summary>
        /// Offsets to texture mipmaps BSPMIPTEX
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.MIPLEVELS)]
        public uint[]? Offsets;
    }
}