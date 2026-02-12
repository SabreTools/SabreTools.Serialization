using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using SabreTools.Data.Extensions;
using SabreTools.Data.Models.COFF;
using SabreTools.Data.Models.COFF.SymbolTableEntries;
using SabreTools.Data.Models.PortableExecutable;
using SabreTools.IO.Extensions;
using static SabreTools.Data.Models.COFF.Constants;
using static SabreTools.Data.Models.PortableExecutable.Constants;

#pragma warning disable IDE0017 // Simplify object initialization
namespace SabreTools.Serialization.Readers
{
    public class PortableExecutable : BaseBinaryReader<Executable>
    {
        /// <inheritdoc/>
        public override Executable? Deserialize(Stream? data)
        {
            // If the data is invalid
            if (data is null || !data.CanRead)
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
                if (stub?.Header is null || stub.Header.NewExeHeaderAddr == 0)
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
                data.SeekIfPossible(newExeOffset, SeekOrigin.Begin);
                byte[] signature = data.ReadBytes(4);
                pex.Signature = Encoding.ASCII.GetString(signature);
                if (pex.Signature != SignatureString)
                    return null;

                #endregion

                #region File Header

                // Parse the file header
                var fileHeader = ParseFileHeader(data);
                if (fileHeader is null)
                    return null;
                if (fileHeader.NumberOfSections > 96)
                    return null;

                // Set the file header
                pex.FileHeader = fileHeader;

                #endregion

                #region Optional Header

                // If the optional header exists
                Data.Models.PortableExecutable.OptionalHeader? optionalHeader = null;
                if (fileHeader.SizeOfOptionalHeader > 0)
                {
                    // Parse the optional header
                    optionalHeader = ParseOptionalHeader(data, fileHeader.SizeOfOptionalHeader);

                    // Set the optional header
                    if (optionalHeader is not null)
                        pex.OptionalHeader = optionalHeader;
                }

                #endregion

                #region Section Table

                // Get the section table offset
                long offset = newExeOffset + 4 + FileHeaderSize + pex.FileHeader.SizeOfOptionalHeader;
                if (offset < initialOffset || offset >= data.Length)
                    return null;

                // Seek to the section table
                data.SeekIfPossible(offset, SeekOrigin.Begin);

                // Set the section table
                pex.SectionTable = new SectionHeader[fileHeader.NumberOfSections];
                for (int i = 0; i < fileHeader.NumberOfSections; i++)
                {
                    pex.SectionTable[i] = ParseSectionHeader(data);
                }

                #endregion

                // Cache the overlay offset
                long endOfSectionData = optionalHeader?.SizeOfHeaders ?? 0;
                Array.ForEach(pex.SectionTable, s => endOfSectionData += s.SizeOfRawData);

                #region Symbol Table and String Table

                offset = initialOffset + fileHeader.PointerToSymbolTable;
                if (offset > initialOffset && offset < data.Length)
                {
                    // Seek to the symbol table
                    data.SeekIfPossible(offset, SeekOrigin.Begin);

                    // Set the symbol and string tables
                    pex.SymbolTable = ParseSymbolTable(data, fileHeader.NumberOfSymbols);
                    pex.StringTable = ParseCOFFStringTable(data);
                }

                #endregion

                // All tables require the optional header to exist
                if (optionalHeader is null)
                    return pex;

                #region Export Table

                // Should also be in a '.edata' section
                if (optionalHeader.ExportTable is not null)
                {
                    offset = initialOffset
                        + optionalHeader.ExportTable.VirtualAddress.ConvertVirtualAddress(pex.SectionTable);
                    if (offset > initialOffset && offset < data.Length)
                    {
                        // Get the required table size
                        int tableSize = (int)optionalHeader.ExportTable.Size;

                        // Read the table data
                        byte[]? tableData = data.ReadFrom(offset, tableSize, retainPosition: true) ?? [];

                        // Parse the export directory table
                        int tableDataOffset = 0;
                        var exportDirectoryTable = ParseExportDirectoryTable(tableData, ref tableDataOffset);

                        // Set the export directory table
                        if (exportDirectoryTable.ExportFlags == 0 && exportDirectoryTable.AddressTableEntries > 0)
                            pex.ExportDirectoryTable = exportDirectoryTable;

                        // If the export table was parsed, read the remaining pieces
                        if (exportDirectoryTable is not null)
                        {
                            // Name
                            offset = initialOffset + exportDirectoryTable.NameRVA.ConvertVirtualAddress(pex.SectionTable);
                            if (offset > initialOffset && offset < data.Length)
                            {
                                data.SeekIfPossible(offset, SeekOrigin.Begin);
                                exportDirectoryTable.Name = data.ReadNullTerminatedAnsiString() ?? string.Empty;
                            }

                            // Address table
                            offset = initialOffset + exportDirectoryTable.ExportAddressTableRVA.ConvertVirtualAddress(pex.SectionTable);
                            if (exportDirectoryTable.AddressTableEntries != 0
                                && offset > initialOffset
                                && offset < data.Length)
                            {
                                data.SeekIfPossible(offset, SeekOrigin.Begin);
                                pex.ExportAddressTable = ParseExportAddressTable(data, exportDirectoryTable.AddressTableEntries);
                            }

                            // Name pointer table
                            offset = initialOffset + exportDirectoryTable.NamePointerRVA.ConvertVirtualAddress(pex.SectionTable);
                            if (exportDirectoryTable.NumberOfNamePointers != 0
                                && offset > initialOffset
                                && offset < data.Length)
                            {
                                data.SeekIfPossible(offset, SeekOrigin.Begin);
                                pex.NamePointerTable = ParseExportNamePointerTable(data, exportDirectoryTable.NumberOfNamePointers);
                            }

                            // Ordinal table
                            offset = initialOffset + exportDirectoryTable.OrdinalTableRVA.ConvertVirtualAddress(pex.SectionTable);
                            if (exportDirectoryTable.NumberOfNamePointers != 0
                                && offset > initialOffset
                                && offset < data.Length)
                            {
                                data.SeekIfPossible(offset, SeekOrigin.Begin);
                                pex.OrdinalTable = ParseExportOrdinalTable(data, exportDirectoryTable.NumberOfNamePointers);
                            }

                            // Name table
                            if (exportDirectoryTable.NumberOfNamePointers != 0 && pex.NamePointerTable?.Pointers is not null)
                                pex.ExportNameTable = ParseExportNameTable(data, initialOffset, pex.NamePointerTable.Pointers, pex.SectionTable);
                        }
                    }
                }

                #endregion

                #region Import Table

                // Should also be in a '.idata' section
                if (optionalHeader.ImportTable is not null)
                {
                    offset = initialOffset
                        + optionalHeader.ImportTable.VirtualAddress.ConvertVirtualAddress(pex.SectionTable);
                    if (offset > initialOffset && offset < data.Length)
                    {
                        // Get the required table size
                        int tableSize = (int)optionalHeader.ImportTable.Size;

                        // Read the table data
                        byte[]? tableData = data.ReadFrom(offset, tableSize, retainPosition: true) ?? [];

                        // Parse the import directory table
                        int tableDataOffset = 0;
                        var importDirectoryTable = ParseImportDirectoryTable(tableData, ref tableDataOffset);

                        // Set the import directory table
                        pex.ImportDirectoryTable = importDirectoryTable;

                        // If the export table was parsed, read the remaining pieces
                        if (pex.ImportDirectoryTable is not null)
                        {
                            // Names
                            for (int i = 0; i < importDirectoryTable.Length; i++)
                            {
                                var entry = importDirectoryTable[i];
                                if (entry is null)
                                    continue;

                                long nameOffset = initialOffset + entry.NameRVA.ConvertVirtualAddress(pex.SectionTable);

                                // If any name RVA is invalid, then the import table is invalid
                                if (nameOffset < initialOffset || nameOffset >= data.Length)
                                {
                                    pex.ImportDirectoryTable = null;
                                    break;
                                }

                                // If the name RVA is non-zero
                                if (nameOffset != initialOffset)
                                {
                                    data.SeekIfPossible(nameOffset, SeekOrigin.Begin);
                                    entry.Name = data.ReadNullTerminatedAnsiString() ?? string.Empty;
                                }
                            }

                            // If an error was not encountered, read the remaining tables
                            if (pex.ImportDirectoryTable is not null)
                            {
                                pex.ImportLookupTables = ParseImportLookupTables(data,
                                    initialOffset,
                                    optionalHeader.Magic,
                                    importDirectoryTable,
                                    pex.SectionTable);
                                pex.ImportAddressTables = ParseImportAddressTables(data,
                                    initialOffset,
                                    optionalHeader.Magic,
                                    importDirectoryTable,
                                    pex.SectionTable);
                                pex.HintNameTable = ParseHintNameTable(data,
                                    initialOffset,
                                    pex.ImportLookupTables,
                                    pex.ImportAddressTables,
                                    pex.SectionTable);
                            }
                        }
                    }
                }

                // TODO: Figure out how to use this in lieu of the current ParseImportAddressTables
                if (optionalHeader.ImportAddressTable is not null)
                {
                    offset = initialOffset
                        + optionalHeader.ImportAddressTable.VirtualAddress.ConvertVirtualAddress(pex.SectionTable);
                    if (offset > initialOffset && offset < data.Length)
                    {
                        // Get the required table size
                        int tableSize = (int)optionalHeader.ImportAddressTable.Size;

                        // Read the table data
                        byte[]? tableData = data.ReadFrom(offset, tableSize, retainPosition: true);

                        // Remaining code goes here
                    }
                }

                #endregion

                // TODO: Pre-parse known resource types
                #region Resource Directory Table

                // Should also be in a '.rsrc' section
                if (optionalHeader.ResourceTable is not null)
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
                        byte[] tableData = data.ReadFrom(offset, tableSize, retainPosition: true) ?? [];
                        if (tableData.Length > 0 && tableData.Length < optionalHeader.ResourceTable.Size)
                            Array.Resize(ref tableData, (int)optionalHeader.ResourceTable.Size);

                        // Set the resource directory table
                        long tableStart = data.Position;
                        int tableOffset = 0;
                        pex.ResourceDirectoryTable = ParseResourceDirectoryTable(tableData, ref tableOffset);

                        // Parse the resource data, if possible
                        ParseResourceData(data,
                            initialOffset,
                            tableData,
                            ref tableOffset,
                            offset,
                            optionalHeader.ResourceTable.Size,
                            pex.ResourceDirectoryTable,
                            pex.SectionTable);

                        #region Hidden Resources

                        // If we have not used up the full size, parse the remaining chunk as a single resource
                        if (pex.ResourceDirectoryTable?.Entries is not null
                            && tableOffset < tableSize
                            && (offset + tableOffset) != endOfSectionData)
                        {
                            // Resize the entry array to accomodate one more
                            var localEntries = pex.ResourceDirectoryTable.Entries;
                            Array.Resize(ref localEntries, localEntries.Length + 1);
                            pex.ResourceDirectoryTable.Entries = localEntries;

                            // Get the length of the remaining data
                            int length = tableSize - tableOffset;

                            // Add the hidden entry
                            pex.ResourceDirectoryTable.Entries[localEntries.Length - 1] = new Data.Models.PortableExecutable.Resource.DirectoryEntry
                            {
                                Name = new Data.Models.PortableExecutable.Resource.DirectoryString { UnicodeString = Encoding.Unicode.GetBytes("HIDDEN RESOURCE") },
                                IntegerID = uint.MaxValue,
                                DataEntryOffset = (uint)tableOffset,
                                DataEntry = new Data.Models.PortableExecutable.Resource.DataEntry
                                {
                                    Size = (uint)length,
                                    Data = tableData.ReadBytes(ref tableOffset, length),
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

                if (optionalHeader.CertificateTable is not null)
                {
                    // The Certificate Table entry points to a table of attribute certificates. These
                    // certificates are not loaded into memory as part of the image. As such, the first
                    // field of this entry, which is normally an RVA, is a file pointer instead.
                    offset = initialOffset + optionalHeader.CertificateTable.VirtualAddress;
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
                if (optionalHeader.BaseRelocationTable is not null)
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
                if (optionalHeader.Debug is not null)
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

                if (optionalHeader.DelayImportDescriptor is not null)
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
        /// Parse a byte array into an AcceleratorTable
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <returns>Filled AcceleratorTable on success, null on error</returns>
        public static Data.Models.PortableExecutable.Resource.Entries.AcceleratorTable? ParseAcceleratorTable(byte[] data)
        {
            // If we have data that's invalid for this resource type, we can't do anything
            if (data.Length % 8 != 0)
                return null;

            // Get the number of entries
            int count = data.Length / 8;

            // Create the output object
            var obj = new Data.Models.PortableExecutable.Resource.Entries.AcceleratorTable();
            obj.Entries = new Data.Models.PortableExecutable.Resource.Entries.AcceleratorTableEntry[count];

            // Read in the table
            int offset = 0;
            for (int i = 0; i < count; i++)
            {
                obj.Entries[i] = ParseAcceleratorTableEntry(data, ref offset);
            }

            return obj;
        }

        /// <summary>
        /// Parse a byte array into a AcceleratorTableEntry
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>A filled AcceleratorTableEntry on success, null on error</returns>
        public static Data.Models.PortableExecutable.Resource.Entries.AcceleratorTableEntry ParseAcceleratorTableEntry(byte[] data, ref int offset)
        {
            var obj = new Data.Models.PortableExecutable.Resource.Entries.AcceleratorTableEntry();

            obj.Flags = (AcceleratorTableFlags)data.ReadUInt16LittleEndian(ref offset);
            obj.Ansi = data.ReadUInt16LittleEndian(ref offset);
            obj.Id = data.ReadUInt16LittleEndian(ref offset);
            obj.Padding = data.ReadUInt16LittleEndian(ref offset);

            return obj;
        }

        /// <summary>
        /// Parse a byte array into an AssemblyManifest
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>Filled AssemblyManifest on success, null on error</returns>
        public static Data.Models.PortableExecutable.Resource.Entries.AssemblyManifest? ParseAssemblyManifest(byte[] data)
        {
            try
            {
                var serializer = new XmlSerializer(typeof(Data.Models.PortableExecutable.Resource.Entries.AssemblyManifest));
                return serializer.Deserialize(new MemoryStream(data)) as Data.Models.PortableExecutable.Resource.Entries.AssemblyManifest;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Parse a byte array into an attribute certificate table
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <returns>Filled attribute certificate on success, null on error</returns>
        public static Data.Models.PortableExecutable.AttributeCertificate.Entry[]? ParseAttributeCertificateTable(byte[]? data)
        {
            if (data is null)
                return null;

            var obj = new List<Data.Models.PortableExecutable.AttributeCertificate.Entry>();

            int offset = 0;
            while (offset < data.Length)
            {
                var entry = ParseAttributeCertificateTableEntry(data, ref offset);
                if (entry is null)
                    break;

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
        public static Data.Models.PortableExecutable.AttributeCertificate.Entry? ParseAttributeCertificateTableEntry(byte[] data, ref int offset)
        {
            var obj = new Data.Models.PortableExecutable.AttributeCertificate.Entry();

            obj.Length = data.ReadUInt32LittleEndian(ref offset);
            if (obj.Length < 8)
                return null;

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
        public static Data.Models.PortableExecutable.BaseRelocation.Block? ParseBaseRelocationBlock(byte[] data, ref int offset)
        {
            var obj = new Data.Models.PortableExecutable.BaseRelocation.Block();

            obj.PageRVA = data.ReadUInt32LittleEndian(ref offset);
            obj.BlockSize = data.ReadUInt32LittleEndian(ref offset);
            if (obj.BlockSize <= 8)
                return obj;

            // Guard against invalid block sizes
            if (offset + obj.BlockSize > data.Length)
                return null;
            if (obj.BlockSize % 2 != 0)
                return obj;

            int entryCount = ((int)obj.BlockSize - 8) / 2;
            obj.TypeOffsetFieldEntries = new Data.Models.PortableExecutable.BaseRelocation.TypeOffsetFieldEntry[entryCount];
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
        public static Data.Models.PortableExecutable.BaseRelocation.Block[]? ParseBaseRelocationTable(byte[]? data)
        {
            if (data is null)
                return null;

            var obj = new List<Data.Models.PortableExecutable.BaseRelocation.Block>();

            int offset = 0;
            while (offset + 8 <= data.Length)
            {
                var block = ParseBaseRelocationBlock(data, ref offset);
                if (block is null)
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
        public static Data.Models.PortableExecutable.BaseRelocation.TypeOffsetFieldEntry ParseBaseRelocationTypeOffsetFieldEntry(byte[] data, ref int offset)
        {
            var obj = new Data.Models.PortableExecutable.BaseRelocation.TypeOffsetFieldEntry();

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
        public static Data.Models.PortableExecutable.DebugData.Entry ParseDebugDirectoryEntry(byte[] data, ref int offset)
        {
            var obj = new Data.Models.PortableExecutable.DebugData.Entry();

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
        public static Data.Models.PortableExecutable.DebugData.Table? ParseDebugTable(byte[]? data)
        {
            if (data is null)
                return null;

            var obj = new Data.Models.PortableExecutable.DebugData.Table();

            var table = new List<Data.Models.PortableExecutable.DebugData.Entry>();

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
        public static Data.Models.PortableExecutable.DelayLoad.DirectoryTable? ParseDelayLoadDirectoryTable(byte[]? data)
        {
            if (data is null)
                return null;

            var obj = new Data.Models.PortableExecutable.DelayLoad.DirectoryTable();

            int offset = 0;
            obj.Attributes = data.ReadUInt32LittleEndian(ref offset);
            obj.NameRVA = data.ReadUInt32LittleEndian(ref offset);
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
        /// Parse a byte array into a DialogBoxResource
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <returns>A filled DialogBoxResource on success, null on error</returns>
        public static Data.Models.PortableExecutable.Resource.Entries.DialogBoxResource? ParseDialogBoxResource(byte[] data)
        {
            // Initialize the iterator
            int offset = 0;

            // Create the output object
            var obj = new Data.Models.PortableExecutable.Resource.Entries.DialogBoxResource();

            // Try to read the signature for an extended dialog box template
            int signatureOffset = sizeof(ushort);
            int possibleSignature = data.ReadUInt16LittleEndian(ref signatureOffset);
            if (possibleSignature == 0xFFFF)
            {
                obj.ExtendedDialogTemplate = ParseDialogTemplateExtended(data, ref offset);

                var dialogItemExtendedTemplates = new List<Data.Models.PortableExecutable.Resource.Entries.DialogItemTemplateExtended>();
                for (int i = 0; i < obj.ExtendedDialogTemplate.DialogItems; i++)
                {
                    var dialogItemTemplate = ParseDialogItemTemplateExtended(data, ref offset);
                    dialogItemExtendedTemplates.Add(dialogItemTemplate);

                    // If we have an invalid item count
                    if (offset >= data.Length)
                        break;
                }

                obj.ExtendedDialogItemTemplates = [.. dialogItemExtendedTemplates];
            }
            else
            {
                obj.DialogTemplate = ParseDialogTemplate(data, ref offset);

                var dialogItemTemplates = new List<Data.Models.PortableExecutable.Resource.Entries.DialogItemTemplate>();
                for (int i = 0; i < obj.DialogTemplate.ItemCount; i++)
                {
                    var dialogItemTemplate = ParseDialogItemTemplate(data, ref offset);
                    dialogItemTemplates.Add(dialogItemTemplate);

                    // If we have an invalid item count
                    if (offset >= data.Length)
                        break;
                }

                obj.DialogItemTemplates = [.. dialogItemTemplates];
            }

            return obj;
        }

        /// <summary>
        /// Parse a byte array into a DialogItemTemplate
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>Filled DialogItemTemplate on success, null on error</returns>
        public static Data.Models.PortableExecutable.Resource.Entries.DialogItemTemplate ParseDialogItemTemplate(byte[] data, ref int offset)
        {
            var obj = new Data.Models.PortableExecutable.Resource.Entries.DialogItemTemplate();

            obj.Style = (WindowStyles)data.ReadUInt32LittleEndian(ref offset);
            obj.ExtendedStyle = (ExtendedWindowStyles)data.ReadUInt32LittleEndian(ref offset);
            obj.PositionX = data.ReadInt16LittleEndian(ref offset);
            obj.PositionY = data.ReadInt16LittleEndian(ref offset);
            obj.WidthX = data.ReadInt16LittleEndian(ref offset);
            obj.HeightY = data.ReadInt16LittleEndian(ref offset);
            obj.ID = data.ReadUInt16LittleEndian(ref offset);

            #region Class resource

            int currentOffset = offset;
            ushort itemClassResourceIdentifier = data.ReadUInt16LittleEndian(ref offset);
            offset = currentOffset;

            // 0xFFFF means ordinal only
            if (itemClassResourceIdentifier == 0xFFFF)
            {
                // Increment the pointer
                _ = data.ReadUInt16LittleEndian(ref offset);

                // Read the ordinal
                obj.ClassResourceOrdinal = (DialogItemTemplateOrdinal)data.ReadUInt16LittleEndian(ref offset);
            }
            else
            {
                // Flag if there's an ordinal at the end
                bool classResourcehasOrdinal = itemClassResourceIdentifier == 0xFFFF;
                if (classResourcehasOrdinal)
                    offset += sizeof(ushort);

                // Read the class resource as a string
                obj.ClassResource = data.ReadNullTerminatedUnicodeString(ref offset) ?? string.Empty;

                // Align to the WORD boundary if we're not at the end
                AlignToBoundary(data, ref offset, 2);
            }

            #endregion

            #region Title resource

            currentOffset = offset;
            ushort itemTitleResourceIdentifier = data.ReadUInt16LittleEndian(ref offset);
            offset = currentOffset;

            // 0xFFFF means ordinal only
            if (itemTitleResourceIdentifier == 0xFFFF)
            {
                // Increment the pointer
                _ = data.ReadUInt16LittleEndian(ref offset);

                // Read the ordinal
                obj.TitleResourceOrdinal = data.ReadUInt16LittleEndian(ref offset);
            }
            else
            {
                // Read the title resource as a string
                obj.TitleResource = data.ReadNullTerminatedUnicodeString(ref offset) ?? string.Empty;

                // Align to the WORD boundary if we're not at the end
                AlignToBoundary(data, ref offset, 2);
            }

            #endregion

            #region Creation data

            obj.CreationDataSize = data.ReadUInt16LittleEndian(ref offset);
            if (obj.CreationDataSize != 0 && obj.CreationDataSize + offset < data.Length)
                obj.CreationData = data.ReadBytes(ref offset, obj.CreationDataSize);

            #endregion

            // Align to the DWORD boundary if we're not at the end
            AlignToBoundary(data, ref offset, 4);

            return obj;
        }

        /// <summary>
        /// Parse a byte array into a DialogItemTemplateExtended
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>Filled DialogItemTemplateExtended on success, null on error</returns>
        public static Data.Models.PortableExecutable.Resource.Entries.DialogItemTemplateExtended ParseDialogItemTemplateExtended(byte[] data, ref int offset)
        {
            var obj = new Data.Models.PortableExecutable.Resource.Entries.DialogItemTemplateExtended();

            obj.HelpID = data.ReadUInt32LittleEndian(ref offset);
            obj.ExtendedStyle = (ExtendedWindowStyles)data.ReadUInt32LittleEndian(ref offset);
            obj.Style = (WindowStyles)data.ReadUInt32LittleEndian(ref offset);
            obj.PositionX = data.ReadInt16LittleEndian(ref offset);
            obj.PositionY = data.ReadInt16LittleEndian(ref offset);
            obj.WidthX = data.ReadInt16LittleEndian(ref offset);
            obj.HeightY = data.ReadInt16LittleEndian(ref offset);
            obj.ID = data.ReadUInt32LittleEndian(ref offset);

            #region Class resource

            int currentOffset = offset;
            ushort itemClassResourceIdentifier = data.ReadUInt16LittleEndian(ref offset);
            offset = currentOffset;

            // 0xFFFF means ordinal only
            if (itemClassResourceIdentifier == 0xFFFF)
            {
                // Increment the pointer
                _ = data.ReadUInt16LittleEndian(ref offset);

                // Read the ordinal
                obj.ClassResourceOrdinal = (DialogItemTemplateOrdinal)data.ReadUInt16LittleEndian(ref offset);
            }
            else
            {
                // Flag if there's an ordinal at the end
                bool classResourcehasOrdinal = itemClassResourceIdentifier == 0xFFFF;
                if (classResourcehasOrdinal)
                    offset += sizeof(ushort);

                // Read the class resource as a string
                obj.ClassResource = data.ReadNullTerminatedUnicodeString(ref offset) ?? string.Empty;

                // Align to the WORD boundary if we're not at the end
                AlignToBoundary(data, ref offset, 2);
            }

            #endregion

            #region Title resource

            currentOffset = offset;
            ushort itemTitleResourceIdentifier = data.ReadUInt16LittleEndian(ref offset);
            offset = currentOffset;

            // 0xFFFF means ordinal only
            if (itemTitleResourceIdentifier == 0xFFFF)
            {
                // Increment the pointer
                _ = data.ReadUInt16LittleEndian(ref offset);

                // Read the ordinal
                obj.TitleResourceOrdinal = data.ReadUInt16LittleEndian(ref offset);
            }
            else
            {
                // Read the title resource as a string
                obj.TitleResource = data.ReadNullTerminatedUnicodeString(ref offset) ?? string.Empty;

                // Align to the WORD boundary if we're not at the end
                AlignToBoundary(data, ref offset, 2);
            }

            #endregion

            #region Creation data

            obj.CreationDataSize = data.ReadUInt16LittleEndian(ref offset);
            if (obj.CreationDataSize != 0)
                obj.CreationData = data.ReadBytes(ref offset, obj.CreationDataSize);

            #endregion

            // Align to the DWORD boundary if we're not at the end
            AlignToBoundary(data, ref offset, 4);

            return obj;
        }

        /// <summary>
        /// Parse a byte array into a DialogTemplate
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>Filled DialogTemplate on success, null on error</returns>
        public static Data.Models.PortableExecutable.Resource.Entries.DialogTemplate ParseDialogTemplate(byte[] data, ref int offset)
        {
            var obj = new Data.Models.PortableExecutable.Resource.Entries.DialogTemplate();

            obj.Style = (WindowStyles)data.ReadUInt32LittleEndian(ref offset);
            obj.ExtendedStyle = (ExtendedWindowStyles)data.ReadUInt32LittleEndian(ref offset);
            obj.ItemCount = data.ReadUInt16LittleEndian(ref offset);
            obj.PositionX = data.ReadInt16LittleEndian(ref offset);
            obj.PositionY = data.ReadInt16LittleEndian(ref offset);
            obj.WidthX = data.ReadInt16LittleEndian(ref offset);
            obj.HeightY = data.ReadInt16LittleEndian(ref offset);

            #region Menu resource

            int currentOffset = offset;
            ushort menuResourceIdentifier = data.ReadUInt16LittleEndian(ref offset);
            offset = currentOffset;

            // 0x0000 means no elements
            if (menuResourceIdentifier == 0x0000)
            {
                // Increment the pointer if it was empty
                offset += sizeof(ushort);
            }
            else
            {
                // Flag if there's an ordinal at the end
                bool menuResourceHasOrdinal = menuResourceIdentifier == 0xFFFF;
                if (menuResourceHasOrdinal)
                    offset += sizeof(ushort);

                // Read the menu resource as a string
                obj.MenuResource = data.ReadNullTerminatedUnicodeString(ref offset) ?? string.Empty;

                // Align to the WORD boundary if we're not at the end
                AlignToBoundary(data, ref offset, 2);

                // Read the ordinal if we have the flag set
                if (menuResourceHasOrdinal)
                    obj.MenuResourceOrdinal = data.ReadUInt16LittleEndian(ref offset);
            }

            #endregion

            #region Class resource

            currentOffset = offset;
            ushort classResourceIdentifier;
            if (offset >= data.Length)
                classResourceIdentifier = 0x0000;
            else
                classResourceIdentifier = data.ReadUInt16LittleEndian(ref offset);
            offset = currentOffset;

            // 0x0000 means no elements
            if (classResourceIdentifier == 0x0000)
            {
                // Increment the pointer if it was empty
                offset += sizeof(ushort);
            }
            else
            {
                // Flag if there's an ordinal at the end
                bool classResourcehasOrdinal = classResourceIdentifier == 0xFFFF;
                if (classResourcehasOrdinal)
                    offset += sizeof(ushort);

                // Read the class resource as a string
                obj.ClassResource = data.ReadNullTerminatedUnicodeString(ref offset) ?? string.Empty;

                // Align to the WORD boundary if we're not at the end
                AlignToBoundary(data, ref offset, 2);

                // Read the ordinal if we have the flag set
                if (classResourcehasOrdinal)
                    obj.ClassResourceOrdinal = data.ReadUInt16LittleEndian(ref offset);
            }

            #endregion

            #region Title resource

            currentOffset = offset;
            ushort titleResourceIdentifier;
            if (offset >= data.Length)
                titleResourceIdentifier = 0x0000;
            else
                titleResourceIdentifier = data.ReadUInt16LittleEndian(ref offset);
            offset = currentOffset;

            // 0x0000 means no elements
            if (titleResourceIdentifier == 0x0000)
            {
                // Increment the pointer if it was empty
                offset += sizeof(ushort);
            }
            else
            {
                // Read the title resource as a string
                obj.TitleResource = data.ReadNullTerminatedUnicodeString(ref offset) ?? string.Empty;

                // Align to the WORD boundary if we're not at the end
                AlignToBoundary(data, ref offset, 2);
            }

            #endregion

            #region Point size and typeface

            // Only if DS_SETFONT is set are the values here used
#if NET20 || NET35
            if ((obj.Style & WindowStyles.DS_SETFONT) != 0)
#else
            if (obj.Style.HasFlag(WindowStyles.DS_SETFONT))
#endif
            {
                obj.PointSizeValue = data.ReadUInt16LittleEndian(ref offset);

                // Read the font name as a string
                obj.Typeface = data.ReadNullTerminatedUnicodeString(ref offset) ?? string.Empty;
            }

            // Align to the DWORD boundary if we're not at the end
            AlignToBoundary(data, ref offset, 4);

            #endregion

            return obj;
        }

        /// <summary>
        /// Parse a byte array into a DialogTemplateExtended
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>Filled DialogTemplateExtended on success, null on error</returns>
        public static Data.Models.PortableExecutable.Resource.Entries.DialogTemplateExtended ParseDialogTemplateExtended(byte[] data, ref int offset)
        {
            var obj = new Data.Models.PortableExecutable.Resource.Entries.DialogTemplateExtended();

            obj.Version = data.ReadUInt16LittleEndian(ref offset);
            obj.Signature = data.ReadUInt16LittleEndian(ref offset);
            obj.HelpID = data.ReadUInt32LittleEndian(ref offset);
            obj.ExtendedStyle = (ExtendedWindowStyles)data.ReadUInt32LittleEndian(ref offset);
            obj.Style = (WindowStyles)data.ReadUInt32LittleEndian(ref offset);
            obj.DialogItems = data.ReadUInt16LittleEndian(ref offset);
            obj.PositionX = data.ReadInt16LittleEndian(ref offset);
            obj.PositionY = data.ReadInt16LittleEndian(ref offset);
            obj.WidthX = data.ReadInt16LittleEndian(ref offset);
            obj.HeightY = data.ReadInt16LittleEndian(ref offset);

            #region Menu resource

            int currentOffset = offset;
            ushort menuResourceIdentifier = data.ReadUInt16LittleEndian(ref offset);
            offset = currentOffset;

            // 0x0000 means no elements
            if (menuResourceIdentifier == 0x0000)
            {
                // Increment the pointer if it was empty
                offset += sizeof(ushort);
            }
            else
            {
                // Flag if there's an ordinal at the end
                bool menuResourceHasOrdinal = menuResourceIdentifier == 0xFFFF;
                if (menuResourceHasOrdinal)
                    offset += sizeof(ushort);

                // Read the menu resource as a string
                obj.MenuResource = data.ReadNullTerminatedUnicodeString(ref offset) ?? string.Empty;

                // Align to the WORD boundary if we're not at the end
                AlignToBoundary(data, ref offset, 2);

                // Read the ordinal if we have the flag set
                if (menuResourceHasOrdinal)
                    obj.MenuResourceOrdinal = data.ReadUInt16LittleEndian(ref offset);
            }

            #endregion

            #region Class resource

            currentOffset = offset;
            ushort classResourceIdentifier = data.ReadUInt16LittleEndian(ref offset);
            offset = currentOffset;

            // 0x0000 means no elements
            if (classResourceIdentifier == 0x0000)
            {
                // Increment the pointer if it was empty
                offset += sizeof(ushort);
            }
            else
            {
                // Flag if there's an ordinal at the end
                bool classResourcehasOrdinal = classResourceIdentifier == 0xFFFF;
                if (classResourcehasOrdinal)
                    offset += sizeof(ushort);

                // Read the class resource as a string
                obj.ClassResource = data.ReadNullTerminatedUnicodeString(ref offset) ?? string.Empty;

                // Align to the WORD boundary if we're not at the end
                AlignToBoundary(data, ref offset, 2);

                // Read the ordinal if we have the flag set
                if (classResourcehasOrdinal)
                    obj.ClassResourceOrdinal = data.ReadUInt16LittleEndian(ref offset);
            }

            #endregion

            #region Title resource

            currentOffset = offset;
            ushort titleResourceIdentifier = data.ReadUInt16LittleEndian(ref offset);
            offset = currentOffset;

            // 0x0000 means no elements
            if (titleResourceIdentifier == 0x0000)
            {
                // Increment the pointer if it was empty
                offset += sizeof(ushort);
            }
            else
            {
                // Read the title resource as a string
                obj.TitleResource = data.ReadNullTerminatedUnicodeString(ref offset) ?? string.Empty;

                // Align to the WORD boundary if we're not at the end
                AlignToBoundary(data, ref offset, 2);
            }

            #endregion

            #region Point size and typeface

            // Only if DS_SETFONT is set are the values here used
#if NET20 || NET35
            if ((obj.Style & WindowStyles.DS_SETFONT) != 0)
#else
            if (obj.Style.HasFlag(WindowStyles.DS_SETFONT))
#endif
            {
                obj.PointSize = data.ReadUInt16LittleEndian(ref offset);
                obj.Weight = data.ReadUInt16LittleEndian(ref offset);
                obj.Italic = data.ReadByte(ref offset);
                obj.CharSet = data.ReadByte(ref offset);
                obj.Typeface = data.ReadNullTerminatedUnicodeString(ref offset) ?? string.Empty;
            }

            // Align to the DWORD boundary if we're not at the end
            AlignToBoundary(data, ref offset, 4);

            #endregion

            return obj;
        }

        /// <summary>
        /// Parse a byte array into a DirEntry
        /// </summary>
        /// <param name="data">Data to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>A filled DirEntry on success, null on error</returns>
        public static Data.Models.PortableExecutable.Resource.Entries.DirEntry ParseDirEntry(byte[] data, ref int offset)
        {
            var obj = new Data.Models.PortableExecutable.Resource.Entries.DirEntry();

            obj.FontOrdinal = data.ReadUInt16LittleEndian(ref offset);
            obj.Entry = ParseFontDirEntry(data, ref offset);

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a ExportAddressTable
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="entries">Number of entries in the table</param>
        /// <returns>Filled ExportAddressTable on success, null on error</returns>
        public static Data.Models.PortableExecutable.Export.AddressTableEntry[] ParseExportAddressTable(Stream data, uint entries)
        {
            var obj = new Data.Models.PortableExecutable.Export.AddressTableEntry[entries];

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
        public static Data.Models.PortableExecutable.Export.AddressTableEntry ParseExportAddressTableEntry(Stream data)
        {
            var obj = new Data.Models.PortableExecutable.Export.AddressTableEntry();

            obj.ExportRVA = data.ReadUInt32LittleEndian();
            obj.ForwarderRVA = obj.ExportRVA;

            return obj;
        }

        /// <summary>
        /// Parse a byte array into a ExportDirectoryTable
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>Filled ExportDirectoryTable on success, null on error</returns>
        public static Data.Models.PortableExecutable.Export.DirectoryTable ParseExportDirectoryTable(byte[] data, ref int offset)
        {
            var obj = new Data.Models.PortableExecutable.Export.DirectoryTable();

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
        public static Data.Models.PortableExecutable.Export.NameTable ParseExportNameTable(Stream data, long initialOffset, uint[] pointers, SectionHeader[] sections)
        {
            var obj = new Data.Models.PortableExecutable.Export.NameTable();

            obj.Strings = new string[pointers.Length];
            for (int i = 0; i < obj.Strings.Length; i++)
            {
                long address = initialOffset + pointers[i].ConvertVirtualAddress(sections);
                if (address > initialOffset && address < data.Length)
                {
                    data.SeekIfPossible(address, SeekOrigin.Begin);

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
        public static Data.Models.PortableExecutable.Export.NamePointerTable ParseExportNamePointerTable(Stream data, uint entries)
        {
            var obj = new Data.Models.PortableExecutable.Export.NamePointerTable();

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
        public static Data.Models.PortableExecutable.Export.OrdinalTable ParseExportOrdinalTable(Stream data, uint entries)
        {
            var obj = new Data.Models.PortableExecutable.Export.OrdinalTable();

            obj.Indexes = new ushort[entries];
            for (int i = 0; i < obj.Indexes.Length; i++)
            {
                ushort pointer = data.ReadUInt16LittleEndian();
                obj.Indexes[i] = pointer;
            }

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a FileHeader
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled FileHeader on success, null on error</returns>
        public static FileHeader ParseFileHeader(Stream data)
        {
            var obj = new FileHeader();

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
        /// Parse a byte array into a FixedFileInfo
        /// </summary>
        /// <param name="data">Data to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>A filled FixedFileInfo on success, null on error</returns>
        public static Data.Models.PortableExecutable.Resource.Entries.FixedFileInfo ParseFixedFileInfo(byte[] data, ref int offset)
        {
            var obj = new Data.Models.PortableExecutable.Resource.Entries.FixedFileInfo();

            obj.Signature = data.ReadUInt32LittleEndian(ref offset);
            obj.StrucVersion = data.ReadUInt32LittleEndian(ref offset);
            obj.FileVersionMS = data.ReadUInt32LittleEndian(ref offset);
            obj.FileVersionLS = data.ReadUInt32LittleEndian(ref offset);
            obj.ProductVersionMS = data.ReadUInt32LittleEndian(ref offset);
            obj.ProductVersionLS = data.ReadUInt32LittleEndian(ref offset);
            obj.FileFlagsMask = data.ReadUInt32LittleEndian(ref offset);
            obj.FileFlags = (FixedFileInfoFlags)data.ReadUInt32LittleEndian(ref offset);
            obj.FileOS = (FixedFileInfoOS)data.ReadUInt32LittleEndian(ref offset);
            obj.FileType = (FixedFileInfoFileType)data.ReadUInt32LittleEndian(ref offset);
            obj.FileSubtype = (FixedFileInfoFileSubtype)data.ReadUInt32LittleEndian(ref offset);
            obj.FileDateMS = data.ReadUInt32LittleEndian(ref offset);
            obj.FileDateLS = data.ReadUInt32LittleEndian(ref offset);

            return obj;
        }

        /// <summary>
        /// Parse a byte array into a FontDirEntry
        /// </summary>
        /// <param name="data">Data to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>A filled FontDirEntry on success, null on error</returns>
        public static Data.Models.PortableExecutable.Resource.Entries.FontDirEntry ParseFontDirEntry(byte[] data, ref int offset)
        {
            var obj = new Data.Models.PortableExecutable.Resource.Entries.FontDirEntry();

            obj.Version = data.ReadUInt16LittleEndian(ref offset);
            obj.Size = data.ReadUInt32LittleEndian(ref offset);
            obj.Copyright = data.ReadBytes(ref offset, 60);
            obj.Type = data.ReadUInt16LittleEndian(ref offset);
            obj.Points = data.ReadUInt16LittleEndian(ref offset);
            obj.VertRes = data.ReadUInt16LittleEndian(ref offset);
            obj.HorizRes = data.ReadUInt16LittleEndian(ref offset);
            obj.Ascent = data.ReadUInt16LittleEndian(ref offset);
            obj.InternalLeading = data.ReadUInt16LittleEndian(ref offset);
            obj.ExternalLeading = data.ReadUInt16LittleEndian(ref offset);
            obj.Italic = data.ReadByte(ref offset);
            obj.Underline = data.ReadByte(ref offset);
            obj.StrikeOut = data.ReadByte(ref offset);
            obj.Weight = data.ReadUInt16LittleEndian(ref offset);
            obj.CharSet = data.ReadByte(ref offset);
            obj.PixWidth = data.ReadUInt16LittleEndian(ref offset);
            obj.PixHeight = data.ReadUInt16LittleEndian(ref offset);
            obj.PitchAndFamily = data.ReadByte(ref offset);
            obj.AvgWidth = data.ReadUInt16LittleEndian(ref offset);
            obj.MaxWidth = data.ReadUInt16LittleEndian(ref offset);
            obj.FirstChar = data.ReadByte(ref offset);
            obj.LastChar = data.ReadByte(ref offset);
            obj.DefaultChar = data.ReadByte(ref offset);
            obj.BreakChar = data.ReadByte(ref offset);
            obj.WidthBytes = data.ReadUInt16LittleEndian(ref offset);
            obj.Device = data.ReadUInt32LittleEndian(ref offset);
            obj.Face = data.ReadUInt32LittleEndian(ref offset);
            obj.Reserved = data.ReadUInt32LittleEndian(ref offset);

            // TODO: Determine how to read these two? Immediately after?
            obj.DeviceName = data.ReadNullTerminatedAnsiString(ref offset) ?? string.Empty;
            obj.FaceName = data.ReadNullTerminatedAnsiString(ref offset) ?? string.Empty;

            return obj;
        }

        /// <summary>
        /// Parse a byte array into a FontGroupHeader
        /// </summary>
        /// <param name="data">Data to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>A filled FontGroupHeader on success, null on error</returns>
        public static Data.Models.PortableExecutable.Resource.Entries.FontGroupHeader ParseFontGroupHeader(byte[] data)
        {
            // Initialize the iterator
            int offset = 0;

            // Create the output object
            var fontGroupHeader = new Data.Models.PortableExecutable.Resource.Entries.FontGroupHeader();

            fontGroupHeader.NumberOfFonts = data.ReadUInt16LittleEndian(ref offset);
            if (fontGroupHeader.NumberOfFonts > 0)
            {
                fontGroupHeader.DE = new Data.Models.PortableExecutable.Resource.Entries.DirEntry[fontGroupHeader.NumberOfFonts];
                for (int i = 0; i < fontGroupHeader.NumberOfFonts; i++)
                {
                    fontGroupHeader.DE[i] = ParseDirEntry(data, ref offset);
                }
            }

            // TODO: Implement entry parsing
            return fontGroupHeader;
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
        public static Data.Models.PortableExecutable.Import.HintNameTableEntry[] ParseHintNameTable(Stream data,
            long initialOffset,
            Dictionary<int, Data.Models.PortableExecutable.Import.LookupTableEntry[]?> importLookupTables,
            Dictionary<int, Data.Models.PortableExecutable.Import.AddressTableEntry[]?> importAddressTables,
            SectionHeader[] sections)
        {
            var importHintNameTable = new List<Data.Models.PortableExecutable.Import.HintNameTableEntry>();

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
                        if (kvp.Value is null || kvp.Value.Length == 0)
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
                        if (kvp.Value is null || kvp.Value.Length == 0)
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
                        data.SeekIfPossible(hintNameTableEntryAddress, SeekOrigin.Begin);

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
        public static Data.Models.PortableExecutable.Import.HintNameTableEntry ParseHintNameTableEntry(Stream data)
        {
            var obj = new Data.Models.PortableExecutable.Import.HintNameTableEntry();

            obj.Hint = data.ReadUInt16LittleEndian();
            string? name = data.ReadNullTerminatedAnsiString();
            if (name is not null)
                obj.Name = name;

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a ImportAddressTable
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="magic">Optional header magic number indicating PE32 or PE32+</param>
        /// <returns>Filled ImportAddressTable on success, null on error</returns>
        public static Data.Models.PortableExecutable.Import.AddressTableEntry[] ParseImportAddressTable(Stream data, OptionalHeaderMagicNumber magic)
        {
            var obj = new List<Data.Models.PortableExecutable.Import.AddressTableEntry>();

            // Loop until the last item (all nulls) are found
            while (data.Position < data.Length)
            {
                var entry = ParseImportAddressTableEntry(data, magic);
                obj.Add(entry);

                // All zero values means the last entry
                if (!entry.OrdinalNameFlag
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
        public static Dictionary<int, Data.Models.PortableExecutable.Import.AddressTableEntry[]?> ParseImportAddressTables(Stream data,
            long initialOffset,
            OptionalHeaderMagicNumber magic,
            Data.Models.PortableExecutable.Import.DirectoryTableEntry[] entries,
            SectionHeader[] sections)
        {
            var obj = new Dictionary<int, Data.Models.PortableExecutable.Import.AddressTableEntry[]?>();

            for (int i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];
                if (entry.ImportAddressTableRVA.ConvertVirtualAddress(sections) == 0)
                    continue;

                long tableAddress = initialOffset
                    + entry.ImportAddressTableRVA.ConvertVirtualAddress(sections);

                if (tableAddress > initialOffset && tableAddress < data.Length)
                {
                    data.SeekIfPossible(tableAddress, SeekOrigin.Begin);
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
        public static Data.Models.PortableExecutable.Import.AddressTableEntry ParseImportAddressTableEntry(Stream data, OptionalHeaderMagicNumber magic)
        {
            var obj = new Data.Models.PortableExecutable.Import.AddressTableEntry();

            if (magic == OptionalHeaderMagicNumber.PE32)
            {
                uint value = data.ReadUInt32LittleEndian();
                obj.OrdinalNameFlag = (value & 0x80000000) != 0;
                if (obj.OrdinalNameFlag)
                    obj.OrdinalNumber = (ushort)(value & ~0x80000000);
                else
                    obj.HintNameTableRVA = value & ~0x80000000;
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
        public static Data.Models.PortableExecutable.Import.DirectoryTableEntry[] ParseImportDirectoryTable(byte[] data, ref int offset)
        {
            var obj = new List<Data.Models.PortableExecutable.Import.DirectoryTableEntry>();

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
        public static Data.Models.PortableExecutable.Import.DirectoryTableEntry ParseImportDirectoryTableEntry(byte[] data, ref int offset)
        {
            var obj = new Data.Models.PortableExecutable.Import.DirectoryTableEntry();

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
        public static Data.Models.PortableExecutable.Import.LookupTableEntry[] ParseImportLookupTable(Stream data, OptionalHeaderMagicNumber magic)
        {
            var obj = new List<Data.Models.PortableExecutable.Import.LookupTableEntry>();

            // Loop until the last item (all nulls) are found
            while (data.Position < data.Length)
            {
                var entry = ParseImportLookupTableEntry(data, magic);
                obj.Add(entry);

                // All zero values means the last entry
                if (!entry.OrdinalNameFlag
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
        public static Dictionary<int, Data.Models.PortableExecutable.Import.LookupTableEntry[]?> ParseImportLookupTables(Stream data,
            long initialOffset,
            OptionalHeaderMagicNumber magic,
            Data.Models.PortableExecutable.Import.DirectoryTableEntry[] entries,
            SectionHeader[] sections)
        {
            // Lookup tables
            var obj = new Dictionary<int, Data.Models.PortableExecutable.Import.LookupTableEntry[]?>();

            for (int i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];
                if (entry.ImportLookupTableRVA.ConvertVirtualAddress(sections) == 0)
                    continue;

                long tableAddress = initialOffset
                    + entry.ImportLookupTableRVA.ConvertVirtualAddress(sections);

                if (tableAddress > initialOffset && tableAddress < data.Length)
                {
                    data.SeekIfPossible(tableAddress, SeekOrigin.Begin);
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
        public static Data.Models.PortableExecutable.Import.LookupTableEntry ParseImportLookupTableEntry(Stream data, OptionalHeaderMagicNumber magic)
        {
            var obj = new Data.Models.PortableExecutable.Import.LookupTableEntry();

            if (magic == OptionalHeaderMagicNumber.PE32)
            {
                uint value = data.ReadUInt32LittleEndian();

                obj.OrdinalNameFlag = (value & 0x80000000) != 0;
                if (obj.OrdinalNameFlag)
                    obj.OrdinalNumber = (ushort)(value & ~0x80000000);
                else
                    obj.HintNameTableRVA = value & ~0x80000000;
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
        /// Read either a `StringFileInfo` or `VarFileInfo` based on the key
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <param name="versionInfo"></param>
        public static void ReadInfoSection(byte[] data, ref int offset, Data.Models.PortableExecutable.Resource.Entries.VersionInfo versionInfo)
        {
            // If the offset is invalid, don't move the pointer
            if (offset < 0 || offset >= versionInfo.Length)
                return;

            // Cache the current offset for reading
            int currentOffset = offset;

            offset += 6;
            string? nextKey = data.ReadNullTerminatedUnicodeString(ref offset);
            offset = currentOffset;

            if (nextKey == "StringFileInfo")
            {
                var stringFileInfo = ParseStringFileInfo(data, ref offset);
                versionInfo.StringFileInfo = stringFileInfo;
            }
            else if (nextKey == "VarFileInfo")
            {
                var varFileInfo = ParseVarFileInfo(data, ref offset);
                versionInfo.VarFileInfo = varFileInfo;
            }
        }

        /// <summary>
        /// Parse a Stream into a LineNumber
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled LineNumber on success, null on error</returns>
        public static LineNumber ParseLineNumber(Stream data)
        {
            var obj = new LineNumber();

            obj.SymbolTableIndex = data.ReadUInt32LittleEndian();
            obj.VirtualAddress = obj.SymbolTableIndex;
            obj.Linenumber = data.ReadUInt16LittleEndian();

            return obj;
        }

        /// <summary>
        /// Parse a byte array into a MenuHeaderExtended
        /// </summary>
        /// <param name="data">Data to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>A filled MenuHeaderExtended on success, null on error</returns>
        public static Data.Models.PortableExecutable.Resource.Entries.MenuHeaderExtended ParseMenuHeaderExtended(byte[] data, ref int offset)
        {
            var obj = new Data.Models.PortableExecutable.Resource.Entries.MenuHeaderExtended();

            obj.Version = data.ReadUInt16LittleEndian(ref offset);
            obj.Offset = data.ReadUInt16LittleEndian(ref offset);
            obj.HelpID = data.ReadUInt32LittleEndian(ref offset);

            return obj;
        }

        /// <summary>
        /// Parse a byte array into a MenuItemExtended
        /// </summary>
        /// <param name="data">Data to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>A filled MenuItemExtended on success, null on error</returns>
        public static Data.Models.PortableExecutable.Resource.Entries.MenuItemExtended ParseMenuItemExtended(byte[] data, ref int offset)
        {
            var obj = new Data.Models.PortableExecutable.Resource.Entries.MenuItemExtended();

            obj.ItemType = (MenuFlags)data.ReadUInt32LittleEndian(ref offset);
            obj.State = (MenuFlags)data.ReadUInt32LittleEndian(ref offset);
            obj.ID = data.ReadUInt32LittleEndian(ref offset);
            obj.Flags = (MenuFlags)data.ReadUInt32LittleEndian(ref offset);
            obj.MenuText = data.ReadNullTerminatedUnicodeString(ref offset) ?? string.Empty;

            return obj;
        }

        /// <summary>
        /// Parse a byte array into a MenuResource
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <returns>Filled MenuResource on success, null on error</returns>
        public static Data.Models.PortableExecutable.Resource.Entries.MenuResource? ParseMenuResource(byte[] data)
        {
            // Initialize the iterator
            int offset = 0;

            // Create the output object
            var obj = new Data.Models.PortableExecutable.Resource.Entries.MenuResource();

            // Try to read the version for an extended header
            int versionOffset = 0;
            int possibleVersion = data.ReadUInt16LittleEndian(ref versionOffset);
            if (possibleVersion == 0x0001)
            {
                var menuHeaderExtended = ParseMenuHeaderExtended(data, ref offset);
                obj.MenuHeader = menuHeaderExtended;

                var extendedMenuItems = new List<Data.Models.PortableExecutable.Resource.Entries.MenuItemExtended>();
                if (offset != 0)
                {
                    offset = menuHeaderExtended.Offset;

                    while (offset < data.Length)
                    {
                        var extendedMenuItem = ParseMenuItemExtended(data, ref offset);
                        extendedMenuItems.Add(extendedMenuItem);

                        // Align to the DWORD boundary if we're not at the end
                        AlignToBoundary(data, ref offset, 4);
                    }
                }

                obj.MenuItems = [.. extendedMenuItems];
            }
            else
            {
                obj.MenuHeader = ParseNormalMenuHeader(data, ref offset);

                #region Menu items

                var menuItems = new List<Data.Models.PortableExecutable.Resource.Entries.MenuItem>();

                while (offset < data.Length)
                {
                    // Determine if this is a popup
                    int flagsOffset = offset;
                    var initialFlags = (MenuFlags)data.ReadUInt16LittleEndian(ref flagsOffset);

                    Data.Models.PortableExecutable.Resource.Entries.MenuItem? menuItem;
#if NET20 || NET35
                    if ((initialFlags & MenuFlags.MF_POPUP) != 0)
#else
                    if (initialFlags.HasFlag(MenuFlags.MF_POPUP))
#endif
                        menuItem = ParsePopupMenuItem(data, ref offset);
                    else
                        menuItem = ParseNormalMenuItem(data, ref offset);

                    // Align to the DWORD boundary if we're not at the end
                    AlignToBoundary(data, ref offset, 4);

                    if (menuItem is null)
                        return null;

                    menuItems.Add(menuItem);
                }

                obj.MenuItems = [.. menuItems];

                #endregion
            }

            return obj;
        }

        /// <summary>
        /// Parse a byte array into a MessageResourceBlock
        /// </summary>
        /// <param name="data">Data to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>A filled MessageResourceBlock on success, null on error</returns>
        public static Data.Models.PortableExecutable.Resource.Entries.MessageResourceBlock ParseMessageResourceBlock(byte[] data, ref int offset)
        {
            var obj = new Data.Models.PortableExecutable.Resource.Entries.MessageResourceBlock();

            obj.LowId = data.ReadUInt32LittleEndian(ref offset);
            obj.HighId = data.ReadUInt32LittleEndian(ref offset);
            obj.OffsetToEntries = data.ReadUInt32LittleEndian(ref offset);

            return obj;
        }

        /// <summary>
        /// Read resource data as a MessageResourceData
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <returns>A filled MessageResourceData on success, null on error</returns>
        public static Data.Models.PortableExecutable.Resource.Entries.MessageResourceData? ParseMessageResourceData(byte[] data)
        {
            // Initialize the iterator
            int offset = 0;

            // Create the output object
            var obj = new Data.Models.PortableExecutable.Resource.Entries.MessageResourceData();

            // Message resource blocks
            obj.NumberOfBlocks = data.ReadUInt32LittleEndian(ref offset);
            if (obj.NumberOfBlocks > 0)
            {
                var messageResourceBlocks = new List<Data.Models.PortableExecutable.Resource.Entries.MessageResourceBlock>();

                for (int i = 0; i < obj.NumberOfBlocks; i++)
                {
                    var messageResourceBlock = ParseMessageResourceBlock(data, ref offset);
                    messageResourceBlocks.Add(messageResourceBlock);
                }

                obj.Blocks = [.. messageResourceBlocks];
            }

            // Message resource entries
            if (obj.Blocks.Length != 0)
            {
                var messageResourceEntries = new Dictionary<uint, Data.Models.PortableExecutable.Resource.Entries.MessageResourceEntry?>();

                for (int i = 0; i < obj.Blocks.Length; i++)
                {
                    var messageResourceBlock = obj.Blocks[i];
                    if (messageResourceBlock is null)
                        continue;

                    offset = (int)messageResourceBlock.OffsetToEntries;

                    for (uint j = messageResourceBlock.LowId; j <= messageResourceBlock.HighId; j++)
                    {
                        messageResourceEntries[j] = ParseMessageResourceEntry(data, ref offset);
                    }
                }

                obj.Entries = messageResourceEntries;
            }

            return obj;
        }

        /// <summary>
        /// Parse a byte array into a MessageResourceEntry
        /// </summary>
        /// <param name="data">Data to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>A filled MessageResourceEntry on success, null on error</returns>
        public static Data.Models.PortableExecutable.Resource.Entries.MessageResourceEntry ParseMessageResourceEntry(byte[] data, ref int offset)
        {
            var obj = new Data.Models.PortableExecutable.Resource.Entries.MessageResourceEntry();

            obj.Length = data.ReadUInt16LittleEndian(ref offset);
            obj.Flags = data.ReadUInt16LittleEndian(ref offset);

            Encoding textEncoding = obj.Flags == 0x0001 ? Encoding.Unicode : Encoding.ASCII;
            byte[] textArray = data.ReadBytes(ref offset, obj.Length - 4);
            obj.Text = textEncoding.GetString(textArray);

            return obj;
        }

        /// <summary>
        /// Parse a byte array into a NormalMenuHeader
        /// </summary>
        /// <param name="data">Data to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>A filled NormalMenuHeader on success, null on error</returns>
        public static Data.Models.PortableExecutable.Resource.Entries.NormalMenuHeader ParseNormalMenuHeader(byte[] data, ref int offset)
        {
            var obj = new Data.Models.PortableExecutable.Resource.Entries.NormalMenuHeader();

            obj.Version = data.ReadUInt16LittleEndian(ref offset);
            obj.HeaderSize = data.ReadUInt16LittleEndian(ref offset);

            return obj;
        }

        /// <summary>
        /// Parse a byte array into a NormalMenuItem
        /// </summary>
        /// <param name="data">Data to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>A filled NormalMenuItem on success, null on error</returns>
        public static Data.Models.PortableExecutable.Resource.Entries.NormalMenuItem ParseNormalMenuItem(byte[] data, ref int offset)
        {
            var obj = new Data.Models.PortableExecutable.Resource.Entries.NormalMenuItem();

            obj.NormalResInfo = (MenuFlags)data.ReadUInt32LittleEndian(ref offset);
            obj.NormalMenuText = data.ReadNullTerminatedUnicodeString(ref offset) ?? string.Empty;

            return obj;
        }

        /// <summary>
        /// Parse a Stream into an OptionalHeader
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="optionalSize">Size of the optional header</param>
        /// <returns>Filled OptionalHeader on success, null on error</returns>
        public static Data.Models.PortableExecutable.OptionalHeader ParseOptionalHeader(Stream data, int optionalSize)
        {
            long initialOffset = data.Position;

            var obj = new Data.Models.PortableExecutable.OptionalHeader();

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

            #region Windows-Specific Fields

            if (obj.Magic == OptionalHeaderMagicNumber.PE32)
                obj.ImageBase = data.ReadUInt32LittleEndian();
            else if (obj.Magic == OptionalHeaderMagicNumber.PE32Plus)
                obj.ImageBase = data.ReadUInt64LittleEndian();
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
                obj.SizeOfStackReserve = data.ReadUInt32LittleEndian();
            else if (obj.Magic == OptionalHeaderMagicNumber.PE32Plus)
                obj.SizeOfStackReserve = data.ReadUInt64LittleEndian();
            if (obj.Magic == OptionalHeaderMagicNumber.PE32)
                obj.SizeOfStackCommit = data.ReadUInt32LittleEndian();
            else if (obj.Magic == OptionalHeaderMagicNumber.PE32Plus)
                obj.SizeOfStackCommit = data.ReadUInt64LittleEndian();
            if (obj.Magic == OptionalHeaderMagicNumber.PE32)
                obj.SizeOfHeapReserve = data.ReadUInt32LittleEndian();
            else if (obj.Magic == OptionalHeaderMagicNumber.PE32Plus)
                obj.SizeOfHeapReserve = data.ReadUInt64LittleEndian();
            if (obj.Magic == OptionalHeaderMagicNumber.PE32)
                obj.SizeOfHeapCommit = data.ReadUInt32LittleEndian();
            else if (obj.Magic == OptionalHeaderMagicNumber.PE32Plus)
                obj.SizeOfHeapCommit = data.ReadUInt64LittleEndian();
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
        /// Parse a byte array into a PopupMenuItem
        /// </summary>
        /// <param name="data">Data to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>A filled PopupMenuItem on success, null on error</returns>
        public static Data.Models.PortableExecutable.Resource.Entries.PopupMenuItem ParsePopupMenuItem(byte[] data, ref int offset)
        {
            var obj = new Data.Models.PortableExecutable.Resource.Entries.PopupMenuItem();

            obj.PopupItemType = (MenuFlags)data.ReadUInt32LittleEndian(ref offset);
            obj.PopupState = (MenuFlags)data.ReadUInt32LittleEndian(ref offset);
            obj.PopupID = data.ReadUInt32LittleEndian(ref offset);
            obj.PopupResInfo = (MenuFlags)data.ReadUInt32LittleEndian(ref offset);
            obj.PopupMenuText = data.ReadNullTerminatedUnicodeString(ref offset) ?? string.Empty;

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a Relocation
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled COFFRelocation on success, null on error</returns>
        public static Relocation ParseRelocation(Stream data)
        {
            var obj = new Relocation();

            obj.VirtualAddress = data.ReadUInt32LittleEndian();
            obj.SymbolTableIndex = data.ReadUInt32LittleEndian();
            obj.TypeIndicator = (RelocationType)data.ReadUInt16LittleEndian();

            return obj;
        }

        /// <summary>
        /// Fill in resource data
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="initialOffset">Initial offset to use in address comparisons</param>
        /// <param name="tableData">Byte array to parse</param>
        /// <param name="dataOffset">Offset into the byte array</param>
        /// <param name="tableStart">Offset to the start of the table</param>
        /// <param name="tableStart">Unpadded length of the table</param>
        /// <param name="table">Resource table to fill in</param>
        /// <param name="sections">Section table to use for virtual address translation</param>
        /// <returns>Filled ResourceDataEntry on success, null on error</returns>
        public static void ParseResourceData(Stream data,
            long initialOffset,
            byte[]? tableData,
            ref int dataOffset,
            long tableStart,
            long tableLength,
            Data.Models.PortableExecutable.Resource.DirectoryTable? table,
            SectionHeader[] sections)
        {
            if (tableData is null)
                return;
            if (table?.Entries is null)
                return;

            foreach (var entry in table.Entries)
            {
                // Handle directory entries directly
                if (entry.DataEntry is not null && entry.DataEntry.Size > 0)
                {
                    // Convert the data RVA to an offset
                    long nextOffset = entry.DataEntry.DataRVA.ConvertVirtualAddress(sections);
                    if (nextOffset < 0)
                        continue;

                    // If the offset is within the table data, read from there
                    if (nextOffset > tableStart && nextOffset - tableStart + entry.DataEntry.Size <= tableLength)
                    {
                        dataOffset = (int)(nextOffset - tableStart);
                        entry.DataEntry.Data = tableData.ReadBytes(ref dataOffset, (int)entry.DataEntry.Size);
                    }

                    // Otherwise, read from the data stream
                    else if (nextOffset + entry.DataEntry.Size <= data.Length)
                    {
                        byte[]? entryData = data.ReadFrom(nextOffset + initialOffset, (int)entry.DataEntry.Size, retainPosition: true);
                        if (entryData is not null)
                            entry.DataEntry.Data = entryData;
                    }
                }

                // Handle subdirectories by recursion
                else if (entry.Subdirectory is not null)
                {
                    ParseResourceData(data,
                        initialOffset,
                        tableData,
                        ref dataOffset,
                        tableStart,
                        tableLength,
                        entry.Subdirectory,
                        sections);
                }
            }
        }

        /// <summary>
        /// Parse a byte array into an ResourceDataEntry
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>Filled ResourceDataEntry on success, null on error</returns>
        public static Data.Models.PortableExecutable.Resource.DataEntry ParseResourceDataEntry(byte[] data, ref int offset)
        {
            var obj = new Data.Models.PortableExecutable.Resource.DataEntry();

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
        /// <returns>Filled ResourceDirectoryEntry on success, null on error</returns>
        public static Data.Models.PortableExecutable.Resource.DirectoryEntry ParseResourceDirectoryEntry(byte[] data, ref int offset)
        {
            var obj = new Data.Models.PortableExecutable.Resource.DirectoryEntry();

            uint nameField = data.ReadUInt32LittleEndian(ref offset);
            if ((nameField & 0x80000000) != 0)
                obj.NameOffset = nameField & ~0x80000000U;
            else
                obj.IntegerID = nameField;

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
        public static Data.Models.PortableExecutable.Resource.DirectoryString ParseResourceDirectoryString(byte[] data, ref int offset)
        {
            var obj = new Data.Models.PortableExecutable.Resource.DirectoryString();

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
        public static Data.Models.PortableExecutable.Resource.DirectoryTable? ParseResourceDirectoryTable(byte[]? tableData, ref int offset)
        {
            if (tableData is null)
                return null;

            var obj = new Data.Models.PortableExecutable.Resource.DirectoryTable();

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
            obj.Entries = new Data.Models.PortableExecutable.Resource.DirectoryEntry[totalEntryCount];
            if (obj.Entries.Length == 0)
                return obj;

            // Perform top-level pass of data
            for (int i = 0; i < totalEntryCount; i++)
            {
                obj.Entries[i] = ParseResourceDirectoryEntry(tableData, ref offset);

                // Read the name from the offset, if needed
                if (obj.Entries[i].NameOffset > 0 && obj.Entries[i].NameOffset < tableData.Length)
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
                    offset = (int)entry.DataEntryOffset;
                    entry.DataEntry = ParseResourceDataEntry(tableData, ref offset);
                }
                else if (entry.SubdirectoryOffset > 0 && entry.SubdirectoryOffset < tableData.Length)
                {
                    offset = (int)entry.SubdirectoryOffset;
                    entry.Subdirectory = ParseResourceDirectoryTable(tableData, ref offset);
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

            obj.COFFRelocations = new Relocation[obj.NumberOfRelocations];
            for (int j = 0; j < obj.NumberOfRelocations; j++)
            {
                // TODO: Seek to correct location and read data
            }

            obj.COFFLineNumbers = new LineNumber[obj.NumberOfLinenumbers];
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
            obj.SectionNumber = (SectionNumber)data.ReadUInt16LittleEndian();
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
                    && (obj.SymbolType & SymbolType.IMAGE_SYM_DTYPE_FUNCTION) != 0
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
                && shortName is not null)
            {
                // Symbol name should be ".file"
                nextSymbolType = 4;
            }
            else if (obj.StorageClass == StorageClass.IMAGE_SYM_CLASS_STATIC
                && shortName is not null)
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
        ///  Read byte data as a string file info resource
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>A filled string file info resource on success, null on error</returns>
        public static Data.Models.PortableExecutable.Resource.Entries.StringFileInfo? ParseStringFileInfo(byte[] data, ref int offset)
        {
            var obj = new Data.Models.PortableExecutable.Resource.Entries.StringFileInfo();

            // Cache the initial offset
            int currentOffset = offset;

            obj.Length = data.ReadUInt16LittleEndian(ref offset);
            obj.ValueLength = data.ReadUInt16LittleEndian(ref offset);
            obj.ResourceType = (VersionResourceType)data.ReadUInt16LittleEndian(ref offset);
            obj.Key = data.ReadNullTerminatedUnicodeString(ref offset) ?? string.Empty;
            if (obj.Key != "StringFileInfo")
            {
                offset -= 6 + (((obj.Key?.Length ?? 0) + 1) * 2);
                return null;
            }

            // Align to the DWORD boundary if we're not at the end
            AlignToBoundary(data, ref offset, 4);

            var stringFileInfoChildren = new List<Data.Models.PortableExecutable.Resource.Entries.StringTable>();
            while ((offset - currentOffset) < obj.Length)
            {
                var stringTable = ParseFileInfoStringTable(data, ref offset, currentOffset);
                stringFileInfoChildren.Add(stringTable);
            }

            obj.Children = [.. stringFileInfoChildren];

            return obj;
        }

        /// <summary>
        ///  Read byte data as a StringData
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>A filled StringData on success, null on error</returns>
        public static Data.Models.PortableExecutable.Resource.Entries.StringData ParseStringData(byte[] data, ref int offset)
        {
            var obj = new Data.Models.PortableExecutable.Resource.Entries.StringData();

            int dataStartOffset = offset;
            obj.Length = data.ReadUInt16LittleEndian(ref offset);
            obj.ValueLength = data.ReadUInt16LittleEndian(ref offset);
            obj.ResourceType = (VersionResourceType)data.ReadUInt16LittleEndian(ref offset);
            obj.Key = data.ReadNullTerminatedUnicodeString(ref offset) ?? string.Empty;

            // Align to the DWORD boundary if we're not at the end
            AlignToBoundary(data, ref offset, 4);

            if (obj.ValueLength > 0)
            {
                int bytesReadable = Math.Min(obj.ValueLength * sizeof(ushort), obj.Length - (offset - dataStartOffset));
                byte[] valueBytes = data.ReadBytes(ref offset, bytesReadable);
                obj.Value = Encoding.Unicode.GetString(valueBytes);
            }

            // Align to the DWORD boundary if we're not at the end
            AlignToBoundary(data, ref offset, 4);

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a COFF StringTable
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled StringTable on success, null on error</returns>
        public static StringTable ParseCOFFStringTable(Stream data)
        {
            var obj = new StringTable();

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
        /// Read byte data as a FileInfo StringTable
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>A filled StringTable resource on success, null on error</returns>
        public static Data.Models.PortableExecutable.Resource.Entries.StringTable ParseFileInfoStringTable(byte[] data, ref int offset, int initialOffset)
        {
            var obj = new Data.Models.PortableExecutable.Resource.Entries.StringTable();

            obj.Length = data.ReadUInt16LittleEndian(ref offset);
            obj.ValueLength = data.ReadUInt16LittleEndian(ref offset);
            obj.ResourceType = (VersionResourceType)data.ReadUInt16LittleEndian(ref offset);
            obj.Key = data.ReadNullTerminatedUnicodeString(ref offset) ?? string.Empty;

            // Align to the DWORD boundary if we're not at the end
            AlignToBoundary(data, ref offset, 4);

            var stringTableChildren = new List<Data.Models.PortableExecutable.Resource.Entries.StringData>();
            while ((offset - initialOffset) < obj.Length)
            {
                var stringData = ParseStringData(data, ref offset);
                stringTableChildren.Add(stringData);
                if (stringData.Length == 0 && stringData.ValueLength == 0)
                    break;
            }

            obj.Children = [.. stringTableChildren];

            return obj;
        }

        /// <summary>
        /// Read byte data as a StringTableResource
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>A filled StringTableResource resource on success, null on error</returns>
        public static Data.Models.PortableExecutable.Resource.Entries.StringTableResource? ParseStringTableResource(byte[] data)
        {
            // Initialize the iterators
            int offset = 0, stringIndex = 0;

            // Create the output table
            var obj = new Data.Models.PortableExecutable.Resource.Entries.StringTableResource();

            // Loop through and add
            while (offset < data.Length)
            {
                string? stringValue = data.ReadPrefixedUnicodeString(ref offset);
                if (stringValue is not null)
                {
                    stringValue = stringValue.Replace("\n", "\\n").Replace("\r", newValue: "\\r");
                    obj.Values[stringIndex++] = stringValue;
                }
            }

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a symbol table
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="count">Number of COFF symbol table entries to read</param>
        /// <returns>Filled symbol table on success, null on error</returns>
        public static BaseEntry[] ParseSymbolTable(Stream data, uint count)
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
        ///  Read byte data as a VarFileInfo
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>A filled VarFileInfo on success, null on error</returns>
        public static Data.Models.PortableExecutable.Resource.Entries.VarFileInfo? ParseVarFileInfo(byte[] data, ref int offset)
        {
            var obj = new Data.Models.PortableExecutable.Resource.Entries.VarFileInfo();

            // Cache the initial offset
            int initialOffset = offset;

            obj.Length = data.ReadUInt16LittleEndian(ref offset);
            obj.ValueLength = data.ReadUInt16LittleEndian(ref offset);
            obj.ResourceType = (VersionResourceType)data.ReadUInt16LittleEndian(ref offset);
            obj.Key = data.ReadNullTerminatedUnicodeString(ref offset) ?? string.Empty;
            if (obj.Key != "VarFileInfo")
                return null;

            // Align to the DWORD boundary if we're not at the end
            AlignToBoundary(data, ref offset, 4);

            var varFileInfoChildren = new List<Data.Models.PortableExecutable.Resource.Entries.VarData>();
            while ((offset - initialOffset) < obj.Length)
            {
                var varData = new Data.Models.PortableExecutable.Resource.Entries.VarData();

                varData.Length = data.ReadUInt16LittleEndian(ref offset);
                varData.ValueLength = data.ReadUInt16LittleEndian(ref offset);
                varData.ResourceType = (VersionResourceType)data.ReadUInt16LittleEndian(ref offset);
                varData.Key = data.ReadNullTerminatedUnicodeString(ref offset) ?? string.Empty;
                if (varData.Key != "Translation")
                {
                    offset -= 6 + (((varData.Key?.Length ?? 0) + 1) * 2);
                    return null;
                }

                // Align to the DWORD boundary if we're not at the end
                AlignToBoundary(data, ref offset, 4);

                // Cache the current offset
                int currentOffset = offset;

                var varDataValue = new List<uint>();
                while ((offset - currentOffset) < varData.ValueLength)
                {
                    uint languageAndCodeIdentifierPair = data.ReadUInt32LittleEndian(ref offset);
                    varDataValue.Add(languageAndCodeIdentifierPair);
                }

                varData.Value = [.. varDataValue];

                varFileInfoChildren.Add(varData);
            }

            obj.Children = [.. varFileInfoChildren];

            return obj;
        }

        /// <summary>
        /// Parse a byte array into a VersionInfo
        /// </summary>
        /// <param name="tableData">Byte array to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>Filled VersionInfo on success, null on error</returns>
        public static Data.Models.PortableExecutable.Resource.Entries.VersionInfo? ParseVersionInfo(byte[] data)
        {
            // Initialize the iterator
            int offset = 0;

            // Create the output object
            var obj = new Data.Models.PortableExecutable.Resource.Entries.VersionInfo();

            obj.Length = data.ReadUInt16LittleEndian(ref offset);
            obj.ValueLength = data.ReadUInt16LittleEndian(ref offset);
            obj.ResourceType = (VersionResourceType)data.ReadUInt16LittleEndian(ref offset);
            obj.Key = data.ReadNullTerminatedUnicodeString(ref offset) ?? string.Empty;
            if (obj.Key != "VS_VERSION_INFO")
                return null;

            while (offset < data.Length && (offset % 4) != 0)
                obj.Padding1 = data.ReadUInt16LittleEndian(ref offset);

            // Read fixed file info
            if (obj.ValueLength > 0 && offset + obj.ValueLength <= data.Length)
            {
                var fixedFileInfo = ParseFixedFileInfo(data, ref offset);
                if (fixedFileInfo?.Signature != 0xFEEF04BD)
                    return null;

                obj.Value = fixedFileInfo;
            }

            while (offset < data.Length && (offset % 4) != 0)
                obj.Padding2 = data.ReadUInt16LittleEndian(ref offset);

            // Determine if we have a StringFileInfo or VarFileInfo twice
            ReadInfoSection(data, ref offset, obj);
            ReadInfoSection(data, ref offset, obj);

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

        #region Helpers

        /// <summary>
        /// Align the array position to a byte-size boundary
        /// </summary>
        /// <param name="input">Input array to try aligning</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <param name="alignment">Number of bytes to align on</param>
        /// <returns>True if the array could be aligned, false otherwise</returns>
        /// TODO: Remove when IO is updated
        private static bool AlignToBoundary(byte[]? input, ref int offset, byte alignment)
        {
            // If the array is invalid
            if (input is null || input.Length == 0)
                return false;

            // If already at the end of the array
            if (offset >= input.Length)
                return false;

            // Align the stream position
            while (offset % alignment != 0 && offset < input.Length)
            {
                _ = input.ReadByteValue(ref offset);
            }

            // Return if the alignment completed
            return offset % alignment == 0;
        }

        #endregion
    }
}
