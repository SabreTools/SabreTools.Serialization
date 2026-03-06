namespace SabreTools.Data.Models.NES
{
    /// <summary>
    /// NES 1.0 header information
    /// </summary>
    /// <see href="https://www.nesdev.org/wiki/INES"/>
    /// <remarks>
    /// Older versions of the iNES emulator ignored bytes 7-15, and several ROM management
    /// tools wrote messages in there. Commonly, these will be filled with "DiskDude!",
    /// which results in 64 being added to the mapper number.
    ///
    /// A general rule of thumb: if the last 4 bytes are not all zero, and the header is
    /// not marked for NES 2.0 format, an emulator should either mask off the upper 4 bits
    /// of the mapper number or simply refuse to load the ROM.
    /// </remarks>
    public class Header1 : Header
    {
        // All common header parts take up bytes 0-7

        /// <summary>
        /// Size of PRG RAM in 8 KB units
        /// </summary>
        /// <remarks>
        /// Value 0 infers 8 KB for compatibility; see PRG RAM circuit.
        /// This was a later extension to the iNES format and not widely used.
        /// NES 2.0 is recommended for specifying PRG RAM size instead.
        /// </remarks>
        public byte PRGRAMSize { get; set; }

        /// <summary>
        /// TV system (rarely used extension)
        /// </summary>
        /// <remarks>
        /// Though in the official specification, very few emulators honor this bit
        /// as virtually no ROM images in circulation make use of it.
        /// </remarks>
        public TVSystem TVSystem { get; set; }

        /// <summary>
        /// TV system, PRG-RAM presence (unofficial, rarely used extension)
        /// </summary>
        /// <remarks>
        /// This byte is not part of the official specification, and relatively
        /// few emulators honor it.
        /// </remarks>
        public Flag10 Flag10 { get; set; }

        /// <summary>
        /// Unused padding to align to 16 bytes
        /// </summary>
        public byte[] Padding { get; set; } = new byte[5];
    }
}
