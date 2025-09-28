using System.Runtime.InteropServices;

namespace SabreTools.Data.Models.SecuROM
{
    /// <summary>
    /// Overlay data associated with SecuROM executables
    /// </summary>
    /// <remarks>
    /// All information in this file has been researched in a clean room
    /// environment by using sample from legally obtained software that
    /// is protected by SecuROM.
    /// </remarks>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public sealed class AddDEntry
    {
        /// <summary>
        /// Physical offset of the embedded file
        /// </summary>
        /// <remarks>Relative to the start of the file</remarks>
        public uint PhysicalOffset;

        /// <summary>
        /// Length of the embedded file
        /// </summary>
        /// <remarks>The last entry seems to be 4 bytes short in 4.47.00.0039</remarks>
        public uint Length;

        /// <summary>
        /// Unknown (0x08)
        /// </summary>
        /// <remarks>
        /// Observed values:
        /// - 33 93 40 00 (4231987) - 4.84.76.7966, 4.84.76.7968
        /// - 65 4D 40 00 (4214117) - Expunged version
        /// - A3 92 40 00 (4231843) - 4.84.00.0054
        /// - A8 0D 30 00 (3149224) - 3.17.00.0017, 3.17.00.0019, 4.47.00.0039
        /// - C3 92 40 00 (4231875) - 4.84.69.0037
        /// - E3 B7 8C 77 (2005710819) - 4.85.04.0002, 4.85.07.0009
        /// </remarks>
        public uint Unknown08h;

        /// <summary>
        /// Unknown (0x0C)
        /// </summary>
        /// <remarks>
        /// Observed values:
        /// - 00 02 00 00 (512) - Expunged version
        /// - 30 16 2F 00 (3085872) - 4.84.69.0037, 4.84.76.7966, 4.84.76.7968
        /// - 48 16 2F 00 (3085896) - 4.84.00.0054
        /// - A8 05 30 00 (3147176) - 3.17.00.0017, 3.17.00.0019, 4.47.00.0039
        /// - DC 0A 2F 00 (3082972) - 4.85.04.0002, 4.85.07.0009
        /// </remarks>
        public uint Unknown0Ch;

        /// <summary>
        /// Unknown (0x10)
        /// </summary>
        /// <remarks>
        /// Observed values:
        /// - 00 08 00 00 (2048) - Expunged version
        /// - 58 07 2F 00 (3082072) - 4.85.04.0002, 4.85.07.0009
        /// - A8 0D 30 00 (3149224) - 3.17.00.0017, 3.17.00.0019, 4.47.00.0039
        /// - CD 00 00 00 (205) - 4.84.00.0054, 4.84.69.0037, 4.84.76.7966, 4.84.76.7968
        /// </remarks>
        public uint Unknown10h;

        /// <summary>
        /// Unknown (0x14)
        /// </summary>
        /// <remarks>
        /// Observed values:
        /// - 00 00 00 00 (0) - 4.85.04.0002, 4.85.07.0009
        /// - 00 08 00 00 (2048) - 4.84.00.0054, 4.84.69.0037, 4.84.76.7966, 4.84.76.7968
        /// - 02 00 00 00 (2) - Expunged version
        /// - 74 FF 12 00 (1245044) - 3.17.00.0017, 3.17.00.0019, 4.47.00.0039
        /// </remarks>
        public uint Unknown14h;

        /// <summary>
        /// Unknown (0x18)
        /// </summary>
        /// <remarks>
        /// Observed values:
        /// - 00 00 00 00 (0) - 4.84.76.7966
        /// - 00 00 01 00 (65536) - 4.84.76.7968
        /// - 00 00 11 FD (4245749760) - 4.84.69.0037
        /// - 00 00 13 00 (1245184) - 4.84.00.0054
        /// - 40 28 41 00 (4270144) - Expunged version
        /// - 76 00 00 00 (118) - 4.85.07.0009
        /// - 7C 00 00 00 (124) - 4.85.04.0002
        /// - C5 4F 40 00 (4214725) - 3.17.00.0017, 3.17.00.0019, 4.47.00.0039
        /// </remarks>
        public uint Unknown18h;

        /// <summary>
        /// Unknown (0x1C)
        /// </summary>
        /// <remarks>
        /// Observed values:
        /// - 00 00 00 00 (0) - 4.84.76.7966
        /// - 00 02 00 00 (512) - 3.17.00.0017, 3.17.00.0019, 4.47.00.0039
        /// - 08 00 A8 08 (145227784) - 4.84.76.7968
        /// - 4D 96 C2 25 (633509453) - 4.84.69.0037
        /// - 78 13 13 00 (1250168) - 4.84.00.0054
        /// - 84 00 00 00 (132) - Expunged version
        /// - F0 0A 2F 00 (3082992) - 4.85.04.0002, 4.85.07.0009
        /// </remarks>
        public uint Unknown1Ch;

        /// <summary>
        /// Entry file name (null-terminated)
        /// </summary>
        /// <remarks>12 bytes long in all samples</remarks>
        [MarshalAs(UnmanagedType.LPStr)]
        public string? FileName;

        /// <summary>
        /// Unknown (0x2C)
        /// </summary>
        /// <remarks>
        /// Observed values:
        /// - 04 FE 64 00 (6618628) - Expunged version
        /// - 20 00 00 00 (32) - 4.84.00.0054, 4.84.69.0037, 4.84.76.7966, 4.84.76.7968
        /// - 27 00 00 00 (39) - 4.85.04.0002
        /// - 2D 00 00 00 (45) - 4.85.07.0009
        /// - 84 00 00 00 (132) - 3.17.00.0017, 3.17.00.0019, 4.47.00.0039
        /// </remarks>
        public uint Unknown2Ch;
    }
}
