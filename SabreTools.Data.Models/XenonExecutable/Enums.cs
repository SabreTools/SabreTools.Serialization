using System;

namespace SabreTools.Data.Models.XenonExecutable
{
    /// <summary>
    /// Flags for Xbox 360 console regions
    /// Source: Original Research
    /// </summary>
    [Flags]
    public enum RegionFlags : uint
    {
        NTSC_U = 0x00_00_00_FF,
        Japan = 0x00_00_01_00,
        China = 0x00_00_02_00,
        Asia = 0x00_00_F8_00,
        Japan_And_Asia = 0x00_00_F9_00,
        NTSC_J_Excludes_China = 0x00_00_FD_00,
        Oceania = 0x00_01_00_00,
        Europe = 0x00_FE_00_00,
        PAL = 0x00_FF_00_00,
        RegionFree = 0xFF_FF_FF_FF,
    }
}
