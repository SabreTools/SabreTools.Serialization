namespace SabreTools.Data.Models.ISO9660
{
    /// <summary>
    /// Abstract Volume Descriptor with common fields used by Primary/Supplementary/Enhanced Volume Descriptors
    /// </summary>
    /// <see cref="https://ecma-international.org/wp-content/uploads/ECMA-119_5th_edition_december_2024.pdf"/>
    public abstract class BaseVolumeDescriptor : VolumeDescriptor
    {
        /// <summary>
        /// 32-byte name of the intended system
        /// Primary: a-characters only, padded to the right with spaces
        /// Supplementary: a1-characters only, padded to the right with spaces
        /// Enhanced: Some other agreed upon character encoding, padded to the right with filler
        /// </summary>
        public byte[] SystemIdentifier { get; set; }

        /// <summary>
        /// 32-byte name of the volume
        /// Primary: d-characters only, padded to the right with spaces
        /// Supplementary: d1-characters only, padded to the right with spaces
        /// Enhanced: Some other agreed upon character encoding, padded to the right with filler
        /// </summary>
        public byte[] VolumeIdentifier { get; set; }

        /// <summary>
        /// 8 unused bytes at offset 72, should be all 0x00
        /// </summary>
        public byte[] Unused8Bytes { get; set; }

        /// <summary>
        /// Number of logical blocks in this volume
        /// Stored as int32-LSB followed by int32-MSB
        /// </summary>
        public int VolumeSpaceSize { get; set; }

        /// <summary>
        /// Number of "disks" in this volume
        /// Stored as int16-LSB followed by int16-MSB
        /// </summary>
        public short VolumeSetSize { get; set; }

        /// <summary>
        /// "Disk" number in this volume set
        /// Stored as int16-LSB followed by int16-MSB
        /// </summary>
        public short VolumeSequenceNumber { get; set; }

        /// <summary>
        /// Number of bytes per logical sector, usually 2048
        /// Stored as int16-LSB followed by int16-MSB
        /// </summary>
        public short LogicalBlockSize { get; set; }

        /// <summary>
        /// Number of bytes in the path table
        /// </summary>
        public int PathTableSize { get; set; }

        /// <summary>
        /// Sector number of the start of the little-endian path table
        /// Stored as int32-LSB
        /// </summary>
        public int PathTableLocationLE { get; set; }

        /// <summary>
        /// Sector number of the start of the optional little-endian path table
        /// Optional: Does not exist if value is 0
        /// Stored as int32-LSB
        /// </summary>
        public int PathTableLocationLEOptional { get; set; }

        /// <summary>
        /// Sector number of the start of the big-endian path table
        /// Stored as int32-MSB
        /// </summary>
        public int PathTableLocationBE { get; set; }

        /// <summary>
        /// Sector number of the start of the optional big-endian path table
        /// Optional: Does not exist if value is 0
        /// Stored as int32-MSB
        /// </summary>
        public int PathTableLocationBEOptional { get; set; }

        /// <summary>
        /// Root directory entry, 34 bytes
        /// DirectoryIdentifier = 0x00
        /// </summary>
        public DirectoryRecord RootDirectory { get; set; }

        /// <summary>
        /// 128-byte name of the volume set
        /// If not specified, all spaces (0x20)
        /// Primary: d-characters only, padded to the right with spaces
        /// Supplementary: d1-characters only, padded to the right with spaces
        /// Enhanced: Some other agreed upon character encoding, padded to the right with filler
        /// </summary>
        public byte[] VolumeSetIdentifier { get; set; }

        /// <summary>
        /// 128-byte name of the publisher
        /// If specified, starts with 0x5F, followed by filename of file in root directory
        /// If not specified, all spaces (0x20)
        /// Primary: a-characters only, padded to the right with spaces
        /// Supplementary: a1-characters only, padded to the right with spaces
        /// Enhanced: Some other agreed upon character encoding, padded to the right with filler
        /// </summary>
        public byte[] PublisherIdentifier { get; set; }

        /// <summary>
        /// 128-byte name of the data preparer
        /// If specified, starts with 0x5F, followed by filename of file in root directory
        /// If not specified, all spaces (0x20)
        /// Primary: a-characters only, padded to the right with spaces
        /// Supplementary: a1-characters only, padded to the right with spaces
        /// Enhanced: Some other agreed upon character encoding, padded to the right with filler
        /// </summary>
        public byte[] DataPreparerIdentifier { get; set; }

        /// <summary>
        /// 128-byte name of the application
        /// If specified, starts with 0x5F, followed by filename of file in root directory
        /// If not specified, all spaces (0x20)
        /// Primary: a-characters only, padded to the right with spaces
        /// Supplementary: a1-characters only, padded to the right with spaces
        /// Enhanced: Some other agreed upon character encoding, padded to the right with filler
        /// </summary>
        public byte[] ApplicationIdentifier { get; set; }

        /// <summary>
        /// 37-byte filename of the Copyright file
        /// If specified, filename of a file in root directory
        /// If not specified, all spaces (0x20)
        /// Primary: d-characters only, padded to the right with spaces
        /// Supplementary: d1-characters only, padded to the right with spaces
        /// Enhanced: Some other agreed upon character encoding, padded to the right with filler
        /// </summary>
        public byte[] CopyrightFileIdentifier { get; set; }

        /// <summary>
        /// 37-byte filename of the Abstract file
        /// If specified, filename of a file in root directory
        /// If not specified, all spaces (0x20)
        /// Primary: d-characters only, padded to the right with spaces
        /// Supplementary: d1-characters only, padded to the right with spaces
        /// Enhanced: Some other agreed upon character encoding, padded to the right with filler
        /// </summary>
        public byte[] AbstractFileIdentifier { get; set; }

        /// <summary>
        /// 37-byte filename of the Bibliographic file
        /// If specified, filename of a file in root directory
        /// If not specified, all spaces (0x20)
        /// Primary: d-characters only, padded to the right with spaces
        /// Supplementary: d1-characters only, padded to the right with spaces
        /// Enhanced: Some other agreed upon character encoding, padded to the right with filler
        /// </summary>
        public byte[] BibliographicFileIdentifier { get; set; }

        /// <summary>
        /// PVD-style DateTime format for the Creation date/time of the Volume
        /// </summary>
        public VolumeDescriptorDateTime VolumeCreationDateTime { get; set; }

        /// <summary>
        /// PVD-style DateTime format for the Modification date/time of the Volume
        /// </summary>
        public VolumeDescriptorDateTime VolumeModificationDateTime { get; set; }

        /// <summary>
        /// PVD-style DateTime format for the Expiration date/time of the Volume
        /// </summary>
        public VolumeDescriptorDateTime VolumeExpirationDateTime { get; set; }

        /// <summary>
        /// PVD-style DateTime format for the Effective date/time of the Volume
        /// </summary>
        public VolumeDescriptorDateTime VolumeEffectiveDateTime { get; set; }

        /// <summary>
        /// Version number of the Records / Path Table format, always 0x01
        /// </summary>
        public byte FileStructureVersion { get; set; }

        /// <summary>
        /// 1 reserved byte, should be 0x00
        /// </summary>
        public byte ReservedByte { get; set; }

        /// <summary>
        /// 512 bytes for Application Use, contents not defined by ISO9660
        /// </summary>
        public byte[] ApplicationUse { get; set; }

        /// <summary>
        /// 653 reserved bytes, should be all 0x00
        /// </summary>
        public byte[] Reserved653Bytes { get; set; }
    }
}
