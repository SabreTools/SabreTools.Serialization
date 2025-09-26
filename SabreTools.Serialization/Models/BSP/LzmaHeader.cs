using System.Runtime.InteropServices;

namespace SabreTools.Serialization.Models.BSP
{
    /// <summary>
    /// BSP files for console platforms such as PlayStation 3 and
    /// Xbox 360 usually have their lumps compressed with LZMA.
    /// In this case, the lump data starts with the following header
    /// (from public/tier1/lzmaDecoder.h), which is used in place of
    /// the standard 13-byte LZMA header.
    /// 
    /// lzmaSize denotes the size (in bytes) of compressed data, it
    /// is equal to the size of a lump minus 17 bytes (lzma header).
    /// actualSize denotes the size of decompressed data. properties[5]
    /// field are used solely for LZMA decoding.
    /// 
    /// There are two special cases for compression: LUMP_PAKFILE is never
    /// compressed as a lump (the contents of the zip are compressed instead)
    /// and each of the game lumps in LUMP_GAME_LUMP are compressed individually.
    /// The compressed size of a game lump can be determined by subtracting
    /// the current game lump's offset with that of the next entry. For this
    /// reason, when game lumps are compressed the last game lump is always
    /// an empty dummy which only contains the offset.
    /// </summary>
    /// <see href="https://developer.valvesoftware.com/wiki/BSP_(Source)"/>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class LzmaHeader
    {
        public uint Id;

        /// <remarks>Little-endian</remarks>
        public uint ActualSize;

        /// <remarks>Little-endian</remarks>
        public uint LzmaSize;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        public byte[]? Properties = new byte[5];
    }
}