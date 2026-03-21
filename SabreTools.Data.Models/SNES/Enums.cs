using System;

namespace SabreTools.Data.Models.SNES
{
    /// <summary>
    /// Pro Fighter first SRAM mode
    /// </summary>
    /// <see href="https://www.raphnet.net/divers/documentation/Sneskart.txt"/>
    public enum FIGDSPMode1 : byte
    {
        /// <summary>
        /// If using DSP1 microchip and if no SRAM (SRAM size=0)
        /// </summary>
        DSP1WithNoSRAM = 0x47,

        /// <summary>
        /// If not using DSP1 and no SRAM (SRAM size=0)
        /// </summary>
        NoDSP1WithNoSRAM = 0x77,

        /// <summary>
        /// If using DSP1 microchip and if using SRAM (SRAM size>0)
        /// </summary>
        DSP1WithSRAM = 0xFD,
    }

    /// <summary>
    /// Pro Fighter second SRAM mode
    /// </summary>
    /// <see href="https://www.raphnet.net/divers/documentation/Sneskart.txt"/>
    public enum FIGDSPMode2 : byte
    {
        /// <summary>
        /// If using DSP1 microchip and if using SRAM (SRAM size>0)
        /// </summary>
        DSP1WithSRAM = 0x82,

        /// <summary>
        /// If using DSP1 microchip and if no SRAM (SRAM size=0)
        /// </summary>
        DSP1WithNoSRAM = 0x83,

        /// <summary>
        /// If not using DSP1 and no SRAM (SRAM size=0)
        /// </summary>
        NoDSP1WithNoSRAM = 0x83,
    }

    /// <summary>
    /// Pro Fighter image mode
    /// </summary>
    /// <see href="https://www.raphnet.net/divers/documentation/Sneskart.txt"/>
    public enum FIGImageMode : byte
    {
        /// <summary>
        /// Last image in set (or single image)
        /// </summary>
        SingleImage = 0x00,

        /// <summary>
        /// Multi image
        /// </summary>
        MultiImage = 0x40,
    }

    /// <summary>
    /// Pro Fighter ROM mode
    /// </summary>
    /// <see href="https://www.raphnet.net/divers/documentation/Sneskart.txt"/>
    public enum FIGRomMode : byte
    {
        LoROM = 0x00,

        HiROM = 0x80,
    }

    /// <summary>
    /// Super Wild Card image information
    /// </summary>
    /// <see href="https://www.raphnet.net/divers/documentation/Sneskart.txt"/>
    [Flags]
    public enum SWCImageInformation : byte
    {
        /// <summary>
        /// Reserved
        /// </summary>
        Reserved0 = 0b00000001,

        /// <summary>
        /// Reserved
        /// </summary>
        Reserved1 = 0b00000010,

        #region SRAM size

        /// <summary>
        /// 256kbit SRAM
        /// </summary>
        SRAM256kbit = 0b00000000,

        /// <summary>
        /// 65kbit SRAM
        /// </summary>
        SRAM65kbit = 0b00000100,

        /// <summary>
        /// 16kbit SRAM
        /// </summary>
        SRAM16kbit = 0b00001000,

        /// <summary>
        /// no SRAM
        /// </summary>
        SRAMNone = 0b00001100,

        #endregion

        #region DRAM mode

        /// <summary>
        /// DRAM memory mapping Mode 20
        /// </summary>
        DRAMMappingMode20 = 0b00000000,

        /// <summary>
        /// DRAM memory mapping Mode 21 (HiROM)
        /// </summary>
        DRAMMappingMode21 = 0b00010000,

        #endregion

        #region SRAM mode

        /// <summary>
        /// SRAM memory mapping Mode 20
        /// </summary>
        SRAMMappingMode20 = 0b00000000,

        /// <summary>
        /// SRAM memory mapping Mode 21 (HiROM)
        /// </summary>
        SRAMMappingMode21 = 0b00100000,

        #endregion

        #region Multi-image

        /// <summary>
        /// Not multi image (no more split files to follow)
        /// </summary>
        NotMultiImage = 0b00000000,

        /// <summary>
        /// Multi image (there is another split file to follow)
        /// </summary>
        MultiImage = 0b01000000,

        #endregion

        #region Run program mode

        /// <summary>
        /// Run program in Mode 1 (JMP RESET Vector)
        /// </summary>
        RunProgramInMode1 = 0b00000000,

        /// <summary>
        /// Run program in Mode 0 (JMP $8000)
        /// </summary>
        RunProgramInMode0 = 0b10000000,

        #endregion
    }
}
