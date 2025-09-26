namespace SabreTools.Data.Models.COFF.SymbolTableEntries
{
    /// <summary>
    /// Auxiliary Format 6: CLR Token Definition (Object Only)
    /// 
    /// This auxiliary symbol generally follows the IMAGE_SYM_CLASS_CLR_TOKEN. It is
    /// used to associate a token with the COFF symbol table's namespace.
    /// </summary>
    /// <see href="https://learn.microsoft.com/en-us/windows/win32/debug/pe-format"/>
    public class CLRTokenDefinition : BaseEntry
    {
        /// <summary>
        /// Must be IMAGE_AUX_SYMBOL_TYPE_TOKEN_DEF (1).
        /// </summary>
        public byte AuxType { get; set; }

        /// <summary>
        /// Reserved, must be zero.
        /// </summary>
        public byte Reserved1 { get; set; }

        /// <summary>
        /// The symbol index of the COFF symbol to which this CLR token definition refers.
        /// </summary>
        public uint SymbolTableIndex { get; set; }

        /// <summary>
        /// Reserved, must be zero.
        /// </summary>
        /// <remarks>12 bytes</remarks>
        public byte[]? Reserved2 { get; set; }
    }
}