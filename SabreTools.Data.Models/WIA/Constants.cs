namespace SabreTools.Data.Models.WIA
{
    public static class Constants
    {
        /// <summary>WIA magic (little-endian u32): "WIA\x01"</summary>
        public const uint WiaMagic = 0x01414957;

        /// <summary>RVZ magic (little-endian u32): "RVZ\x01"</summary>
        public const uint RvzMagic = 0x015A5652;

        /// <summary>Size of WiaHeader1 in bytes</summary>
        public const int Header1Size = 0x48;

        /// <summary>Size of WiaHeader2 in bytes</summary>
        public const int Header2Size = 0xDC;

        /// <summary>Size of a PartitionEntry in bytes</summary>
        public const int PartitionEntrySize = 0x30;

        /// <summary>Size of a PartitionDataEntry in bytes</summary>
        public const int PartitionDataEntrySize = 0x10;

        /// <summary>Size of a RawDataEntry in bytes</summary>
        public const int RawDataEntrySize = 0x18;

        /// <summary>Size of a WiaGroupEntry in bytes</summary>
        public const int WiaGroupEntrySize = 0x08;

        /// <summary>Size of an RvzGroupEntry in bytes</summary>
        public const int RvzGroupEntrySize = 0x0C;

        /// <summary>Size of a HashExceptionEntry in bytes (2-byte offset + 20-byte SHA-1)</summary>
        public const int HashExceptionEntrySize = 0x16;

        /// <summary>Number of bytes of disc header stored in WiaHeader2.DiscHeader</summary>
        public const int DiscHeaderStoredSize = 0x80;

        // WIA version numbers
        public const uint WiaVersion = 0x01000000;
        public const uint WiaVersionWriteCompatible = 0x01000000;
        public const uint RvzVersion = 0x01000000;
        public const uint RvzVersionWriteCompatible = 0x00030000;

        // Default chunk size (2 MiB = one Wii group)
        public const uint DefaultChunkSize = 2 * 1024 * 1024;
    }
}
