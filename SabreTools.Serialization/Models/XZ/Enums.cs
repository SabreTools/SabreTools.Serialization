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
        Crc32 = 0x0001,
        Reserved0x02 = 0x0002,
        Reserved0x03 = 0x0003,
        Crc64 = 0x0004,
        Reserved0x05 = 0x0005,
        Reserved0x06 = 0x0006,
        Reserved0x07 = 0x0007,
        Reserved0x08 = 0x0008,
        Reserved0x09 = 0x0009,
        Sha256 = 0x000A,
        Reserved0x0B = 0x000B,
        Reserved0x0C = 0x000C,
        Reserved0x0D = 0x000D,
        Reserved0x0E = 0x000E,
        Reserved0x0F = 0x000F,
    }
}
