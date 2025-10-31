namespace SabreTools.Data.Models.COFF.SymbolTableEntries
{
    /// <summary>
    /// Auxiliary Format 2: .bf and .ef Symbols
    ///
    /// For each function definition in the symbol table, three items describe
    /// the beginning, ending, and number of lines. Each of these symbols has
    /// storage class FUNCTION (101):
    ///
    /// A symbol record named .bf (begin function). The Value field is unused.
    ///
    /// A symbol record named .lf (lines in function). The Value field gives the
    /// number of lines in the function.
    ///
    /// A symbol record named .ef (end of function). The Value field has the same
    /// number as the Total Size field in the function-definition symbol record.
    ///
    // The .bf and .ef symbol records (but not .lf records) are followed by an
    // auxiliary record with the following format:
    /// </summary>
    /// <see href="https://learn.microsoft.com/en-us/windows/win32/debug/pe-format"/>
    public class Descriptor : BaseEntry
    {
        /// <summary>
        /// Unused
        /// </summary>
        public uint Unused1 { get; set; }

        /// <summary>
        /// The actual ordinal line number (1, 2, 3, and so on) within the source file,
        /// corresponding to the .bf or .ef record.
        /// </summary>
        public ushort Linenumber { get; set; }

        /// <summary>
        /// Unused
        /// </summary>
        /// <remarks>6 bytes</remarks>
        public byte[] Unused2 { get; set; } = new byte[6];

        /// <summary>
        /// The symbol-table index of the next .bf symbol record. If the function is the
        /// last in the symbol table, this field is set to zero. It is not used for
        /// .ef records.
        /// </summary>
        public uint PointerToNextFunction { get; set; }

        /// <summary>
        /// Unused
        /// </summary>
        public ushort Unused3 { get; set; }
    }
}
