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
        public byte PrgRomSize { get; set; }

        /// <summary>
        /// Size of CHR ROM in 8 KB units
        /// </summary>
        /// <remarks>Value 0 means the board uses CHR RAM</remarks>
        public byte ChrRomSize { get; set; }

        #region Byte 6

        /// <summary>
        /// Nametable arrangement
        /// </summary>
        /// <remarks>Byte 6, Bit 0</remarks>
        public NametableArrangement NametableArrangement { get; set; }

        /// <summary>
        /// Cartridge contains battery-backed PRG RAM ($6000-7FFF)
        /// or other persistent memory
        /// </summary>
        /// <remarks>Byte 6, Bit 1</remarks>
        public bool BatteryBackedPrgRam { get; set; }

        /// <summary>
        /// 512-byte trainer at $7000-$71FF
        /// </summary>
        /// <remarks>Byte 6, Bit 2</remarks>
        public bool TrainerPresent { get; set; }

        /// <summary>
        /// Alternative nametable layout
        /// </summary>
        /// <remarks>Byte 6, Bit 3</remarks>
        public bool AlternativeNametableLayout { get; set; }

        /// <summary>
        /// Lower nibble of Mapper number
        /// </summary>
        /// <remarks>Byte 6, Bits 4-7</remarks>
        public byte MapperLowerNibble { get; set; }

        #endregion

        #region Byte 7

        /// <summary>
        /// Nametable arrangement
        /// </summary>
        /// <remarks>
        /// Byte 7, Bits 0-1
        ///
        /// The PlayChoice-10 bit is not part of the official specification,
        /// and most emulators simply ignore the extra 8 KB of data. PlayChoice
        /// games are designed to look good with the 2C03 RGB PPU, which handles
        /// color emphasis differently from a standard NES PPU.
        ///
        /// Vs. games have a coin slot and different palettes. The detection
        /// of which palette a particular game uses is left unspecified.
        /// </remarks>
        public ConsoleType ConsoleType { get; set; }

        /// <summary>
        /// Indicates NES 2.0 format
        /// </summary>
        /// <remarks>
        /// Byte 7, Bits 2-3 == 0x02
        ///
        /// NES 2.0 is a more recent extension to the format that allows more
        /// flexibility in ROM and RAM size, among other things.
        /// </remarks>
        public bool NES20 { get; set; }

        /// <summary>
        /// Upper nibble of Mapper number
        /// </summary>
        /// <remarks>Byte 7, Bits 4-7</remarks>
        public byte MapperUpperNibble { get; set; }

        #endregion
    }
}
