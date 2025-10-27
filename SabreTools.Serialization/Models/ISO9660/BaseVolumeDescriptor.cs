namespace SabreTools.Data.Models.ISO9660
{
    /// <summary>
    /// Abstract Volume Descriptor with common fields used by Primary/Supplementary/Enhanced Volume Descriptors
    /// </summary>
    /// <see cref="https://ecma-international.org/wp-content/uploads/ECMA-119_5th_edition_december_2024.pdf"/>
    public abstract class BaseVolumeDescriptor : VolumeDescriptor
    {
        // Virtual variable of 1 byte goes here
        // PrimaryVolumeDescriptor: UnusedByte
        // SupplementaryVolumeDescriptor: VolumeFlags

        /// <summary>
        /// 32-byte name of the intended system
        /// Primary: a-characters only, padded to the right with spaces
        /// Supplementary: a1-characters only, padded to the right with spaces
        /// Enhanced: Some other agreed upon character encoding, padded to the right with filler
        /// </summary>
        public byte[]? SystemIdentifier { get; set; }

        /// <summary>
        /// 32-byte name of the volume
        /// Primary: d-characters only, padded to the right with spaces
        /// Supplementary: d1-characters only, padded to the right with spaces
        /// Enhanced: Some other agreed upon character encoding, padded to the right with filler
        /// </summary>
        public byte[]? VolumeIdentifier { get; set; }

        /// <summary>
        /// 8 unused bytes at offset 72, should be all 0x00
        /// </summary>
        public byte[]? Unused8Bytes { get; set; }

        /// <summary>
        /// Number of logical blocks in this volume
        /// </summary>
        public BothEndianInt32? VolumeSpaceSize { get; set; }

        // Virtual variable of 32 bytes goes here:
        // PrimaryVolumeDescriptor: Unused32Bytes
        // SupplementaryVolumeDescriptor: EscapeSequences

        /// <summary>
        /// Number of Volumes (discs) in this VolumeSet
        /// </summary>
        public BothEndianInt16? VolumeSetSize { get; set; }

        /// <summary>
        /// Volume (disc) number in this volume set
        /// </summary>
        public BothEndianInt16? VolumeSequenceNumber { get; set; }

        /// <summary>
        /// Number of bytes per logical sector, usually 2048
        /// </summary>
        public BothEndianInt16? LogicalBlockSize { get; set; }

        /// <summary>
        /// Number of bytes in the path table
        /// </summary>
        public BothEndianInt32? PathTableSize { get; set; }

        /// <summary>
        /// Sector number of the start of the little-endian path table, type L
        /// Stored as int32-LSB
        /// </summary>
        public int PathTableLocationL { get; set; }

        /// <summary>
        /// Sector number of the start of the optional little-endian path table, type L
        /// The "optional path table" does not exist if this value is 0
        /// Stored as int32-LSB
        /// </summary>
        public int OptionalPathTableLocationL { get; set; }

        /// <summary>
        /// Sector number of the start of the big-endian path table, type M
        /// Stored as int32-MSB
        /// </summary>
        public int PathTableLocationM { get; set; }

        /// <summary>
        /// Sector number of the start of the optional big-endian path table, type M
        /// The "optional path table" Does not exist if this value is 0
        /// Stored as int32-MSB
        /// </summary>
        public int OptionalPathTableLocationM { get; set; }

        /// <summary>
        /// Root directory entry, 34 bytes
        /// DirectoryIdentifier = 0x00
        /// </summary>
        public DirectoryRecord? RootDirectoryRecord { get; set; }

        /// <summary>
        /// 128-byte name of the volume set
        /// If not specified, all spaces (0x20)
        /// Primary: d-characters only, padded to the right with spaces
        /// Supplementary: d1-characters only, padded to the right with spaces
        /// Enhanced: Some other agreed upon character encoding, padded to the right with filler
        /// </summary>
        public byte[]? VolumeSetIdentifier { get; set; }

        /// <summary>
        /// 128-byte name of the publisher
        /// If specified, starts with 0x5F, followed by filename of file in root directory
        /// If not specified, all spaces (0x20)
        /// Primary: a-characters only, padded to the right with spaces
        /// Supplementary: a1-characters only, padded to the right with spaces
        /// Enhanced: Some other agreed upon character encoding, padded to the right with filler
        /// </summary>
        public byte[]? PublisherIdentifier { get; set; }

        /// <summary>
        /// 128-byte name of the data preparer
        /// If specified, starts with 0x5F, followed by filename of file in root directory
        /// If not specified, all spaces (0x20)
        /// Primary: a-characters only, padded to the right with spaces
        /// Supplementary: a1-characters only, padded to the right with spaces
        /// Enhanced: Some other agreed upon character encoding, padded to the right with filler
        /// </summary>
        public byte[]? DataPreparerIdentifier { get; set; }

        /// <summary>
        /// 128-byte name of the application
        /// If specified, starts with 0x5F, followed by filename of file in root directory
        /// If not specified, all spaces (0x20)
        /// Primary: a-characters only, padded to the right with spaces
        /// Supplementary: a1-characters only, padded to the right with spaces
        /// Enhanced: Some other agreed upon character encoding, padded to the right with filler
        /// </summary>
        public byte[]? ApplicationIdentifier { get; set; }

        /// <summary>
        /// 37-byte filename of the Copyright file
        /// If specified, filename of a file in root directory
        /// If not specified, all spaces (0x20)
        /// Primary: d-characters only, padded to the right with spaces
        /// Supplementary: d1-characters only, padded to the right with spaces
        /// Enhanced: Some other agreed upon character encoding, padded to the right with filler
        /// </summary>
        public byte[]? CopyrightFileIdentifier { get; set; }

        /// <summary>
        /// 37-byte filename of the Abstract file
        /// If specified, filename of a file in root directory
        /// If not specified, all spaces (0x20)
        /// Primary: d-characters only, padded to the right with spaces
        /// Supplementary: d1-characters only, padded to the right with spaces
        /// Enhanced: Some other agreed upon character encoding, padded to the right with filler
        /// </summary>
        public byte[]? AbstractFileIdentifier { get; set; }

        /// <summary>
        /// 37-byte filename of the Bibliographic file
        /// If specified, filename of a file in root directory
        /// If not specified, all spaces (0x20)
        /// Primary: d-characters only, padded to the right with spaces
        /// Supplementary: d1-characters only, padded to the right with spaces
        /// Enhanced: Some other agreed upon character encoding, padded to the right with filler
        /// </summary>
        public byte[]? BibliographicFileIdentifier { get; set; }

        /// <summary>
        /// PVD-style DateTime format for the Creation date/time of the Volume
        /// </summary>
        public DecDateTime? VolumeCreationDateTime { get; set; }

        /// <summary>
        /// PVD-style DateTime format for the Modification date/time of the Volume
        /// </summary>
        public DecDateTime? VolumeModificationDateTime { get; set; }

        /// <summary>
        /// PVD-style DateTime format for the Expiration date/time of the Volume
        /// </summary>
        public DecDateTime? VolumeExpirationDateTime { get; set; }

        /// <summary>
        /// PVD-style DateTime format for the Effective date/time of the Volume
        /// </summary>
        public DecDateTime? VolumeEffectiveDateTime { get; set; }

        /// <summary>
        /// Version number of the Records / Path Table format
        /// For Primary/Supplementary, this is 0x01
        /// For Enhanced, this is 0x02
        /// </summary>
        public byte FileStructureVersion { get; set; }

        /// <summary>
        /// 1 reserved byte, should be 0x00
        /// </summary>
        public byte ReservedByte { get; set; }

        /// <summary>
        /// 512 bytes for Application Use, contents not defined by ISO9660
        /// </summary>
        public byte[]? ApplicationUse { get; set; }

        /// <summary>
        /// 653 reserved bytes, should be all 0x00
        /// </summary>
        public byte[]? Reserved653Bytes { get; set; }
    }
}
