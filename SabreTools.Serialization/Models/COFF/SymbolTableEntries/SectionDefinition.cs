namespace SabreTools.Data.Models.COFF.SymbolTableEntries
{
    /// <summary>
    /// Auxiliary Format 5: Section Definitions
    /// 
    /// This format follows a symbol-table record that defines a section. Such a
    /// record has a symbol name that is the name of a section (such as .text or
    /// .drectve) and has storage class STATIC (3). The auxiliary record provides
    /// information about the section to which it refers. Thus, it duplicates some
    /// of the information in the section header.
    /// </summary>
    /// <see href="https://learn.microsoft.com/en-us/windows/win32/debug/pe-format"/>
    public class SectionDefinition : BaseEntry
    {
        /// <summary>
        /// The size of section data; the same as SizeOfRawData in the section header.
        /// </summary>
        public uint Length { get; set; }

        /// <summary>
        /// The number of relocation entries for the section.
        /// </summary>
        public ushort NumberOfRelocations { get; set; }

        /// <summary>
        /// The number of line-number entries for the section.
        /// </summary>
        public ushort NumberOfLinenumbers { get; set; }

        /// <summary>
        /// The checksum for communal data. It is applicable if the IMAGE_SCN_LNK_COMDAT
        /// flag is set in the section header.
        /// </summary>
        public uint CheckSum { get; set; }

        /// <summary>
        /// One-based index into the section table for the associated section. This is
        /// used when the COMDAT selection setting is 5.
        /// </summary>
        public ushort Number { get; set; }

        /// <summary>
        /// The COMDAT selection number. This is applicable if the section is a
        /// COMDAT section.
        /// </summary>
        public byte Selection { get; set; }

        /// <summary>
        /// Unused
        /// </summary>
        /// <remarks>3 bytes</remarks>
        public byte[]? Unused { get; set; }
    }
}