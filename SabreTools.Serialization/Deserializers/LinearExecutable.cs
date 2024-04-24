using System.Collections.Generic;
using System.IO;
using System.Text;
using SabreTools.IO.Extensions;
using SabreTools.Models.LinearExecutable;
using static SabreTools.Models.LinearExecutable.Constants;

namespace SabreTools.Serialization.Deserializers
{
    public class LinearExecutable : BaseBinaryDeserializer<Executable>
    {
        /// <inheritdoc/>
        public override Executable? Deserialize(Stream? data)
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            // If the offset is out of bounds
            if (data.Position < 0 || data.Position >= data.Length)
                return null;

            // Cache the current offset
            int initialOffset = (int)data.Position;

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
            if (informationBlock == null)
                return null;

            // Set the executable header
            executable.InformationBlock = informationBlock;

            #endregion

            #region Object Table

            // Get the object table offset
            long offset = informationBlock.ObjectTableOffset + stub.Header.NewExeHeaderAddr;
            if (offset > stub.Header.NewExeHeaderAddr && offset < data.Length)
            {
                // Seek to the object table
                data.Seek(offset, SeekOrigin.Begin);

                // Create the object table
                executable.ObjectTable = new ObjectTableEntry[informationBlock.ObjectTableCount];

                // Try to parse the object table
                for (int i = 0; i < executable.ObjectTable.Length; i++)
                {
                    var entry = ParseObjectTableEntry(data);
                    if (entry == null)
                        return null;

                    executable.ObjectTable[i] = entry;
                }
            }

            #endregion

            #region Object Page Map

            // Get the object page map offset
            offset = informationBlock.ObjectPageMapOffset + stub.Header.NewExeHeaderAddr;
            if (offset > stub.Header.NewExeHeaderAddr && offset < data.Length)
            {
                // Seek to the object page map
                data.Seek(offset, SeekOrigin.Begin);

                // Create the object page map
                executable.ObjectPageMap = new ObjectPageMapEntry[informationBlock.ObjectTableCount];

                // Try to parse the object page map
                for (int i = 0; i < executable.ObjectPageMap.Length; i++)
                {
                    var entry = ParseObjectPageMapEntry(data);
                    if (entry == null)
                        return null;

                    executable.ObjectPageMap[i] = entry;
                }
            }

            #endregion

            #region Object Iterate Data Map

            offset = informationBlock.ObjectIterateDataMapOffset + stub.Header.NewExeHeaderAddr;
            if (offset > stub.Header.NewExeHeaderAddr && offset < data.Length)
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
            offset = informationBlock.ResourceTableOffset + stub.Header.NewExeHeaderAddr;
            if (offset > stub.Header.NewExeHeaderAddr && offset < data.Length)
            {
                // Seek to the resource table
                data.Seek(offset, SeekOrigin.Begin);

                // Create the resource table
                executable.ResourceTable = new ResourceTableEntry[informationBlock.ResourceTableCount];

                // Try to parse the resource table
                for (int i = 0; i < executable.ResourceTable.Length; i++)
                {
                    var entry = ParseResourceTableEntry(data);
                    if (entry == null)
                        return null;

                    executable.ResourceTable[i] = entry;
                }
            }

            #endregion

            #region Resident Names Table

            // Get the resident names table offset
            offset = informationBlock.ResidentNamesTableOffset + stub.Header.NewExeHeaderAddr;
            if (offset > stub.Header.NewExeHeaderAddr && offset < data.Length)
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
            offset = informationBlock.EntryTableOffset + stub.Header.NewExeHeaderAddr;
            if (offset > stub.Header.NewExeHeaderAddr && offset < data.Length)
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
            offset = informationBlock.ModuleDirectivesTableOffset + stub.Header.NewExeHeaderAddr;
            if (offset > stub.Header.NewExeHeaderAddr && offset < data.Length)
            {
                // Seek to the module format directives table
                data.Seek(offset, SeekOrigin.Begin);

                // Create the module format directives table
                executable.ModuleFormatDirectivesTable = new ModuleFormatDirectivesTableEntry[informationBlock.ModuleDirectivesCount];

                // Try to parse the module format directives table
                for (int i = 0; i < executable.ModuleFormatDirectivesTable.Length; i++)
                {
                    var entry = ParseModuleFormatDirectivesTableEntry(data);
                    if (entry == null)
                        return null;

                    executable.ModuleFormatDirectivesTable[i] = entry;
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
            offset = informationBlock.FixupPageTableOffset + stub.Header.NewExeHeaderAddr;
            if (offset > stub.Header.NewExeHeaderAddr && offset < data.Length)
            {
                // Seek to the fix-up page table
                data.Seek(offset, SeekOrigin.Begin);

                // Create the fix-up page table
                executable.FixupPageTable = new FixupPageTableEntry[executable.ObjectPageMap?.Length ?? 0 + 1];

                // Try to parse the fix-up page table
                for (int i = 0; i < executable.FixupPageTable.Length; i++)
                {
                    var entry = ParseFixupPageTableEntry(data);
                    if (entry == null)
                        return null;

                    executable.FixupPageTable[i] = entry;
                }
            }

            #endregion

            #region Fix-up Record Table

            // Get the fix-up record table offset
            offset = informationBlock.FixupRecordTableOffset + stub.Header.NewExeHeaderAddr;
            if (offset > stub.Header.NewExeHeaderAddr && offset < data.Length)
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
            offset = informationBlock.ImportedModulesNameTableOffset + stub.Header.NewExeHeaderAddr;
            if (offset > stub.Header.NewExeHeaderAddr && offset < data.Length)
            {
                // Seek to the imported module name table
                data.Seek(offset, SeekOrigin.Begin);

                // Create the imported module name table
                executable.ImportModuleNameTable = new ImportModuleNameTableEntry[informationBlock.ImportedModulesCount];

                // Try to parse the imported module name table
                for (int i = 0; i < executable.ImportModuleNameTable.Length; i++)
                {
                    var entry = ParseImportModuleNameTableEntry(data);
                    if (entry == null)
                        return null;

                    executable.ImportModuleNameTable[i] = entry;
                }
            }

            #endregion

            #region Imported Module Procedure Name Table

            // Get the imported module procedure name table offset
            offset = informationBlock.ImportProcedureNameTableOffset + stub.Header.NewExeHeaderAddr;
            if (offset > stub.Header.NewExeHeaderAddr && offset < data.Length)
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
                    if (entry == null)
                        return null;

                    importModuleProcedureNameTable.Add(entry);
                }

                // Assign the resident names table
                executable.ImportModuleProcedureNameTable = [.. importModuleProcedureNameTable];
            }

            #endregion

            #region Per-Page Checksum Table

            // Get the per-page checksum table offset
            offset = informationBlock.PerPageChecksumTableOffset + stub.Header.NewExeHeaderAddr;
            if (offset > stub.Header.NewExeHeaderAddr && offset < data.Length)
            {
                // Seek to the per-page checksum name table
                data.Seek(offset, SeekOrigin.Begin);

                // Create the per-page checksum name table
                executable.PerPageChecksumTable = new PerPageChecksumTableEntry[informationBlock.ModuleNumberPages];

                // Try to parse the per-page checksum name table
                for (int i = 0; i < executable.PerPageChecksumTable.Length; i++)
                {
                    var entry = ParsePerPageChecksumTableEntry(data);
                    if (entry == null)
                        return null;

                    executable.PerPageChecksumTable[i] = entry;
                }
            }

            #endregion

            #region Non-Resident Names Table

            // Get the non-resident names table offset
            offset = informationBlock.NonResidentNamesTableOffset + stub.Header.NewExeHeaderAddr;
            if (offset > stub.Header.NewExeHeaderAddr && offset < data.Length)
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
            offset = informationBlock.DebugInformationOffset + stub.Header.NewExeHeaderAddr;
            if (offset > stub.Header.NewExeHeaderAddr && offset < data.Length)
            {
                // Seek to the debug information
                data.Seek(offset, SeekOrigin.Begin);

                // Try to parse the debug information
                var debugInformation = ParseDebugInformation(data, informationBlock.DebugInformationLength);
                if (debugInformation == null)
                    return null;

                // Set the debug information
                executable.DebugInformation = debugInformation;
            }

            #endregion

            return executable;
        }

        /// <summary>
        /// Parse a Stream into an information block
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled information block on success, null on error</returns>
        public static InformationBlock? ParseInformationBlock(Stream data)
        {
            var informationBlock = data.ReadType<InformationBlock>();

            if (informationBlock == null)
                return null;
            if (informationBlock.Signature != LESignatureString && informationBlock.Signature != LXSignatureString)
                return null;

            return informationBlock;
        }

        /// <summary>
        /// Parse a Stream into an object table entry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled object table entry on success, null on error</returns>
        public static ObjectTableEntry? ParseObjectTableEntry(Stream data)
        {
            return data.ReadType<ObjectTableEntry>();
        }

        /// <summary>
        /// Parse a Stream into an object page map entry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled object page map entry on success, null on error</returns>
        public static ObjectPageMapEntry? ParseObjectPageMapEntry(Stream data)
        {
            return data.ReadType<ObjectPageMapEntry>();
        }

        /// <summary>
        /// Parse a Stream into a resource table entry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled resource table entry on success, null on error</returns>
        public static ResourceTableEntry? ParseResourceTableEntry(Stream data)
        {
            return data.ReadType<ResourceTableEntry>();
        }

        /// <summary>
        /// Parse a Stream into a resident names table entry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled resident names table entry on success, null on error</returns>
        public static ResidentNamesTableEntry ParseResidentNamesTableEntry(Stream data)
        {
            // TODO: Use marshalling here instead of building
            var entry = new ResidentNamesTableEntry();

            entry.Length = data.ReadByteValue();
            if (entry.Length > 0)
            {
                byte[]? name = data.ReadBytes(entry.Length);
                if (name != null)
                    entry.Name = Encoding.ASCII.GetString(name).TrimEnd('\0');
            }
            entry.OrdinalNumber = data.ReadUInt16();

            return entry;
        }

        /// <summary>
        /// Parse a Stream into an entry table bundle
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled entry table bundle on success, null on error</returns>
        public static EntryTableBundle? ParseEntryTableBundle(Stream data)
        {
            // TODO: Use marshalling here instead of building
            var bundle = new EntryTableBundle();

            bundle.Entries = data.ReadByteValue();
            if (bundle.Entries == 0)
                return bundle;

            bundle.BundleType = (BundleType)data.ReadByteValue();
            bundle.TableEntries = new EntryTableEntry[bundle.Entries];
            for (int i = 0; i < bundle.Entries; i++)
            {
                var entry = new EntryTableEntry();

                switch (bundle.BundleType & ~BundleType.ParameterTypingInformationPresent)
                {
                    case BundleType.UnusedEntry:
                        // Empty entry with no information
                        break;

                    case BundleType.SixteenBitEntry:
                        entry.SixteenBitObjectNumber = data.ReadUInt16();
                        entry.SixteenBitEntryFlags = (EntryFlags)data.ReadByteValue();
                        entry.SixteenBitOffset = data.ReadUInt16();
                        break;

                    case BundleType.TwoEightySixCallGateEntry:
                        entry.TwoEightySixObjectNumber = data.ReadUInt16();
                        entry.TwoEightySixEntryFlags = (EntryFlags)data.ReadByteValue();
                        entry.TwoEightySixOffset = data.ReadUInt16();
                        entry.TwoEightySixCallgate = data.ReadUInt16();
                        break;

                    case BundleType.ThirtyTwoBitEntry:
                        entry.ThirtyTwoBitObjectNumber = data.ReadUInt16();
                        entry.ThirtyTwoBitEntryFlags = (EntryFlags)data.ReadByteValue();
                        entry.ThirtyTwoBitOffset = data.ReadUInt32();
                        break;

                    case BundleType.ForwarderEntry:
                        entry.ForwarderReserved = data.ReadUInt16();
                        entry.ForwarderFlags = (ForwarderFlags)data.ReadByteValue();
                        entry.ForwarderModuleOrdinalNumber = data.ReadUInt16();
                        entry.ProcedureNameOffset = data.ReadUInt32();
                        entry.ImportOrdinalNumber = data.ReadUInt32();
                        break;

                    default:
                        return null;
                }

                bundle.TableEntries[i] = entry;
            }

            return bundle;
        }

        /// <summary>
        /// Parse a Stream into a module format directives table entry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled module format directives table entry on success, null on error</returns>
        public static ModuleFormatDirectivesTableEntry? ParseModuleFormatDirectivesTableEntry(Stream data)
        {
            return data.ReadType<ModuleFormatDirectivesTableEntry>();
        }

        /// <summary>
        /// Parse a Stream into a verify record directive table entry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled verify record directive table entry on success, null on error</returns>
        public static VerifyRecordDirectiveTableEntry? ParseVerifyRecordDirectiveTableEntry(Stream data)
        {
            return data.ReadType<VerifyRecordDirectiveTableEntry>();
        }

        /// <summary>
        /// Parse a Stream into a fix-up page table entry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled fix-up page table entry on success, null on error</returns>
        public static FixupPageTableEntry? ParseFixupPageTableEntry(Stream data)
        {
            return data.ReadType<FixupPageTableEntry>();
        }

        /// <summary>
        /// Parse a Stream into a fix-up record table entry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled fix-up record table entry on success, null on error</returns>
        public static FixupRecordTableEntry? ParseFixupRecordTableEntry(Stream data)
        {
            // TODO: Use marshalling here instead of building
            var entry = new FixupRecordTableEntry();

            entry.SourceType = (FixupRecordSourceType)data.ReadByteValue();
            entry.TargetFlags = (FixupRecordTargetFlags)data.ReadByteValue();

            // Source list flag
#if NET20 || NET35
            if ((entry.SourceType & FixupRecordSourceType.SourceListFlag) != 0)
#else
            if (entry.SourceType.HasFlag(FixupRecordSourceType.SourceListFlag))
#endif
                entry.SourceOffsetListCount = data.ReadByteValue();
            else
                entry.SourceOffset = data.ReadUInt16();

            // OBJECT / TRGOFF
#if NET20 || NET35
            if ((entry.TargetFlags & FixupRecordTargetFlags.InternalReference) != 0)
#else
            if (entry.TargetFlags.HasFlag(FixupRecordTargetFlags.InternalReference))
#endif
            {
                // 16-bit Object Number/Module Ordinal Flag
#if NET20 || NET35
                if ((entry.TargetFlags & FixupRecordTargetFlags.SixteenBitObjectNumberModuleOrdinalFlag) != 0)
#else
                if (entry.TargetFlags.HasFlag(FixupRecordTargetFlags.SixteenBitObjectNumberModuleOrdinalFlag))
#endif
                    entry.TargetObjectNumberWORD = data.ReadUInt16();
                else
                    entry.TargetObjectNumberByte = data.ReadByteValue();

                // 16-bit Selector fixup
#if NET20 || NET35
                if ((entry.SourceType & FixupRecordSourceType.SixteenBitSelectorFixup) == 0)
#else
                if (!entry.SourceType.HasFlag(FixupRecordSourceType.SixteenBitSelectorFixup))
#endif
                {
                    // 32-bit Target Offset Flag
#if NET20 || NET35
                    if ((entry.TargetFlags & FixupRecordTargetFlags.ThirtyTwoBitTargetOffsetFlag) != 0)
#else
                    if (entry.TargetFlags.HasFlag(FixupRecordTargetFlags.ThirtyTwoBitTargetOffsetFlag))
#endif
                        entry.TargetOffsetDWORD = data.ReadUInt32();
                    else
                        entry.TargetOffsetWORD = data.ReadUInt16();
                }
            }

            // MOD ORD# / IMPORT ORD / ADDITIVE
#if NET20 || NET35
            else if ((entry.TargetFlags & FixupRecordTargetFlags.ImportedReferenceByOrdinal) != 0)
#else
            else if (entry.TargetFlags.HasFlag(FixupRecordTargetFlags.ImportedReferenceByOrdinal))
#endif
            {
                // 16-bit Object Number/Module Ordinal Flag
#if NET20 || NET35
                if ((entry.TargetFlags & FixupRecordTargetFlags.SixteenBitObjectNumberModuleOrdinalFlag) != 0)
#else
                if (entry.TargetFlags.HasFlag(FixupRecordTargetFlags.SixteenBitObjectNumberModuleOrdinalFlag))
#endif
                    entry.OrdinalIndexImportModuleNameTableWORD = data.ReadUInt16();
                else
                    entry.OrdinalIndexImportModuleNameTableByte = data.ReadByteValue();

                // 8-bit Ordinal Flag & 32-bit Target Offset Flag
#if NET20 || NET35
                if ((entry.TargetFlags & FixupRecordTargetFlags.EightBitOrdinalFlag) != 0)
#else
                if (entry.TargetFlags.HasFlag(FixupRecordTargetFlags.EightBitOrdinalFlag))
#endif
                    entry.ImportedOrdinalNumberByte = data.ReadByteValue();
#if NET20 || NET35
                else if ((entry.TargetFlags & FixupRecordTargetFlags.ThirtyTwoBitTargetOffsetFlag) != 0)
#else
                else if (entry.TargetFlags.HasFlag(FixupRecordTargetFlags.ThirtyTwoBitTargetOffsetFlag))
#endif
                    entry.ImportedOrdinalNumberDWORD = data.ReadUInt32();
                else
                    entry.ImportedOrdinalNumberWORD = data.ReadUInt16();

                // Additive Fixup Flag
#if NET20 || NET35
                if ((entry.TargetFlags & FixupRecordTargetFlags.AdditiveFixupFlag) != 0)
#else
                if (entry.TargetFlags.HasFlag(FixupRecordTargetFlags.AdditiveFixupFlag))
#endif
                {
                    // 32-bit Additive Flag
#if NET20 || NET35
                    if ((entry.TargetFlags & FixupRecordTargetFlags.ThirtyTwoBitAdditiveFixupFlag) != 0)
#else
                    if (entry.TargetFlags.HasFlag(FixupRecordTargetFlags.ThirtyTwoBitAdditiveFixupFlag))
#endif
                        entry.AdditiveFixupValueDWORD = data.ReadUInt32();
                    else
                        entry.AdditiveFixupValueWORD = data.ReadUInt16();
                }
            }

            // MOD ORD# / PROCEDURE NAME OFFSET / ADDITIVE
#if NET20 || NET35
            else if ((entry.TargetFlags & FixupRecordTargetFlags.ImportedReferenceByName) != 0)
#else
            else if (entry.TargetFlags.HasFlag(FixupRecordTargetFlags.ImportedReferenceByName))
#endif
            {
                // 16-bit Object Number/Module Ordinal Flag
#if NET20 || NET35
                if ((entry.TargetFlags & FixupRecordTargetFlags.SixteenBitObjectNumberModuleOrdinalFlag) != 0)
#else
                if (entry.TargetFlags.HasFlag(FixupRecordTargetFlags.SixteenBitObjectNumberModuleOrdinalFlag))
#endif
                    entry.OrdinalIndexImportModuleNameTableWORD = data.ReadUInt16();
                else
                    entry.OrdinalIndexImportModuleNameTableByte = data.ReadByteValue();

                // 32-bit Target Offset Flag
#if NET20 || NET35
                if ((entry.TargetFlags & FixupRecordTargetFlags.ThirtyTwoBitTargetOffsetFlag) != 0)
#else
                if (entry.TargetFlags.HasFlag(FixupRecordTargetFlags.ThirtyTwoBitTargetOffsetFlag))
#endif
                    entry.OffsetImportProcedureNameTableDWORD = data.ReadUInt32();
                else
                    entry.OffsetImportProcedureNameTableWORD = data.ReadUInt16();

                // Additive Fixup Flag
#if NET20 || NET35
                if ((entry.TargetFlags & FixupRecordTargetFlags.AdditiveFixupFlag) != 0)
#else
                if (entry.TargetFlags.HasFlag(FixupRecordTargetFlags.AdditiveFixupFlag))
#endif
                {
                    // 32-bit Additive Flag
#if NET20 || NET35
                    if ((entry.TargetFlags & FixupRecordTargetFlags.ThirtyTwoBitAdditiveFixupFlag) != 0)
#else
                    if (entry.TargetFlags.HasFlag(FixupRecordTargetFlags.ThirtyTwoBitAdditiveFixupFlag))
#endif
                        entry.AdditiveFixupValueDWORD = data.ReadUInt32();
                    else
                        entry.AdditiveFixupValueWORD = data.ReadUInt16();
                }
            }

            // ORD # / ADDITIVE
#if NET20 || NET35
            else if ((entry.TargetFlags & FixupRecordTargetFlags.InternalReferenceViaEntryTable) != 0)
#else
            else if (entry.TargetFlags.HasFlag(FixupRecordTargetFlags.InternalReferenceViaEntryTable))
#endif
            {
                // 16-bit Object Number/Module Ordinal Flag
#if NET20 || NET35
                if ((entry.TargetFlags & FixupRecordTargetFlags.SixteenBitObjectNumberModuleOrdinalFlag) != 0)
#else
                if (entry.TargetFlags.HasFlag(FixupRecordTargetFlags.SixteenBitObjectNumberModuleOrdinalFlag))
#endif
                    entry.OrdinalIndexImportModuleNameTableWORD = data.ReadUInt16();
                else
                    entry.OrdinalIndexImportModuleNameTableByte = data.ReadByteValue();

                // Additive Fixup Flag
#if NET20 || NET35
                if ((entry.TargetFlags & FixupRecordTargetFlags.AdditiveFixupFlag) != 0)
#else
                if (entry.TargetFlags.HasFlag(FixupRecordTargetFlags.AdditiveFixupFlag))
#endif
                {
                    // 32-bit Additive Flag
#if NET20 || NET35
                    if ((entry.TargetFlags & FixupRecordTargetFlags.ThirtyTwoBitAdditiveFixupFlag) != 0)
#else
                    if (entry.TargetFlags.HasFlag(FixupRecordTargetFlags.ThirtyTwoBitAdditiveFixupFlag))
#endif
                        entry.AdditiveFixupValueDWORD = data.ReadUInt32();
                    else
                        entry.AdditiveFixupValueWORD = data.ReadUInt16();
                }
            }

            // No other top-level flags recognized
            else
            {
                return null;
            }

            #region SCROFFn

#if NET20 || NET35
            if ((entry.SourceType & FixupRecordSourceType.SourceListFlag) != 0)
#else
            if (entry.SourceType.HasFlag(FixupRecordSourceType.SourceListFlag))
#endif
            {
                entry.SourceOffsetList = new ushort[entry.SourceOffsetListCount];
                for (int i = 0; i < entry.SourceOffsetList.Length; i++)
                {
                    entry.SourceOffsetList[i] = data.ReadUInt16();
                }
            }

            #endregion

            return entry;
        }

        /// <summary>
        /// Parse a Stream into a import module name table entry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled import module name table entry on success, null on error</returns>
        public static ImportModuleNameTableEntry ParseImportModuleNameTableEntry(Stream data)
        {
            // TODO: Use marshalling here instead of building
            var entry = new ImportModuleNameTableEntry();

            entry.Length = data.ReadByteValue();
            if (entry.Length > 0)
            {
                byte[]? name = data.ReadBytes(entry.Length);
                if (name != null)
                    entry.Name = Encoding.ASCII.GetString(name).TrimEnd('\0');
            }

            return entry;
        }

        /// <summary>
        /// Parse a Stream into a import module name table entry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled import module name table entry on success, null on error</returns>
        public static ImportModuleProcedureNameTableEntry ParseImportModuleProcedureNameTableEntry(Stream data)
        {
            // TODO: Use marshalling here instead of building
            var entry = new ImportModuleProcedureNameTableEntry();

            entry.Length = data.ReadByteValue();
            if (entry.Length > 0)
            {
                byte[]? name = data.ReadBytes(entry.Length);
                if (name != null)
                    entry.Name = Encoding.ASCII.GetString(name).TrimEnd('\0');
            }

            return entry;
        }

        /// <summary>
        /// Parse a Stream into a per-page checksum table entry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled per-page checksum table entry on success, null on error</returns>
        public static PerPageChecksumTableEntry? ParsePerPageChecksumTableEntry(Stream data)
        {
            return data.ReadType<PerPageChecksumTableEntry>();
        }

        /// <summary>
        /// Parse a Stream into a non-resident names table entry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled non-resident names table entry on success, null on error</returns>
        public static NonResidentNamesTableEntry ParseNonResidentNameTableEntry(Stream data)
        {
            // TODO: Use marshalling here instead of building
            var entry = new NonResidentNamesTableEntry();

            entry.Length = data.ReadByteValue();
            if (entry.Length > 0)
            {
                byte[]? name = data.ReadBytes(entry.Length);
                if (name != null)
                    entry.Name = Encoding.ASCII.GetString(name).TrimEnd('\0');
            }
            entry.OrdinalNumber = data.ReadUInt16();

            return entry;
        }

        /// <summary>
        /// Parse a Stream into a debug information
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="size">Total size of the debug information</param>
        /// <returns>Filled debug information on success, null on error</returns>
        public static DebugInformation? ParseDebugInformation(Stream data, long size)
        {
            // TODO: Use marshalling here instead of building
            var debugInformation = new DebugInformation();

            byte[]? signature = data.ReadBytes(3);
            if (signature == null)
                return null;

            debugInformation.Signature = Encoding.ASCII.GetString(signature);
            if (debugInformation.Signature != DebugInformationSignatureString)
                return null;

            debugInformation.FormatType = (DebugFormatType)data.ReadByteValue();
            debugInformation.DebuggerData = data.ReadBytes((int)(size - 4));

            return debugInformation;
        }
    }
}