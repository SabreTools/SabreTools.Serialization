namespace SabreTools.Data.Models.COFF.SymbolTableEntries
{
    /// <summary>
    /// Auxiliary Format 3: Weak Externals
    ///
    /// "Weak externals" are a mechanism for object files that allows flexibility at
    /// link time. A module can contain an unresolved external symbol (sym1), but it
    /// can also include an auxiliary record that indicates that if sym1 is not
    /// present at link time, another external symbol (sym2) is used to resolve
    /// references instead.
    ///
    /// If a definition of sym1 is linked, then an external reference to the symbol
    /// is resolved normally. If a definition of sym1 is not linked, then all references
    /// to the weak external for sym1 refer to sym2 instead. The external symbol, sym2,
    /// must always be linked; typically, it is defined in the module that contains
    /// the weak reference to sym1.
    ///
    /// Weak externals are represented by a symbol table record with EXTERNAL storage
    /// class, UNDEF section number, and a value of zero. The weak-external symbol
    /// record is followed by an auxiliary record with the following format:
    /// </summary>
    /// <see href="https://learn.microsoft.com/en-us/windows/win32/debug/pe-format"/>
    public class WeakExternal : BaseEntry
    {
        /// <summary>
        /// The symbol-table index of sym2, the symbol to be linked if sym1 is not found.
        /// </summary>
        public uint TagIndex { get; set; }

        /// <summary>
        /// A value of IMAGE_WEAK_EXTERN_SEARCH_NOLIBRARY indicates that no library search
        /// for sym1 should be performed.
        /// A value of IMAGE_WEAK_EXTERN_SEARCH_LIBRARY indicates that a library search for
        /// sym1 should be performed.
        /// A value of IMAGE_WEAK_EXTERN_SEARCH_ALIAS indicates that sym1 is an alias for sym2.
        /// </summary>
        public uint Characteristics { get; set; }

        /// <summary>
        /// Unused
        /// </summary>
        /// <remarks>10 bytes</remarks>
        public byte[] Unused { get; set; } = new byte[10];
    }
}
