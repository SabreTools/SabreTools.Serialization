namespace SabreTools.Data.Models.NES
{
    /// <summary>
    /// NES 2.0 header information
    /// </summary>
    /// <see href="https://www.nesdev.org/wiki/NES_2.0"/>
    public class Header2 : Header
    {
        // All common header parts take up bytes 0-7

        /// <summary>
        /// Mapper MSB/Submapper
        /// </summary>
        /// <remarks>
        /// Bits 0-3 - Mapper number bits 8-11
        /// Bits 4-7 - Submapper number
        /// </remarks>
        public byte MapperMSBSubmapper { get; set; }

        /// <summary>
        /// PRG-ROM/CHR-ROM size MSB
        /// </summary>
        /// <remarks>
        /// Bits 0-3 - PRG-ROM size MSB
        /// Bits 4-7 - CHR-ROM size MSB
        /// </remarks>
        public byte PRGCHRMSB { get; set; }

        /// <summary>
        /// PRG-RAM/EEPROM size
        /// </summary>
        /// <remarks>
        /// Bits 0-3 - PRG-RAM (volatile) shift count
        /// Bits 4-7 - PRG-NVRAM/EEPROM (non-volatile) shift count
        ///
        /// If the shift count is zero, there is no CHR-(NV)RAM.
        /// If the shift count is non-zero, the actual size is
        /// "64 << shift count" bytes, i.e. 8192 bytes for a shift count of 7.
        /// </remarks>
        public byte PRGRAMEEPROMSize { get; set; }

        /// <summary>
        /// CHR-RAM size
        /// </summary>
        /// <remarks>
        /// Bits 0-3 - CHR-RAM size (volatile) shift count
        /// Bits 4-7 - CHR-NVRAM size (non-volatile) shift count
        ///
        /// If the shift count is zero, there is no CHR-(NV)RAM.
        /// If the shift count is non-zero, the actual size is
        /// "64 << shift count" bytes, i.e. 8192 bytes for a shift count of 7.
        /// </remarks>
        public byte CHRRAMSize { get; set; }

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
        public ExtendedSystemType ExtendedSystemType { get; set; }

        /// <summary>
        /// Number of miscellaneous ROMs present
        /// </summary>
        /// <remarks>Only bits 0-1 are used</remarks>
        public bool MiscellaneousROMs { get; set; }

        /// <summary>
        /// Default Expansion Device
        /// </summary>
        public DefaultExpansionDevice DefaultExpansionDevice { get; set; }
    }
}
