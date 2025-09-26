namespace SabreTools.Serialization.Models.COFF.SymbolTableEntries
{
    /// <summary>
    /// The symbol table in this section is inherited from the traditional
    /// COFF format. It is distinct from Microsoft Visual C++ debug information.
    /// A file can contain both a COFF symbol table and Visual C++ debug
    /// information, and the two are kept separate. Some Microsoft tools use
    /// the symbol table for limited but important purposes, such as
    /// communicating COMDAT information to the linker. Section names and file
    /// names, as well as code and data symbols, are listed in the symbol table.
    /// 
    /// The location of the symbol table is indicated in the COFF header.
    /// 
    /// The symbol table is an array of records, each 18 bytes long. Each record
    /// is either a standard or auxiliary symbol-table record. A standard record
    /// defines a symbol or name.
    /// 
    /// Auxiliary symbol table records always follow, and apply to, some standard
    /// symbol table record. An auxiliary record can have any format that the tools
    /// can recognize, but 18 bytes must be allocated for them so that symbol table
    /// is maintained as an array of regular size. Currently, Microsoft tools
    /// recognize auxiliary formats for the following kinds of records: function
    /// definitions, function begin and end symbols (.bf and .ef), weak externals,
    /// file names, and section definitions.
    /// 
    /// The traditional COFF design also includes auxiliary-record formats for arrays
    /// and structures.Microsoft tools do not use these, but instead place that
    /// symbolic information in Visual C++ debug format in the debug sections.
    /// </summary>
    /// <see href="https://learn.microsoft.com/en-us/windows/win32/debug/pe-format"/>
    public abstract class BaseEntry { }
}
