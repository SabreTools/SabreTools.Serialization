namespace SabreTools.Data.Models.UDF
{
    /// <summary>
    /// UDF Primary Volume Descriptor (3/10.1)
    /// </summary>
    /// <see href="https://ecma-international.org/wp-content/uploads/ECMA-167_3rd_edition_june_1997.pdf"/>
    public sealed class PrimaryVolumeDescriptor : Descriptor
    {
        /// <summary>
        /// Volume Descriptor Sequence Number, the highest number prevails
        /// </summary>
        public uint VolumeDescriptorSequenceNumber { get; set; }

        /// <summary>
        /// Primary Volume Descriptor Number
        /// </summary>
        public uint PrimaryVolumeDescriptorNumber { get; set; }

        /// <summary>
        /// Volume identifier, dstring (last byte = length)
        /// </summary>
        public byte[] VolumeIdentifier { get; set; } = new byte[32];

        /// <summary>
        /// Volume sequence number
        /// </summary>
        public ushort VolumeSequenceNumber { get; set; }

        /// <summary>
        /// Maximum volume sequence number
        /// </summary>
        public ushort MaximumVolumeSequenceNumber { get; set; }

        /// <summary>
        /// Interchange level
        /// </summary>
        public ushort InterchangeLevel { get; set; }

        /// <summary>
        /// Maximum interchange level
        /// </summary>
        public ushort MaximumInterchangeLevel { get; set; }

        /// <summary>
        /// Character set list
        /// </summary>
        public uint CharacterSetList { get; set; }

        /// <summary>
        /// Maximum character set list
        /// </summary>
        public uint MaximumCharacterSetList { get; set; }

        /// <summary>
        /// Volume set identifier, dstring (last byte = length)
        /// </summary>
        public byte[] VolumeSetIdentifier { get; set; } = new byte[128];

        /// <summary>
        /// Descriptor character set
        /// </summary>
        public CharacterSetSpecification DescriptorCharacterSet { get; set; } = new();

        /// <summary>
        /// Explanatory character set
        /// </summary>
        public CharacterSetSpecification ExplanatoryCharacterSet { get; set; } = new();

        /// <summary>
        /// Extent of the volume abstract
        /// </summary>
        public ExtentDescriptor VolumeAbstract { get; set; } = new();

        /// <summary>
        /// Extent of the volume copyright notice
        /// </summary>
        public ExtentDescriptor VolumeCopyrightNotice { get; set; } = new();

        /// <summary>
        /// Application identifier
        /// </summary>
        public EntityIdentifier ApplicationIdentifier { get; set; } = new();

        /// <summary>
        /// Recording date and time
        /// </summary>
        public Timestamp RecordingDateAndTime { get; set; } = new();

        /// <summary>
        /// Implementation identifier
        /// </summary>
        public EntityIdentifier ImplementationIdentifier { get; set; } = new();

        /// <summary>
        /// Implementation use, 64 bytes
        /// </summary>
        public byte[] ImplementationUse { get; set; } = new byte[64];

        /// <summary>
        /// Predecessor Volume Descriptor Sequence location
        /// </summary>
        public uint PredecessorVolumeDescriptorSequenceLocation { get; set; }

        /// <summary>
        /// Flags (bit 0: volume set identification is common to all volumes of the set)
        /// </summary>
        public ushort Flags { get; set; }

        /// <summary>
        /// Reserved, 22 bytes of 0x00
        /// </summary>
        public byte[] Reserved { get; set; } = new byte[22];
    }
}
