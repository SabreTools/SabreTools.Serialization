namespace SabreTools.Data.Models.UDF
{
    /// <summary>
    /// UDF Information Control Block Tag (icbtag, 4/14.6)
    /// </summary>
    /// <see href="https://ecma-international.org/wp-content/uploads/ECMA-167_3rd_edition_june_1997.pdf"/>
    public sealed class ICBTag
    {
        /// <summary>
        /// Prior recorded number of direct entries
        /// </summary>
        public uint PriorRecordedNumberOfDirectEntries { get; set; }

        /// <summary>
        /// Strategy type (UDF: 4, or 4096 for write-once media)
        /// </summary>
        public ushort StrategyType { get; set; }

        /// <summary>
        /// Strategy parameter, 2 bytes
        /// </summary>
        public byte[] StrategyParameter { get; set; } = new byte[2];

        /// <summary>
        /// Maximum number of entries
        /// </summary>
        public ushort MaximumNumberOfEntries { get; set; }

        /// <summary>
        /// Reserved, 0x00
        /// </summary>
        public byte Reserved { get; set; }

        /// <summary>
        /// File type
        /// </summary>
        public FileType FileType { get; set; }

        /// <summary>
        /// Parent ICB location
        /// </summary>
        public LogicalBlockAddress ParentICBLocation { get; set; } = new();

        /// <summary>
        /// Flags, the lower 3 bits hold the AllocationDescriptorType
        /// </summary>
        public ushort Flags { get; set; }
    }
}
