using SabreTools.Numerics;

namespace SabreTools.Data.Models.ISO9660
{
    /// <summary>
    /// High Sierra Standard File Structure Volume Descriptor, the counterpart of the Primary Volume Descriptor
    /// High Sierra Volume Descriptor with VolumeDescriptorType = 0x01
    /// </summary>
    /// <see href="https://github.com/torvalds/linux/blob/master/include/uapi/linux/iso_fs.h"/>
    public sealed class HighSierraPrimaryVolumeDescriptor : HighSierraVolumeDescriptor
    {
        /// <summary>
        /// 1 unused byte at offset 15
        /// </summary>
        public byte UnusedByte { get; set; }

        /// <summary>
        /// 32-byte name of the intended system, a-characters
        /// </summary>
        public byte[] SystemIdentifier { get; set; } = new byte[32];

        /// <summary>
        /// 32-byte name of the volume, d-characters
        /// </summary>
        public byte[] VolumeIdentifier { get; set; } = new byte[32];

        /// <summary>
        /// 8 unused bytes at offset 80
        /// </summary>
        public byte[] Unused8Bytes { get; set; } = new byte[8];

        /// <summary>
        /// Number of logical blocks in this volume
        /// </summary>
        public BothInt32 VolumeSpaceSize { get; set; } = 0;

        /// <summary>
        /// 32 unused bytes at offset 96
        /// </summary>
        public byte[] Unused32Bytes { get; set; } = new byte[32];

        /// <summary>
        /// Number of Volumes (discs) in this VolumeSet
        /// </summary>
        public BothInt16 VolumeSetSize { get; set; } = 0;

        /// <summary>
        /// Volume (disc) number in this volume set
        /// </summary>
        public BothInt16 VolumeSequenceNumber { get; set; } = 0;

        /// <summary>
        /// Number of bytes per logical block, usually 2048
        /// </summary>
        public BothInt16 LogicalBlockSize { get; set; } = 0;

        /// <summary>
        /// Number of bytes in the path table
        /// </summary>
        public BothInt32 PathTableSize { get; set; } = 0;

        /// <summary>
        /// Sector number of the start of the little-endian path table, type L
        /// Stored as int32-LSB
        /// </summary>
        public int PathTableLocationL { get; set; }

        /// <summary>
        /// Sector number of the start of the first optional little-endian path table, type L
        /// The optional path table does not exist if this value is 0
        /// Stored as int32-LSB
        /// </summary>
        public int OptionalPathTableLocationL { get; set; }

        /// <summary>
        /// Sector number of the start of the second optional little-endian path table, type L
        /// The optional path table does not exist if this value is 0
        /// Stored as int32-LSB
        /// </summary>
        public int SecondOptionalPathTableLocationL { get; set; }

        /// <summary>
        /// Sector number of the start of the third optional little-endian path table, type L
        /// The optional path table does not exist if this value is 0
        /// Stored as int32-LSB
        /// </summary>
        public int ThirdOptionalPathTableLocationL { get; set; }

        /// <summary>
        /// Sector number of the start of the big-endian path table, type M
        /// Stored as int32-MSB
        /// </summary>
        public int PathTableLocationM { get; set; }

        /// <summary>
        /// Sector number of the start of the first optional big-endian path table, type M
        /// The optional path table does not exist if this value is 0
        /// Stored as int32-MSB
        /// </summary>
        public int OptionalPathTableLocationM { get; set; }

        /// <summary>
        /// Sector number of the start of the second optional big-endian path table, type M
        /// The optional path table does not exist if this value is 0
        /// Stored as int32-MSB
        /// </summary>
        public int SecondOptionalPathTableLocationM { get; set; }

        /// <summary>
        /// Sector number of the start of the third optional big-endian path table, type M
        /// The optional path table does not exist if this value is 0
        /// Stored as int32-MSB
        /// </summary>
        public int ThirdOptionalPathTableLocationM { get; set; }

        /// <summary>
        /// Root directory entry, 34 bytes
        /// DirectoryIdentifier = 0x00
        /// </summary>
        public DirectoryRecord RootDirectoryRecord { get; set; } = new();

        /// <summary>
        /// 128-byte name of the volume set
        /// </summary>
        public byte[] VolumeSetIdentifier { get; set; } = new byte[128];

        /// <summary>
        /// 128-byte name of the publisher
        /// </summary>
        public byte[] PublisherIdentifier { get; set; } = new byte[128];

        /// <summary>
        /// 128-byte name of the data preparer
        /// </summary>
        public byte[] DataPreparerIdentifier { get; set; } = new byte[128];

        /// <summary>
        /// 128-byte name of the application
        /// </summary>
        public byte[] ApplicationIdentifier { get; set; } = new byte[128];

        /// <summary>
        /// 32-byte filename of the Copyright file
        /// If specified, filename of a file in root directory
        /// </summary>
        public byte[] CopyrightFileIdentifier { get; set; } = new byte[32];

        /// <summary>
        /// 32-byte filename of the Abstract file
        /// If specified, filename of a file in root directory
        /// </summary>
        public byte[] AbstractFileIdentifier { get; set; } = new byte[32];

        /// <summary>
        /// 16-byte PVD-style DateTime for the Creation date/time of the Volume
        /// High Sierra records no time zone offset, TimezoneOffset is 0
        /// </summary>
        public DecDateTime VolumeCreationDateTime { get; set; } = new();

        /// <summary>
        /// 16-byte PVD-style DateTime for the Modification date/time of the Volume
        /// High Sierra records no time zone offset, TimezoneOffset is 0
        /// </summary>
        public DecDateTime VolumeModificationDateTime { get; set; } = new();

        /// <summary>
        /// 16-byte PVD-style DateTime for the Expiration date/time of the Volume
        /// High Sierra records no time zone offset, TimezoneOffset is 0
        /// </summary>
        public DecDateTime VolumeExpirationDateTime { get; set; } = new();

        /// <summary>
        /// 16-byte PVD-style DateTime for the Effective date/time of the Volume
        /// High Sierra records no time zone offset, TimezoneOffset is 0
        /// </summary>
        public DecDateTime VolumeEffectiveDateTime { get; set; } = new();

        /// <summary>
        /// Version number of the Records / Path Table format
        /// </summary>
        public byte FileStructureVersion { get; set; }

        /// <summary>
        /// 1 reserved byte at offset 855
        /// </summary>
        public byte ReservedByte { get; set; }

        /// <summary>
        /// 512 bytes for Application Use
        /// </summary>
        public byte[] ApplicationUse { get; set; } = new byte[512];

        /// <summary>
        /// 680 reserved bytes at offset 1368
        /// </summary>
        public byte[] Reserved680Bytes { get; set; } = new byte[680];
    }
}
