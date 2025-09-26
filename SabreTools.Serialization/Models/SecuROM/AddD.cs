using System.Runtime.InteropServices;

namespace SabreTools.Serialization.Models.SecuROM
{
    /// <summary>
    /// Overlay data associated with SecuROM executables
    /// </summary>
    /// <remarks>
    /// All information in this file has been researched in a clean room
    /// environment by using sample from legally obtained software that
    /// is protected by SecuROM.
    /// </remarks>
    public sealed class AddD
    {
        /// <summary>
        /// "AddD", Identifier?
        /// </summary>
        public uint Signature { get; set; }

        /// <summary>s
        /// Unknown (Entry count?)
        /// </summary>
        /// <remarks>
        /// 3 in EXPUNGED, 3.17.00.0017, 3.17.00.0019
        /// 3 in 4.47.00.0039, 4.84.00.0054, 4.84.69.0037, 4.84.76.7966, 4.84.76.7968, 4.85.07.0009
        /// </remarks>
        public uint EntryCount { get; set; }

        /// <summary>
        /// Version, always 8 bytes?
        /// </summary>
        [MarshalAs(UnmanagedType.LPStr)]
        public string? Version;

        /// <summary>
        /// Unknown (Build? Formatted as a string)
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public char[]? Build;

        /// <summary>
        /// Unknown (0x14h), Variable number of bytes before entry table
        /// </summary>
        /// <remarks>
        /// 44 bytes in EXPUNGED, 3.17.00.0017, 3.17.00.0019, 4.47.00.0039
        /// 112 bytes in 4.84.00.0054, 4.84.69.0037, 4.84.76.7966, 4.84.76.7968, 4.85.07.0009
        /// 112 byte range contains a fixed-length string at 0x2C, possibly a product ID?
        /// "801400-001" in 4.84.00.0054
        /// "594130-001" in 4.84.69.0037
        /// "554900-001" in 4.84.76.7966
        /// "554900-001" in 4.84.76.7968
        /// "548520-001" in 4.85.07.0009
        /// </remarks>
        public byte[]? Unknown14h { get; set; }

        /// <summary>
        /// Entry table
        /// </summary>
        public AddDEntry[]? Entries { get; set; }
    }
}
