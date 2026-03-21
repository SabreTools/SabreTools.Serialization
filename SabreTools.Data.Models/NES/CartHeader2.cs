namespace SabreTools.Data.Models.NES
{
    /// <summary>
    /// NES 2.0 header information
    /// </summary>
    /// <see href="https://www.nesdev.org/wiki/NES_2.0"/>
    public class CartHeader2 : CartHeader
    {
        // All common header parts take up bytes 0-7

        #region Byte 8

        /// <summary>
        /// Mapper MSB bits 8-11
        /// </summary>
        /// <remarks>Byte 8, Bits 0-3</remarks>
        public byte MapperMSB { get; set; }

        /// <summary>
        /// Submapper
        /// </summary>
        /// <remarks>Byte 8, Bits 4-7</remarks>
        public byte Submapper { get; set; }

        #endregion

        #region Byte 9

        /// <summary>
        /// PRG-ROM size MSB bits 8-11
        /// </summary>
        /// <remarks>Byte 9, Bits 0-3</remarks>
        public byte PrgRomSizeMSB { get; set; }

        /// <summary>
        /// CHR-ROM size MSB bits 8-11
        /// </summary>
        /// <remarks>Byte 9, Bits 4-7</remarks>
        public byte ChrRomSizeMSB { get; set; }

        #endregion

        #region Byte 10

        /// <summary>
        /// PRG-RAM (volatile) shift count
        /// </summary>
        /// <remarks>
        /// Byte 10, Bits 0-3
        ///
        /// If the shift count is zero, there is no RAM.
        /// If the shift count is non-zero, the actual size is
        /// "64 << shift count" bytes, i.e. 8192 bytes for a shift count of 7.
        /// </remarks>
        public byte PrgRamShiftCount { get; set; }

        /// <summary>
        /// PRG-NVRAM/EEPROM (non-volatile) shift count
        /// </summary>
        /// <remarks>
        /// Byte 10, Bits 4-7
        ///
        /// If the shift count is zero, there is no NVRAM.
        /// If the shift count is non-zero, the actual size is
        /// "64 << shift count" bytes, i.e. 8192 bytes for a shift count of 7.
        /// </remarks>
        public byte PrgNvramEepromShiftCount { get; set; }

        #endregion

        #region Byte 11

        /// <summary>
        /// CHR-RAM size (volatile) shift count
        /// </summary>
        /// <remarks>
        /// Byte 11, Bits 0-3
        ///
        /// If the shift count is zero, there is no RAM.
        /// If the shift count is non-zero, the actual size is
        /// "64 << shift count" bytes, i.e. 8192 bytes for a shift count of 7.
        /// </remarks>
        public byte ChrRamShiftCount { get; set; }

        /// <summary>
        /// CHR-NVRAM size (non-volatile) shift count
        /// </summary>
        /// <remarks>
        /// Byte 11, Bits 4-7
        ///
        /// If the shift count is zero, there is no NVRAM.
        /// If the shift count is non-zero, the actual size is
        /// "64 << shift count" bytes, i.e. 8192 bytes for a shift count of 7.
        /// </remarks>
        public byte ChrNvramShiftCount { get; set; }

        #endregion

        /// <summary>
        /// CPU/PPU timing mode
        /// </summary>
        public CPUPPUTiming CPUPPUTiming { get; set; }

        #region Byte 13

        #region Standard System and PlayChoice-10

        /// <summary>
        /// Reserved byte, unused
        /// </summary>
        /// <remarks>
        /// Valid when <see cref="CartHeader.ConsoleType"/> == <see cref="ConsoleType.StandardSystem"/>.
        /// Valid when <see cref="CartHeader.ConsoleType"/> == <see cref="ConsoleType.PlayChoice10"/>.
        /// </remarks>
        public byte Reserved13 { get; set; }

        #endregion

        #region Vs. Unisystem

        /// <summary>
        /// Vs. System Type
        /// </summary>
        /// <remarks>
        /// Byte 13, Bits 0-3
        ///
        /// Valid when <see cref="CartHeader.ConsoleType"/> == <see cref="ConsoleType.VSUnisystem"/>
        /// </remarks>
        public VsSystemType VsSystemType { get; set; }

        /// <summary>
        /// Vs. Hardware Type
        /// </summary>
        /// <remarks>
        /// Byte 13, Bits 4-7
        ///
        /// Valid when <see cref="CartHeader.ConsoleType"/> == <see cref="ConsoleType.VSUnisystem"/>
        /// </remarks>
        public VsHardwareType VsHardwareType { get; set; }

        #endregion

        #region Extended Console Type

        /// <summary>
        /// Extended Console Type
        /// </summary>
        /// <remarks>
        /// Byte 13, Bits 0-3
        ///
        /// Valid when <see cref="CartHeader.ConsoleType"/> == <see cref="ConsoleType.ExtendedConsoleType"/>
        /// </remarks>
        public ExtendedConsoleType ExtendedConsoleType { get; set; }

        /// <summary>
        /// Unknown reserved bits
        /// </summary>
        /// <remarks>Byte 13, Bits 4-7</remarks>
        public byte Byte13ReservedBits47 { get; set; }

        #endregion

        #endregion

        /// <summary>
        /// Number of miscellaneous ROMs present
        /// </summary>
        /// <remarks>Only bits 0-1 are used</remarks>
        public byte MiscellaneousROMs { get; set; }

        /// <summary>
        /// Default Expansion Device
        /// </summary>
        public DefaultExpansionDevice DefaultExpansionDevice { get; set; }
    }
}
