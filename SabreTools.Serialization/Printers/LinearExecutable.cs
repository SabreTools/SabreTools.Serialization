using System.Text;
using SabreTools.Models.LinearExecutable;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Printers
{
    public class LinearExecutable : IPrinter<Executable>
    {
        /// <inheritdoc/>
        public void PrintInformation(StringBuilder builder, Executable model)
            => Print(builder, model);

        public static void Print(StringBuilder builder, Executable executable)
        {
            builder.AppendLine("New Executable Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine();

            // Stub
            Print(builder, executable.Stub?.Header);

            // Information Block
            Print(builder, executable.InformationBlock);

            // Tables
            Print(builder, executable.ObjectTable);
            Print(builder, executable.ObjectPageMap);
            Print(builder, executable.ResourceTable);
            Print(builder, executable.ResidentNamesTable);
            Print(builder, executable.EntryTable);
            Print(builder, executable.ModuleFormatDirectivesTable);
            Print(builder, executable.VerifyRecordDirectiveTable);
            Print(builder, executable.FixupPageTable);
            Print(builder, executable.FixupRecordTable);
            Print(builder, executable.ImportModuleNameTable);
            Print(builder, executable.ImportModuleProcedureNameTable);
            Print(builder, executable.PerPageChecksumTable);
            Print(builder, executable.NonResidentNamesTable);

            // Debug
            Print(builder, executable.DebugInformation);
        }

        private static void Print(StringBuilder builder, Models.MSDOS.ExecutableHeader? header)
        {
            builder.AppendLine("  MS-DOS Stub Header Information:");
            builder.AppendLine("  -------------------------");
            if (header == null)
            {
                builder.AppendLine("  No MS-DOS stub header");
                builder.AppendLine();
                return;
            }

            builder.AppendLine(header.Magic, "  Magic number");
            builder.AppendLine(header.LastPageBytes, "  Last page bytes");
            builder.AppendLine(header.Pages, "  Pages");
            builder.AppendLine(header.RelocationItems, "  Relocation items");
            builder.AppendLine(header.HeaderParagraphSize, "  Header paragraph size");
            builder.AppendLine(header.MinimumExtraParagraphs, "  Minimum extra paragraphs");
            builder.AppendLine(header.MaximumExtraParagraphs, "  Maximum extra paragraphs");
            builder.AppendLine(header.InitialSSValue, "  Initial SS value");
            builder.AppendLine(header.InitialSPValue, "  Initial SP value");
            builder.AppendLine(header.Checksum, "  Checksum");
            builder.AppendLine(header.InitialIPValue, "  Initial IP value");
            builder.AppendLine(header.InitialCSValue, "  Initial CS value");
            builder.AppendLine(header.RelocationTableAddr, "  Relocation table address");
            builder.AppendLine(header.OverlayNumber, "  Overlay number");
            builder.AppendLine();

            builder.AppendLine("  MS-DOS Stub Extended Header Information:");
            builder.AppendLine("  -------------------------");
            builder.AppendLine(header.Reserved1, "  Reserved words");
            builder.AppendLine(header.OEMIdentifier, "  OEM identifier");
            builder.AppendLine(header.OEMInformation, "  OEM information");
            builder.AppendLine(header.Reserved2, "  Reserved words");
            builder.AppendLine(header.NewExeHeaderAddr, "  New EXE header address");
            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, InformationBlock? block)
        {
            builder.AppendLine("  Information Block Information:");
            builder.AppendLine("  -------------------------");
            if (block == null)
            {
                builder.AppendLine("  No information block");
                builder.AppendLine();
                return;
            }

            builder.AppendLine(block.Signature, "  Signature");
            builder.AppendLine($"  Byte order: {block.ByteOrder} (0x{block.ByteOrder:X})");
            builder.AppendLine($"  Word order: {block.WordOrder} (0x{block.WordOrder:X})");
            builder.AppendLine(block.ExecutableFormatLevel, "  Executable format level");
            builder.AppendLine($"  CPU type: {block.CPUType} (0x{block.CPUType:X})");
            builder.AppendLine($"  Module OS: {block.ModuleOS} (0x{block.ModuleOS:X})");
            builder.AppendLine(block.ModuleVersion, "  Module version");
            builder.AppendLine($"  Module type flags: {block.ModuleTypeFlags} (0x{block.ModuleTypeFlags:X})");
            builder.AppendLine(block.ModuleNumberPages, "  Module number pages");
            builder.AppendLine(block.InitialObjectCS, "  Initial object CS");
            builder.AppendLine(block.InitialEIP, "  Initial EIP");
            builder.AppendLine(block.InitialObjectSS, "  Initial object SS");
            builder.AppendLine(block.InitialESP, "  Initial ESP");
            builder.AppendLine(block.MemoryPageSize, "  Memory page size");
            builder.AppendLine(block.BytesOnLastPage, "  Bytes on last page");
            builder.AppendLine(block.FixupSectionSize, "  Fix-up section size");
            builder.AppendLine(block.FixupSectionChecksum, "  Fix-up section checksum");
            builder.AppendLine(block.LoaderSectionSize, "  Loader section size");
            builder.AppendLine(block.LoaderSectionChecksum, "  Loader section checksum");
            builder.AppendLine(block.ObjectTableOffset, "  Object table offset");
            builder.AppendLine(block.ObjectTableCount, "  Object table count");
            builder.AppendLine(block.ObjectPageMapOffset, "  Object page map offset");
            builder.AppendLine(block.ObjectIterateDataMapOffset, "  Object iterate data map offset");
            builder.AppendLine(block.ResourceTableOffset, "  Resource table offset");
            builder.AppendLine(block.ResourceTableCount, "  Resource table count");
            builder.AppendLine(block.ResidentNamesTableOffset, "  Resident names table offset");
            builder.AppendLine(block.EntryTableOffset, "  Entry table offset");
            builder.AppendLine(block.ModuleDirectivesTableOffset, "  Module directives table offset");
            builder.AppendLine(block.ModuleDirectivesCount, "  Module directives table count");
            builder.AppendLine(block.FixupPageTableOffset, "  Fix-up page table offset");
            builder.AppendLine(block.FixupRecordTableOffset, "  Fix-up record table offset");
            builder.AppendLine(block.ImportedModulesNameTableOffset, "  Imported modules name table offset");
            builder.AppendLine(block.ImportedModulesCount, "  Imported modules count");
            builder.AppendLine(block.ImportProcedureNameTableOffset, "  Imported procedure name table count");
            builder.AppendLine(block.PerPageChecksumTableOffset, "  Per-page checksum table offset");
            builder.AppendLine(block.DataPagesOffset, "  Data pages offset");
            builder.AppendLine(block.PreloadPageCount, "  Preload page count");
            builder.AppendLine(block.NonResidentNamesTableOffset, "  Non-resident names table offset");
            builder.AppendLine(block.NonResidentNamesTableLength, "  Non-resident names table length");
            builder.AppendLine(block.NonResidentNamesTableChecksum, "  Non-resident names table checksum");
            builder.AppendLine(block.AutomaticDataObject, "  Automatic data object");
            builder.AppendLine(block.DebugInformationOffset, "  Debug information offset");
            builder.AppendLine(block.DebugInformationLength, "  Debug information length");
            builder.AppendLine(block.PreloadInstancePagesNumber, "  Preload instance pages number");
            builder.AppendLine(block.DemandInstancePagesNumber, "  Demand instance pages number");
            builder.AppendLine(block.ExtraHeapAllocation, "  Extra heap allocation");
            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, ObjectTableEntry[]? entries)
        {
            builder.AppendLine("  Object Table Information:");
            builder.AppendLine("  -------------------------");
            if (entries == null || entries.Length == 0)
            {
                builder.AppendLine("  No object table entries");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];

                builder.AppendLine($"  Object Table Entry {i}");
                builder.AppendLine(entry.VirtualSegmentSize, "    Virtual segment size");
                builder.AppendLine(entry.RelocationBaseAddress, "    Relocation base address");
                builder.AppendLine($"    Object flags: {entry.ObjectFlags} (0x{entry.ObjectFlags:X})");
                builder.AppendLine(entry.PageTableIndex, "    Page table index");
                builder.AppendLine(entry.PageTableEntries, "    Page table entries");
                builder.AppendLine(entry.Reserved, "    Reserved");
            }

            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, ObjectPageMapEntry[]? entries)
        {
            builder.AppendLine("  Object Page Map Information:");
            builder.AppendLine("  -------------------------");
            if (entries == null || entries.Length == 0)
            {
                builder.AppendLine("  No object page map entries");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];

                builder.AppendLine($"  Object Page Map Entry {i}");
                builder.AppendLine(entry.PageDataOffset, "    Page data offset");
                builder.AppendLine(entry.DataSize, "    Data size");
                builder.AppendLine($"    Flags: {entry.Flags} (0x{entry.Flags:X})");
            }

            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, ResourceTableEntry[]? entries)
        {
            builder.AppendLine("  Resource Table Information:");
            builder.AppendLine("  -------------------------");
            if (entries == null || entries.Length == 0)
            {
                builder.AppendLine("  No resource table entries");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];

                builder.AppendLine($"  Resource Table Entry {i}");
                builder.AppendLine($"    Type ID: {entry.TypeID} (0x{entry.TypeID:X})");
                builder.AppendLine(entry.NameID, "    Name ID");
                builder.AppendLine(entry.ResourceSize, "    Resource size");
                builder.AppendLine(entry.ObjectNumber, "    Object number");
                builder.AppendLine(entry.Offset, "    Offset");
            }

            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, ResidentNamesTableEntry[]? entries)
        {
            builder.AppendLine("  Resident Names Table Information:");
            builder.AppendLine("  -------------------------");
            if (entries == null || entries.Length == 0)
            {
                builder.AppendLine("  No resident names table entries");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];

                builder.AppendLine($"  Resident Names Table Entry {i}");
                builder.AppendLine(entry.Length, "    Length");
                builder.AppendLine(entry.Name, "    Name");
                builder.AppendLine(entry.OrdinalNumber, "    Ordinal number");
            }

            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, EntryTableBundle[]? entries)
        {
            builder.AppendLine("  Entry Table Information:");
            builder.AppendLine("  -------------------------");
            if (entries == null || entries.Length == 0)
            {
                builder.AppendLine("  No entry table bundles");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];

                builder.AppendLine($"  Entry Table Bundle {i}");
                builder.AppendLine(entry.Entries, "    Entries");
                builder.AppendLine($"    Bundle type: {entry.BundleType} (0x{entry.BundleType:X})");
                builder.AppendLine();
                Print(builder, entry.TableEntries, entry.BundleType);
            }

            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, EntryTableEntry[]? entries, BundleType type)
        {
            builder.AppendLine("    Entry Table Entries:");
            builder.AppendLine("    -------------------------");
            if (entries == null || entries.Length == 0)
            {
                builder.AppendLine("    No entry table entries");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];

                builder.AppendLine($"    Entry Table Entry {i}");
                switch (type & ~BundleType.ParameterTypingInformationPresent)
                {
                    case BundleType.UnusedEntry:
                        builder.AppendLine("      Unused, empty entry");
                        break;

                    case BundleType.SixteenBitEntry:
                        builder.AppendLine(entry.SixteenBitObjectNumber, "      Object number");
                        builder.AppendLine($"      Entry flags: {entry.SixteenBitEntryFlags} (0x{entry.SixteenBitEntryFlags:X})");
                        builder.AppendLine(entry.SixteenBitOffset, "      Offset");
                        break;

                    case BundleType.TwoEightySixCallGateEntry:
                        builder.AppendLine(entry.TwoEightySixObjectNumber, "      Object number");
                        builder.AppendLine($"      Entry flags: {entry.TwoEightySixEntryFlags} (0x{entry.TwoEightySixEntryFlags:X})");
                        builder.AppendLine(entry.TwoEightySixOffset, "      Offset");
                        builder.AppendLine(entry.TwoEightySixCallgate, "      Callgate");
                        break;

                    case BundleType.ThirtyTwoBitEntry:
                        builder.AppendLine(entry.ThirtyTwoBitObjectNumber, "      Object number");
                        builder.AppendLine($"      Entry flags: {entry.ThirtyTwoBitEntryFlags} (0x{entry.ThirtyTwoBitEntryFlags:X})");
                        builder.AppendLine(entry.ThirtyTwoBitOffset, "      Offset");
                        break;

                    case BundleType.ForwarderEntry:
                        builder.AppendLine(entry.ForwarderReserved, "      Reserved");
                        builder.AppendLine($"      Forwarder flags: {entry.ForwarderFlags} (0x{entry.ForwarderFlags:X})");
                        builder.AppendLine(entry.ForwarderModuleOrdinalNumber, "      Module ordinal number");
                        builder.AppendLine(entry.ProcedureNameOffset, "      Procedure name offset");
                        builder.AppendLine(entry.ImportOrdinalNumber, "      Import ordinal number");
                        break;

                    default:
                        builder.AppendLine($"      Unknown entry type {type}");
                        break;
                }
            }
        }

        private static void Print(StringBuilder builder, ModuleFormatDirectivesTableEntry[]? entries)
        {
            builder.AppendLine("  Module Format Directives Table Information:");
            builder.AppendLine("  -------------------------");
            if (entries == null || entries.Length == 0)
            {
                builder.AppendLine("  No module format directives table entries");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];

                builder.AppendLine($"  Moduile Format Directives Table Entry {i}");
                builder.AppendLine($"    Directive number: {entry.DirectiveNumber} (0x{entry.DirectiveNumber:X})");
                builder.AppendLine(entry.DirectiveDataLength, "    Directive data length");
                builder.AppendLine(entry.DirectiveDataOffset, "    Directive data offset");
            }

            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, VerifyRecordDirectiveTableEntry[]? entries)
        {
            builder.AppendLine("  Verify Record Directive Table Information:");
            builder.AppendLine("  -------------------------");
            if (entries == null || entries.Length == 0)
            {
                builder.AppendLine("  No verify record directive table entries");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];

                builder.AppendLine($"  Verify Record Directive Table Entry {i}");
                builder.AppendLine(entry.EntryCount, "    Entry count");
                builder.AppendLine(entry.OrdinalIndex, "    Ordinal index");
                builder.AppendLine(entry.Version, "    Version");
                builder.AppendLine(entry.ObjectEntriesCount, "    Object entries count");
                builder.AppendLine(entry.ObjectNumberInModule, "    Object number in module");
                builder.AppendLine(entry.ObjectLoadBaseAddress, "    Object load base address");
                builder.AppendLine(entry.ObjectVirtualAddressSize, "    Object virtual address size");
            }

            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, FixupPageTableEntry[]? entries)
        {
            builder.AppendLine("  Fix-up Page Table Information:");
            builder.AppendLine("  -------------------------");
            if (entries == null || entries.Length == 0)
            {
                builder.AppendLine("  No fix-up page table entries");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];

                builder.AppendLine($"  Fix-up Page Table Entry {i}");
                builder.AppendLine(entry.Offset, "    Offset");
            }

            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, FixupRecordTableEntry[]? entries)
        {
            builder.AppendLine("  Fix-up Record Table Information:");
            builder.AppendLine("  -------------------------");
            if (entries == null || entries.Length == 0)
            {
                builder.AppendLine("  No fix-up record table entries");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];

                builder.AppendLine($"  Fix-up Record Table Entry {i}");
                builder.AppendLine($"    Source type: {entry.SourceType} (0x{entry.SourceType:X})");
                builder.AppendLine($"    Target flags: {entry.TargetFlags} (0x{entry.TargetFlags:X})");

                // Source list flag
#if NET20 || NET35
                if ((entry.SourceType & FixupRecordSourceType.SourceListFlag) != 0)
#else
                if (entry.SourceType.HasFlag(FixupRecordSourceType.SourceListFlag))
#endif
                    builder.AppendLine(entry.SourceOffsetListCount, "    Source offset list count");
                else
                    builder.AppendLine(entry.SourceOffset, "    Source offset");

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
                        builder.AppendLine(entry.TargetObjectNumberWORD, "    Target object number");
                    else
                        builder.AppendLine(entry.TargetObjectNumberByte, "    Target object number");

                    // 16-bit Selector fixup
#if NET20 || NET35
                    if ((entry.SourceType & FixupRecordSourceType.SixteenBitSelectorFixup) != 0)
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
                            builder.AppendLine(entry.TargetOffsetDWORD, "    Target offset");
                        else
                            builder.AppendLine(entry.TargetOffsetWORD, "    Target offset");
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
                    if (!entry.TargetFlags.HasFlag(FixupRecordTargetFlags.SixteenBitObjectNumberModuleOrdinalFlag))
#endif
                        builder.AppendLine(entry.OrdinalIndexImportModuleNameTableWORD, "    Ordinal index import module name table");
                    else
                        builder.AppendLine(entry.OrdinalIndexImportModuleNameTableByte, "    Ordinal index import module name table");

                    // 8-bit Ordinal Flag & 32-bit Target Offset Flag
#if NET20 || NET35
                    if ((entry.TargetFlags & FixupRecordTargetFlags.EightBitOrdinalFlag) != 0)
#else
                    if (!entry.TargetFlags.HasFlag(FixupRecordTargetFlags.EightBitOrdinalFlag))
#endif
                        builder.AppendLine(entry.ImportedOrdinalNumberByte, "    Imported ordinal number");
#if NET20 || NET35
                    else if ((entry.TargetFlags & FixupRecordTargetFlags.ThirtyTwoBitTargetOffsetFlag) != 0)
#else
                    else if (!entry.TargetFlags.HasFlag(FixupRecordTargetFlags.ThirtyTwoBitTargetOffsetFlag))
#endif
                        builder.AppendLine(entry.ImportedOrdinalNumberDWORD, "    Imported ordinal number");
                    else
                        builder.AppendLine(entry.ImportedOrdinalNumberWORD, "    Imported ordinal number");

                    // Additive Fixup Flag
#if NET20 || NET35
                    if ((entry.TargetFlags & FixupRecordTargetFlags.AdditiveFixupFlag) != 0)
#else
                    if (!entry.TargetFlags.HasFlag(FixupRecordTargetFlags.AdditiveFixupFlag))
#endif
                    {
                        // 32-bit Additive Flag
#if NET20 || NET35
                        if ((entry.TargetFlags & FixupRecordTargetFlags.ThirtyTwoBitAdditiveFixupFlag) != 0)
#else
                        if (!entry.TargetFlags.HasFlag(FixupRecordTargetFlags.ThirtyTwoBitAdditiveFixupFlag))
#endif
                            builder.AppendLine(entry.AdditiveFixupValueDWORD, "    Additive fixup value");
                        else
                            builder.AppendLine(entry.AdditiveFixupValueWORD, "    Additive fixup value");
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
                    if (!entry.TargetFlags.HasFlag(FixupRecordTargetFlags.SixteenBitObjectNumberModuleOrdinalFlag))
#endif
                        builder.AppendLine(entry.OrdinalIndexImportModuleNameTableWORD, "    Ordinal index import module name table");
                    else
                        builder.AppendLine(entry.OrdinalIndexImportModuleNameTableByte, "    Ordinal index import module name table");

                    // 32-bit Target Offset Flag
#if NET20 || NET35
                    if ((entry.TargetFlags & FixupRecordTargetFlags.ThirtyTwoBitTargetOffsetFlag) != 0)
#else
                    if (!entry.TargetFlags.HasFlag(FixupRecordTargetFlags.ThirtyTwoBitTargetOffsetFlag))
#endif
                        builder.AppendLine(entry.OffsetImportProcedureNameTableDWORD, "    Offset import procedure name table");
                    else
                        builder.AppendLine(entry.OffsetImportProcedureNameTableWORD, "    Offset import procedure name table");

                    // Additive Fixup Flag
#if NET20 || NET35
                    if ((entry.TargetFlags & FixupRecordTargetFlags.AdditiveFixupFlag) != 0)
#else
                    if (!entry.TargetFlags.HasFlag(FixupRecordTargetFlags.AdditiveFixupFlag))
#endif
                    {
                        // 32-bit Additive Flag
#if NET20 || NET35
                        if ((entry.TargetFlags & FixupRecordTargetFlags.ThirtyTwoBitAdditiveFixupFlag) != 0)
#else
                        if (!entry.TargetFlags.HasFlag(FixupRecordTargetFlags.ThirtyTwoBitAdditiveFixupFlag))
#endif
                            builder.AppendLine(entry.AdditiveFixupValueDWORD, "    Additive fixup value");
                        else
                            builder.AppendLine(entry.AdditiveFixupValueWORD, "    Additive fixup value");
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
                    if (!entry.TargetFlags.HasFlag(FixupRecordTargetFlags.SixteenBitObjectNumberModuleOrdinalFlag))
#endif
                        builder.AppendLine(entry.TargetObjectNumberWORD, "    Target object number");
                    else
                        builder.AppendLine(entry.TargetObjectNumberByte, "    Target object number");

                    // Additive Fixup Flag
#if NET20 || NET35
                    if ((entry.TargetFlags & FixupRecordTargetFlags.AdditiveFixupFlag) != 0)
#else
                    if (!entry.TargetFlags.HasFlag(FixupRecordTargetFlags.AdditiveFixupFlag))
#endif
                    {
                        // 32-bit Additive Flag
#if NET20 || NET35
                        if ((entry.TargetFlags & FixupRecordTargetFlags.ThirtyTwoBitAdditiveFixupFlag) != 0)
#else
                        if (!entry.TargetFlags.HasFlag(FixupRecordTargetFlags.ThirtyTwoBitAdditiveFixupFlag))
#endif
                            builder.AppendLine(entry.AdditiveFixupValueDWORD, "    Additive fixup value");
                        else
                            builder.AppendLine(entry.AdditiveFixupValueWORD, "    Additive fixup value");
                    }
                }

                // No other top-level flags recognized
                else
                {
                    builder.AppendLine("    Unknown entry format");
                }

                builder.AppendLine();
                builder.AppendLine("    Source Offset List:");
                builder.AppendLine("    -------------------------");
                if (entry.SourceOffsetList == null || entry.SourceOffsetList.Length == 0)
                {
                    builder.AppendLine("    No source offset list entries");
                }
                else
                {
                    for (int j = 0; j < entry.SourceOffsetList.Length; j++)
                    {
                        builder.AppendLine(entry.SourceOffsetList[j], $"    Source Offset List Entry {j}");
                    }
                }
                builder.AppendLine();
            }
        }

        private static void Print(StringBuilder builder, ImportModuleNameTableEntry[]? entries)
        {
            builder.AppendLine("  Import Module Name Table Information:");
            builder.AppendLine("  -------------------------");
            if (entries == null || entries.Length == 0)
            {
                builder.AppendLine("  No import module name table entries");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];

                builder.AppendLine($"  Import Module Name Table Entry {i}");
                builder.AppendLine(entry.Length, "    Length");
                builder.AppendLine(entry.Name, "    Name");
            }

            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, ImportModuleProcedureNameTableEntry[]? entries)
        {
            builder.AppendLine("  Import Module Procedure Name Table Information:");
            builder.AppendLine("  -------------------------");
            if (entries == null || entries.Length == 0)
            {
                builder.AppendLine("  No import module procedure name table entries");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];

                builder.AppendLine($"  Import Module Procedure Name Table Entry {i}");
                builder.AppendLine(entry.Length, "    Length");
                builder.AppendLine(entry.Name, "    Name");
            }

            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, PerPageChecksumTableEntry[]? entries)
        {
            builder.AppendLine("  Per-Page Checksum Table Information:");
            builder.AppendLine("  -------------------------");
            if (entries == null || entries.Length == 0)
            {
                builder.AppendLine("  No per-page checksum table entries");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];

                builder.AppendLine($" Per-Page Checksum Table Entry {i}");
                builder.AppendLine(entry.Checksum, "    Checksum");
            }

            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, NonResidentNamesTableEntry[]? entries)
        {
            builder.AppendLine("  Non-Resident Names Table Information:");
            builder.AppendLine("  -------------------------");
            if (entries == null || entries.Length == 0)
            {
                builder.AppendLine("  No non-resident names table entries");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];

                builder.AppendLine($"  Non-Resident Names Table Entry {i}");
                builder.AppendLine(entry.Length, "    Length");
                builder.AppendLine(entry.Name, "    Name");
                builder.AppendLine(entry.OrdinalNumber, "    Ordinal number");
            }

            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, DebugInformation? di)
        {
            builder.AppendLine("  Debug Information:");
            builder.AppendLine("  -------------------------");
            if (di == null)
            {
                builder.AppendLine("  No debug information");
                builder.AppendLine();
                return;
            }

            builder.AppendLine(di.Signature, "  Signature");
            builder.AppendLine($"  Format type: {di.FormatType} (0x{di.FormatType:X})");
            // Debugger data
            builder.AppendLine();
        }
    }
}
