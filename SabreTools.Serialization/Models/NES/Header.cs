namespace SabreTools.Data.Models.NES
{
    /// <summary>
    /// Common NES header pieces
    /// </summary>
    /// <see href="https://www.nesdev.org/wiki/INES"/>
    /// <see href="https://www.nesdev.org/wiki/NES_2.0"/>
    public abstract class Header
    {
        /// <summary>
        /// Constant $4E $45 $53 $1A (ASCII "NES" followed by MS-DOS end-of-file)
        /// </summary>
        public byte[] IdentificationString { get; set; } = new byte[4];

        /// <summary>
        /// Size of PRG ROM in 16 KB units
        /// </summary>
        public byte PRGROMSize { get; set; }

        /// <summary>
        /// Size of CHR ROM in 8 KB units
        /// </summary>
        /// <remarks>Value 0 means the board uses CHR RAM</remarks>
        public byte CHRROMSize { get; set; }

        /// <summary>
        /// Mapper, mirroring, battery, trainer
        /// </summary>
        public Flag6 Flag6 { get; set; }

        /// <summary>
        /// Mapper, VS/Playchoice, NES 2.0
        /// </summary>
        /// <remarks>
        /// The PlayChoice-10 bit is not part of the official specification,
        /// and most emulators simply ignore the extra 8 KB of data. PlayChoice
        /// games are designed to look good with the 2C03 RGB PPU, which handles
        /// color emphasis differently from a standard NES PPU.
        ///
        /// Vs. games have a coin slot and different palettes. The detection
        /// of which palette a particular game uses is left unspecified.
        ///
        /// NES 2.0 is a more recent extension to the format that allows more
        /// flexibility in ROM and RAM size, among other things.
        /// </remarks>
        public Flag7 Flag7 { get; set; }
    }
}
