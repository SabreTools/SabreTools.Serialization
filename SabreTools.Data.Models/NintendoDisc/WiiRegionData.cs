namespace SabreTools.Data.Models.NintendoDisc
{
    /// <summary>
    /// Wii disc region data block (0x20 bytes at disc offset 0x4E000)
    /// </summary>
    /// <see href="https://wiibrew.org/wiki/Wii_disc#Region_setting"/>
    public sealed class WiiRegionData
    {
        /// <summary>
        /// Region setting uint:
        /// 0 = Japan, 1 = USA, 2 = Europe, 3 = Korea,
        /// 4 = China, 5 = Taiwan, 6 = Germany, 7 = France
        /// </summary>
        public uint RegionSetting { get; set; }

        /// <summary>
        /// Age ratings for various regions (0x10 bytes)
        /// </summary>
        /// <remarks>16 bytes</remarks>
        public byte[] AgeRatings { get; set; } = new byte[16];
    }
}
