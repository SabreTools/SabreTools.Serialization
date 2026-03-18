namespace SabreTools.Data.Models.PKZIP
{
    /// <summary>
    /// WinZip AES encryption data
    /// </summary>
    /// <remarks>Header ID = 0x9901</remarks>
    /// <see href="https://github.com/adamhathcock/sharpcompress/blob/master/src/SharpCompress/Common/Zip/Headers/LocalEntryHeaderExtraFactory.cs"/>
    public class AesEncryptionStructure : ExtensibleDataField
    {
        /// <summary>
        /// Compression type
        /// </summary>
        /// <remarks>Values 0f 0x01 and 0x02 are accepted</remarks>
        public ushort CompressionType { get; set; }

        /// <summary>
        /// Vendor ID
        /// </summary>
        /// <remarks>Should be 0x4541</remarks>
        public ushort VendorID { get; set; }

        /// <summary>
        /// Unknown
        /// </summary>
        /// <remarks>This value is unreferenced</remarks>
        public byte Unknown { get; set; }

        /// <summary>
        /// Compression method
        /// </summary>
        public CompressionMethod CompressionMethod { get; set; }
    }
}
