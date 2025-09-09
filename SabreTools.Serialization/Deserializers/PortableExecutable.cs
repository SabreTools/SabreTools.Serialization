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

                // Get the new executable offset
                long newExeOffset = initialOffset + stub.Header.NewExeHeaderAddr;
                if (newExeOffset < initialOffset || newExeOffset > data.Length)
                    return null;

                // Try to parse the executable header
                data.Seek(newExeOffset, SeekOrigin.Begin);
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
                if (coffFileHeader.NumberOfSections > 96)
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

                // TODO: Technically this needs to be seeked to
                // It should be found by taking the the position after the
                // signature and COFF file header and then adding the size
                // of the optional header. This is effectively what the code
                // is already doing since it is parsed after the optional
                // header and not guessing otherwise.

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
                if (symbolTableAddress > initialOffset && symbolTableAddress < data.Length)
                {
                    // Seek to the COFF symbol table
                    data.Seek(symbolTableAddress, SeekOrigin.Begin);

                    // Set the COFF symbol table
                    executable.COFFSymbolTable = ParseCOFFSymbolTable(data, coffFileHeader.NumberOfSymbols);

                    // Set the COFF string table
                    if (executable.COFFSymbolTable != null)
                        executable.COFFStringTable = ParseCOFFStringTable(data);
                }

                #endregion

                #region Export Table

                // Should also be in a '.edata' section
                if (optionalHeader?.ExportTable != null)
                {
                    long exportTableAddress = initialOffset
                        + optionalHeader.ExportTable.VirtualAddress.ConvertVirtualAddress(executable.SectionTable);
                    if (exportTableAddress > initialOffset && exportTableAddress < data.Length)
                    {
                        // Seek to the export table
                        data.Seek(exportTableAddress, SeekOrigin.Begin);

                        // Set the export table
                        executable.ExportTable = ParseExportTable(data, initialOffset, executable.SectionTable);
                    }
                }

                #endregion

                #region Import Table

                // Should also be in a '.idata' section
                if (optionalHeader?.ImportTable != null)
                {
                    long importTableAddress = initialOffset
                        + optionalHeader.ImportTable.VirtualAddress.ConvertVirtualAddress(executable.SectionTable);
                    if (importTableAddress > initialOffset && importTableAddress < data.Length)
                    {
                        // Seek to the import table
                        data.Seek(importTableAddress, SeekOrigin.Begin);

                        // Set the import table
                        executable.ImportTable = ParseImportTable(data, initialOffset, optionalHeader.Magic, executable.SectionTable);
                    }
                }

                #endregion

                #region Resource Directory Table

                // Should also be in a '.rsrc' section
                if (optionalHeader?.ResourceTable != null)
                {
                    long resourceTableAddress = initialOffset
                        + optionalHeader.ResourceTable.VirtualAddress.ConvertVirtualAddress(executable.SectionTable);
                    if (resourceTableAddress > initialOffset && resourceTableAddress < data.Length)
                    {
                        // Seek to the resource directory table
                        data.Seek(resourceTableAddress, SeekOrigin.Begin);

                        // Set the resource directory table
                        executable.ResourceDirectoryTable = ParseResourceDirectoryTable(data, initialOffset, data.Position, executable.SectionTable, true);
                    }
                }

                #endregion

                // TODO: Exception Table

                #region Certificate Table

                if (optionalHeader?.CertificateTable != null)
                {
                    long certificateTableAddress = initialOffset
                        + optionalHeader.CertificateTable.VirtualAddress.ConvertVirtualAddress(executable.SectionTable);
                    if (certificateTableAddress > initialOffset && certificateTableAddress < data.Length)
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
                    if (baseRelocationTableAddress > initialOffset && baseRelocationTableAddress < data.Length)
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
                    if (debugTableAddress > initialOffset && debugTableAddress < data.Length)
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
                    if (delayLoadDirectoryTableAddress > initialOffset && delayLoadDirectoryTableAddress < data.Length)
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
            var obj = new List<AttributeCertificateTableEntry>();

            while (data.Position < endOffset && data.Position < data.Length)
            {
                var entry = ParseAttributeCertificateTableEntry(data);
                obj.Add(entry);

                // Align to the 8-byte boundary
                data.AlignToBoundary(8);
            }

            return [.. obj];
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
        /// Parse a Stream into an BaseRelocationBlock
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled BaseRelocationBlock on success, null on error</returns>
        public static BaseRelocationBlock? ParseBaseRelocationBlock(Stream data)
        {
            var obj = new BaseRelocationBlock();

            obj.PageRVA = data.ReadUInt32LittleEndian();
            obj.BlockSize = data.ReadUInt32LittleEndian();
            if (obj.BlockSize == 0)
                return null;

            var entries = new List<BaseRelocationTypeOffsetFieldEntry>();
            int totalSize = 8;
            while (totalSize < obj.BlockSize && data.Position < data.Length)
            {
                var entry = ParseBaseRelocationTypeOffsetFieldEntry(data);
                entries.Add(entry);
                totalSize += 2;
            }

            obj.TypeOffsetFieldEntries = [.. entries];

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
            var obj = new List<BaseRelocationBlock>();

            while (data.Position < endOffset && data.Position < data.Length)
            {
                var block = ParseBaseRelocationBlock(data);
                if (block == null)
                    break;

                obj.Add(block);

                // Align to the DWORD boundary if we're not at the end
                data.AlignToBoundary(4);
            }

            return [.. obj];
        }

        /// <summary>
        /// Parse a Stream into a BaseRelocationTypeOffsetFieldEntry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled BaseRelocationTypeOffsetFieldEntry on success, null on error</returns>
        public static BaseRelocationTypeOffsetFieldEntry ParseBaseRelocationTypeOffsetFieldEntry(Stream data)
        {
            var obj = new BaseRelocationTypeOffsetFieldEntry();

            ushort typeAndOffsetField = data.ReadUInt16LittleEndian();
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
        public static BaseEntry[]? ParseCOFFSymbolTable(Stream data, uint count)
        {
            var obj = new BaseEntry[count];

            int auxSymbolsRemaining = 0;
            int currentSymbolType = 0;

            for (int i = 0; i < count; i++)
            {
                // Standard COFF Symbol Table Entry
                if (currentSymbolType == 0)
                {
                    var entry = ParseStandardRecord(data, out currentSymbolType);
                    if (entry == null)
                        return null;

                    obj[i] = entry;

                    auxSymbolsRemaining = entry.NumberOfAuxSymbols;
                    if (auxSymbolsRemaining == 0)
                    {
                        currentSymbolType = 0;
                        continue;
                    }
                }

                // Auxiliary Format 1: Function Definitions
                else if (currentSymbolType == 1)
                {
                    obj[i] = ParseFunctionDefinition(data);
                    auxSymbolsRemaining--;
                }

                // Auxiliary Format 2: .bf and .ef Symbols
                else if (currentSymbolType == 2)
                {
                    obj[i] = ParseDescriptor(data);
                    auxSymbolsRemaining--;
                }

                // Auxiliary Format 3: Weak Externals
                else if (currentSymbolType == 3)
                {
                    obj[i] = ParseWeakExternal(data);
                    auxSymbolsRemaining--;
                }

                // Auxiliary Format 4: Files
                else if (currentSymbolType == 4)
                {
                    obj[i] = ParseFileRecord(data);
                    auxSymbolsRemaining--;
                }

                // Auxiliary Format 5: Section Definitions
                else if (currentSymbolType == 5)
                {
                    obj[i] = ParseSectionDefinition(data);
                    auxSymbolsRemaining--;
                }

                // Auxiliary Format 6: CLR Token Definition
                else if (currentSymbolType == 6)
                {
                    obj[i] = ParseCLRTokenDefinition(data);
                    auxSymbolsRemaining--;
                }

                // Invalid case, should never happen
                else
                {
                    return null;
                }

                // If we hit the last aux symbol, go back to normal format
                if (auxSymbolsRemaining == 0)
                    currentSymbolType = 0;
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
            var obj = new DebugTable();

            var debugDirectoryTable = new List<DebugDirectoryEntry>();

            while (data.Position < endOffset && data.Position < data.Length)
            {
                var debugDirectoryEntry = ParseDebugDirectoryEntry(data);
                debugDirectoryTable.Add(debugDirectoryEntry);
            }

            obj.DebugDirectoryTable = [.. debugDirectoryTable];

            // TODO: Should we read the debug data in? Most of it is unformatted or undocumented
            // TODO: Implement .debug$F (Object Only) / IMAGE_DEBUG_TYPE_FPO

            return obj;
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
        /// Parse a Stream into a ExportTable
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="initialOffset">Initial offset to use in address comparisons</param>
        /// <param name="sections">Section table to use for virtual address translation</param>
        /// <returns>Filled ExportTable on success, null on error</returns>
        public static ExportTable ParseExportTable(Stream data, long initialOffset, SectionHeader[] sections)
        {
            var obj = new ExportTable();

            var directoryTable = ParseExportDirectoryTable(data);
            obj.ExportDirectoryTable = directoryTable;

            // Name
            long nameAddress = initialOffset
                + directoryTable.NameRVA.ConvertVirtualAddress(sections);
            if (nameAddress > initialOffset && nameAddress < data.Length)
            {
                data.Seek(nameAddress, SeekOrigin.Begin);
                directoryTable.Name = data.ReadNullTerminatedAnsiString(); ;
            }

            // Address table
            long exportAddressTableAddress = initialOffset
                + directoryTable.ExportAddressTableRVA.ConvertVirtualAddress(sections);
            if (directoryTable.AddressTableEntries != 0
                && exportAddressTableAddress > initialOffset
                && exportAddressTableAddress < data.Length)
            {
                data.Seek(exportAddressTableAddress, SeekOrigin.Begin);
                obj.ExportAddressTable = ParseExportAddressTable(data, directoryTable.AddressTableEntries);
            }

            // Name pointer table
            long namePointerTableAddress = initialOffset
                + directoryTable.NamePointerRVA.ConvertVirtualAddress(sections);
            if (directoryTable.NumberOfNamePointers != 0
                && namePointerTableAddress > initialOffset
                && namePointerTableAddress < data.Length)
            {
                data.Seek(namePointerTableAddress, SeekOrigin.Begin);
                obj.NamePointerTable = ParseExportNamePointerTable(data, directoryTable.NumberOfNamePointers);
            }

            // Ordinal table
            long ordinalTableAddress = initialOffset
                + directoryTable.OrdinalTableRVA.ConvertVirtualAddress(sections);
            if (directoryTable.NumberOfNamePointers != 0
                && ordinalTableAddress > initialOffset
                && ordinalTableAddress < data.Length)
            {
                data.Seek(ordinalTableAddress, SeekOrigin.Begin);
                obj.OrdinalTable = ParseExportOrdinalTable(data, directoryTable.NumberOfNamePointers);
            }

            // Name table
            if (directoryTable.NumberOfNamePointers != 0 && obj.NamePointerTable?.Pointers != null)
                obj.ExportNameTable = ParseExportNameTable(data, initialOffset, obj.NamePointerTable.Pointers, sections);

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a ExportDirectoryTable
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled ExportDirectoryTable on success, null on error</returns>
        public static ExportDirectoryTable ParseExportDirectoryTable(Stream data)
        {
            var obj = new ExportDirectoryTable();

            obj.ExportFlags = data.ReadUInt32LittleEndian();
            obj.TimeDateStamp = data.ReadUInt32LittleEndian();
            obj.MajorVersion = data.ReadUInt16LittleEndian();
            obj.MinorVersion = data.ReadUInt16LittleEndian();
            obj.NameRVA = data.ReadUInt32LittleEndian();
            obj.OrdinalBase = data.ReadUInt32LittleEndian();
            obj.AddressTableEntries = data.ReadUInt32LittleEndian();
            obj.NumberOfNamePointers = data.ReadUInt32LittleEndian();
            obj.ExportAddressTableRVA = data.ReadUInt32LittleEndian();
            obj.NamePointerRVA = data.ReadUInt32LittleEndian();
            obj.OrdinalTableRVA = data.ReadUInt32LittleEndian();

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
                obj.Pointers[i] = data.ReadUInt32LittleEndian(); ;
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

            while (true)
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
        /// Parse a Stream into a ImportDirectoryTable
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="initialOffset">Initial offset to use in address comparisons</param>
        /// <param name="sections">Section table to use for virtual address translation</param>
        /// <returns>Filled ImportDirectoryTable on success, null on error</returns>
        public static ImportDirectoryTableEntry[] ParseImportDirectoryTable(Stream data, long initialOffset, SectionHeader[] sections)
        {
            var obj = new List<ImportDirectoryTableEntry>();

            // Loop until the last item (all nulls) are found
            while (data.Position < data.Length)
            {
                var entry = ParseImportDirectoryTableEntry(data);
                obj.Add(entry);

                // All zero values means the last entry
                if (entry.ImportLookupTableRVA == 0
                    && entry.TimeDateStamp == 0
                    && entry.ForwarderChain == 0
                    && entry.NameRVA == 0
                    && entry.ImportAddressTableRVA == 0)
                    break;
            }

            // Names
            for (int i = 0; i < obj.Count; i++)
            {
                var entry = obj[i];
                if (entry == null)
                    continue;

                if (entry.NameRVA.ConvertVirtualAddress(sections) == 0)
                    continue;

                long nameAddress = initialOffset
                    + entry.NameRVA.ConvertVirtualAddress(sections);
                if (nameAddress > initialOffset && nameAddress < data.Length)
                {
                    data.Seek(nameAddress, SeekOrigin.Begin);

                    string? name = data.ReadNullTerminatedAnsiString();
                    entry.Name = name;
                }
            }

            return [.. obj];
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
        /// Parse a Stream into a ImportLookupTable
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="magic">Optional header magic number indicating PE32 or PE32+</param>
        /// <returns>Filled ImportLookupTable on success, null on error</returns>
        public static ImportLookupTableEntry[] ParseImportLookupTable(Stream data, OptionalHeaderMagicNumber magic)
        {
            var ojb = new List<ImportLookupTableEntry>();

            while (true)
            {
                var entry = ParseImportLookupTableEntry(data, magic);
                ojb.Add(entry);

                // All zero values means the last entry
                if (entry.OrdinalNameFlag == false
                    && entry.OrdinalNumber == 0
                    && entry.HintNameTableRVA == 0)
                    break;
            }

            return [.. ojb];
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
        /// Parse a Stream into a import table
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="initialOffset">Initial offset to use in address comparisons</param>
        /// <param name="magic">Optional header magic number indicating PE32 or PE32+</param>
        /// <param name="sections">Section table to use for virtual address translation</param>
        /// <returns>Filled import table on success, null on error</returns>
        public static ImportTable ParseImportTable(Stream data, long initialOffset, OptionalHeaderMagicNumber magic, SectionHeader[] sections)
        {
            var obj = new ImportTable();

            obj.ImportDirectoryTable = ParseImportDirectoryTable(data, initialOffset, sections);
            obj.ImportLookupTables = ParseImportLookupTables(data, initialOffset, magic, obj.ImportDirectoryTable, sections);
            obj.ImportAddressTables = ParseImportAddressTables(data, initialOffset, magic, obj.ImportDirectoryTable, sections);
            obj.HintNameTable = ParseHintNameTable(data, initialOffset, obj.ImportLookupTables, obj.ImportAddressTables, sections);

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
        /// Parse a Stream into an ResourceDataEntry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="initialOffset">Initial offset to use in address comparisons</param>
        /// <param name="sections">Section table to use for virtual address translation</param>
        /// <returns>Filled ResourceDataEntry on success, null on error</returns>
        public static ResourceDataEntry ParseResourceDataEntry(Stream data, long initialOffset, SectionHeader[] sections)
        {
            var obj = new ResourceDataEntry();

            obj.DataRVA = data.ReadUInt32LittleEndian();
            obj.Size = data.ReadUInt32LittleEndian();
            obj.Codepage = data.ReadUInt32LittleEndian();
            obj.Reserved = data.ReadUInt32LittleEndian();

            // Read the data from the offset
            long offset = initialOffset + obj.DataRVA.ConvertVirtualAddress(sections);
            if (offset > initialOffset && obj.Size > 0 && offset + obj.Size < data.Length)
            {
                data.Seek(offset, SeekOrigin.Begin);
                obj.Data = data.ReadBytes((int)obj.Size);
            }

            return obj;
        }

        /// <summary>
        /// Parse a Stream into an ResourceDirectoryEntry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="tableStart">Table start address for relative reads</param>
        /// <returns>Filled ResourceDirectoryEntry on success, null on error</returns>
        public static ResourceDirectoryEntry ParseResourceDirectoryEntry(Stream data, long tableStart)
        {
            var obj = new ResourceDirectoryEntry();

            uint first = data.ReadUInt32LittleEndian();
            if ((first & 0x80000000) != 0)
                obj.NameOffset = first & ~0x80000000;
            else
                obj.IntegerID = first;

            uint second = data.ReadUInt32LittleEndian();
            if ((second & 0x80000000) != 0)
                obj.SubdirectoryOffset = second & ~0x80000000;
            else
                obj.DataEntryOffset = second;

            // Read the name from the offset, if needed
            if (obj.NameOffset > 0)
            {
                long nameOffset = tableStart + obj.NameOffset;
                if (nameOffset > tableStart && nameOffset < data.Length)
                {
                    long currentOffset = data.Position;
                    data.Seek(nameOffset, SeekOrigin.Begin);
                    obj.Name = ParseResourceDirectoryString(data);
                    data.Seek(currentOffset, SeekOrigin.Begin);
                }
            }

            return obj;
        }

        /// <summary>
        /// Parse a Stream into an ResourceDirectoryString
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled ResourceDirectoryString on success, null on error</returns>
        public static ResourceDirectoryString ParseResourceDirectoryString(Stream data)
        {
            var obj = new ResourceDirectoryString();

            obj.Length = data.ReadUInt16LittleEndian();
            if (obj.Length > 0 && data.Position + (obj.Length * 2) <= data.Length)
                obj.UnicodeString = data.ReadBytes(obj.Length * 2);

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a ResourceDirectoryTable
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="initialOffset">Initial offset to use in address comparisons</param>
        /// <param name="tableStart">Table start address for relative reads</param>
        /// <param name="sections">Section table to use for virtual address translation</param>
        /// <param name="topLevel">Indicates if this is the top level or not</param>
        /// <returns>Filled ResourceDirectoryTable on success, null on error</returns>
        public static ResourceDirectoryTable? ParseResourceDirectoryTable(Stream data, long initialOffset, long tableStart, SectionHeader[] sections, bool topLevel = false)
        {
            var obj = new ResourceDirectoryTable();

            obj.Characteristics = data.ReadUInt32LittleEndian();
            if (obj.Characteristics != 0)
                return null;

            obj.TimeDateStamp = data.ReadUInt32LittleEndian();
            obj.MajorVersion = data.ReadUInt16LittleEndian();
            obj.MinorVersion = data.ReadUInt16LittleEndian();
            obj.NumberOfNameEntries = data.ReadUInt16LittleEndian();
            obj.NumberOfIDEntries = data.ReadUInt16LittleEndian();

            // If we have no entries
            int totalEntryCount = obj.NumberOfNameEntries + obj.NumberOfIDEntries;
            if (totalEntryCount == 0)
                return obj;

            // Perform top-level pass of data
            obj.Entries = new ResourceDirectoryEntry[totalEntryCount];
            for (int i = 0; i < totalEntryCount; i++)
            {
                obj.Entries[i] = ParseResourceDirectoryEntry(data, tableStart);
            }

            // Loop through and process the entries
            foreach (var entry in obj.Entries)
            {
                if (entry == null)
                    continue;

                if (entry.DataEntryOffset > 0)
                {
                    long offset = tableStart + entry.DataEntryOffset;
                    if (offset > tableStart && offset < data.Length)
                    {
                        data.Seek(offset, SeekOrigin.Begin);
                        var resourceDataEntry = ParseResourceDataEntry(data, initialOffset, sections);
                        entry.DataEntry = resourceDataEntry;
                    }
                }
                else if (entry.SubdirectoryOffset > 0)
                {
                    long offset = tableStart + entry.SubdirectoryOffset;
                    if (offset > tableStart && offset < data.Length)
                    {
                        data.Seek(offset, SeekOrigin.Begin);
                        entry.Subdirectory = ParseResourceDirectoryTable(data, initialOffset, tableStart, sections);
                    }
                }
            }

            // If we are not at the top level
            if (!topLevel)
                return obj;

            // If we're not aligned to a section
            var firstSection = Array.Find(sections, s => s != null && s.PointerToRawData == tableStart);
            if (firstSection == null)
                return obj;

            // Get the section size
            int size = (int)firstSection.SizeOfRawData;

            // Align to the 512-byte boundary, we find the start of an MS-DOS header, or the end of the file
            while (data.Position - tableStart < size && data.Position % 0x200 != 0 && data.Position < data.Length - 1)
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
            if (data.Position - tableStart < size)
            {
                var localEntries = obj.Entries;
                Array.Resize(ref localEntries, totalEntryCount + 1);
                obj.Entries = localEntries;
                int length = (int)(size - (data.Position - tableStart));

                obj.Entries[totalEntryCount] = new ResourceDirectoryEntry
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
        public static StandardRecord? ParseStandardRecord(Stream data, out int currentSymbolType)
        {
            var entry = new StandardRecord();
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
            else
            {
                currentSymbolType = -1;
                return null;
            }

            return entry;
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
