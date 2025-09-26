using System.Collections.Generic;
using System.IO;
using System.Text;
using SabreTools.Data.Models.LinearExecutable;
using SabreTools.IO.Extensions;
using static SabreTools.Data.Models.LinearExecutable.Constants;

namespace SabreTools.Serialization.Deserializers
{
    public class LinearExecutable : BaseBinaryDeserializer<Executable>
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

                #region Information Block

                // Try to parse the executable header
                data.Seek(initialOffset + stub.Header.NewExeHeaderAddr, SeekOrigin.Begin);
                var informationBlock = ParseInformationBlock(data);
                if (informationBlock.Signature != LESignatureString && informationBlock.Signature != LXSignatureString)
                    return null;

                // Set the executable header
                executable.InformationBlock = informationBlock;

                #endregion

                #region Object Table

                // Get the object table offset
                long offset = initialOffset
                    + stub.Header.NewExeHeaderAddr
                    + informationBlock.ObjectTableOffset;
                if (offset > initialOffset + stub.Header.NewExeHeaderAddr && offset < data.Length)
                {
                    // Seek to the object table
                    data.Seek(offset, SeekOrigin.Begin);

                    // Create the object table
                    executable.ObjectTable = new ObjectTableEntry[informationBlock.ObjectTableCount];

                    // Try to parse the object table
                    for (int i = 0; i < executable.ObjectTable.Length; i++)
                    {
                        executable.ObjectTable[i] = ParseObjectTableEntry(data);
                    }
                }

                #endregion

                #region Object Page Map

                // Get the object page map offset
                offset = initialOffset
                    + stub.Header.NewExeHeaderAddr
                    + informationBlock.ObjectPageMapOffset;
                if (offset > initialOffset + stub.Header.NewExeHeaderAddr && offset < data.Length)
                {
                    // Seek to the object page map
                    data.Seek(offset, SeekOrigin.Begin);

                    // Create the object page map
                    executable.ObjectPageMap = new ObjectPageMapEntry[informationBlock.ObjectTableCount];

                    // Try to parse the object page map
                    for (int i = 0; i < executable.ObjectPageMap.Length; i++)
                    {
                        executable.ObjectPageMap[i] = ParseObjectPageMapEntry(data);
                    }
                }

                #endregion

                #region Object Iterate Data Map

                offset = initialOffset
                    + stub.Header.NewExeHeaderAddr
                    + informationBlock.ObjectIterateDataMapOffset;
                if (offset > initialOffset + stub.Header.NewExeHeaderAddr && offset < data.Length)
                {
                    // Seek to the object page map
                    data.Seek(offset, SeekOrigin.Begin);

                    // TODO: Implement when model found
                    // No model has been found in the documentation about what
                    // each of the entries looks like for this map.
                }

                #endregion

                #region Resource Table

                // Get the resource table offset
                offset = initialOffset
                    + stub.Header.NewExeHeaderAddr
                    + informationBlock.ResourceTableOffset;
                if (offset > initialOffset + stub.Header.NewExeHeaderAddr && offset < data.Length)
                {
                    // Seek to the resource table
                    data.Seek(offset, SeekOrigin.Begin);

                    // Create the resource table
                    executable.ResourceTable = new ResourceTableEntry[informationBlock.ResourceTableCount];

                    // Try to parse the resource table
                    for (int i = 0; i < executable.ResourceTable.Length; i++)
                    {
                        executable.ResourceTable[i] = ParseResourceTableEntry(data);
                    }
                }

                #endregion

                #region Resident Names Table

                // Get the resident names table offset
                offset = initialOffset
                    + stub.Header.NewExeHeaderAddr
                    + informationBlock.ResidentNamesTableOffset;
                if (offset > initialOffset + stub.Header.NewExeHeaderAddr && offset < data.Length)
                {
                    // Seek to the resident names table
                    data.Seek(offset, SeekOrigin.Begin);

                    // Create the resident names table
                    var residentNamesTable = new List<ResidentNamesTableEntry>();

                    // Try to parse the resident names table
                    while (true)
                    {
                        var entry = ParseResidentNamesTableEntry(data);
                        residentNamesTable.Add(entry);

                        // If we have a 0-length entry
                        if (entry.Length == 0)
                            break;
                    }

                    // Assign the resident names table
                    executable.ResidentNamesTable = [.. residentNamesTable];
                }

                #endregion

                #region Entry Table

                // Get the entry table offset
                offset = initialOffset
                    + stub.Header.NewExeHeaderAddr
                    + informationBlock.EntryTableOffset;
                if (offset > initialOffset + stub.Header.NewExeHeaderAddr && offset < data.Length)
                {
                    // Seek to the entry table
                    data.Seek(offset, SeekOrigin.Begin);

                    // Create the entry table
                    var entryTable = new List<EntryTableBundle>();

                    // Try to parse the entry table
                    while (true)
                    {
                        var bundle = ParseEntryTableBundle(data);
                        if (bundle != null)
                            entryTable.Add(bundle);

                        // If we have a 0-length entry
                        if (bundle == null || bundle.Entries == 0)
                            break;
                    }

                    // Assign the entry table
                    executable.EntryTable = [.. entryTable];
                }

                #endregion

                #region Module Format Directives Table

                // Get the module format directives table offset
                offset = initialOffset
                    + stub.Header.NewExeHeaderAddr
                    + informationBlock.ModuleDirectivesTableOffset;
                if (offset > initialOffset + stub.Header.NewExeHeaderAddr && offset < data.Length)
                {
                    // Seek to the module format directives table
                    data.Seek(offset, SeekOrigin.Begin);

                    // Create the module format directives table
                    executable.ModuleFormatDirectivesTable = new ModuleFormatDirectivesTableEntry[informationBlock.ModuleDirectivesCount];

                    // Try to parse the module format directives table
                    for (int i = 0; i < executable.ModuleFormatDirectivesTable.Length; i++)
                    {
                        executable.ModuleFormatDirectivesTable[i] = ParseModuleFormatDirectivesTableEntry(data);
                    }
                }

                #endregion

                #region Verify Record Directive Table

                // TODO: Figure out where the offset to this table is stored
                // The documentation suggests it's either part of or immediately following
                // the Module Format Directives Table

                #endregion

                #region Fix-up Page Table

                // Get the fix-up page table offset
                offset = initialOffset
                    + stub.Header.NewExeHeaderAddr
                    + informationBlock.FixupPageTableOffset;
                if (offset > initialOffset + stub.Header.NewExeHeaderAddr && offset < data.Length)
                {
                    // Seek to the fix-up page table
                    data.Seek(offset, SeekOrigin.Begin);

                    // Create the fix-up page table
                    executable.FixupPageTable = new FixupPageTableEntry[executable.ObjectPageMap?.Length ?? 0 + 1];

                    // Try to parse the fix-up page table
                    for (int i = 0; i < executable.FixupPageTable.Length; i++)
                    {
                        executable.FixupPageTable[i] = ParseFixupPageTableEntry(data);
                    }
                }

                #endregion

                #region Fix-up Record Table

                // Get the fix-up record table offset
                offset = initialOffset
                    + stub.Header.NewExeHeaderAddr
                    + informationBlock.FixupRecordTableOffset;
                if (offset > initialOffset + stub.Header.NewExeHeaderAddr && offset < data.Length)
                {
                    // Seek to the fix-up record table
                    data.Seek(offset, SeekOrigin.Begin);

                    // Create the fix-up record table
                    executable.FixupRecordTable = new FixupRecordTableEntry[executable.ObjectPageMap?.Length ?? 0 + 1];

                    // Try to parse the fix-up record table
                    for (int i = 0; i < executable.FixupRecordTable.Length; i++)
                    {
                        var entry = ParseFixupRecordTableEntry(data);
                        if (entry == null)
                            return null;

                        executable.FixupRecordTable[i] = entry;
                    }
                }

                #endregion

                #region Imported Module Name Table

                // Get the imported module name table offset
                offset = initialOffset
                    + stub.Header.NewExeHeaderAddr
                    + informationBlock.ImportedModulesNameTableOffset;
                if (offset > initialOffset + stub.Header.NewExeHeaderAddr && offset < data.Length)
                {
                    // Seek to the imported module name table
                    data.Seek(offset, SeekOrigin.Begin);

                    // Create the imported module name table
                    executable.ImportModuleNameTable = new ImportModuleNameTableEntry[informationBlock.ImportedModulesCount];

                    // Try to parse the imported module name table
                    for (int i = 0; i < executable.ImportModuleNameTable.Length; i++)
                    {
                        executable.ImportModuleNameTable[i] = ParseImportModuleNameTableEntry(data);
                    }
                }

                #endregion

                #region Imported Module Procedure Name Table

                // Get the imported module procedure name table offset
                offset = initialOffset
                    + stub.Header.NewExeHeaderAddr
                    + informationBlock.ImportProcedureNameTableOffset;
                if (offset > initialOffset + stub.Header.NewExeHeaderAddr && offset < data.Length)
                {
                    // Seek to the imported module procedure name table
                    data.Seek(offset, SeekOrigin.Begin);

                    // Get the size of the imported module procedure name table
                    long tableSize = informationBlock.FixupPageTableOffset
                        + informationBlock.FixupSectionSize
                        - informationBlock.ImportProcedureNameTableOffset;

                    // Create the imported module procedure name table
                    var importModuleProcedureNameTable = new List<ImportModuleProcedureNameTableEntry>();

                    // Try to parse the imported module procedure name table
                    while (data.Position < offset + tableSize)
                    {
                        var entry = ParseImportModuleProcedureNameTableEntry(data);
                        importModuleProcedureNameTable.Add(entry);
                    }

                    // Assign the resident names table
                    executable.ImportModuleProcedureNameTable = [.. importModuleProcedureNameTable];
                }

                #endregion

                #region Per-Page Checksum Table

                // Get the per-page checksum table offset
                offset = initialOffset
                    + stub.Header.NewExeHeaderAddr
                    + informationBlock.PerPageChecksumTableOffset;
                if (offset > initialOffset + stub.Header.NewExeHeaderAddr && offset < data.Length)
                {
                    // Seek to the per-page checksum name table
                    data.Seek(offset, SeekOrigin.Begin);

                    // Create the per-page checksum name table
                    executable.PerPageChecksumTable = new PerPageChecksumTableEntry[informationBlock.ModuleNumberPages];

                    // Try to parse the per-page checksum name table
                    for (int i = 0; i < executable.PerPageChecksumTable.Length; i++)
                    {
                        executable.PerPageChecksumTable[i] = ParsePerPageChecksumTableEntry(data);
                    }
                }

                #endregion

                #region Non-Resident Names Table

                // Get the non-resident names table offset
                offset = initialOffset
                    + stub.Header.NewExeHeaderAddr
                    + informationBlock.NonResidentNamesTableOffset;
                if (offset > initialOffset + stub.Header.NewExeHeaderAddr && offset < data.Length)
                {
                    // Seek to the non-resident names table
                    data.Seek(offset, SeekOrigin.Begin);

                    // Create the non-resident names table
                    var nonResidentNamesTable = new List<NonResidentNamesTableEntry>();

                    // Try to parse the non-resident names table
                    while (true)
                    {
                        var entry = ParseNonResidentNameTableEntry(data);
                        nonResidentNamesTable.Add(entry);

                        // If we have a 0-length entry
                        if (entry.Length == 0)
                            break;
                    }

                    // Assign the non-resident names table
                    executable.NonResidentNamesTable = [.. nonResidentNamesTable];
                }

                #endregion

                #region Debug Information

                // Get the debug information offset
                offset = initialOffset
                    + stub.Header.NewExeHeaderAddr
                    + informationBlock.DebugInformationOffset;
                if (offset > initialOffset + stub.Header.NewExeHeaderAddr && offset < data.Length)
                {
                    // Seek to the debug information
                    data.Seek(offset, SeekOrigin.Begin);

                    // Try to parse the debug information
                    var debugInformation = ParseDebugInformation(data, informationBlock.DebugInformationLength);
                    if (debugInformation.Signature != DebugInformationSignatureString)
                        return null;

                    // Set the debug information
                    executable.DebugInformation = debugInformation;
                }

                #endregion

                return executable;
            }
            catch
            {
                // Ignore the actual error
                return null;
            }
        }

        /// <summary>
        /// Parse a Stream into a DebugInformation
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="size">Total size of the debug information</param>
        /// <returns>Filled DebugInformation on success, null on error</returns>
        public static DebugInformation ParseDebugInformation(Stream data, long size)
        {
            var obj = new DebugInformation();

            byte[] signature = data.ReadBytes(3);
            obj.Signature = Encoding.ASCII.GetString(signature);
            obj.FormatType = (DebugFormatType)data.ReadByteValue();
            obj.DebuggerData = data.ReadBytes((int)(size - 4));

            return obj;
        }

        /// <summary>
        /// Parse a Stream into an EntryTableBundle
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled EntryTableBundle on success, null on error</returns>
        public static EntryTableBundle? ParseEntryTableBundle(Stream data)
        {
            var obj = new EntryTableBundle();

            obj.Entries = data.ReadByteValue();
            if (obj.Entries == 0)
                return obj;

            obj.BundleType = (BundleType)data.ReadByteValue();
            obj.TableEntries = new EntryTableEntry[obj.Entries];
            for (int i = 0; i < obj.Entries; i++)
            {
                var entry = new EntryTableEntry();

                switch (obj.BundleType & ~BundleType.ParameterTypingInformationPresent)
                {
                    case BundleType.UnusedEntry:
                        // Empty entry with no information
                        break;

                    case BundleType.SixteenBitEntry:
                        entry.SixteenBitObjectNumber = data.ReadUInt16LittleEndian();
                        entry.SixteenBitEntryFlags = (EntryFlags)data.ReadByteValue();
                        entry.SixteenBitOffset = data.ReadUInt16LittleEndian();
                        break;

                    case BundleType.TwoEightySixCallGateEntry:
                        entry.TwoEightySixObjectNumber = data.ReadUInt16LittleEndian();
                        entry.TwoEightySixEntryFlags = (EntryFlags)data.ReadByteValue();
                        entry.TwoEightySixOffset = data.ReadUInt16LittleEndian();
                        entry.TwoEightySixCallgate = data.ReadUInt16LittleEndian();
                        break;

                    case BundleType.ThirtyTwoBitEntry:
                        entry.ThirtyTwoBitObjectNumber = data.ReadUInt16LittleEndian();
                        entry.ThirtyTwoBitEntryFlags = (EntryFlags)data.ReadByteValue();
                        entry.ThirtyTwoBitOffset = data.ReadUInt32LittleEndian();
                        break;

                    case BundleType.ForwarderEntry:
                        entry.ForwarderReserved = data.ReadUInt16LittleEndian();
                        entry.ForwarderFlags = (ForwarderFlags)data.ReadByteValue();
                        entry.ForwarderModuleOrdinalNumber = data.ReadUInt16LittleEndian();
                        entry.ProcedureNameOffset = data.ReadUInt32LittleEndian();
                        entry.ImportOrdinalNumber = data.ReadUInt32LittleEndian();
                        break;

                    default:
                        return null;
                }

                obj.TableEntries[i] = entry;
            }

            return obj;
        }

        /// <summary>
        /// Parse a Stream into an FixupPageTableEntry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled FixupPageTableEntry on success, null on error</returns>
        public static FixupPageTableEntry ParseFixupPageTableEntry(Stream data)
        {
            var obj = new FixupPageTableEntry();

            obj.Offset = data.ReadUInt32LittleEndian();

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a FixupRecordTableEntry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled FixupRecordTableEntry on success, null on error</returns>
        public static FixupRecordTableEntry? ParseFixupRecordTableEntry(Stream data)
        {
            var obj = new FixupRecordTableEntry();

            obj.SourceType = (FixupRecordSourceType)data.ReadByteValue();
            obj.TargetFlags = (FixupRecordTargetFlags)data.ReadByteValue();

            // Source list flag
#if NET20 || NET35
            if ((obj.SourceType & FixupRecordSourceType.SourceListFlag) != 0)
#else
            if (obj.SourceType.HasFlag(FixupRecordSourceType.SourceListFlag))
#endif
                obj.SourceOffsetListCount = data.ReadByteValue();
            else
                obj.SourceOffset = data.ReadUInt16LittleEndian();

            // OBJECT / TRGOFF
#if NET20 || NET35
            if ((obj.TargetFlags & FixupRecordTargetFlags.InternalReference) != 0)
#else
            if (obj.TargetFlags.HasFlag(FixupRecordTargetFlags.InternalReference))
#endif
            {
                // 16-bit Object Number/Module Ordinal Flag
#if NET20 || NET35
                if ((obj.TargetFlags & FixupRecordTargetFlags.SixteenBitObjectNumberModuleOrdinalFlag) != 0)
#else
                if (obj.TargetFlags.HasFlag(FixupRecordTargetFlags.SixteenBitObjectNumberModuleOrdinalFlag))
#endif
                    obj.TargetObjectNumberWORD = data.ReadUInt16LittleEndian();
                else
                    obj.TargetObjectNumberByte = data.ReadByteValue();

                // 16-bit Selector fixup
#if NET20 || NET35
                if ((obj.SourceType & FixupRecordSourceType.SixteenBitSelectorFixup) == 0)
#else
                if (!obj.SourceType.HasFlag(FixupRecordSourceType.SixteenBitSelectorFixup))
#endif
                {
                    // 32-bit Target Offset Flag
#if NET20 || NET35
                    if ((obj.TargetFlags & FixupRecordTargetFlags.ThirtyTwoBitTargetOffsetFlag) != 0)
#else
                    if (obj.TargetFlags.HasFlag(FixupRecordTargetFlags.ThirtyTwoBitTargetOffsetFlag))
#endif
                        obj.TargetOffsetDWORD = data.ReadUInt32LittleEndian();
                    else
                        obj.TargetOffsetWORD = data.ReadUInt16LittleEndian();
                }
            }

            // MOD ORD# / IMPORT ORD / ADDITIVE
#if NET20 || NET35
            else if ((obj.TargetFlags & FixupRecordTargetFlags.ImportedReferenceByOrdinal) != 0)
#else
            else if (obj.TargetFlags.HasFlag(FixupRecordTargetFlags.ImportedReferenceByOrdinal))
#endif
            {
                // 16-bit Object Number/Module Ordinal Flag
#if NET20 || NET35
                if ((obj.TargetFlags & FixupRecordTargetFlags.SixteenBitObjectNumberModuleOrdinalFlag) != 0)
#else
                if (obj.TargetFlags.HasFlag(FixupRecordTargetFlags.SixteenBitObjectNumberModuleOrdinalFlag))
#endif
                    obj.OrdinalIndexImportModuleNameTableWORD = data.ReadUInt16LittleEndian();
                else
                    obj.OrdinalIndexImportModuleNameTableByte = data.ReadByteValue();

                // 8-bit Ordinal Flag & 32-bit Target Offset Flag
#if NET20 || NET35
                if ((obj.TargetFlags & FixupRecordTargetFlags.EightBitOrdinalFlag) != 0)
#else
                if (obj.TargetFlags.HasFlag(FixupRecordTargetFlags.EightBitOrdinalFlag))
#endif
                    obj.ImportedOrdinalNumberByte = data.ReadByteValue();
#if NET20 || NET35
                else if ((obj.TargetFlags & FixupRecordTargetFlags.ThirtyTwoBitTargetOffsetFlag) != 0)
#else
                else if (obj.TargetFlags.HasFlag(FixupRecordTargetFlags.ThirtyTwoBitTargetOffsetFlag))
#endif
                    obj.ImportedOrdinalNumberDWORD = data.ReadUInt32LittleEndian();
                else
                    obj.ImportedOrdinalNumberWORD = data.ReadUInt16LittleEndian();

                // Additive Fixup Flag
#if NET20 || NET35
                if ((obj.TargetFlags & FixupRecordTargetFlags.AdditiveFixupFlag) != 0)
#else
                if (obj.TargetFlags.HasFlag(FixupRecordTargetFlags.AdditiveFixupFlag))
#endif
                {
                    // 32-bit Additive Flag
#if NET20 || NET35
                    if ((obj.TargetFlags & FixupRecordTargetFlags.ThirtyTwoBitAdditiveFixupFlag) != 0)
#else
                    if (obj.TargetFlags.HasFlag(FixupRecordTargetFlags.ThirtyTwoBitAdditiveFixupFlag))
#endif
                        obj.AdditiveFixupValueDWORD = data.ReadUInt32LittleEndian();
                    else
                        obj.AdditiveFixupValueWORD = data.ReadUInt16LittleEndian();
                }
            }

            // MOD ORD# / PROCEDURE NAME OFFSET / ADDITIVE
#if NET20 || NET35
            else if ((obj.TargetFlags & FixupRecordTargetFlags.ImportedReferenceByName) != 0)
#else
            else if (obj.TargetFlags.HasFlag(FixupRecordTargetFlags.ImportedReferenceByName))
#endif
            {
                // 16-bit Object Number/Module Ordinal Flag
#if NET20 || NET35
                if ((obj.TargetFlags & FixupRecordTargetFlags.SixteenBitObjectNumberModuleOrdinalFlag) != 0)
#else
                if (obj.TargetFlags.HasFlag(FixupRecordTargetFlags.SixteenBitObjectNumberModuleOrdinalFlag))
#endif
                    obj.OrdinalIndexImportModuleNameTableWORD = data.ReadUInt16LittleEndian();
                else
                    obj.OrdinalIndexImportModuleNameTableByte = data.ReadByteValue();

                // 32-bit Target Offset Flag
#if NET20 || NET35
                if ((obj.TargetFlags & FixupRecordTargetFlags.ThirtyTwoBitTargetOffsetFlag) != 0)
#else
                if (obj.TargetFlags.HasFlag(FixupRecordTargetFlags.ThirtyTwoBitTargetOffsetFlag))
#endif
                    obj.OffsetImportProcedureNameTableDWORD = data.ReadUInt32LittleEndian();
                else
                    obj.OffsetImportProcedureNameTableWORD = data.ReadUInt16LittleEndian();

                // Additive Fixup Flag
#if NET20 || NET35
                if ((obj.TargetFlags & FixupRecordTargetFlags.AdditiveFixupFlag) != 0)
#else
                if (obj.TargetFlags.HasFlag(FixupRecordTargetFlags.AdditiveFixupFlag))
#endif
                {
                    // 32-bit Additive Flag
#if NET20 || NET35
                    if ((obj.TargetFlags & FixupRecordTargetFlags.ThirtyTwoBitAdditiveFixupFlag) != 0)
#else
                    if (obj.TargetFlags.HasFlag(FixupRecordTargetFlags.ThirtyTwoBitAdditiveFixupFlag))
#endif
                        obj.AdditiveFixupValueDWORD = data.ReadUInt32LittleEndian();
                    else
                        obj.AdditiveFixupValueWORD = data.ReadUInt16LittleEndian();
                }
            }

            // ORD # / ADDITIVE
#if NET20 || NET35
            else if ((obj.TargetFlags & FixupRecordTargetFlags.InternalReferenceViaEntryTable) != 0)
#else
            else if (obj.TargetFlags.HasFlag(FixupRecordTargetFlags.InternalReferenceViaEntryTable))
#endif
            {
                // 16-bit Object Number/Module Ordinal Flag
#if NET20 || NET35
                if ((obj.TargetFlags & FixupRecordTargetFlags.SixteenBitObjectNumberModuleOrdinalFlag) != 0)
#else
                if (obj.TargetFlags.HasFlag(FixupRecordTargetFlags.SixteenBitObjectNumberModuleOrdinalFlag))
#endif
                    obj.OrdinalIndexImportModuleNameTableWORD = data.ReadUInt16LittleEndian();
                else
                    obj.OrdinalIndexImportModuleNameTableByte = data.ReadByteValue();

                // Additive Fixup Flag
#if NET20 || NET35
                if ((obj.TargetFlags & FixupRecordTargetFlags.AdditiveFixupFlag) != 0)
#else
                if (obj.TargetFlags.HasFlag(FixupRecordTargetFlags.AdditiveFixupFlag))
#endif
                {
                    // 32-bit Additive Flag
#if NET20 || NET35
                    if ((obj.TargetFlags & FixupRecordTargetFlags.ThirtyTwoBitAdditiveFixupFlag) != 0)
#else
                    if (obj.TargetFlags.HasFlag(FixupRecordTargetFlags.ThirtyTwoBitAdditiveFixupFlag))
#endif
                        obj.AdditiveFixupValueDWORD = data.ReadUInt32LittleEndian();
                    else
                        obj.AdditiveFixupValueWORD = data.ReadUInt16LittleEndian();
                }
            }

            // No other top-level flags recognized
            else
            {
                return null;
            }

            #region SCROFFn

#if NET20 || NET35
            if ((obj.SourceType & FixupRecordSourceType.SourceListFlag) != 0)
#else
            if (obj.SourceType.HasFlag(FixupRecordSourceType.SourceListFlag))
#endif
            {
                obj.SourceOffsetList = new ushort[obj.SourceOffsetListCount];
                for (int i = 0; i < obj.SourceOffsetList.Length; i++)
                {
                    obj.SourceOffsetList[i] = data.ReadUInt16LittleEndian();
                }
            }

            #endregion

            return obj;
        }

        /// <summary>
        /// Parse a Stream into an ImportModuleNameTableEntry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled ImportModuleNameTableEntry on success, null on error</returns>
        public static ImportModuleNameTableEntry ParseImportModuleNameTableEntry(Stream data)
        {
            var obj = new ImportModuleNameTableEntry();

            obj.Length = data.ReadByteValue();
            if (obj.Length > 0)
            {
                byte[] name = data.ReadBytes(obj.Length);
                obj.Name = Encoding.ASCII.GetString(name).TrimEnd('\0');
            }

            return obj;
        }

        /// <summary>
        /// Parse a Stream into an ImportModuleProcedureNameTableEntry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled ImportModuleProcedureNameTableEntry on success, null on error</returns>
        public static ImportModuleProcedureNameTableEntry ParseImportModuleProcedureNameTableEntry(Stream data)
        {
            var obj = new ImportModuleProcedureNameTableEntry();

            obj.Length = data.ReadByteValue();
            if (obj.Length > 0)
            {
                byte[] name = data.ReadBytes(obj.Length);
                obj.Name = Encoding.ASCII.GetString(name).TrimEnd('\0');
            }

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a InformationBlock
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled InformationBlock on success, null on error</returns>
        public static InformationBlock ParseInformationBlock(Stream data)
        {
            var obj = new InformationBlock();

            byte[] signature = data.ReadBytes(2);
            obj.Signature = Encoding.ASCII.GetString(signature);
            obj.ByteOrder = (ByteOrder)data.ReadByteValue();
            obj.WordOrder = (WordOrder)data.ReadByteValue();
            obj.ExecutableFormatLevel = data.ReadUInt32LittleEndian();
            obj.CPUType = (CPUType)data.ReadUInt16LittleEndian();
            obj.ModuleOS = (OperatingSystem)data.ReadUInt16LittleEndian();
            obj.ModuleVersion = data.ReadUInt32LittleEndian();
            obj.ModuleTypeFlags = (ModuleFlags)data.ReadUInt32LittleEndian();
            obj.ModuleNumberPages = data.ReadUInt32LittleEndian();
            obj.InitialObjectCS = data.ReadUInt32LittleEndian();
            obj.InitialEIP = data.ReadUInt32LittleEndian();
            obj.InitialObjectSS = data.ReadUInt32LittleEndian();
            obj.InitialESP = data.ReadUInt32LittleEndian();
            obj.MemoryPageSize = data.ReadUInt32LittleEndian();
            obj.BytesOnLastPage = data.ReadUInt32LittleEndian();
            obj.FixupSectionSize = data.ReadUInt32LittleEndian();
            obj.FixupSectionChecksum = data.ReadUInt32LittleEndian();
            obj.LoaderSectionSize = data.ReadUInt32LittleEndian();
            obj.LoaderSectionChecksum = data.ReadUInt32LittleEndian();
            obj.ObjectTableOffset = data.ReadUInt32LittleEndian();
            obj.ObjectTableCount = data.ReadUInt32LittleEndian();
            obj.ObjectPageMapOffset = data.ReadUInt32LittleEndian();
            obj.ObjectIterateDataMapOffset = data.ReadUInt32LittleEndian();
            obj.ResourceTableOffset = data.ReadUInt32LittleEndian();
            obj.ResourceTableCount = data.ReadUInt32LittleEndian();
            obj.ResidentNamesTableOffset = data.ReadUInt32LittleEndian();
            obj.EntryTableOffset = data.ReadUInt32LittleEndian();
            obj.ModuleDirectivesTableOffset = data.ReadUInt32LittleEndian();
            obj.ModuleDirectivesCount = data.ReadUInt32LittleEndian();
            obj.FixupPageTableOffset = data.ReadUInt32LittleEndian();
            obj.FixupRecordTableOffset = data.ReadUInt32LittleEndian();
            obj.ImportedModulesNameTableOffset = data.ReadUInt32LittleEndian();
            obj.ImportedModulesCount = data.ReadUInt32LittleEndian();
            obj.ImportProcedureNameTableOffset = data.ReadUInt32LittleEndian();
            obj.PerPageChecksumTableOffset = data.ReadUInt32LittleEndian();
            obj.DataPagesOffset = data.ReadUInt32LittleEndian();
            obj.PreloadPageCount = data.ReadUInt32LittleEndian();
            obj.NonResidentNamesTableOffset = data.ReadUInt32LittleEndian();
            obj.NonResidentNamesTableLength = data.ReadUInt32LittleEndian();
            obj.NonResidentNamesTableChecksum = data.ReadUInt32LittleEndian();
            obj.AutomaticDataObject = data.ReadUInt32LittleEndian();
            obj.DebugInformationOffset = data.ReadUInt32LittleEndian();
            obj.DebugInformationLength = data.ReadUInt32LittleEndian();
            obj.PreloadInstancePagesNumber = data.ReadUInt32LittleEndian();
            obj.DemandInstancePagesNumber = data.ReadUInt32LittleEndian();
            obj.ExtraHeapAllocation = data.ReadUInt32LittleEndian();

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a ModuleFormatDirectivesTableEntry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled ModuleFormatDirectivesTableEntry on success, null on error</returns>
        public static ModuleFormatDirectivesTableEntry ParseModuleFormatDirectivesTableEntry(Stream data)
        {
            var obj = new ModuleFormatDirectivesTableEntry();

            obj.DirectiveNumber = (DirectiveNumber)data.ReadUInt16LittleEndian();
            obj.DirectiveDataLength = data.ReadUInt16LittleEndian();
            obj.DirectiveDataOffset = data.ReadUInt32LittleEndian();

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a non-resident names table entry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled non-resident names table entry on success, null on error</returns>
        public static NonResidentNamesTableEntry ParseNonResidentNameTableEntry(Stream data)
        {
            var obj = new NonResidentNamesTableEntry();

            obj.Length = data.ReadByteValue();
            if (obj.Length > 0)
            {
                byte[] name = data.ReadBytes(obj.Length);
                obj.Name = Encoding.ASCII.GetString(name).TrimEnd('\0');
            }

            obj.OrdinalNumber = data.ReadUInt16LittleEndian();

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a ObjectPageMapEntry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled ObjectPageMapEntry on success, null on error</returns>
        public static ObjectPageMapEntry ParseObjectPageMapEntry(Stream data)
        {
            var obj = new ObjectPageMapEntry();

            obj.PageDataOffset = data.ReadUInt32LittleEndian();
            obj.DataSize = data.ReadUInt16LittleEndian();
            obj.Flags = (ObjectPageFlags)data.ReadUInt16LittleEndian();

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a ObjectTableEntry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled ObjectTableEntry on success, null on error</returns>
        public static ObjectTableEntry ParseObjectTableEntry(Stream data)
        {
            var obj = new ObjectTableEntry();

            obj.RelocationBaseAddress = data.ReadUInt32LittleEndian();
            obj.ObjectFlags = (ObjectFlags)data.ReadUInt16LittleEndian();
            obj.PageTableIndex = data.ReadUInt32LittleEndian();
            obj.PageTableEntries = data.ReadUInt32LittleEndian();
            obj.Reserved = data.ReadUInt32LittleEndian();

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a PerPageChecksumTableEntry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled PerPageChecksumTableEntry on success, null on error</returns>
        public static PerPageChecksumTableEntry ParsePerPageChecksumTableEntry(Stream data)
        {
            var obj = new PerPageChecksumTableEntry();

            obj.Checksum = data.ReadUInt32LittleEndian();

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a ResourceTableEntry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled ResourceTableEntry on success, null on error</returns>
        public static ResourceTableEntry ParseResourceTableEntry(Stream data)
        {
            var obj = new ResourceTableEntry();

            obj.TypeID = (ResourceTableEntryType)data.ReadUInt32LittleEndian();
            obj.NameID = data.ReadUInt16LittleEndian();
            obj.ResourceSize = data.ReadUInt32LittleEndian();
            obj.ObjectNumber = data.ReadUInt16LittleEndian();
            obj.Offset = data.ReadUInt32LittleEndian();

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a ResidentNamesTableEntry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled ResidentNamesTableEntry on success, null on error</returns>
        public static ResidentNamesTableEntry ParseResidentNamesTableEntry(Stream data)
        {
            var obj = new ResidentNamesTableEntry();

            obj.Length = data.ReadByteValue();
            if (obj.Length > 0)
            {
                byte[] name = data.ReadBytes(obj.Length);
                obj.Name = Encoding.ASCII.GetString(name).TrimEnd('\0');
            }

            obj.OrdinalNumber = data.ReadUInt16LittleEndian();

            return obj;
        }
    }
}
