namespace SabreTools.Data.Models.NintendoDisc
{
    public static class Constants
    {
        #region Disc identification magic values

        /// <summary>Magic word present at offset 0x01C on GameCube discs</summary>
        public const uint GCMagicWord = 0xC2339F3D;

        /// <summary>Magic word present at offset 0x018 on Wii discs</summary>
        public const uint WiiMagicWord = 0x5D1C9EA3;

        #endregion
        public const int DiscHeaderSize = 0x440;

        #region BI2 data

        public const int Bi2Address = 0x000440;
        public const int Bi2Size = 0x2000;

        #endregion

        #region Apploader

        public const int ApploaderAddress = 0x002440;
        public const int ApploaderCodeSizeOffset = 0x14;
        public const int ApploaderTrailerSizeOffset = 0x18;
        public const int ApploaderHeaderSize = 0x20;

        #endregion

        #region Wii-specific disc layout

        public const int WiiPartitionTableAddress = 0x40000;
        public const int WiiPartitionGroupCount = 4;
        public const int WiiRegionDataAddress = 0x04E000;
        public const int WiiRegionDataSize = 0x20;

        #endregion

        #region Wii partition header fields

        // Offsets relative to partition start
        public const int WiiTicketSize = 0x2A4;
        public const int WiiTmdSizeAddress = 0x2A4;
        public const int WiiTmdOffsetAddress = 0x2A8;
        public const int WiiCertSizeAddress = 0x2AC;
        public const int WiiCertOffsetAddress = 0x2B0;
        public const int WiiH3OffsetAddress = 0x2B4;
        public const int WiiH3Size = 0x18000;
        public const int WiiDataOffsetAddress = 0x2B8;

        #endregion

        #region Wii block / group structure

        public const int WiiBlockSize = 0x8000;
        public const int WiiBlockHeaderSize = 0x0400;
        public const int WiiBlockDataSize = 0x7C00;
        public const int WiiBlocksPerGroup = 64;
        public const int WiiBlockHashSize = 0x0400;
        public const int WiiGroupSize = WiiBlocksPerGroup * WiiBlockSize;
        public const int WiiGroupDataSize = WiiBlocksPerGroup * WiiBlockDataSize;

        #endregion

        #region DVD sector size

        public const int DvdSectorSize = 0x800;

        #endregion

        #region Wii ticket fields

        // Offsets relative to ticket start
        public const int TicketEncryptedTitleKeyOffset = 0x1BF;
        public const int TicketTitleIdOffset = 0x1DC;
        public const int TicketCommonKeyIndexOffset = 0x1F1;

        #endregion
    }
}
