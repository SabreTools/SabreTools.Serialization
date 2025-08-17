using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SabreTools.IO.Extensions;
using SabreTools.Models.PortableExecutable;
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
                var executable = new Executable();

                #region MS-DOS Stub

                // Parse the MS-DOS stub
                var stub = new MSDOS().Deserialize(data);
                if (stub?.Header == null || stub.Header.NewExeHeaderAddr == 0)
                    return null;

                // Set the MS-DOS stub
                executable.Stub = stub;

                #endregion

                #region Signature

                data.Seek(initialOffset + stub.Header.NewExeHeaderAddr, SeekOrigin.Begin);
                byte[] signature = data.ReadBytes(4);
                executable.Signature = Encoding.ASCII.GetString(signature);
                if (executable.Signature != SignatureString)
                    return null;

                #endregion

                #region COFF File Header

                // Parse the COFF file header
                var coffFileHeader = ParseCOFFFileHeader(data);
                if (coffFileHeader == null)
                    return null;

                // Set the COFF file header
                executable.COFFFileHeader = coffFileHeader;

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
                        executable.OptionalHeader = optionalHeader;
                }

                #endregion

                #region Section Table

                // Set the section table
                executable.SectionTable = new SectionHeader[coffFileHeader.NumberOfSections];
                for (int i = 0; i < coffFileHeader.NumberOfSections; i++)
                {
                    executable.SectionTable[i] = ParseSectionHeader(data);
                }

                #endregion

                #region COFF Symbol Table and COFF String Table

                // TODO: Validate that this is correct with an "old" PE
                long symbolTableAddress = initialOffset + coffFileHeader.PointerToSymbolTable;
                if (symbolTableAddress != 0 && symbolTableAddress < data.Length)
                {
                    // Seek to the COFF symbol table
                    data.Seek(symbolTableAddress, SeekOrigin.Begin);

                    // Set the COFF symbol table
                    executable.COFFSymbolTable = ParseCOFFSymbolTable(data, coffFileHeader.NumberOfSymbols);

                    // Set the COFF string table
                    executable.COFFStringTable = ParseCOFFStringTable(data);
                }

                #endregion

                #region Export Table

                // Should also be in a '.edata' section
                if (optionalHeader?.ExportTable != null)
                {
                    long exportTableAddress = initialOffset
                        + optionalHeader.ExportTable.VirtualAddress.ConvertVirtualAddress(executable.SectionTable);
                    if (exportTableAddress != 0 && exportTableAddress < data.Length)
                    {
                        // Seek to the export table
                        data.Seek(exportTableAddress, SeekOrigin.Begin);

                        // Set the export table
                        executable.ExportTable = ParseExportTable(data, executable.SectionTable);
                    }
                }

                #endregion

                #region Import Table

                // Should also be in a '.idata' section
                if (optionalHeader?.ImportTable != null)
                {
                    long importTableAddress = initialOffset
                        + optionalHeader.ImportTable.VirtualAddress.ConvertVirtualAddress(executable.SectionTable);
                    if (importTableAddress != 0 && importTableAddress < data.Length)
                    {
                        // Seek to the import table
                        data.Seek(importTableAddress, SeekOrigin.Begin);

                        // Set the import table
                        executable.ImportTable = ParseImportTable(data, optionalHeader.Magic, executable.SectionTable);
                    }
                }

                #endregion

                #region Resource Directory Table

                // Should also be in a '.rsrc' section
                if (optionalHeader?.ResourceTable != null)
                {
                    long resourceTableAddress = initialOffset
                        + optionalHeader.ResourceTable.VirtualAddress.ConvertVirtualAddress(executable.SectionTable);
                    if (resourceTableAddress != 0 && resourceTableAddress < data.Length)
                    {
                        // Seek to the resource directory table
                        data.Seek(resourceTableAddress, SeekOrigin.Begin);

                        // Set the resource directory table
                        executable.ResourceDirectoryTable = ParseResourceDirectoryTable(data, data.Position, executable.SectionTable, true);
                    }
                }

                #endregion

                // TODO: Exception Table

                #region Certificate Table

                if (optionalHeader?.CertificateTable != null)
                {
                    long certificateTableAddress = initialOffset
                        + optionalHeader.CertificateTable.VirtualAddress.ConvertVirtualAddress(executable.SectionTable);
                    if (certificateTableAddress != 0 && certificateTableAddress < data.Length)
                    {
                        // Seek to the attribute certificate table
                        data.Seek(certificateTableAddress, SeekOrigin.Begin);
                        long endOffset = certificateTableAddress + optionalHeader.CertificateTable.Size;

                        // Set the attribute certificate table
                        executable.AttributeCertificateTable = ParseAttributeCertificateTable(data, endOffset);
                    }
                }

                #endregion

                #region Base Relocation Table

                // Should also be in a '.reloc' section
                if (optionalHeader?.BaseRelocationTable != null)
                {
                    long baseRelocationTableAddress = initialOffset
                        + optionalHeader.BaseRelocationTable.VirtualAddress.ConvertVirtualAddress(executable.SectionTable);
                    if (baseRelocationTableAddress != 0 && baseRelocationTableAddress < data.Length)
                    {
                        // Seek to the base relocation table
                        data.Seek(baseRelocationTableAddress, SeekOrigin.Begin);
                        long endOffset = baseRelocationTableAddress + optionalHeader.BaseRelocationTable.Size;

                        // Set the base relocation table
                        executable.BaseRelocationTable = ParseBaseRelocationTable(data, endOffset);
                    }
                }

                #endregion

                #region Debug Table

                // Should also be in a '.debug' section
                if (optionalHeader?.Debug != null)
                {
                    long debugTableAddress = initialOffset
                        + optionalHeader.Debug.VirtualAddress.ConvertVirtualAddress(executable.SectionTable);
                    if (debugTableAddress != 0 && debugTableAddress < data.Length)
                    {
                        // Seek to the debug table
                        data.Seek(debugTableAddress, SeekOrigin.Begin);
                        long endOffset = debugTableAddress + optionalHeader.Debug.Size;

                        // Set the debug table
                        executable.DebugTable = ParseDebugTable(data, endOffset);
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

                if (optionalHeader?.DelayImportDescriptor != null)
                {
                    long delayLoadDirectoryTableAddress = initialOffset
                        + optionalHeader.DelayImportDescriptor.VirtualAddress.ConvertVirtualAddress(executable.SectionTable);
                    if (delayLoadDirectoryTableAddress != 0 && delayLoadDirectoryTableAddress < data.Length)
                    {
                        // Seek to the delay-load directory table
                        data.Seek(delayLoadDirectoryTableAddress, SeekOrigin.Begin);

                        // Set the delay-load directory table
                        executable.DelayLoadDirectoryTable = ParseDelayLoadDirectoryTable(data);
                    }
                }

                #endregion

                // TODO: CLR Runtime Header
                // TODO: Reserved

                return executable;
            }
            catch
            {
                // Ignore the actual error
                return null;
            }
        }

        /// <summary>
        /// Parse a Stream into an attribute certificate table
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="endOffset">First address not part of the attribute certificate table</param>
        /// <returns>Filled attribute certificate on success, null on error</returns>
        public static AttributeCertificateTableEntry[] ParseAttributeCertificateTable(Stream data, long endOffset)
        {
            var attributeCertificateTable = new List<AttributeCertificateTableEntry>();

            while (data.Position < endOffset && data.Position < data.Length)
            {
                var entry = ParseAttributeCertificateTableEntry(data);
                attributeCertificateTable.Add(entry);

                // Align to the 8-byte boundary
                data.AlignToBoundary(8);
            }

            return [.. attributeCertificateTable];
        }

        /// <summary>
        /// Parse a Stream into an AttributeCertificateTableEntry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled AttributeCertificateTableEntry on success, null on error</returns>
        public static AttributeCertificateTableEntry ParseAttributeCertificateTableEntry(Stream data)
        {
            var obj = new AttributeCertificateTableEntry();

            obj.Length = data.ReadUInt32LittleEndian();
            obj.Revision = (WindowsCertificateRevision)data.ReadUInt16LittleEndian();
            obj.CertificateType = (WindowsCertificateType)data.ReadUInt16LittleEndian();

            int certificateDataLength = (int)(obj.Length - 8);
            if (certificateDataLength > 0 && data.Position + certificateDataLength <= data.Length)
                obj.Certificate = data.ReadBytes(certificateDataLength);

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a base relocation table
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="endOffset">First address not part of the base relocation table</param>
        /// <returns>Filled base relocation table on success, null on error</returns>
        public static BaseRelocationBlock[] ParseBaseRelocationTable(Stream data, long endOffset)
        {
            var baseRelocationTable = new List<BaseRelocationBlock>();

            while (data.Position < endOffset && data.Position < data.Length)
            {
                var baseRelocationBlock = new BaseRelocationBlock();

                baseRelocationBlock.PageRVA = data.ReadUInt32LittleEndian();
                baseRelocationBlock.BlockSize = data.ReadUInt32LittleEndian();
                if (baseRelocationBlock.BlockSize == 0)
                    break;

                var typeOffsetFieldEntries = new List<BaseRelocationTypeOffsetFieldEntry>();
                int totalSize = 8;
                while (totalSize < baseRelocationBlock.BlockSize && data.Position < data.Length)
                {
                    var baseRelocationTypeOffsetFieldEntry = new BaseRelocationTypeOffsetFieldEntry();

                    ushort typeAndOffsetField = data.ReadUInt16LittleEndian();
                    baseRelocationTypeOffsetFieldEntry.BaseRelocationType = (BaseRelocationTypes)(typeAndOffsetField >> 12);
                    baseRelocationTypeOffsetFieldEntry.Offset = (ushort)(typeAndOffsetField & 0x0FFF);

                    typeOffsetFieldEntries.Add(baseRelocationTypeOffsetFieldEntry);
                    totalSize += 2;
                }

                baseRelocationBlock.TypeOffsetFieldEntries = [.. typeOffsetFieldEntries];

                baseRelocationTable.Add(baseRelocationBlock);

                // Align to the DWORD boundary if we're not at the end
                data.AlignToBoundary(4);
            }

            return [.. baseRelocationTable];
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
        public static COFFSymbolTableEntry[] ParseCOFFSymbolTable(Stream data, uint count)
        {
            var coffSymbolTable = new COFFSymbolTableEntry[count];

            int auxSymbolsRemaining = 0;
            int currentSymbolType = 0;

            for (int i = 0; i < count; i++)
            {
                // Standard COFF Symbol Table Entry
                if (currentSymbolType == 0)
                {
                    var entry = new COFFSymbolTableEntry();
                    entry.ShortName = data.ReadBytes(8);
                    if (entry.ShortName != null)
                        entry.Zeroes = BitConverter.ToUInt32(entry.ShortName, 0);

                    if (entry.Zeroes == 0)
                    {
                        if (entry.ShortName != null)
                            entry.Offset = BitConverter.ToUInt32(entry.ShortName, 4);

                        entry.ShortName = null;
                    }
                    entry.Value = data.ReadUInt32LittleEndian();
                    entry.SectionNumber = data.ReadUInt16LittleEndian();
                    entry.SymbolType = (SymbolType)data.ReadUInt16LittleEndian();
                    entry.StorageClass = (StorageClass)data.ReadByteValue();
                    entry.NumberOfAuxSymbols = data.ReadByteValue();
                    coffSymbolTable[i] = entry;

                    auxSymbolsRemaining = entry.NumberOfAuxSymbols;
                    if (auxSymbolsRemaining == 0)
                        continue;

                    if (entry.StorageClass == StorageClass.IMAGE_SYM_CLASS_EXTERNAL
                        && entry.SymbolType == SymbolType.IMAGE_SYM_TYPE_FUNC
                        && entry.SectionNumber > 0)
                    {
                        currentSymbolType = 1;
                    }
                    else if (entry.StorageClass == StorageClass.IMAGE_SYM_CLASS_FUNCTION
                        && entry.ShortName != null
                        && ((entry.ShortName[0] == 0x2E && entry.ShortName[1] == 0x62 && entry.ShortName[2] == 0x66)  // .bf
                            || (entry.ShortName[0] == 0x2E && entry.ShortName[1] == 0x65 && entry.ShortName[2] == 0x66))) // .ef
                    {
                        currentSymbolType = 2;
                    }
                    else if (entry.StorageClass == StorageClass.IMAGE_SYM_CLASS_EXTERNAL
                        && entry.SectionNumber == (ushort)SectionNumber.IMAGE_SYM_UNDEFINED
                        && entry.Value == 0)
                    {
                        currentSymbolType = 3;
                    }
                    else if (entry.StorageClass == StorageClass.IMAGE_SYM_CLASS_FILE)
                    {
                        // TODO: Symbol name should be ".file"
                        currentSymbolType = 4;
                    }
                    else if (entry.StorageClass == StorageClass.IMAGE_SYM_CLASS_STATIC)
                    {
                        // TODO: Should have the name of a section (like ".text")
                        currentSymbolType = 5;
                    }
                    else if (entry.StorageClass == StorageClass.IMAGE_SYM_CLASS_CLR_TOKEN)
                    {
                        currentSymbolType = 6;
                    }
                }

                // Auxiliary Format 1: Function Definitions
                else if (currentSymbolType == 1)
                {
                    var entry = new COFFSymbolTableEntry();
                    entry.AuxFormat1TagIndex = data.ReadUInt32LittleEndian();
                    entry.AuxFormat1TotalSize = data.ReadUInt32LittleEndian();
                    entry.AuxFormat1PointerToLinenumber = data.ReadUInt32LittleEndian();
                    entry.AuxFormat1PointerToNextFunction = data.ReadUInt32LittleEndian();
                    entry.AuxFormat1Unused = data.ReadUInt16LittleEndian();
                    coffSymbolTable[i] = entry;
                    auxSymbolsRemaining--;
                }

                // Auxiliary Format 2: .bf and .ef Symbols
                else if (currentSymbolType == 2)
                {
                    var entry = new COFFSymbolTableEntry();
                    entry.AuxFormat2Unused1 = data.ReadUInt32LittleEndian();
                    entry.AuxFormat2Linenumber = data.ReadUInt16LittleEndian();
                    entry.AuxFormat2Unused2 = data.ReadBytes(6);
                    entry.AuxFormat2PointerToNextFunction = data.ReadUInt32LittleEndian();
                    entry.AuxFormat2Unused3 = data.ReadUInt16LittleEndian();
                    coffSymbolTable[i] = entry;
                    auxSymbolsRemaining--;
                }

                // Auxiliary Format 3: Weak Externals
                else if (currentSymbolType == 3)
                {
                    var entry = new COFFSymbolTableEntry();
                    entry.AuxFormat3TagIndex = data.ReadUInt32LittleEndian();
                    entry.AuxFormat3Characteristics = data.ReadUInt32LittleEndian();
                    entry.AuxFormat3Unused = data.ReadBytes(10);
                    coffSymbolTable[i] = entry;
                    auxSymbolsRemaining--;
                }

                // Auxiliary Format 4: Files
                else if (currentSymbolType == 4)
                {
                    var entry = new COFFSymbolTableEntry();
                    entry.AuxFormat4FileName = data.ReadBytes(18);
                    coffSymbolTable[i] = entry;
                    auxSymbolsRemaining--;
                }

                // Auxiliary Format 5: Section Definitions
                else if (currentSymbolType == 5)
                {
                    var entry = new COFFSymbolTableEntry();
                    entry.AuxFormat5Length = data.ReadUInt32LittleEndian();
                    entry.AuxFormat5NumberOfRelocations = data.ReadUInt16LittleEndian();
                    entry.AuxFormat5NumberOfLinenumbers = data.ReadUInt16LittleEndian();
                    entry.AuxFormat5CheckSum = data.ReadUInt32LittleEndian();
                    entry.AuxFormat5Number = data.ReadUInt16LittleEndian();
                    entry.AuxFormat5Selection = data.ReadByteValue();
                    entry.AuxFormat5Unused = data.ReadBytes(3);
                    coffSymbolTable[i] = entry;
                    auxSymbolsRemaining--;
                }

                // Auxiliary Format 6: CLR Token Definition
                else if (currentSymbolType == 6)
                {
                    var entry = new COFFSymbolTableEntry();
                    entry.AuxFormat6AuxType = data.ReadByteValue();
                    entry.AuxFormat6Reserved1 = data.ReadByteValue();
                    entry.AuxFormat6SymbolTableIndex = data.ReadUInt32LittleEndian();
                    entry.AuxFormat6Reserved2 = data.ReadBytes(12);
                    coffSymbolTable[i] = entry;
                    auxSymbolsRemaining--;
                }

                // If we hit the last aux symbol, go back to normal format
                if (auxSymbolsRemaining == 0)
                    currentSymbolType = 0;
            }

            return coffSymbolTable;
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
        /// Parse a Stream into a DebugDirectoryEntry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled DebugDirectoryEntry on success, null on error</returns>
        public static DebugDirectoryEntry ParseDebugDirectoryEntry(Stream data)
        {
            var obj = new DebugDirectoryEntry();

            obj.Characteristics = data.ReadUInt32LittleEndian();
            obj.TimeDateStamp = data.ReadUInt32LittleEndian();
            obj.MajorVersion = data.ReadUInt16LittleEndian();
            obj.MinorVersion = data.ReadUInt16LittleEndian();
            obj.DebugType = (DebugType)data.ReadUInt32LittleEndian();
            obj.SizeOfData = data.ReadUInt32LittleEndian();
            obj.AddressOfRawData = data.ReadUInt32LittleEndian();
            obj.PointerToRawData = data.ReadUInt32LittleEndian();

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a DebugTable
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="endOffset">First address not part of the debug table</param>
        /// <returns>Filled DebugTable on success, null on error</returns>
        public static DebugTable ParseDebugTable(Stream data, long endOffset)
        {
            var debugTable = new DebugTable();

            var debugDirectoryTable = new List<DebugDirectoryEntry>();

            while (data.Position < endOffset && data.Position < data.Length)
            {
                var debugDirectoryEntry = ParseDebugDirectoryEntry(data);
                debugDirectoryTable.Add(debugDirectoryEntry);
            }

            debugTable.DebugDirectoryTable = [.. debugDirectoryTable];

            // TODO: Should we read the debug data in? Most of it is unformatted or undocumented
            // TODO: Implement .debug$F (Object Only) / IMAGE_DEBUG_TYPE_FPO

            return debugTable;
        }

        /// <summary>
        /// Parse a Stream into a DelayLoadDirectoryTable
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled DelayLoadDirectoryTable on success, null on error</returns>
        public static DelayLoadDirectoryTable ParseDelayLoadDirectoryTable(Stream data)
        {
            var obj = new DelayLoadDirectoryTable();

            obj.Attributes = data.ReadUInt32LittleEndian();
            obj.Name = data.ReadUInt32LittleEndian();
            obj.ModuleHandle = data.ReadUInt32LittleEndian();
            obj.DelayImportAddressTable = data.ReadUInt32LittleEndian();
            obj.DelayImportNameTable = data.ReadUInt32LittleEndian();
            obj.BoundDelayImportTable = data.ReadUInt32LittleEndian();
            obj.UnloadDelayImportTable = data.ReadUInt32LittleEndian();
            obj.TimeStamp = data.ReadUInt32LittleEndian();

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
        /// Parse a Stream into a ExportTable
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="sections">Section table to use for virtual address translation</param>
        /// <returns>Filled ExportTable on success, null on error</returns>
        public static ExportTable ParseExportTable(Stream data, SectionHeader[] sections)
        {
            var exportTable = new ExportTable();

            var exportDirectoryTable = new ExportDirectoryTable();

            exportDirectoryTable.ExportFlags = data.ReadUInt32LittleEndian();
            exportDirectoryTable.TimeDateStamp = data.ReadUInt32LittleEndian();
            exportDirectoryTable.MajorVersion = data.ReadUInt16LittleEndian();
            exportDirectoryTable.MinorVersion = data.ReadUInt16LittleEndian();
            exportDirectoryTable.NameRVA = data.ReadUInt32LittleEndian();
            exportDirectoryTable.OrdinalBase = data.ReadUInt32LittleEndian();
            exportDirectoryTable.AddressTableEntries = data.ReadUInt32LittleEndian();
            exportDirectoryTable.NumberOfNamePointers = data.ReadUInt32LittleEndian();
            exportDirectoryTable.ExportAddressTableRVA = data.ReadUInt32LittleEndian();
            exportDirectoryTable.NamePointerRVA = data.ReadUInt32LittleEndian();
            exportDirectoryTable.OrdinalTableRVA = data.ReadUInt32LittleEndian();

            exportTable.ExportDirectoryTable = exportDirectoryTable;

            // Name
            if (exportDirectoryTable.NameRVA.ConvertVirtualAddress(sections) != 0)
            {
                uint nameAddress = exportDirectoryTable.NameRVA.ConvertVirtualAddress(sections);
                data.Seek(nameAddress, SeekOrigin.Begin);

                string? name = data.ReadNullTerminatedAnsiString();
                exportDirectoryTable.Name = name;
            }

            // Address table
            if (exportDirectoryTable.AddressTableEntries != 0 && exportDirectoryTable.ExportAddressTableRVA.ConvertVirtualAddress(sections) != 0)
            {
                uint exportAddressTableAddress = exportDirectoryTable.ExportAddressTableRVA.ConvertVirtualAddress(sections);
                data.Seek(exportAddressTableAddress, SeekOrigin.Begin);

                var exportAddressTable = new ExportAddressTableEntry[exportDirectoryTable.AddressTableEntries];

                for (int i = 0; i < exportDirectoryTable.AddressTableEntries; i++)
                {
                    exportAddressTable[i] = ParseExportAddressTableEntry(data);
                }

                exportTable.ExportAddressTable = exportAddressTable;
            }

            // Name pointer table
            if (exportDirectoryTable.NumberOfNamePointers != 0 && exportDirectoryTable.NamePointerRVA.ConvertVirtualAddress(sections) != 0)
            {
                uint namePointerTableAddress = exportDirectoryTable.NamePointerRVA.ConvertVirtualAddress(sections);
                data.Seek(namePointerTableAddress, SeekOrigin.Begin);

                var namePointerTable = new ExportNamePointerTable();

                namePointerTable.Pointers = new uint[exportDirectoryTable.NumberOfNamePointers];
                for (int i = 0; i < exportDirectoryTable.NumberOfNamePointers; i++)
                {
                    uint pointer = data.ReadUInt32LittleEndian();
                    namePointerTable.Pointers[i] = pointer;
                }

                exportTable.NamePointerTable = namePointerTable;
            }

            // Ordinal table
            if (exportDirectoryTable.NumberOfNamePointers != 0 && exportDirectoryTable.OrdinalTableRVA.ConvertVirtualAddress(sections) != 0)
            {
                uint ordinalTableAddress = exportDirectoryTable.OrdinalTableRVA.ConvertVirtualAddress(sections);
                data.Seek(ordinalTableAddress, SeekOrigin.Begin);

                var exportOrdinalTable = new ExportOrdinalTable();

                exportOrdinalTable.Indexes = new ushort[exportDirectoryTable.NumberOfNamePointers];
                for (int i = 0; i < exportDirectoryTable.NumberOfNamePointers; i++)
                {
                    ushort pointer = data.ReadUInt16LittleEndian();
                    exportOrdinalTable.Indexes[i] = pointer;
                }

                exportTable.OrdinalTable = exportOrdinalTable;
            }

            // Name table
            if (exportDirectoryTable.NumberOfNamePointers != 0 && exportTable.NamePointerTable?.Pointers != null)
            {
                var exportNameTable = new ExportNameTable();

                exportNameTable.Strings = new string[exportDirectoryTable.NumberOfNamePointers];
                for (int i = 0; i < exportDirectoryTable.NumberOfNamePointers; i++)
                {
                    uint nameAddress = exportTable.NamePointerTable.Pointers[i].ConvertVirtualAddress(sections); ;
                    data.Seek(nameAddress, SeekOrigin.Begin);

                    string? str = data.ReadNullTerminatedAnsiString();
                    exportNameTable.Strings[i] = str ?? string.Empty;
                }

                exportTable.ExportNameTable = exportNameTable;
            }

            return exportTable;
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
        /// Parse a Stream into a ImportDirectoryTableEntry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled ImportDirectoryTableEntry on success, null on error</returns>
        public static ImportDirectoryTableEntry ParseImportDirectoryTableEntry(Stream data)
        {
            var obj = new ImportDirectoryTableEntry();

            obj.ImportLookupTableRVA = data.ReadUInt32LittleEndian();
            obj.TimeDateStamp = data.ReadUInt32LittleEndian();
            obj.ForwarderChain = data.ReadUInt32LittleEndian();
            obj.NameRVA = data.ReadUInt32LittleEndian();
            obj.ImportAddressTableRVA = data.ReadUInt32LittleEndian();

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a import table
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="magic">Optional header magic number indicating PE32 or PE32+</param>
        /// <param name="sections">Section table to use for virtual address translation</param>
        /// <returns>Filled import table on success, null on error</returns>
        public static ImportTable ParseImportTable(Stream data, OptionalHeaderMagicNumber magic, SectionHeader[] sections)
        {
            var importTable = new ImportTable();

            // Import directory table
            var importDirectoryTable = new List<ImportDirectoryTableEntry>();

            // Loop until the last item (all nulls) are found
            while (true)
            {
                var entry = ParseImportDirectoryTableEntry(data);
                importDirectoryTable.Add(entry);

                // All zero values means the last entry
                if (entry.ImportLookupTableRVA == 0
                    && entry.TimeDateStamp == 0
                    && entry.ForwarderChain == 0
                    && entry.NameRVA == 0
                    && entry.ImportAddressTableRVA == 0)
                    break;
            }

            importTable.ImportDirectoryTable = [.. importDirectoryTable];

            // Names
            for (int i = 0; i < importTable.ImportDirectoryTable.Length; i++)
            {
                var importDirectoryTableEntry = importTable.ImportDirectoryTable[i];
                if (importDirectoryTableEntry == null)
                    continue;

                if (importDirectoryTableEntry.NameRVA.ConvertVirtualAddress(sections) == 0)
                    continue;

                uint nameAddress = importDirectoryTableEntry.NameRVA.ConvertVirtualAddress(sections);
                data.Seek(nameAddress, SeekOrigin.Begin);

                string? name = data.ReadNullTerminatedAnsiString();
                importDirectoryTableEntry.Name = name;
            }

            // Lookup tables
            var importLookupTables = new Dictionary<int, ImportLookupTableEntry[]?>();

            for (int i = 0; i < importTable.ImportDirectoryTable.Length; i++)
            {
                var importDirectoryTableEntry = importTable.ImportDirectoryTable[i];
                if (importDirectoryTableEntry == null)
                    continue;

                if (importDirectoryTableEntry.ImportLookupTableRVA.ConvertVirtualAddress(sections) == 0)
                    continue;

                uint tableAddress = importDirectoryTableEntry.ImportLookupTableRVA.ConvertVirtualAddress(sections);
                data.Seek(tableAddress, SeekOrigin.Begin);

                var entryLookupTable = new List<ImportLookupTableEntry>();

                while (true)
                {
                    var entry = new ImportLookupTableEntry();

                    if (magic == OptionalHeaderMagicNumber.PE32)
                    {
                        uint entryValue = data.ReadUInt32LittleEndian();
                        entry.OrdinalNameFlag = (entryValue & 0x80000000) != 0;
                        if (entry.OrdinalNameFlag)
                            entry.OrdinalNumber = (ushort)(entryValue & ~0x80000000);
                        else
                            entry.HintNameTableRVA = (uint)(entryValue & ~0x80000000);
                    }
                    else if (magic == OptionalHeaderMagicNumber.PE32Plus)
                    {
                        ulong entryValue = data.ReadUInt64LittleEndian();
                        entry.OrdinalNameFlag = (entryValue & 0x8000000000000000) != 0;
                        if (entry.OrdinalNameFlag)
                            entry.OrdinalNumber = (ushort)(entryValue & ~0x8000000000000000);
                        else
                            entry.HintNameTableRVA = (uint)(entryValue & ~0x8000000000000000);
                    }

                    entryLookupTable.Add(entry);

                    // All zero values means the last entry
                    if (entry.OrdinalNameFlag == false
                        && entry.OrdinalNumber == 0
                        && entry.HintNameTableRVA == 0)
                        break;
                }

                importLookupTables[i] = [.. entryLookupTable];
            }

            importTable.ImportLookupTables = importLookupTables;

            // Address tables
            var importAddressTables = new Dictionary<int, ImportAddressTableEntry[]?>();

            for (int i = 0; i < importTable.ImportDirectoryTable.Length; i++)
            {
                var importDirectoryTableEntry = importTable.ImportDirectoryTable[i];
                if (importDirectoryTableEntry == null)
                    continue;

                if (importDirectoryTableEntry.ImportAddressTableRVA.ConvertVirtualAddress(sections) == 0)
                    continue;

                uint tableAddress = importDirectoryTableEntry.ImportAddressTableRVA.ConvertVirtualAddress(sections);
                data.Seek(tableAddress, SeekOrigin.Begin);

                var addressLookupTable = new List<ImportAddressTableEntry>();

                while (true)
                {
                    var entry = new ImportAddressTableEntry();

                    if (magic == OptionalHeaderMagicNumber.PE32)
                    {
                        uint entryValue = data.ReadUInt32LittleEndian();
                        entry.OrdinalNameFlag = (entryValue & 0x80000000) != 0;
                        if (entry.OrdinalNameFlag)
                            entry.OrdinalNumber = (ushort)(entryValue & ~0x80000000);
                        else
                            entry.HintNameTableRVA = (uint)(entryValue & ~0x80000000);
                    }
                    else if (magic == OptionalHeaderMagicNumber.PE32Plus)
                    {
                        ulong entryValue = data.ReadUInt64LittleEndian();
                        entry.OrdinalNameFlag = (entryValue & 0x8000000000000000) != 0;
                        if (entry.OrdinalNameFlag)
                            entry.OrdinalNumber = (ushort)(entryValue & ~0x8000000000000000);
                        else
                            entry.HintNameTableRVA = (uint)(entryValue & ~0x8000000000000000);
                    }

                    addressLookupTable.Add(entry);

                    // All zero values means the last entry
                    if (entry.OrdinalNameFlag == false
                        && entry.OrdinalNumber == 0
                        && entry.HintNameTableRVA == 0)
                        break;
                }

                importAddressTables[i] = [.. addressLookupTable];
            }

            importTable.ImportAddressTables = importAddressTables;

            // Hint/Name table
            var importHintNameTable = new List<HintNameTableEntry>();

            if ((importTable.ImportLookupTables != null && importTable.ImportLookupTables.Count > 0)
                || importTable.ImportAddressTables != null && importTable.ImportAddressTables.Count > 0)
            {
                // Get the addresses of the hint/name table entries
                var hintNameTableEntryAddresses = new List<int>();

                // If we have import lookup tables
                if (importTable.ImportLookupTables != null && importLookupTables.Count > 0)
                {
                    var addresses = new List<int>();
                    foreach (var kvp in importTable.ImportLookupTables)
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
                if (importTable.ImportAddressTables != null && importTable.ImportAddressTables.Count > 0)
                {
                    var addresses = new List<int>();
                    foreach (var kvp in importTable.ImportAddressTables)
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
                    int hintNameTableEntryAddress = hintNameTableEntryAddresses[i];
                    data.Seek(hintNameTableEntryAddress, SeekOrigin.Begin);

                    var hintNameTableEntry = ParseHintNameTableEntry(data);
                    importHintNameTable.Add(hintNameTableEntry);
                }
            }

            importTable.HintNameTable = [.. importHintNameTable];

            return importTable;
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

            if (obj.Magic == OptionalHeaderMagicNumber.PE32)
                obj.BaseOfData = data.ReadUInt32LittleEndian();

            #endregion

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
        /// Parse a Stream into a ResourceDirectoryTable
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="initialOffset">Initial offset to use in address comparisons</param>
        /// <param name="sections">Section table to use for virtual address translation</param>
        /// <param name="topLevel">Indicates if this is the top level or not</param>
        /// <returns>Filled ResourceDirectoryTable on success, null on error</returns>
        public static ResourceDirectoryTable ParseResourceDirectoryTable(Stream data, long initialOffset, SectionHeader[] sections, bool topLevel = false)
        {
            var resourceDirectoryTable = new ResourceDirectoryTable();

            resourceDirectoryTable.Characteristics = data.ReadUInt32LittleEndian();
            resourceDirectoryTable.TimeDateStamp = data.ReadUInt32LittleEndian();
            resourceDirectoryTable.MajorVersion = data.ReadUInt16LittleEndian();
            resourceDirectoryTable.MinorVersion = data.ReadUInt16LittleEndian();
            resourceDirectoryTable.NumberOfNameEntries = data.ReadUInt16LittleEndian();
            resourceDirectoryTable.NumberOfIDEntries = data.ReadUInt16LittleEndian();

            // If we have no entries
            int totalEntryCount = resourceDirectoryTable.NumberOfNameEntries + resourceDirectoryTable.NumberOfIDEntries;
            if (totalEntryCount == 0)
                return resourceDirectoryTable;

            // Perform top-level pass of data
            resourceDirectoryTable.Entries = new ResourceDirectoryEntry[totalEntryCount];
            for (int i = 0; i < totalEntryCount; i++)
            {
                var entry = new ResourceDirectoryEntry();
                uint offset = data.ReadUInt32LittleEndian();
                if ((offset & 0x80000000) != 0)
                    entry.NameOffset = offset & ~0x80000000;
                else
                    entry.IntegerID = offset;

                offset = data.ReadUInt32LittleEndian();
                if ((offset & 0x80000000) != 0)
                    entry.SubdirectoryOffset = offset & ~0x80000000;
                else
                    entry.DataEntryOffset = offset;

                // Read the name from the offset, if needed
                if (entry.NameOffset > 0)
                {
                    long currentOffset = data.Position;
                    offset = entry.NameOffset + (uint)initialOffset;
                    data.Seek(offset, SeekOrigin.Begin);

                    var resourceDirectoryString = new ResourceDirectoryString();

                    resourceDirectoryString.Length = data.ReadUInt16LittleEndian();
                    if (resourceDirectoryString.Length > 0 && data.Position + resourceDirectoryString.Length <= data.Length)
                        resourceDirectoryString.UnicodeString = data.ReadBytes(resourceDirectoryString.Length * 2);

                    entry.Name = resourceDirectoryString;

                    data.Seek(currentOffset, SeekOrigin.Begin);
                }

                resourceDirectoryTable.Entries[i] = entry;
            }

            // Loop through and process the entries
            foreach (var entry in resourceDirectoryTable.Entries)
            {
                if (entry == null)
                    continue;

                if (entry.DataEntryOffset > 0)
                {
                    uint offset = entry.DataEntryOffset + (uint)initialOffset;
                    data.Seek(offset, SeekOrigin.Begin);

                    var resourceDataEntry = new ResourceDataEntry();
                    resourceDataEntry.DataRVA = data.ReadUInt32LittleEndian();
                    resourceDataEntry.Size = data.ReadUInt32LittleEndian();
                    resourceDataEntry.Codepage = data.ReadUInt32LittleEndian();
                    resourceDataEntry.Reserved = data.ReadUInt32LittleEndian();

                    // Read the data from the offset
                    offset = resourceDataEntry.DataRVA.ConvertVirtualAddress(sections);
                    if (offset > 0 && resourceDataEntry.Size > 0 && offset + (int)resourceDataEntry.Size < data.Length)
                    {
                        data.Seek(offset, SeekOrigin.Begin);
                        resourceDataEntry.Data = data.ReadBytes((int)resourceDataEntry.Size);
                    }

                    entry.DataEntry = resourceDataEntry;
                }
                else if (entry.SubdirectoryOffset > 0)
                {
                    uint offset = entry.SubdirectoryOffset + (uint)initialOffset;
                    data.Seek(offset, SeekOrigin.Begin);

                    entry.Subdirectory = ParseResourceDirectoryTable(data, initialOffset, sections);
                }
            }

            // If we are not at the top level
            if (!topLevel)
                return resourceDirectoryTable;

            // If we're not aligned to a section
            var firstSection = Array.Find(sections, s => s != null && s.PointerToRawData == initialOffset);
            if (firstSection == null)
                return resourceDirectoryTable;

            // Get the section size
            int size = (int)firstSection.SizeOfRawData;

            // Align to the 512-byte boundary, we find the start of an MS-DOS header, or the end of the file
            while (data.Position - initialOffset < size && data.Position % 0x200 != 0 && data.Position < data.Length - 1)
            {
                // If we find the start of an MS-DOS header
                if (data.ReadUInt16LittleEndian() == Models.MSDOS.Constants.SignatureUInt16)
                {
                    data.Seek(-2, origin: SeekOrigin.Current);
                    break;
                }

                // Otherwise
                data.Seek(-1, origin: SeekOrigin.Current);
            }

            // If we have not used up the full size, parse the remaining chunk as a single resource
            if (data.Position - initialOffset < size)
            {
                var localEntries = resourceDirectoryTable.Entries;
                Array.Resize(ref localEntries, totalEntryCount + 1);
                resourceDirectoryTable.Entries = localEntries;
                int length = (int)(size - (data.Position - initialOffset));

                resourceDirectoryTable.Entries[totalEntryCount] = new ResourceDirectoryEntry
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

            return resourceDirectoryTable;
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
    }
}
