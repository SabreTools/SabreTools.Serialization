using System;

namespace SabreTools.Data.Models.PCEngineCDROM
{
    /// <summary>
    /// Opening Mode flags
    /// </summary>
    [Flags]
    public enum OpenMode : byte
    {
        /// <summary>
        /// "Data Read to VRAM"
        /// Bit set if data is to be read to VRAM
        /// </summary>
        READ_TO_VRAM = 0x01,

        /// <summary>
        /// "Data Read to ADPCM Buffer"
        /// Bit set if data is to be read to the ADPCM buffer
        /// </summary>
        READ_TO_ADPCM = 0x02,

        /// <summary>
        /// "BG Display"
        /// Bit set if background display is off
        /// Unset if it should be on
        /// </summary>
        DISPLAY_OFF = 0x20,

        /// <summary>
        /// "ADPCM Play"
        /// Bit set if ADPCM should not play
        /// Unset if it should play
        /// </summary>
        ADCPM_DISABLED = 0x40,

        /// <summary>
        /// "ADPCM Play Mode"
        /// Bit set if ADPCM should play on repeat
        /// Unset if it should play only once
        /// </summary>
        ADPCM_REPEAT = 0x80,

        /// <summary>
        /// Mask for bits that are reserved
        /// </summary>
        RESERVED_MASK = 0x1C,
    }
}
