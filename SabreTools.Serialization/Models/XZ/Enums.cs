using System;

namespace SabreTools.Data.Models.XZ
{
    [Flags]
    public enum BlockFlags : byte
    {
        #region Bits 0-1 - Number of filters

        OneFilter = 0x00,
        TwoFilters = 0x01,
        ThreeFiltrs = 0x02,
        FourFilters = 0x03,

        #endregion

        /// <summary>
        /// Compressed size field present
        /// </summary>
        CompressedSize = 0x40,

        /// <summary>
        /// Uncompressed size field present
        /// </summary>
        UncompressedSize = 0x80,
    }

    public enum HeaderFlags : ushort
    {
        None = 0x0000,
        Crc32 = 0x0100,
        Reserved0x02 = 0x0200,
        Reserved0x03 = 0x0300,
        Crc64 = 0x0400,
        Reserved0x05 = 0x0500,
        Reserved0x06 = 0x0600,
        Reserved0x07 = 0x0700,
        Reserved0x08 = 0x0800,
        Reserved0x09 = 0x0900,
        Sha256 = 0x0A00,
        Reserved0x0B = 0x0B00,
        Reserved0x0C = 0x0C00,
        Reserved0x0D = 0x0D00,
        Reserved0x0E = 0x0E00,
        Reserved0x0F = 0x0F00,
    }
}
