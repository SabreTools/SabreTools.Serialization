namespace SabreTools.Data.Models.UDF
{
    /// <summary>
    /// UDF / ECMA-167 constant values and arrays
    /// </summary>
    /// <see href="https://ecma-international.org/wp-content/uploads/ECMA-167_3rd_edition_june_1997.pdf"/>
    /// <see href="http://www.osta.org/specs/pdf/udf260.pdf"/>
    public static class Constants
    {
        #region Volume Recognition Constants

        /// <summary>
        /// Size of a logical sector on optical media
        /// </summary>
        public const int SectorSize = 2048;

        /// <summary>
        /// Sector of the first Volume Structure Descriptor (after the 32KiB System Area)
        /// </summary>
        public const int VolumeRecognitionSequenceSector = 16;

        /// <summary>
        /// Sector of the first Anchor Volume Descriptor Pointer
        /// </summary>
        public const int AnchorVolumeDescriptorPointerSector = 256;

        /// <summary>
        /// Beginning Extended Area Descriptor identifier, "BEA01"
        /// </summary>
        public static readonly byte[] BeginningExtendedAreaDescriptor = [0x42, 0x45, 0x41, 0x30, 0x31];

        /// <summary>
        /// NSR Descriptor identifier for ECMA-167 2nd edition (UDF 1.02 to 1.50), "NSR02"
        /// </summary>
        public static readonly byte[] NSR02 = [0x4E, 0x53, 0x52, 0x30, 0x32];

        /// <summary>
        /// NSR Descriptor identifier for ECMA-167 3rd edition (UDF 2.00 and later), "NSR03"
        /// </summary>
        public static readonly byte[] NSR03 = [0x4E, 0x53, 0x52, 0x30, 0x33];

        /// <summary>
        /// Terminating Extended Area Descriptor identifier, "TEA01"
        /// </summary>
        public static readonly byte[] TerminatingExtendedAreaDescriptor = [0x54, 0x45, 0x41, 0x30, 0x31];

        /// <summary>
        /// ISO 9660 volume descriptor identifier, "CD001" (bridge discs)
        /// </summary>
        public static readonly byte[] CD001 = [0x43, 0x44, 0x30, 0x30, 0x31];

        /// <summary>
        /// Boot Descriptor identifier, "BOOT2"
        /// </summary>
        public static readonly byte[] BOOT2 = [0x42, 0x4F, 0x4F, 0x54, 0x32];

        /// <summary>
        /// ECMA-168 volume descriptor identifier, "CDW02"
        /// </summary>
        public static readonly byte[] CDW02 = [0x43, 0x44, 0x57, 0x30, 0x32];

        #endregion

        #region Partition Map Constants

        /// <summary>
        /// Type 1 Partition Map type
        /// </summary>
        public const byte Type1PartitionMap = 1;

        /// <summary>
        /// Type 2 Partition Map type
        /// </summary>
        public const byte Type2PartitionMap = 2;

        /// <summary>
        /// Partition type identifier of a UDF Metadata Partition (UDF 2.50 and later)
        /// </summary>
        public const string MetadataPartitionIdentifier = "*UDF Metadata Partition";

        /// <summary>
        /// Partition type identifier of a UDF Sparable Partition
        /// </summary>
        public const string SparablePartitionIdentifier = "*UDF Sparable Partition";

        /// <summary>
        /// Partition type identifier of a UDF Virtual Partition
        /// </summary>
        public const string VirtualPartitionIdentifier = "*UDF Virtual Partition";

        #endregion

        #region Character Set Constants

        /// <summary>
        /// OSTA Compressed Unicode, 8 bits per character
        /// </summary>
        public const byte CompressionId8 = 8;

        /// <summary>
        /// OSTA Compressed Unicode, 16 bits per character (big-endian)
        /// </summary>
        public const byte CompressionId16 = 16;

        #endregion

        #region Allocation Descriptor Constants

        /// <summary>
        /// Mask of the extent length in an allocation descriptor (the top two bits hold the extent type)
        /// </summary>
        public const uint ExtentLengthMask = 0x3FFFFFFF;

        /// <summary>
        /// Shift of the extent type in an allocation descriptor's extent length
        /// </summary>
        public const int ExtentTypeShift = 30;

        /// <summary>
        /// Mask of the allocation descriptor type in an ICB tag's flags
        /// </summary>
        public const ushort AllocationDescriptorTypeMask = 0x0007;

        #endregion
    }
}
