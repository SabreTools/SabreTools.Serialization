namespace SabreTools.Data.Models.COFF.SymbolTableEntries
{
    /// <summary>
    /// A standard record defines a symbol or name.
    /// </summary>
    /// <see href="https://learn.microsoft.com/en-us/windows/win32/debug/pe-format"/>
    public class StandardRecord : BaseEntry
    {
        #region Symbol Name

        /// <summary>
        /// An array of 8 bytes. This array is padded with nulls on the right if
        /// the name is less than 8 bytes long.
        /// </summary>
        public byte[]? ShortName { get; set; } = new byte[8];

        /// <summary>
        /// A field that is set to all zeros if the name is longer than 8 bytes.
        /// </summary>
        public uint Zeroes { get; set; }

        /// <summary>
        /// An offset into the string table.
        /// </summary>
        public uint Offset { get; set; }

        #endregion

        /// <summary>
        /// The value that is associated with the symbol. The interpretation of this
        /// field depends on SectionNumber and StorageClass. A typical meaning is the
        /// relocatable address.
        /// </summary>
        public uint Value { get; set; }

        /// <summary>
        /// The signed integer that identifies the section, using a one-based index
        /// into the section table. Some values have special meaning.
        /// </summary>
        public SectionNumber SectionNumber { get; set; }

        /// <summary>
        /// A number that represents type. Microsoft tools set this field to 0x20
        /// (function) or 0x0 (not a function).
        /// </summary>
        public SymbolType SymbolType { get; set; }

        /// <summary>
        /// An enumerated value that represents storage class.
        /// </summary>
        public StorageClass StorageClass { get; set; }

        /// <summary>
        /// The number of auxiliary symbol table entries that follow this record.
        /// </summary>
        public byte NumberOfAuxSymbols { get; set; }
    }
}