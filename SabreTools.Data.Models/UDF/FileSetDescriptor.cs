namespace SabreTools.Data.Models.UDF
{
    /// <summary>
    /// UDF File Set Descriptor (4/14.1), the location of the root directory
    /// </summary>
    /// <see href="https://ecma-international.org/wp-content/uploads/ECMA-167_3rd_edition_june_1997.pdf"/>
    public sealed class FileSetDescriptor : Descriptor
    {
        /// <summary>
        /// Recording date and time
        /// </summary>
        public Timestamp RecordingDateAndTime { get; set; } = new();

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
        /// File set number
        /// </summary>
        public uint FileSetNumber { get; set; }

        /// <summary>
        /// File Set Descriptor number
        /// </summary>
        public uint FileSetDescriptorNumber { get; set; }

        /// <summary>
        /// Logical volume identifier character set
        /// </summary>
        public CharacterSetSpecification LogicalVolumeIdentifierCharacterSet { get; set; } = new();

        /// <summary>
        /// Logical volume identifier, dstring (last byte = length)
        /// </summary>
        public byte[] LogicalVolumeIdentifier { get; set; } = new byte[128];

        /// <summary>
        /// File set character set
        /// </summary>
        public CharacterSetSpecification FileSetCharacterSet { get; set; } = new();

        /// <summary>
        /// File set identifier, dstring (last byte = length)
        /// </summary>
        public byte[] FileSetIdentifier { get; set; } = new byte[32];

        /// <summary>
        /// Copyright file identifier, dstring (last byte = length)
        /// </summary>
        public byte[] CopyrightFileIdentifier { get; set; } = new byte[32];

        /// <summary>
        /// Abstract file identifier, dstring (last byte = length)
        /// </summary>
        public byte[] AbstractFileIdentifier { get; set; } = new byte[32];

        /// <summary>
        /// ICB of the root directory
        /// </summary>
        public LongAllocationDescriptor RootDirectoryICB { get; set; } = new();

        /// <summary>
        /// Domain identifier
        /// </summary>
        public EntityIdentifier DomainIdentifier { get; set; } = new();

        /// <summary>
        /// Next extent of File Set Descriptors, length 0 if none
        /// </summary>
        public LongAllocationDescriptor NextExtent { get; set; } = new();

        /// <summary>
        /// ICB of the system stream directory (ECMA-167 3rd edition), length 0 if none
        /// </summary>
        public LongAllocationDescriptor SystemStreamDirectoryICB { get; set; } = new();

        /// <summary>
        /// Reserved, 32 bytes of 0x00
        /// </summary>
        public byte[] Reserved { get; set; } = new byte[32];
    }
}
