namespace SabreTools.Data.Models.NintendoDisc
{
    /// <see href="https://wiki.tockdom.com/wiki/DOL_(File_Format)"/>
    public class DOLHeader
    {
        /// <summary>
        /// Section offsets indicate where each section's data begins relative
        /// to the start of this header. 0 for unused sections.
        /// </summary>
        /// <remarks>
        /// Big-endian
        /// Indexes 0-6 are text sections.
        /// Indexes 7-17 are data sections.
        /// </remarks>
        public uint[] SectionOffsetTable { get; set; } = new uint[18];

        /// <summary>
        /// Section address indicates where each section should be copied to by
        /// the loader as a virtual memory address. 0 for unused sections.
        /// </summary>
        /// <remarks>
        /// Big-endian
        /// Indexes 0-6 are text sections.
        /// Indexes 7-17 are data sections.
        /// </remarks>
        public uint[] SectionAddressTable { get; set; } = new uint[18];

        /// <summary>
        /// Section lengths indicate the size in bytes of each section.
        /// 0 for unused sections.
        /// </summary>
        /// <remarks>
        /// Big-endian
        /// Indexes 0-6 are text sections.
        /// Indexes 7-17 are data sections.
        /// </remarks>
        public uint[] SectionLengthsTable { get; set; } = new uint[18];

        /// <summary>
        /// bss address indicates the start of the zero initialised (bss) range.
        /// </summary>
        /// <remarks>Big-endian</remarks>
        public uint BSSAddress { get; set; }

        /// <summary>
        /// bss length indicates the size in bytes of the zero initialised (bss) range.
        /// </summary>
        /// <remarks>Big-endian</remarks>
        public uint BSSLength { get; set; }

        /// <summary>
        /// Entry point indicates the virtual memory address of a function to run after
        /// the DOL has been loaded in order to start the program.
        /// This function should not return.
        /// </summary>
        /// <remarks>Big-endian</remarks>
        public uint EntryPoint { get; set; }

        /// <summary>
        /// Padding
        /// </summary>
        public byte[] Padding { get; set; } = new byte[0x1C];
    }
}
