using System.Collections.Generic;

namespace SabreTools.Data.Models.PortableExecutable
{
    /// <summary>
    /// The following list describes the Microsoft PE executable format, with the
    /// base of the image header at the top. The section from the MS-DOS 2.0
    /// Compatible EXE Header through to the unused section just before the PE header
    /// is the MS-DOS 2.0 Section, and is used for MS-DOS compatibility only.
    /// </summary>
    /// <see href="https://learn.microsoft.com/en-us/windows/win32/debug/pe-format"/>
    public sealed class Executable
    {
        /// <summary>
        /// MS-DOS executable stub
        /// </summary>
        public MSDOS.Executable? Stub { get; set; }

        /// <summary>
        /// After the MS-DOS stub, at the file offset specified at offset 0x3c, is a 4-byte
        /// signature that identifies the file as a PE format image file. This signature is "PE\0\0"
        /// (the letters "P" and "E" followed by two null bytes).
        /// </summary>
        public string? Signature { get; set; }

        /// <summary>
        /// File header
        /// </summary>
        public COFF.FileHeader? FileHeader { get; set; }

        /// <summary>
        /// Microsoft extended optional header
        /// </summary>
        public OptionalHeader? OptionalHeader { get; set; }

        /// <summary>
        /// Section table
        /// </summary>
        public COFF.SectionHeader[]? SectionTable { get; set; }

        /// <summary>
        /// Symbol table
        /// </summary>
        public COFF.SymbolTableEntries.BaseEntry[]? SymbolTable { get; set; }

        /// <summary>
        /// String table
        /// </summary>
        public COFF.StringTable? StringTable { get; set; }

        #region Data Directories

        /// <summary>
        /// The export data section, named .edata, contains information about symbols that other images
        /// can access through dynamic linking. Exported symbols are generally found in DLLs, but DLLs
        /// can also import symbols.
        /// 
        /// An overview of the general structure of the export section is described below. The tables
        /// described are usually contiguous in the file in the order shown (though this is not
        /// required). Only the export directory table and export address table are required to export
        /// symbols as ordinals. (An ordinal is an export that is accessed directly by its export
        /// address table index.) The name pointer table, ordinal table, and export name table all
        /// exist to support use of export names.
        /// </summary>
        /// <see href="https://learn.microsoft.com/en-us/windows/win32/debug/pe-format"/>
        #region Export Table (.edata)

        /// <summary>
        /// A table with just one row (unlike the debug directory). This table indicates the
        /// locations and sizes of the other export tables.
        /// </summary>
        public Export.DirectoryTable? ExportDirectoryTable { get; set; }

        /// <summary>
        /// An array of RVAs of exported symbols. These are the actual addresses of the exported
        /// functions and data within the executable code and data sections. Other image files
        /// can import a symbol by using an index to this table (an ordinal) or, optionally, by
        /// using the public name that corresponds to the ordinal if a public name is defined.
        /// </summary>
        public Export.AddressTableEntry[]? ExportAddressTable { get; set; }

        /// <summary>
        /// An array of pointers to the public export names, sorted in ascending order.
        /// </summary>
        public Export.NamePointerTable? NamePointerTable { get; set; }

        /// <summary>
        /// An array of the ordinals that correspond to members of the name pointer table. The
        /// correspondence is by position; therefore, the name pointer table and the ordinal table
        /// must have the same number of members. Each ordinal is an index into the export address
        /// table.
        /// </summary>
        public Export.OrdinalTable? OrdinalTable { get; set; }

        /// <summary>
        /// A series of null-terminated ASCII strings. Members of the name pointer table point into
        /// this area. These names are the public names through which the symbols are imported and
        /// exported; they are not necessarily the same as the private names that are used within
        /// the image file. 
        /// </summary>
        public Export.NameTable? ExportNameTable { get; set; }

        #endregion

        /// <summary>
        /// All image files that import symbols, including virtually all executable (EXE) files,
        /// have an .idata section. A typical file layout for the import information follows:
        /// 
        ///     - Directory Table
        ///       Null Directory Entry
        ///     - DLL1 Import Lookup Table
        ///       Null
        ///     - DLL2 Import Lookup Table
        ///       Null
        ///     - DLL3 Import Lookup Table
        ///       Null
        ///     - Hint-Name Table
        /// </summary>
        /// <see href="https://learn.microsoft.com/en-us/windows/win32/debug/pe-format"/>
        #region Import Table (.idata) and Import Address Table

        /// <summary>
        /// The import information begins with the import directory table, which describes the
        /// remainder of the import information. 
        /// </summary>
        public Import.DirectoryTableEntry[]? ImportDirectoryTable { get; set; }

        /// <summary>
        /// An import lookup table is an array of 32-bit numbers for PE32 or an array of 64-bit
        /// numbers for PE32+.
        /// </summary>
        public Dictionary<int, Import.LookupTableEntry[]?>? ImportLookupTables { get; set; }

        /// <summary>
        /// These addresses are the actual memory addresses of the symbols, although technically
        /// they are still called "virtual addresses".
        /// </summary>
        public Dictionary<int, Import.AddressTableEntry[]?>? ImportAddressTables { get; set; }

        /// <summary>
        /// One hint/name table suffices for the entire import section.
        /// </summary>
        public Import.HintNameTableEntry[]? HintNameTable { get; set; }

        #endregion

        #region Resource Table (.rsrc)

        /// <summary>
        /// Resource directory table (.rsrc)
        /// </summary>
        public Resource.DirectoryTable? ResourceDirectoryTable { get; set; }

        #endregion

        // TODO: Handle Exception Table

        #region Certificate Table

        /// <summary>
        /// Attribute certificate table
        /// </summary>
        public AttributeCertificate.Entry[]? AttributeCertificateTable { get; set; }

        #endregion

        #region Base Relocation Table (.reloc)

        /// <summary>
        /// Base relocation table
        /// </summary>
        public BaseRelocation.Block[]? BaseRelocationTable { get; set; }

        #endregion

        #region Debug Data (.debug*)

        /// <summary>
        /// Debug table
        /// </summary>
        public DebugData.Table? DebugTable { get; set; }

        #endregion

        // TODO: Handle Architecture
        // TODO: Handle Global Ptr
        // TODO: Thread Local Storage (.tls)
        // TODO: Load Configuration Table
        // TODO: Bound Import Table

        #region Delay Load Table

        /// <summary>
        /// Delay-load directory table
        /// </summary>
        public DelayLoad.DirectoryTable? DelayLoadDirectoryTable { get; set; }

        #endregion

        // TODO: CLR Runtime Header (.cormeta)
        // TODO: Reserved

        #endregion

        #region Named Sections

        // TODO: Support grouped sections in section reading and parsing
        // https://learn.microsoft.com/en-us/windows/win32/debug/pe-format#grouped-sections-object-only
        // Grouped sections are ordered and mean that the data in the sections contributes
        // to the "base" section (the one without the "$X" suffix). This may negatively impact
        // the use of some of the different types of executables.

        // .cormeta - CLR metadata is stored in this section. It is used to indicate that
        // the object file contains managed code. The format of the metadata is not
        // documented, but can be handed to the CLR interfaces for handling metadata.

        // .drectve - A section is a directive section if it has the IMAGE_SCN_LNK_INFO
        // flag set in the section header and has the .drectve section name. The linker
        // removes a .drectve section after processing the information, so the section
        // does not appear in the image file that is being linked.
        //
        // A .drectve section consists of a string of text that can be encoded as ANSI
        // or UTF-8. If the UTF-8 byte order marker (BOM, a three-byte prefix that
        // consists of 0xEF, 0xBB, and 0xBF) is not present, the directive string is
        // interpreted as ANSI. The directive string is a series of linker options that
        // are separated by spaces. Each option contains a hyphen, the option name, and
        // any appropriate attribute. If an option contains spaces, the option must be
        // enclosed in quotes. The .drectve section must not have relocations or line
        // numbers.
        //
        // TODO: Can we implement reading/parsing the .drectve section?

        // .pdata Section  - Multiple formats per entry

        // .sxdata - The valid exception handlers of an object are listed in the .sxdata
        // section of that object. The section is marked IMAGE_SCN_LNK_INFO. It contains
        // the COFF symbol index of each valid handler, using 4 bytes per index.
        //
        // Additionally, the compiler marks a COFF object as registered SEH by emitting
        // the absolute symbol "@feat.00" with the LSB of the value field set to 1. A
        // COFF object with no registered SEH handlers would have the "@feat.00" symbol,
        // but no .sxdata section.
        //
        // TODO: Can we implement reading/parsing the .sxdata section?

        #endregion

        // TODO: Determine if "Archive (Library) File Format" is worth modelling
    }
}
