namespace SabreTools.Data.Models.NES
{
    /// <summary>
    /// NES 2.0 header information
    /// </summary>
    /// <see href="https://www.nesdev.org/wiki/NES_2.0"/>
    public class Header2 : Header
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
        public byte PRGROMSizeMSB { get; set; }

        /// <summary>
        /// CHR-ROM size MSB bits 8-11
        /// </summary>
        /// <remarks>Byte 9, Bits 4-7</remarks>
        public byte CHRROMSizeMSB { get; set; }

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
        public byte PRGRAMShiftCount { get; set; }

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
        public byte PRGNVRAMEEPROMShiftCount { get; set; }

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
        public byte CHRRAMShiftCount { get; set; }

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
        public byte CHRNVRAMShiftCount { get; set; }

        #endregion

        /// <summary>
        /// CPU/PPU timing mode
        /// </summary>
        public CPUPPUTiming CPUPPUTiming { get; set; }

        /// <summary>
        /// Vs. System Type and Extended Console Type
        /// </summary>
        /// <remarks>
        /// When Byte 7 AND 3 =1: Vs. System Type
        /// When Byte 7 AND 3 =3: Extended Console Type
        /// </remarks>
        public byte ExtendedSystemType { get; set; }

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
