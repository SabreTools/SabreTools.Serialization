using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SabreTools.IO.Extensions;
using SabreTools.Models.PortableExecutable;
using SabreTools.Models.PortableExecutable.COFFSymbolTableEntries;
using static SabreTools.Models.PortableExecutable.Constants;

namespace SabreTools.Serialization.Deserializers
{
    public class PortableExecutable : BaseBinaryDeserializer<Executable>
    {
        /// <inheritdoc/>
        public override Executable? Deserialize(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                // Cache the current offset
                long initialOffset = data.Position;

                // Create a new executable to fill
                var pex = new Executable();

                #region MS-DOS Stub

                // Parse the MS-DOS stub
                var stub = new MSDOS().Deserialize(data);
                if (stub?.Header == null || stub.Header.NewExeHeaderAddr == 0)
                    return null;

                // Set the MS-DOS stub
                pex.Stub = stub;

                #endregion

                #region Signature

                // Get the new executable offset
                long newExeOffset = initialOffset + stub.Header.NewExeHeaderAddr;
                if (newExeOffset < initialOffset || newExeOffset > data.Length)
                    return null;

                // Try to parse the executable header
                data.Seek(newExeOffset, SeekOrigin.Begin);
                byte[] signature = data.ReadBytes(4);
                pex.Signature = Encoding.ASCII.GetString(signature);
                if (pex.Signature != SignatureString)
                    return null;

                #endregion

                #region COFF File Header

                // Parse the COFF file header
                var coffFileHeader = ParseCOFFFileHeader(data);
                if (coffFileHeader == null)
                    return null;
                if (coffFileHeader.NumberOfSections > 96)
                    return null;

                // Set the COFF file header
                pex.COFFFileHeader = coffFileHeader;

                #endregion

                #region Optional Header

                // If the optional header exists
                OptionalHeader? optionalHeader = null;
                if (coffFileHeader.SizeOfOptionalHeader > 0)
                {
                    // Parse the optional header
                    optionalHeader = ParseOptionalHeader(data, coffFileHeader.SizeOfOptionalHeader);

                    // Set the optional header
                    if (optionalHeader != null)
                        pex.OptionalHeader = optionalHeader;
                }

                #endregion

                #region Section Table

                // TODO: Technically this needs to be seeked to
                // It should be found by taking the the position after the
                // signature and COFF file header and then adding the size
                // of the optional header. This is effectively what the code
                // is already doing since it is parsed after the optional
                // header and not guessing otherwise.

                // Set the section table
                pex.SectionTable = new SectionHeader[coffFileHeader.NumberOfSections];
                for (int i = 0; i < coffFileHeader.NumberOfSections; i++)
                {
                    pex.SectionTable[i] = ParseSectionHeader(data);
                }

                #endregion

                #region COFF Symbol Table and COFF String Table

                long offset = initialOffset + coffFileHeader.PointerToSymbolTable;
                if (offset > initialOffset && offset < data.Length)
                {
                    // Seek to the COFF symbol table
                    data.Seek(offset, SeekOrigin.Begin);

                    // Set the COFF symbol and string tables
                    pex.COFFSymbolTable = ParseCOFFSymbolTable(data, coffFileHeader.NumberOfSymbols);
                    pex.COFFStringTable = ParseCOFFStringTable(data);
                }

                #endregion

                // All tables require the optional header to exist
                if (optionalHeader == null)
                    return pex;

                #region Export Table

                // Should also be in a '.edata' section
                if (optionalHeader.ExportTable != null)
                {
                    offset = initialOffset
                        + optionalHeader.ExportTable.VirtualAddress.ConvertVirtualAddress(pex.SectionTable);
                    if (offset > initialOffset && offset < data.Length)
                    {
                        // Get the required table size
                        int tableSize = (int)optionalHeader.ExportTable.Size;

                        // Read the table data
                        byte[]? tableData = data.ReadFrom(offset, tableSize, retainPosition: true);

                        // Set the export table
                        pex.ExportTable = ParseExportTable(tableData);

                        // If the export table was parsed, read the remaining pieces
                        if (pex.ExportTable?.ExportDirectoryTable != null)
                        {
                            // Cache the export directory table for easier access
                            var exportDirectoryTable = pex.ExportTable.ExportDirectoryTable;

                            // Name
                            offset = initialOffset + exportDirectoryTable.NameRVA.ConvertVirtualAddress(pex.SectionTable);
                            if (offset > initialOffset && offset < data.Length)
                            {
                                data.Seek(offset, SeekOrigin.Begin);
                                exportDirectoryTable.Name = data.ReadNullTerminatedAnsiString();
                            }

                            // Address table
                            offset = initialOffset + exportDirectoryTable.ExportAddressTableRVA.ConvertVirtualAddress(pex.SectionTable);
                            if (exportDirectoryTable.AddressTableEntries != 0
                                && offset > initialOffset
                                && offset < data.Length)
                            {
                                data.Seek(offset, SeekOrigin.Begin);
                                pex.ExportTable.ExportAddressTable = ParseExportAddressTable(data, exportDirectoryTable.AddressTableEntries);
                            }

                            // Name pointer table
                            offset = initialOffset + exportDirectoryTable.NamePointerRVA.ConvertVirtualAddress(pex.SectionTable);
                            if (exportDirectoryTable.NumberOfNamePointers != 0
                                && offset > initialOffset
                                && offset < data.Length)
                            {
                                data.Seek(offset, SeekOrigin.Begin);
                                pex.ExportTable.NamePointerTable = ParseExportNamePointerTable(data, exportDirectoryTable.NumberOfNamePointers);
                            }

                            // Ordinal table
                            offset = initialOffset + exportDirectoryTable.OrdinalTableRVA.ConvertVirtualAddress(pex.SectionTable);
                            if (exportDirectoryTable.NumberOfNamePointers != 0
                                && offset > initialOffset
                                && offset < data.Length)
                            {
                                data.Seek(offset, SeekOrigin.Begin);
                                pex.ExportTable.OrdinalTable = ParseExportOrdinalTable(data, exportDirectoryTable.NumberOfNamePointers);
                            }

                            // Name table
                            if (exportDirectoryTable.NumberOfNamePointers != 0 && pex.ExportTable.NamePointerTable?.Pointers != null)
                                pex.ExportTable.ExportNameTable = ParseExportNameTable(data, initialOffset, pex.ExportTable.NamePointerTable.Pointers, pex.SectionTable);
                        }
                    }
                }

                #endregion

                #region Import Table

                // Should also be in a '.idata' section
                if (optionalHeader.ImportTable != null)
                {
                    offset = initialOffset
                        + optionalHeader.ImportTable.VirtualAddress.ConvertVirtualAddress(pex.SectionTable);
                    if (offset > initialOffset && offset < data.Length)
                    {
                        // Get the required table size
                        int tableSize = (int)optionalHeader.ImportTable.Size;

                        // Read the table data
                        byte[]? tableData = data.ReadFrom(offset, tableSize, retainPosition: true);

                        // Set the import table
                        pex.ImportTable = ParseImportTable(tableData);

                        // If the export table was parsed, read the remaining pieces
                        if (pex.ImportTable?.ImportDirectoryTable != null)
                        {
                            // Cache the import directory table for easier access
                            var importDirectoryTable = pex.ImportTable.ImportDirectoryTable;

                            // Names
                            for (int i = 0; i < importDirectoryTable.Length; i++)
                            {
                                var entry = importDirectoryTable[i];
                                if (entry == null)
                                    continue;

                                long nameOffset = initialOffset + entry.NameRVA.ConvertVirtualAddress(pex.SectionTable);
                                if (nameOffset > initialOffset && nameOffset < data.Length)
                                {
                                    data.Seek(nameOffset, SeekOrigin.Begin);
                                    entry.Name = data.ReadNullTerminatedAnsiString();
                                }
                            }

                            pex.ImportTable.ImportLookupTables = ParseImportLookupTables(data,
                                initialOffset,
                                optionalHeader.Magic,
                                importDirectoryTable,
                                pex.SectionTable);
                            pex.ImportTable.ImportAddressTables = ParseImportAddressTables(data,
                                initialOffset,
                                optionalHeader.Magic,
                                importDirectoryTable,
                                pex.SectionTable);
                            pex.ImportTable.HintNameTable = ParseHintNameTable(data,
                                initialOffset,
                                pex.ImportTable.ImportLookupTables,
                                pex.ImportTable.ImportAddressTables,
                                pex.SectionTable);
                        }
                    }
                }

                #endregion

                #region Resource Directory Table

                // Should also be in a '.rsrc' section
                if (optionalHeader.ResourceTable != null)
                {
                    offset = initialOffset
                        + optionalHeader.ResourceTable.VirtualAddress.ConvertVirtualAddress(pex.SectionTable);
                    if (offset > initialOffset && offset < data.Length)
                    {
                        // Get the required table size
                        int tableSize = (int)optionalHeader.ResourceTable.Size;
                        long paddingSize = optionalHeader.FileAlignment - ((offset + tableSize) % optionalHeader.FileAlignment);
                        tableSize += (int)paddingSize;
                        tableSize = (int)Math.Min(tableSize, data.Length - offset);

                        // Read the table data
                        byte[]? tableData = data.ReadFrom(offset, tableSize, retainPosition: true);

                        // Set the resource directory table
                        long tableStart = data.Position;
                        int tableOffset = 0;
                        pex.ResourceDirectoryTable = ParseResourceDirectoryTable(tableData, ref tableOffset);

                        // Parse the resource data, if possible
                        ParseResourceData(data, initialOffset, pex.ResourceDirectoryTable, pex.SectionTable);

                        #region Hidden Resources

                        // If we have not used up the full size, parse the remaining chunk as a single resource
                        if (pex.ResourceDirectoryTable?.Entries != null && tableOffset < tableSize)
                        {
                            // Resize the entry array to accomodate one more
                            var localEntries = pex.ResourceDirectoryTable.Entries;
                            Array.Resize(ref localEntries, localEntries.Length + 1);
                            pex.ResourceDirectoryTable.Entries = localEntries;

                            // Get the length of the remaining data
                            int length = tableSize - tableOffset;

                            // Add the hidden entry
                            pex.ResourceDirectoryTable.Entries[localEntries.Length - 1] = new ResourceDirectoryEntry
                            {
                                Name = new ResourceDirectoryString { UnicodeString = Encoding.Unicode.GetBytes("HIDDEN RESOURCE") },
                                IntegerID = uint.MaxValue,
                                DataEntryOffset = (uint)data.Position,
                                DataEntry = new ResourceDataEntry
                                {
                                    Size = (uint)length,
                                    Data = data.ReadBytes(length),
                                    Codepage = (uint)Encoding.Unicode.CodePage,
                                },
                            };
                        }

                        #endregion
                    }
                }

                #endregion

                // TODO: Exception Table

                #region Certificate Table

                if (optionalHeader.CertificateTable != null)
                {
                    offset = initialOffset
                        + optionalHeader.CertificateTable.VirtualAddress.ConvertVirtualAddress(pex.SectionTable);
                    if (offset > initialOffset && offset < data.Length)
                    {
                        // Get the required table size
                        int tableSize = (int)optionalHeader.CertificateTable.Size;

                        // Read the table data
                        byte[]? tableData = data.ReadFrom(offset, tableSize, retainPosition: true);

                        // Set the attribute certificate table
                        pex.AttributeCertificateTable = ParseAttributeCertificateTable(tableData);
                    }
                }

                #endregion

                #region Base Relocation Table

                // Should also be in a '.reloc' section
                if (optionalHeader.BaseRelocationTable != null)
                {
                    offset = initialOffset
                        + optionalHeader.BaseRelocationTable.VirtualAddress.ConvertVirtualAddress(pex.SectionTable);
                    if (offset > initialOffset && offset < data.Length)
                    {
                        // Get the required table size
                        int tableSize = (int)optionalHeader.BaseRelocationTable.Size;

                        // Read the table data
                        byte[]? tableData = data.ReadFrom(offset, tableSize, retainPosition: true);

                        // Set the base relocation table
                        pex.BaseRelocationTable = ParseBaseRelocationTable(tableData);
                    }
                }

                #endregion

                #region Debug Table

                // Should also be in a '.debug' section
                if (optionalHeader.Debug != null)
                {
                    offset = initialOffset
                        + optionalHeader.Debug.VirtualAddress.ConvertVirtualAddress(pex.SectionTable);
                    if (offset > initialOffset && offset < data.Length)
                    {
                        // Get the required table size
                        int tableSize = (int)optionalHeader.Debug.Size;

                        // Read the table data
                        byte[]? tableData = data.ReadFrom(offset, tableSize, retainPosition: true);

                        // Set the debug table
                        pex.DebugTable = ParseDebugTable(tableData);
                    }
                }

                #endregion

                // TODO: Architecture Table
                // TODO: Global Pointer Register
                // TODO: Thread Local Storage (TLS) Table (.tls section)
                // TODO: Load Config Table
                // TODO: Bound Import Table
                // TODO: Import Address Table

                #region Delay-Load Directory Table

                if (optionalHeader.DelayImportDescriptor != null)
                {
                    offset = initialOffset
                        + optionalHeader.DelayImportDescriptor.VirtualAddress.ConvertVirtualAddress(pex.SectionTable);
                    if (offset > initialOffset && offset < data.Length)
                    {
                        // Get the required table size
                        int tableSize = (int)optionalHeader.DelayImportDescriptor.Size;

                        // Read the table data
                        byte[]? tableData = data.ReadFrom(offset, tableSize, retainPosition: true);

                        // Set the delay-load directory table
                        pex.DelayLoadDirectoryTable = ParseDelayLoadDirectoryTable(tableData);
                    }
                }

                #endregion

                // TODO: CLR Runtime Header
                // TODO: Reserved

                return pex;
            }
            catch
            {
                // Ignore the actual error
                return null;
            }
        }

        /// <summary>
        /// Parse a byte array into an attribute certificate table
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <returns>Filled attribute certificate on success, null on error</returns>
        public static AttributeCertificateTableEntry[]? ParseAttributeCertificateTable(byte[]? data)
        {
            if (data == null)
                return null;

            var obj = new List<AttributeCertificateTableEntry>();

            int offset = 0;
            while (offset < data.Length)
            {
                var entry = ParseAttributeCertificateTableEntry(data, ref offset);
                obj.Add(entry);

                // Align to the QWORD boundary if we're not at the end
                while (offset < data.Length && offset % 8 != 0)
                {
                    offset++;
                }
            }

            return [.. obj];
        }

        /// <summary>
        /// Parse a byte array into an AttributeCertificateTableEntry
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <returns>Filled AttributeCertificateTableEntry on success, null on error</returns>
        public static AttributeCertificateTableEntry ParseAttributeCertificateTableEntry(byte[] data, ref int offset)
        {
            var obj = new AttributeCertificateTableEntry();

            obj.Length = data.ReadUInt32LittleEndian(ref offset);
            obj.Revision = (WindowsCertificateRevision)data.ReadUInt16LittleEndian(ref offset);
            obj.CertificateType = (WindowsCertificateType)data.ReadUInt16LittleEndian(ref offset);

            int certificateDataLength = (int)obj.Length - 8;
            if (certificateDataLength > 0 && offset + certificateDataLength <= data.Length)
                obj.Certificate = data.ReadBytes(ref offset, certificateDataLength);

            return obj;
        }

        /// <summary>
        /// Parse a byte array into an BaseRelocationBlock
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>Filled BaseRelocationBlock on success, null on error</returns>
        public static BaseRelocationBlock? ParseBaseRelocationBlock(byte[] data, ref int offset)
        {
            var obj = new BaseRelocationBlock();

            obj.PageRVA = data.ReadUInt32LittleEndian(ref offset);
            obj.BlockSize = data.ReadUInt32LittleEndian(ref offset);
            if (obj.BlockSize <= 8)
                return obj;

            // Guard against invalid block sizes
            if (obj.BlockSize % 2 != 0)
                return obj;
            if (offset + obj.BlockSize > data.Length)
                return obj;

            int entryCount = ((int)obj.BlockSize - 8) / 2;
            obj.TypeOffsetFieldEntries = new BaseRelocationTypeOffsetFieldEntry[entryCount];
            for (int i = 0; i < obj.TypeOffsetFieldEntries.Length; i++)
            {
                if (offset + 2 >= data.Length)
                    break;

                obj.TypeOffsetFieldEntries[i] = ParseBaseRelocationTypeOffsetFieldEntry(data, ref offset);
            }

            return obj;
        }

        /// <summary>
        /// Parse a byte array into a base relocation table
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <returns>Filled base relocation table on success, null on error</returns>
        public static BaseRelocationBlock[]? ParseBaseRelocationTable(byte[]? data)
        {
            if (data == null)
                return null;

            var obj = new List<BaseRelocationBlock>();

            int offset = 0;
            while (offset < data.Length)
            {
                var block = ParseBaseRelocationBlock(data, ref offset);
                if (block == null)
                    break;

                obj.Add(block);

                // Align to the DWORD boundary if we're not at the end
                while (offset < data.Length && offset % 4 != 0)
                {
                    offset++;
                }
            }

            return [.. obj];
        }

        /// <summary>
        /// Parse a byte array into a BaseRelocationTypeOffsetFieldEntry
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>Filled BaseRelocationTypeOffsetFieldEntry on success, null on error</returns>
        public static BaseRelocationTypeOffsetFieldEntry ParseBaseRelocationTypeOffsetFieldEntry(byte[] data, ref int offset)
        {
            var obj = new BaseRelocationTypeOffsetFieldEntry();

            ushort typeAndOffsetField = data.ReadUInt16LittleEndian(ref offset);
            obj.BaseRelocationType = (BaseRelocationTypes)(typeAndOffsetField >> 12);
            obj.Offset = (ushort)(typeAndOffsetField & 0x0FFF);

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a CLRTokenDefinition
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled CLRTokenDefinition on success, null on error</returns>
        public static CLRTokenDefinition ParseCLRTokenDefinition(Stream data)
        {
            var obj = new CLRTokenDefinition();

            obj.AuxType = data.ReadByteValue();
            obj.Reserved1 = data.ReadByteValue();
            obj.SymbolTableIndex = data.ReadUInt32LittleEndian();
            obj.Reserved2 = data.ReadBytes(12);

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a COFFFileHeader
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled COFFFileHeader on success, null on error</returns>
        public static COFFFileHeader ParseCOFFFileHeader(Stream data)
        {
            var obj = new COFFFileHeader();

            obj.Machine = (MachineType)data.ReadUInt16LittleEndian();
            obj.NumberOfSections = data.ReadUInt16LittleEndian();
            obj.TimeDateStamp = data.ReadUInt32LittleEndian();
            obj.PointerToSymbolTable = data.ReadUInt32LittleEndian();
            obj.NumberOfSymbols = data.ReadUInt32LittleEndian();
            obj.SizeOfOptionalHeader = data.ReadUInt16LittleEndian();
            obj.Characteristics = (Characteristics)data.ReadUInt16LittleEndian();

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a COFFLineNumber
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled COFFLineNumber on success, null on error</returns>
        public static COFFLineNumber ParseCOFFLineNumber(Stream data)
        {
            var obj = new COFFLineNumber();

            obj.SymbolTableIndex = data.ReadUInt32LittleEndian();
            obj.VirtualAddress = obj.SymbolTableIndex;
            obj.Linenumber = data.ReadUInt16LittleEndian();

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a COFFRelocation
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled COFFRelocation on success, null on error</returns>
        public static COFFRelocation ParseCOFFRelocation(Stream data)
        {
            var obj = new COFFRelocation();

            obj.VirtualAddress = data.ReadUInt32LittleEndian();
            obj.SymbolTableIndex = data.ReadUInt32LittleEndian();
            obj.TypeIndicator = (RelocationType)data.ReadUInt16LittleEndian();

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a COFF string table
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled COFF string table on success, null on error</returns>
        public static COFFStringTable ParseCOFFStringTable(Stream data)
        {
            var obj = new COFFStringTable();

            obj.TotalSize = data.ReadUInt32LittleEndian();
            if (obj.TotalSize <= 4)
                return obj;

            var strings = new List<string>();

            uint totalSize = obj.TotalSize;
            while (totalSize > 0 && data.Position < data.Length)
            {
                long initialPosition = data.Position;
                string? str = data.ReadNullTerminatedAnsiString();
                strings.Add(str ?? string.Empty);
                totalSize -= (uint)(data.Position - initialPosition);
            }

            obj.Strings = [.. strings];

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a COFF symbol table
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="count">Number of COFF symbol table entries to read</param>
        /// <returns>Filled COFF symbol table on success, null on error</returns>
        public static BaseEntry[] ParseCOFFSymbolTable(Stream data, uint count)
        {
            var obj = new BaseEntry[count];

            int auxSymbolsRemaining = 0;
            int nextSymbolType = 0;

            for (int i = 0; i < count; i++)
            {
                // Standard COFF Symbol Table Entry
                if (nextSymbolType == 0)
                {
                    var entry = ParseStandardRecord(data, out nextSymbolType);
                    obj[i] = entry;
                    auxSymbolsRemaining = entry.NumberOfAuxSymbols;
                }

                // Auxiliary Format 1: Function Definitions
                else if (nextSymbolType == 1)
                {
                    obj[i] = ParseFunctionDefinition(data);
                    auxSymbolsRemaining--;
                }

                // Auxiliary Format 2: .bf and .ef Symbols
                else if (nextSymbolType == 2)
                {
                    obj[i] = ParseDescriptor(data);
                    auxSymbolsRemaining--;
                }

                // Auxiliary Format 3: Weak Externals
                else if (nextSymbolType == 3)
                {
                    obj[i] = ParseWeakExternal(data);
                    auxSymbolsRemaining--;
                }

                // Auxiliary Format 4: Files
                else if (nextSymbolType == 4)
                {
                    obj[i] = ParseFileRecord(data);
                    auxSymbolsRemaining--;
                }

                // Auxiliary Format 5: Section Definitions
                else if (nextSymbolType == 5)
                {
                    obj[i] = ParseSectionDefinition(data);
                    auxSymbolsRemaining--;
                }

                // Auxiliary Format 6: CLR Token Definition
                else if (nextSymbolType == 6)
                {
                    obj[i] = ParseCLRTokenDefinition(data);
                    auxSymbolsRemaining--;
                }

                // Invalid case, should be skipped
                else
                {
                    _ = data.ReadBytes(18);
                    auxSymbolsRemaining--;
                }

                // If we hit the last aux symbol, go back to normal format
                if (auxSymbolsRemaining == 0)
                    nextSymbolType = 0;
            }

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a DataDirectory
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled DataDirectory on success, null on error</returns>
        public static DataDirectory ParseDataDirectory(Stream data)
        {
            var obj = new DataDirectory();

            obj.VirtualAddress = data.ReadUInt32LittleEndian();
            obj.Size = data.ReadUInt32LittleEndian();

            return obj;
        }

        /// <summary>
        /// Parse a byte array into a DebugDirectoryEntry
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>Filled DebugDirectoryEntry on success, null on error</returns>
        public static DebugDirectoryEntry ParseDebugDirectoryEntry(byte[] data, ref int offset)
        {
            var obj = new DebugDirectoryEntry();

            obj.Characteristics = data.ReadUInt32LittleEndian(ref offset);
            obj.TimeDateStamp = data.ReadUInt32LittleEndian(ref offset);
            obj.MajorVersion = data.ReadUInt16LittleEndian(ref offset);
            obj.MinorVersion = data.ReadUInt16LittleEndian(ref offset);
            obj.DebugType = (DebugType)data.ReadUInt32LittleEndian(ref offset);
            obj.SizeOfData = data.ReadUInt32LittleEndian(ref offset);
            obj.AddressOfRawData = data.ReadUInt32LittleEndian(ref offset);
            obj.PointerToRawData = data.ReadUInt32LittleEndian(ref offset);

            return obj;
        }

        /// <summary>
        /// Parse a byte array into a DebugTable
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <returns>Filled DebugTable on success, null on error</returns>
        public static DebugTable? ParseDebugTable(byte[]? data)
        {
            if (data == null)
                return null;

            var obj = new DebugTable();

            var table = new List<DebugDirectoryEntry>();

            int offset = 0;
            while (offset < data.Length)
            {
                var entry = ParseDebugDirectoryEntry(data, ref offset);
                table.Add(entry);
            }

            obj.DebugDirectoryTable = [.. table];

            // TODO: Should we read the debug data in? Most of it is unformatted or undocumented
            // TODO: Implement .debug$F (Object Only) / IMAGE_DEBUG_TYPE_FPO
            // TODO: Implement .debug$S (Object Only)
            // TODO: Implement .debug$P (Object Only)
            // TODO: Implement .debug$T (Object Only)

            return obj;
        }

        /// <summary>
        /// Parse a byte array into a DelayLoadDirectoryTable
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <returns>Filled DelayLoadDirectoryTable on success, null on error</returns>
        public static DelayLoadDirectoryTable? ParseDelayLoadDirectoryTable(byte[]? data)
        {
            if (data == null)
                return null;

            var obj = new DelayLoadDirectoryTable();

            int offset = 0;
            obj.Attributes = data.ReadUInt32LittleEndian(ref offset);
            obj.Name = data.ReadUInt32LittleEndian(ref offset);
            obj.ModuleHandle = data.ReadUInt32LittleEndian(ref offset);
            obj.DelayImportAddressTable = data.ReadUInt32LittleEndian(ref offset);
            obj.DelayImportNameTable = data.ReadUInt32LittleEndian(ref offset);
            obj.BoundDelayImportTable = data.ReadUInt32LittleEndian(ref offset);
            obj.UnloadDelayImportTable = data.ReadUInt32LittleEndian(ref offset);
            obj.TimeStamp = data.ReadUInt32LittleEndian(ref offset);

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a Descriptor
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Descriptor on success, null on error</returns>
        public static Descriptor ParseDescriptor(Stream data)
        {
            var obj = new Descriptor();

            obj.Unused1 = data.ReadUInt32LittleEndian();
            obj.Linenumber = data.ReadUInt16LittleEndian();
            obj.Unused2 = data.ReadBytes(6);
            obj.PointerToNextFunction = data.ReadUInt32LittleEndian();
            obj.Unused3 = data.ReadUInt16LittleEndian();

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a ExportAddressTable
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="entries">Number of entries in the table</param>
        /// <returns>Filled ExportAddressTable on success, null on error</returns>
        public static ExportAddressTableEntry[] ParseExportAddressTable(Stream data, uint entries)
        {
            var obj = new ExportAddressTableEntry[entries];

            for (int i = 0; i < obj.Length; i++)
            {
                obj[i] = ParseExportAddressTableEntry(data);
            }

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a ExportAddressTableEntry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled ExportAddressTableEntry on success, null on error</returns>
        public static ExportAddressTableEntry ParseExportAddressTableEntry(Stream data)
        {
            var obj = new ExportAddressTableEntry();

            obj.ExportRVA = data.ReadUInt32LittleEndian();
            obj.ForwarderRVA = obj.ExportRVA;

            return obj;
        }

        /// <summary>
        /// Parse a byte array into a ExportTable
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <returns>Filled ExportTable on success, null on error</returns>
        public static ExportTable? ParseExportTable(byte[]? data)
        {
            if (data == null)
                return null;

            var obj = new ExportTable();

            // Parse the export directory table
            int offset = 0;
            var directoryTable = ParseExportDirectoryTable(data, ref offset);
            if (directoryTable.ExportFlags != 0)
                return obj;

            // Set the export directory table
            obj.ExportDirectoryTable = directoryTable;

            return obj;
        }

        /// <summary>
        /// Parse a byte array into a ExportDirectoryTable
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>Filled ExportDirectoryTable on success, null on error</returns>
        public static ExportDirectoryTable ParseExportDirectoryTable(byte[] data, ref int offset)
        {
            var obj = new ExportDirectoryTable();

            obj.ExportFlags = data.ReadUInt32LittleEndian(ref offset);
            obj.TimeDateStamp = data.ReadUInt32LittleEndian(ref offset);
            obj.MajorVersion = data.ReadUInt16LittleEndian(ref offset);
            obj.MinorVersion = data.ReadUInt16LittleEndian(ref offset);
            obj.NameRVA = data.ReadUInt32LittleEndian(ref offset);
            obj.OrdinalBase = data.ReadUInt32LittleEndian(ref offset);
            obj.AddressTableEntries = data.ReadUInt32LittleEndian(ref offset);
            obj.NumberOfNamePointers = data.ReadUInt32LittleEndian(ref offset);
            obj.ExportAddressTableRVA = data.ReadUInt32LittleEndian(ref offset);
            obj.NamePointerRVA = data.ReadUInt32LittleEndian(ref offset);
            obj.OrdinalTableRVA = data.ReadUInt32LittleEndian(ref offset);

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a ExportNameTable
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="initialOffset">Initial offset to use in address comparisons</param>
        /// <param name="pointers">Set of pointers to process</param>
        /// <param name="sections">Section table to use for virtual address translation</param>
        /// <returns>Filled ExportNameTable on success, null on error</returns>
        public static ExportNameTable ParseExportNameTable(Stream data, long initialOffset, uint[] pointers, SectionHeader[] sections)
        {
            var obj = new ExportNameTable();

            obj.Strings = new string[pointers.Length];
            for (int i = 0; i < obj.Strings.Length; i++)
            {
                long address = initialOffset + pointers[i].ConvertVirtualAddress(sections);
                if (address > initialOffset && address < data.Length)
                {
                    data.Seek(address, SeekOrigin.Begin);

                    string? str = data.ReadNullTerminatedAnsiString();
                    obj.Strings[i] = str ?? string.Empty;
                }
                else
                {
                    obj.Strings[i] = string.Empty;
                }
            }

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a ExportNamePointerTable
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="entries">Number of entries in the table</param>
        /// <returns>Filled ExportNamePointerTable on success, null on error</returns>
        public static ExportNamePointerTable ParseExportNamePointerTable(Stream data, uint entries)
        {
            var obj = new ExportNamePointerTable();

            obj.Pointers = new uint[entries];
            for (int i = 0; i < obj.Pointers.Length; i++)
            {
                obj.Pointers[i] = data.ReadUInt32LittleEndian();
            }

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a ExportOrdinalTable
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="entries">Number of entries in the table</param>
        /// <returns>Filled ExportOrdinalTable on success, null on error</returns>
        public static ExportOrdinalTable ParseExportOrdinalTable(Stream data, uint entries)
        {
            var obj = new ExportOrdinalTable();

            obj.Indexes = new ushort[entries];
            for (int i = 0; i < obj.Indexes.Length; i++)
            {
                ushort pointer = data.ReadUInt16LittleEndian();
                obj.Indexes[i] = pointer;
            }

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a FileRecord
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled FileRecord on success, null on error</returns>
        public static FileRecord ParseFileRecord(Stream data)
        {
            var obj = new FileRecord();

            obj.FileName = data.ReadBytes(18);

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a FunctionDefinition
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled FunctionDefinition on success, null on error</returns>
        public static FunctionDefinition ParseFunctionDefinition(Stream data)
        {
            var obj = new FunctionDefinition();

            obj.TagIndex = data.ReadUInt32LittleEndian();
            obj.TotalSize = data.ReadUInt32LittleEndian();
            obj.PointerToLinenumber = data.ReadUInt32LittleEndian();
            obj.PointerToNextFunction = data.ReadUInt32LittleEndian();
            obj.Unused = data.ReadUInt16LittleEndian();

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a HintNameTable
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="initialOffset">Initial offset to use in address comparisons</param>
        /// <param name="importLookupTables">Import lookup tables</param>
        /// <param name="importAddressTables">Import address tables</param>
        /// <param name="sections">Section table to use for virtual address translation</param>
        /// <returns>Filled HintNameTable on success, null on error</returns>
        public static HintNameTableEntry[] ParseHintNameTable(Stream data,
            long initialOffset,
            Dictionary<int, ImportLookupTableEntry[]?> importLookupTables,
            Dictionary<int, ImportAddressTableEntry[]?> importAddressTables,
            SectionHeader[] sections)
        {
            var importHintNameTable = new List<HintNameTableEntry>();

            if (importLookupTables.Count > 0 || importAddressTables.Count > 0)
            {
                // Get the addresses of the hint/name table entries
                var hintNameTableEntryAddresses = new List<int>();

                // If we have import lookup tables
                if (importLookupTables.Count > 0)
                {
                    var addresses = new List<int>();
                    foreach (var kvp in importLookupTables)
                    {
                        if (kvp.Value == null || kvp.Value.Length == 0)
                            continue;

                        var vaddrs = Array.ConvertAll(kvp.Value,
                             ilte => (int)ilte.HintNameTableRVA.ConvertVirtualAddress(sections));
                        addresses.AddRange(vaddrs);
                    }

                    hintNameTableEntryAddresses.AddRange(addresses);
                }

                // If we have import address tables
                if (importAddressTables.Count > 0)
                {
                    var addresses = new List<int>();
                    foreach (var kvp in importAddressTables)
                    {
                        if (kvp.Value == null || kvp.Value.Length == 0)
                            continue;

                        var vaddrs = Array.ConvertAll(kvp.Value,
                            iate => (int)iate.HintNameTableRVA.ConvertVirtualAddress(sections));
                        addresses.AddRange(vaddrs);
                    }

                    hintNameTableEntryAddresses.AddRange(addresses);
                }

                // Sanitize the addresses
                var temp = new List<int>();
                foreach (int addr in hintNameTableEntryAddresses)
                {
                    if (addr == 0)
                        continue;
                    if (temp.Contains(addr))
                        continue;

                    temp.Add(addr);
                }

                // If we have any addresses, add them to the table in order
                hintNameTableEntryAddresses.Sort();
                for (int i = 0; i < hintNameTableEntryAddresses.Count; i++)
                {
                    long hintNameTableEntryAddress = initialOffset
                        + hintNameTableEntryAddresses[i];

                    if (hintNameTableEntryAddress > initialOffset && hintNameTableEntryAddress < data.Length)
                    {
                        data.Seek(hintNameTableEntryAddress, SeekOrigin.Begin);

                        var hintNameTableEntry = ParseHintNameTableEntry(data);
                        importHintNameTable.Add(hintNameTableEntry);
                    }
                }
            }

            return [.. importHintNameTable];
        }

        /// <summary>
        /// Parse a Stream into a HintNameTableEntry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled HintNameTableEntry on success, null on error</returns>
        public static HintNameTableEntry ParseHintNameTableEntry(Stream data)
        {
            var obj = new HintNameTableEntry();

            obj.Hint = data.ReadUInt16LittleEndian();
            obj.Name = data.ReadNullTerminatedAnsiString();

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a ImportAddressTable
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="magic">Optional header magic number indicating PE32 or PE32+</param>
        /// <returns>Filled ImportAddressTable on success, null on error</returns>
        public static ImportAddressTableEntry[] ParseImportAddressTable(Stream data, OptionalHeaderMagicNumber magic)
        {
            var obj = new List<ImportAddressTableEntry>();

            // Loop until the last item (all nulls) are found
            while (data.Position < data.Length)
            {
                var entry = ParseImportAddressTableEntry(data, magic);
                obj.Add(entry);

                // All zero values means the last entry
                if (entry.OrdinalNameFlag == false
                    && entry.OrdinalNumber == 0
                    && entry.HintNameTableRVA == 0)
                    break;
            }

            return [.. obj];
        }

        /// <summary>
        /// Parse a Stream into a ImportAddressTables
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="initialOffset">Initial offset to use in address comparisons</param>
        /// <param name="magic">Optional header magic number indicating PE32 or PE32+</param>
        /// <param name="entries">Directory table entries containing the addresses</param>
        /// <param name="sections">Section table to use for virtual address translation</param>
        /// <returns>Filled ImportAddressTables on success, null on error</returns>
        public static Dictionary<int, ImportAddressTableEntry[]?> ParseImportAddressTables(Stream data,
            long initialOffset,
            OptionalHeaderMagicNumber magic,
            ImportDirectoryTableEntry[] entries,
            SectionHeader[] sections)
        {
            var obj = new Dictionary<int, ImportAddressTableEntry[]?>();

            for (int i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];
                if (entry.ImportAddressTableRVA.ConvertVirtualAddress(sections) == 0)
                    continue;

                long tableAddress = initialOffset
                    + entry.ImportAddressTableRVA.ConvertVirtualAddress(sections);

                if (tableAddress > initialOffset && tableAddress < data.Length)
                {
                    data.Seek(tableAddress, SeekOrigin.Begin);
                    obj[i] = ParseImportAddressTable(data, magic);
                }
            }

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a ImportAddressTableEntry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="magic">Optional header magic number</param>
        /// <returns>Filled ImportAddressTableEntry on success, null on error</returns>
        public static ImportAddressTableEntry ParseImportAddressTableEntry(Stream data, OptionalHeaderMagicNumber magic)
        {
            var obj = new ImportAddressTableEntry();

            if (magic == OptionalHeaderMagicNumber.PE32)
            {
                uint value = data.ReadUInt32LittleEndian();
                obj.OrdinalNameFlag = (value & 0x80000000) != 0;
                if (obj.OrdinalNameFlag)
                    obj.OrdinalNumber = (ushort)(value & ~0x80000000);
                else
                    obj.HintNameTableRVA = (uint)(value & ~0x80000000);
            }
            else if (magic == OptionalHeaderMagicNumber.PE32Plus)
            {
                ulong value = data.ReadUInt64LittleEndian();
                obj.OrdinalNameFlag = (value & 0x8000000000000000) != 0;
                if (obj.OrdinalNameFlag)
                    obj.OrdinalNumber = (ushort)(value & ~0x8000000000000000);
                else
                    obj.HintNameTableRVA = (uint)(value & ~0x8000000000000000);
            }

            return obj;
        }

        /// <summary>
        /// Parse a byte array into a ImportDirectoryTable
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>Filled ImportDirectoryTable on success, null on error</returns>
        public static ImportDirectoryTableEntry[] ParseImportDirectoryTable(byte[] data, ref int offset)
        {
            var obj = new List<ImportDirectoryTableEntry>();

            // Loop until the last item (all nulls) are found
            while (offset < data.Length)
            {
                var entry = ParseImportDirectoryTableEntry(data, ref offset);
                obj.Add(entry);

                // All zero values means the last entry
                if (entry.ImportLookupTableRVA == 0
                    && entry.TimeDateStamp == 0
                    && entry.ForwarderChain == 0
                    && entry.NameRVA == 0
                    && entry.ImportAddressTableRVA == 0)
                    break;
            }

            return [.. obj];
        }

        /// <summary>
        /// Parse a byte array into a ImportDirectoryTableEntry
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>Filled ImportDirectoryTableEntry on success, null on error</returns>
        public static ImportDirectoryTableEntry ParseImportDirectoryTableEntry(byte[] data, ref int offset)
        {
            var obj = new ImportDirectoryTableEntry();

            obj.ImportLookupTableRVA = data.ReadUInt32LittleEndian(ref offset);
            obj.TimeDateStamp = data.ReadUInt32LittleEndian(ref offset);
            obj.ForwarderChain = data.ReadUInt32LittleEndian(ref offset);
            obj.NameRVA = data.ReadUInt32LittleEndian(ref offset);
            obj.ImportAddressTableRVA = data.ReadUInt32LittleEndian(ref offset);

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a ImportLookupTable
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="magic">Optional header magic number indicating PE32 or PE32+</param>
        /// <returns>Filled ImportLookupTable on success, null on error</returns>
        public static ImportLookupTableEntry[] ParseImportLookupTable(Stream data, OptionalHeaderMagicNumber magic)
        {
            var obj = new List<ImportLookupTableEntry>();

            // Loop until the last item (all nulls) are found
            while (data.Position < data.Length)
            {
                var entry = ParseImportLookupTableEntry(data, magic);
                obj.Add(entry);

                // All zero values means the last entry
                if (entry.OrdinalNameFlag == false
                    && entry.OrdinalNumber == 0
                    && entry.HintNameTableRVA == 0)
                    break;
            }

            return [.. obj];
        }

        /// <summary>
        /// Parse a Stream into a ImportLookupTables
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="initialOffset">Initial offset to use in address comparisons</param>
        /// <param name="magic">Optional header magic number indicating PE32 or PE32+</param>
        /// <param name="entries">Directory table entries containing the addresses</param>
        /// <param name="sections">Section table to use for virtual address translation</param>
        /// <returns>Filled ImportLookupTables on success, null on error</returns>
        public static Dictionary<int, ImportLookupTableEntry[]?> ParseImportLookupTables(Stream data,
            long initialOffset,
            OptionalHeaderMagicNumber magic,
            ImportDirectoryTableEntry[] entries,
            SectionHeader[] sections)
        {
            // Lookup tables
            var obj = new Dictionary<int, ImportLookupTableEntry[]?>();

            for (int i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];
                if (entry.ImportLookupTableRVA.ConvertVirtualAddress(sections) == 0)
                    continue;

                long tableAddress = initialOffset
                    + entry.ImportLookupTableRVA.ConvertVirtualAddress(sections);

                if (tableAddress > initialOffset && tableAddress < data.Length)
                {
                    data.Seek(tableAddress, SeekOrigin.Begin);
                    obj[i] = ParseImportLookupTable(data, magic);
                }
            }

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a ImportLookupTableEntry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="magic">Optional header magic number</param>
        /// <returns>Filled ImportLookupTableEntry on success, null on error</returns>
        public static ImportLookupTableEntry ParseImportLookupTableEntry(Stream data, OptionalHeaderMagicNumber magic)
        {
            var obj = new ImportLookupTableEntry();

            if (magic == OptionalHeaderMagicNumber.PE32)
            {
                uint value = data.ReadUInt32LittleEndian();

                obj.OrdinalNameFlag = (value & 0x80000000) != 0;
                if (obj.OrdinalNameFlag)
                    obj.OrdinalNumber = (ushort)(value & ~0x80000000);
                else
                    obj.HintNameTableRVA = (uint)(value & ~0x80000000);
            }
            else if (magic == OptionalHeaderMagicNumber.PE32Plus)
            {
                ulong value = data.ReadUInt64LittleEndian();

                obj.OrdinalNameFlag = (value & 0x8000000000000000) != 0;
                if (obj.OrdinalNameFlag)
                    obj.OrdinalNumber = (ushort)(value & ~0x8000000000000000);
                else
                    obj.HintNameTableRVA = (uint)(value & ~0x8000000000000000);
            }

            return obj;
        }

        /// <summary>
        /// Parse a byte array into a import table
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <returns>Filled import table on success, null on error</returns>
        public static ImportTable? ParseImportTable(byte[]? tableData)
        {
            if (tableData == null)
                return null;

            var obj = new ImportTable();

            // Parse the import directory table
            int offset = 0;
            var directoryTable = ParseImportDirectoryTable(tableData, ref offset);

            // Set the export directory table
            obj.ImportDirectoryTable = directoryTable;

            return obj;
        }

        /// <summary>
        /// Parse a Stream into an OptionalHeader
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="optionalSize">Size of the optional header</param>
        /// <returns>Filled OptionalHeader on success, null on error</returns>
        public static OptionalHeader ParseOptionalHeader(Stream data, int optionalSize)
        {
            long initialOffset = data.Position;

            var obj = new OptionalHeader();

            #region Standard Fields

            obj.Magic = (OptionalHeaderMagicNumber)data.ReadUInt16LittleEndian();
            obj.MajorLinkerVersion = data.ReadByteValue();
            obj.MinorLinkerVersion = data.ReadByteValue();
            obj.SizeOfCode = data.ReadUInt32LittleEndian();
            obj.SizeOfInitializedData = data.ReadUInt32LittleEndian();
            obj.SizeOfUninitializedData = data.ReadUInt32LittleEndian();
            obj.AddressOfEntryPoint = data.ReadUInt32LittleEndian();
            obj.BaseOfCode = data.ReadUInt32LittleEndian();

            // ROM images do not have the remainder defined(?)
            if (obj.Magic == OptionalHeaderMagicNumber.ROMImage)
                return obj;

            if (obj.Magic == OptionalHeaderMagicNumber.PE32)
                obj.BaseOfData = data.ReadUInt32LittleEndian();

            #endregion

            // TODO: When Models is updated, consolidate the _PE32(Plus) below
            #region Windows-Specific Fields

            if (obj.Magic == OptionalHeaderMagicNumber.PE32)
                obj.ImageBase_PE32 = data.ReadUInt32LittleEndian();
            else if (obj.Magic == OptionalHeaderMagicNumber.PE32Plus)
                obj.ImageBase_PE32Plus = data.ReadUInt64LittleEndian();
            obj.SectionAlignment = data.ReadUInt32LittleEndian();
            obj.FileAlignment = data.ReadUInt32LittleEndian();
            obj.MajorOperatingSystemVersion = data.ReadUInt16LittleEndian();
            obj.MinorOperatingSystemVersion = data.ReadUInt16LittleEndian();
            obj.MajorImageVersion = data.ReadUInt16LittleEndian();
            obj.MinorImageVersion = data.ReadUInt16LittleEndian();
            obj.MajorSubsystemVersion = data.ReadUInt16LittleEndian();
            obj.MinorSubsystemVersion = data.ReadUInt16LittleEndian();
            obj.Win32VersionValue = data.ReadUInt32LittleEndian();
            obj.SizeOfImage = data.ReadUInt32LittleEndian();
            obj.SizeOfHeaders = data.ReadUInt32LittleEndian();
            obj.CheckSum = data.ReadUInt32LittleEndian();
            obj.Subsystem = (WindowsSubsystem)data.ReadUInt16LittleEndian();
            obj.DllCharacteristics = (DllCharacteristics)data.ReadUInt16LittleEndian();
            if (obj.Magic == OptionalHeaderMagicNumber.PE32)
                obj.SizeOfStackReserve_PE32 = data.ReadUInt32LittleEndian();
            else if (obj.Magic == OptionalHeaderMagicNumber.PE32Plus)
                obj.SizeOfStackReserve_PE32Plus = data.ReadUInt64LittleEndian();
            if (obj.Magic == OptionalHeaderMagicNumber.PE32)
                obj.SizeOfStackCommit_PE32 = data.ReadUInt32LittleEndian();
            else if (obj.Magic == OptionalHeaderMagicNumber.PE32Plus)
                obj.SizeOfStackCommit_PE32Plus = data.ReadUInt64LittleEndian();
            if (obj.Magic == OptionalHeaderMagicNumber.PE32)
                obj.SizeOfHeapReserve_PE32 = data.ReadUInt32LittleEndian();
            else if (obj.Magic == OptionalHeaderMagicNumber.PE32Plus)
                obj.SizeOfHeapReserve_PE32Plus = data.ReadUInt64LittleEndian();
            if (obj.Magic == OptionalHeaderMagicNumber.PE32)
                obj.SizeOfHeapCommit_PE32 = data.ReadUInt32LittleEndian();
            else if (obj.Magic == OptionalHeaderMagicNumber.PE32Plus)
                obj.SizeOfHeapCommit_PE32Plus = data.ReadUInt64LittleEndian();
            obj.LoaderFlags = data.ReadUInt32LittleEndian();
            obj.NumberOfRvaAndSizes = data.ReadUInt32LittleEndian();

            #endregion

            #region Data Directories

            if (obj.NumberOfRvaAndSizes >= 1 && data.Position - initialOffset < optionalSize)
                obj.ExportTable = ParseDataDirectory(data);

            if (obj.NumberOfRvaAndSizes >= 2 && data.Position - initialOffset < optionalSize)
                obj.ImportTable = ParseDataDirectory(data);

            if (obj.NumberOfRvaAndSizes >= 3 && data.Position - initialOffset < optionalSize)
                obj.ResourceTable = ParseDataDirectory(data);

            if (obj.NumberOfRvaAndSizes >= 4 && data.Position - initialOffset < optionalSize)
                obj.ExceptionTable = ParseDataDirectory(data);

            if (obj.NumberOfRvaAndSizes >= 5 && data.Position - initialOffset < optionalSize)
                obj.CertificateTable = ParseDataDirectory(data);

            if (obj.NumberOfRvaAndSizes >= 6 && data.Position - initialOffset < optionalSize)
                obj.BaseRelocationTable = ParseDataDirectory(data);

            if (obj.NumberOfRvaAndSizes >= 7 && data.Position - initialOffset < optionalSize)
                obj.Debug = ParseDataDirectory(data);

            if (obj.NumberOfRvaAndSizes >= 8 && data.Position - initialOffset < optionalSize)
                obj.Architecture = data.ReadUInt64LittleEndian();

            if (obj.NumberOfRvaAndSizes >= 9 && data.Position - initialOffset < optionalSize)
                obj.GlobalPtr = ParseDataDirectory(data);

            if (obj.NumberOfRvaAndSizes >= 10 && data.Position - initialOffset < optionalSize)
                obj.ThreadLocalStorageTable = ParseDataDirectory(data);

            if (obj.NumberOfRvaAndSizes >= 11 && data.Position - initialOffset < optionalSize)
                obj.LoadConfigTable = ParseDataDirectory(data);

            if (obj.NumberOfRvaAndSizes >= 12 && data.Position - initialOffset < optionalSize)
                obj.BoundImport = ParseDataDirectory(data);

            if (obj.NumberOfRvaAndSizes >= 13 && data.Position - initialOffset < optionalSize)
                obj.ImportAddressTable = ParseDataDirectory(data);

            if (obj.NumberOfRvaAndSizes >= 14 && data.Position - initialOffset < optionalSize)
                obj.DelayImportDescriptor = ParseDataDirectory(data);

            if (obj.NumberOfRvaAndSizes >= 15 && data.Position - initialOffset < optionalSize)
                obj.CLRRuntimeHeader = ParseDataDirectory(data);

            if (obj.NumberOfRvaAndSizes >= 16 && data.Position - initialOffset < optionalSize)
                obj.Reserved = data.ReadUInt64LittleEndian();

            #endregion

            return obj;
        }

        /// <summary>
        /// Fill in resource data
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="initialOffset">Initial offset to use in address comparisons</param>
        /// <param name="table">Resource table to fill in</param>
        /// <param name="sections">Section table to use for virtual address translation</param>
        /// <returns>Filled ResourceDataEntry on success, null on error</returns>
        public static void ParseResourceData(Stream data, long initialOffset, ResourceDirectoryTable? table, SectionHeader[] sections)
        {
            if (table?.Entries == null)
                return;

            foreach (var entry in table.Entries)
            {
                // Handle directory entries directly
                if (entry.DataEntry != null && entry.DataEntry.Size > 0)
                {
                    // Read the data from the offset
                    long dataOffset = initialOffset + entry.DataEntry.DataRVA.ConvertVirtualAddress(sections);
                    if (dataOffset > initialOffset && dataOffset + entry.DataEntry.Size < data.Length)
                        entry.DataEntry.Data = data.ReadFrom(dataOffset, (int)entry.DataEntry.Size, retainPosition: true);
                }

                // Handle subdirectories by recursion
                else if (entry.Subdirectory != null)
                    ParseResourceData(data, initialOffset, entry.Subdirectory, sections);
            }
        }

        /// <summary>
        /// Parse a byte array into an ResourceDataEntry
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>Filled ResourceDataEntry on success, null on error</returns>
        public static ResourceDataEntry ParseResourceDataEntry(byte[] data, ref int offset)
        {
            var obj = new ResourceDataEntry();

            obj.DataRVA = data.ReadUInt32LittleEndian(ref offset);
            obj.Size = data.ReadUInt32LittleEndian(ref offset);
            obj.Codepage = data.ReadUInt32LittleEndian(ref offset);
            obj.Reserved = data.ReadUInt32LittleEndian(ref offset);

            return obj;
        }

        /// <summary>
        /// Parse a byte array into an ResourceDirectoryEntry
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <param name="nameEntry">Indicates if the value is a name entry or not</param>
        /// <returns>Filled ResourceDirectoryEntry on success, null on error</returns>
        public static ResourceDirectoryEntry ParseResourceDirectoryEntry(byte[] data, ref int offset, bool nameEntry)
        {
            var obj = new ResourceDirectoryEntry();

            // TODO: Figure out why the high bit is set for names
            // The original version of this code also had this fix, but there
            // was no comment or documentation as to why. The official MSDN
            // documentation makes no mention of the high bit being set here,
            // only for the offset below.
            if (nameEntry)
                obj.NameOffset = data.ReadUInt32LittleEndian(ref offset) & ~0x80000000U;
            else
                obj.IntegerID = data.ReadUInt32LittleEndian(ref offset);

            uint offsetField = data.ReadUInt32LittleEndian(ref offset);
            if ((offsetField & 0x80000000) != 0)
                obj.SubdirectoryOffset = offsetField & ~0x80000000;
            else
                obj.DataEntryOffset = offsetField;

            return obj;
        }

        /// <summary>
        /// Parse a byte array into an ResourceDirectoryString
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>Filled ResourceDirectoryString on success, null on error</returns>
        public static ResourceDirectoryString ParseResourceDirectoryString(byte[] data, ref int offset)
        {
            var obj = new ResourceDirectoryString();

            obj.Length = data.ReadUInt16LittleEndian(ref offset);
            if (obj.Length > 0 && offset + (obj.Length * 2) <= data.Length)
                obj.UnicodeString = data.ReadBytes(ref offset, obj.Length * 2);

            return obj;
        }

        /// <summary>
        /// Parse a byte array into a ResourceDirectoryTable
        /// </summary>
        /// <param name="tableData">Byte array to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>Filled ResourceDirectoryTable on success, null on error</returns>
        public static ResourceDirectoryTable? ParseResourceDirectoryTable(byte[]? tableData, ref int offset)
        {
            if (tableData == null)
                return null;

            var obj = new ResourceDirectoryTable();

            obj.Characteristics = tableData.ReadUInt32LittleEndian(ref offset);
            if (obj.Characteristics != 0)
                return null;

            obj.TimeDateStamp = tableData.ReadUInt32LittleEndian(ref offset);
            obj.MajorVersion = tableData.ReadUInt16LittleEndian(ref offset);
            obj.MinorVersion = tableData.ReadUInt16LittleEndian(ref offset);
            obj.NumberOfNameEntries = tableData.ReadUInt16LittleEndian(ref offset);
            obj.NumberOfIDEntries = tableData.ReadUInt16LittleEndian(ref offset);

            // Create the entry array
            int totalEntryCount = obj.NumberOfNameEntries + obj.NumberOfIDEntries;
            obj.Entries = new ResourceDirectoryEntry[totalEntryCount];
            if (obj.Entries.Length == 0)
                return obj;

            // Perform top-level pass of data
            for (int i = 0; i < totalEntryCount; i++)
            {
                bool nameEntry = i < obj.NumberOfNameEntries;
                obj.Entries[i] = ParseResourceDirectoryEntry(tableData, ref offset, nameEntry);

                // Read the name from the offset, if needed
                if (nameEntry && obj.Entries[i].NameOffset > 0 && obj.Entries[i].NameOffset < tableData.Length)
                {
                    int nameOffset = (int)obj.Entries[i].NameOffset;
                    obj.Entries[i].Name = ParseResourceDirectoryString(tableData, ref nameOffset);
                }
            }

            // Loop through and process the entries
            foreach (var entry in obj.Entries)
            {
                if (entry.DataEntryOffset > 0 && entry.DataEntryOffset < tableData.Length)
                {
                    int entryOffset = (int)entry.DataEntryOffset;
                    entry.DataEntry = ParseResourceDataEntry(tableData, ref entryOffset);
                }
                else if (entry.SubdirectoryOffset > 0 && entry.SubdirectoryOffset < tableData.Length)
                {
                    int subdirOffset = (int)entry.SubdirectoryOffset;
                    entry.Subdirectory = ParseResourceDirectoryTable(tableData, ref subdirOffset);
                }
            }

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a SectionDefinition
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled SectionDefinition on success, null on error</returns>
        public static SectionDefinition ParseSectionDefinition(Stream data)
        {
            var obj = new SectionDefinition();

            obj.Length = data.ReadUInt32LittleEndian();
            obj.NumberOfRelocations = data.ReadUInt16LittleEndian();
            obj.NumberOfLinenumbers = data.ReadUInt16LittleEndian();
            obj.CheckSum = data.ReadUInt32LittleEndian();
            obj.Number = data.ReadUInt16LittleEndian();
            obj.Selection = data.ReadByteValue();
            obj.Unused = data.ReadBytes(3);

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a SectionHeader
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled SectionHeader on success, null on error</returns>
        public static SectionHeader ParseSectionHeader(Stream data)
        {
            var obj = new SectionHeader();

            obj.Name = data.ReadBytes(8);
            obj.VirtualSize = data.ReadUInt32LittleEndian();
            obj.VirtualAddress = data.ReadUInt32LittleEndian();
            obj.SizeOfRawData = data.ReadUInt32LittleEndian();
            obj.PointerToRawData = data.ReadUInt32LittleEndian();
            obj.PointerToRelocations = data.ReadUInt32LittleEndian();
            obj.PointerToLinenumbers = data.ReadUInt32LittleEndian();
            obj.NumberOfRelocations = data.ReadUInt16LittleEndian();
            obj.NumberOfLinenumbers = data.ReadUInt16LittleEndian();
            obj.Characteristics = (SectionFlags)data.ReadUInt32LittleEndian();

            obj.COFFRelocations = new COFFRelocation[obj.NumberOfRelocations];
            for (int j = 0; j < obj.NumberOfRelocations; j++)
            {
                // TODO: Seek to correct location and read data
            }

            obj.COFFLineNumbers = new COFFLineNumber[obj.NumberOfLinenumbers];
            for (int j = 0; j < obj.NumberOfLinenumbers; j++)
            {
                // TODO: Seek to correct location and read data
            }

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a StandardRecord
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled StandardRecord on success, null on error</returns>
        public static StandardRecord ParseStandardRecord(Stream data, out int nextSymbolType)
        {
            var obj = new StandardRecord();

            obj.ShortName = data.ReadBytes(8);

            obj.Zeroes = BitConverter.ToUInt32(obj.ShortName, 0);
            obj.Offset = BitConverter.ToUInt32(obj.ShortName, 4);
            string? shortName = null;
            if (obj.Zeroes != 0)
                shortName = Encoding.UTF8.GetString(obj.ShortName);

            obj.Value = data.ReadUInt32LittleEndian();
            obj.SectionNumber = data.ReadUInt16LittleEndian();
            obj.SymbolType = (SymbolType)data.ReadUInt16LittleEndian();
            obj.StorageClass = (StorageClass)data.ReadByteValue();
            obj.NumberOfAuxSymbols = data.ReadByteValue();

            // Do not determine the aux symbol if none exists
            if (obj.NumberOfAuxSymbols == 0)
            {
                nextSymbolType = 0;
                return obj;
            }

            // Determine the next symbol type
            if (obj.StorageClass == StorageClass.IMAGE_SYM_CLASS_EXTERNAL
                    && (obj.SymbolType & SymbolType.IMAGE_SYM_TYPE_FUNC) != 0
                    && obj.SectionNumber > 0)
            {
                nextSymbolType = 1;
            }
            else if (obj.StorageClass == StorageClass.IMAGE_SYM_CLASS_FUNCTION
                && (shortName?.StartsWith(".bf") == true
                    || shortName?.StartsWith(".ef") == true))
            {
                nextSymbolType = 2;
            }
            else if (obj.StorageClass == StorageClass.IMAGE_SYM_CLASS_EXTERNAL
                && obj.SectionNumber == (ushort)SectionNumber.IMAGE_SYM_UNDEFINED
                && obj.Value == 0)
            {
                nextSymbolType = 3;
            }
            else if (obj.StorageClass == StorageClass.IMAGE_SYM_CLASS_FILE
                && shortName != null)
            {
                // Symbol name should be ".file"
                nextSymbolType = 4;
            }
            else if (obj.StorageClass == StorageClass.IMAGE_SYM_CLASS_STATIC
                && shortName != null)
            {
                // Should have the name of a section (like ".text")
                nextSymbolType = 5;
            }
            else if (obj.StorageClass == StorageClass.IMAGE_SYM_CLASS_CLR_TOKEN)
            {
                nextSymbolType = 6;
            }
            else
            {
                nextSymbolType = -1;
            }

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a WeakExternal
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled WeakExternal on success, null on error</returns>
        public static WeakExternal ParseWeakExternal(Stream data)
        {
            var obj = new WeakExternal();

            obj.TagIndex = data.ReadUInt32LittleEndian();
            obj.Characteristics = data.ReadUInt32LittleEndian();
            obj.Unused = data.ReadBytes(10);

            return obj;
        }
    }
}
