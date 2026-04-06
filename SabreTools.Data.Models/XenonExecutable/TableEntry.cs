namespace SabreTools.Data.Models.XenonExecutable
{
    /// <summary>
    /// Xenon (Xbox 360) Executable format certificate table
    /// </summary>
    /// <see href="http://oskarsapps.mine.nu/xexdump"/>
    /// <see href="https://free60.org/System-Software/Formats/XEX/"/>
    public class TableEntry
    {
        /// <summary>
        /// Table entry ID
        /// Known values:
        /// 0x00000011, 0x00000012, 0x00000013 (Retail games)
        /// 0x00000101, 0x00000102, 0x00000103 (Applications)
        /// Unique IDs from XboxMCX-V.XEX (Windows Vista):
        ///   0x00000023, 0x000000E1 (Once, one after another)
        ///   0x00000031, 0x000000D2 (Once, one after another)
        ///   0x00000072, 0x00000093 (Once, one after another)
        ///   0x00000063 (Once, final entry)
        /// Unique IDs from XboxMCX-V.XEX (Windows 7):
        ///   0x000000D3, 0x00000031 (Once, one after another)
        ///   0x000000A1, 0x00000062 (Once, one after another)
        ///   0x00000072, 0x00000093 (Once, one after another)
        ///   0x00000023 (Once, final entry)
        /// </summary>
        /// <remarks>Big-endian</remarks>
        public uint ID { get; set; } = new();

        /// <summary>
        /// Table entry data, 20 bytes
        /// </summary>
        public byte[]? Data { get; set; } = new byte[20];
    }
}
