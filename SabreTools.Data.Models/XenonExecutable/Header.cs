namespace SabreTools.Data.Models.XenonExecutable
{
    /// <summary>
    /// Xenon (Xbox 360) Executable format header
    /// </summary>
    /// <see href="http://oskarsapps.mine.nu/xexdump"/>
    /// <see href="https://free60.org/System-Software/Formats/XEX/"/>
    public class Header
    {
        /// <summary>
        /// "XEX2"
        /// </summary>
        public byte[] MagicNumber { get; set; } = new byte[4];

        /// <summary>
        /// Bit field with lowest 8 bits having known values: (lowest to highest)
        /// Title Module, Exports To Title, System Debugger, DLL Module, Module Patch, Patch Full, Patch Delta, User Mode
        /// </summary>
        /// <remarks>Big-endian</remarks>
        public uint ModuleFlags { get; set; }

        /// <summary>
        /// Address at which the PE data begins
        /// </summary>
        /// <remarks>Big-endian</remarks>
        public uint PEDataOffset { get; set; }

        /// <summary>
        /// Reserved field, should be zeroed
        /// </summary>
        /// <remarks>Big-endian</remarks>
        public uint Reserved { get; set; }

        /// <summary>
        /// Address at which the security info begins
        /// </summary>
        /// <remarks>Big-endian</remarks>
        public uint SecurityInfoOffset { get; set; }

        /// <summary>
        /// Number of optional headers
        /// </summary>
        /// <remarks>Big-endian</remarks>
        public uint OptionalHeaderCount { get; set; }

        /// <summary>
        /// Pptional headers that follow the main header
        /// </summary>
        public OptionalHeader[]? OptionalHeaders { get; set; }
    }
}
