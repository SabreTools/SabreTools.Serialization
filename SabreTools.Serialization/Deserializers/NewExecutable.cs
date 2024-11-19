using System.Collections.Generic;
using System.IO;
using SabreTools.IO.Extensions;
using SabreTools.Models.NewExecutable;
using static SabreTools.Models.NewExecutable.Constants;

namespace SabreTools.Serialization.Deserializers
{
    public class NewExecutable : BaseBinaryDeserializer<Executable>
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

            #region Executable Header

            // Try to parse the executable header
            data.Seek(initialOffset + stub.Header.NewExeHeaderAddr, SeekOrigin.Begin);
            var executableHeader = ParseExecutableHeader(data);
            if (executableHeader == null)
                return null;

            // Set the executable header
            executable.Header = executableHeader;

            #endregion

            #region Segment Table

            // If the offset for the segment table doesn't exist
            int tableAddress = initialOffset
                + (int)stub.Header.NewExeHeaderAddr
                + executableHeader.SegmentTableOffset;
            if (tableAddress >= data.Length)
                return executable;

            // Try to parse the segment table
            data.Seek(tableAddress, SeekOrigin.Begin);
            var segmentTable = ParseSegmentTable(data, executableHeader.FileSegmentCount);
            if (segmentTable == null)
                return null;

            // Set the segment table
            executable.SegmentTable = segmentTable;

            #endregion

            #region Resource Table

            // If the offset for the segment table doesn't exist
            tableAddress = initialOffset
                + (int)stub.Header.NewExeHeaderAddr
                + executableHeader.ResourceTableOffset;
            if (tableAddress >= data.Length)
                return executable;

            // Try to parse the resource table
            data.Seek(tableAddress, SeekOrigin.Begin);
            var resourceTable = ParseResourceTable(data, executableHeader.ResourceEntriesCount);
            if (resourceTable == null)
                return null;

            // Set the resource table
            executable.ResourceTable = resourceTable;

            #endregion

            #region Resident-Name Table

            // If the offset for the resident-name table doesn't exist
            tableAddress = initialOffset
                + (int)stub.Header.NewExeHeaderAddr
                + executableHeader.ResidentNameTableOffset;
            int endOffset = initialOffset
                + (int)stub.Header.NewExeHeaderAddr
                + executableHeader.ModuleReferenceTableOffset;
            if (tableAddress >= data.Length)
                return executable;

            // Try to parse the resident-name table
            data.Seek(tableAddress, SeekOrigin.Begin);
            var residentNameTable = ParseResidentNameTable(data, endOffset);
            if (residentNameTable == null)
                return null;

            // Set the resident-name table
            executable.ResidentNameTable = residentNameTable;

            #endregion

            #region Module-Reference Table

            // If the offset for the module-reference table doesn't exist
            tableAddress = initialOffset
                + (int)stub.Header.NewExeHeaderAddr
                + executableHeader.ModuleReferenceTableOffset;
            if (tableAddress >= data.Length)
                return executable;

            // Try to parse the module-reference table
            data.Seek(tableAddress, SeekOrigin.Begin);
            var moduleReferenceTable = ParseModuleReferenceTable(data, executableHeader.ModuleReferenceTableSize);
            if (moduleReferenceTable == null)
                return null;

            // Set the module-reference table
            executable.ModuleReferenceTable = moduleReferenceTable;

            #endregion

            #region Imported-Name Table

            // If the offset for the imported-name table doesn't exist
            tableAddress = initialOffset
                + (int)stub.Header.NewExeHeaderAddr
                + executableHeader.ImportedNamesTableOffset;
            endOffset = initialOffset
                + (int)stub.Header.NewExeHeaderAddr
                + executableHeader.EntryTableOffset;
            if (tableAddress >= data.Length)
                return executable;

            // Try to parse the imported-name table
            data.Seek(tableAddress, SeekOrigin.Begin);
            var importedNameTable = ParseImportedNameTable(data, endOffset);
            if (importedNameTable == null)
                return null;

            // Set the imported-name table
            executable.ImportedNameTable = importedNameTable;

            #endregion

            #region Entry Table

            // If the offset for the imported-name table doesn't exist
            tableAddress = initialOffset
                + (int)stub.Header.NewExeHeaderAddr
                + executableHeader.EntryTableOffset;
            endOffset = initialOffset
                + (int)stub.Header.NewExeHeaderAddr
                + executableHeader.EntryTableOffset
                + executableHeader.EntryTableSize;
            if (tableAddress >= data.Length)
                return executable;

            // Try to parse the imported-name table
            data.Seek(tableAddress, SeekOrigin.Begin);
            var entryTable = ParseEntryTable(data, endOffset);
            if (entryTable == null)
                return null;

            // Set the entry table
            executable.EntryTable = entryTable;

            #endregion

            #region Nonresident-Name Table

            // If the offset for the nonresident-name table doesn't exist
            tableAddress = initialOffset
                + (int)executableHeader.NonResidentNamesTableOffset;
            endOffset = initialOffset
                + (int)executableHeader.NonResidentNamesTableOffset
                + executableHeader.NonResidentNameTableSize;
            if (tableAddress >= data.Length)
                return executable;

            // Try to parse the nonresident-name table
            data.Seek(tableAddress, SeekOrigin.Begin);
            var nonResidentNameTable = ParseNonResidentNameTable(data, endOffset);
            if (nonResidentNameTable == null)
                return null;

            // Set the nonresident-name table
            executable.NonResidentNameTable = nonResidentNameTable;

            #endregion

            return executable;
        }

        /// <summary>
        /// Parse a Stream into a New Executable header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled executable header on success, null on error</returns>
        public static ExecutableHeader? ParseExecutableHeader(Stream data)
        {
            var header = data.ReadType<ExecutableHeader>();

            if (header?.Magic != SignatureString)
                return null;

            return header;
        }

        /// <summary>
        /// Parse a Stream into a segment table
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="count">Number of segment table entries to read</param>
        /// <returns>Filled segment table on success, null on error</returns>
        public static SegmentTableEntry[]? ParseSegmentTable(Stream data, int count)
        {
            // TODO: Use marshalling here instead of building
            var segmentTable = new SegmentTableEntry[count];

            for (int i = 0; i < count; i++)
            {
                var entry = ParseSegmentTableEntry(data);
                if (entry == null)
                    return null;

                segmentTable[i] = entry;
            }

            return segmentTable;
        }

        /// <summary>
        /// Parse a Stream into a segment table entry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled segment table entry on success, null on error</returns>
        public static SegmentTableEntry? ParseSegmentTableEntry(Stream data)
        {
            return data.ReadType<SegmentTableEntry>();
        }

        /// <summary>
        /// Parse a Stream into a resource table
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="count">Number of resource table entries to read</param>
        /// <returns>Filled resource table on success, null on error</returns>
        public static ResourceTable? ParseResourceTable(Stream data, ushort count)
        {
            long initialOffset = data.Position;

            // TODO: Use marshalling here instead of building
            var resourceTable = new ResourceTable();

            resourceTable.AlignmentShiftCount = data.ReadUInt16();
            var resourceTypes = new List<ResourceTypeInformationEntry>();

            for (int i = 0; i < count; i++)
            {
                var entry = new ResourceTypeInformationEntry();

                entry.TypeID = data.ReadUInt16();
                entry.ResourceCount = data.ReadUInt16();
                entry.Reserved = data.ReadUInt32();

                // A zero type ID marks the end of the resource type information blocks.
                if (entry.TypeID == 0)
                {
                    resourceTypes.Add(entry);
                    break;
                }

                entry.Resources = new ResourceTypeResourceEntry[entry.ResourceCount];
                for (int j = 0; j < entry.ResourceCount; j++)
                {
                    // TODO: Should we read and store the resource data?
                    var resource = ParseResourceTypeResourceEntry(data);
                    if (resource == null)
                        return null;

                    entry.Resources[j] = resource;
                }
                resourceTypes.Add(entry);
            }

            resourceTable.ResourceTypes = [.. resourceTypes];

            // Get the full list of unique string offsets
            var stringOffsets = new List<ushort>();
            foreach (var rtie in resourceTable.ResourceTypes)
            {
                // Skip invalid entries
                if (rtie == null || rtie.TypeID == 0)
                    continue;

                // Handle offset types
                if (!rtie.IsIntegerType() && !stringOffsets.Contains(rtie.TypeID))
                    stringOffsets.Add(rtie.TypeID);

                // Handle types with resources
                foreach (var rtre in rtie.Resources ?? [])
                {
                    // Skip invalid entries
                    if (rtre == null || rtre.IsIntegerType() || rtre.ResourceID == 0)
                        continue;

                    // Skip already added entries
                    if (stringOffsets.Contains(rtre.ResourceID))
                        continue;

                    stringOffsets.Add(rtre.ResourceID);
                }
            }

            // Order the offsets list
            stringOffsets.Sort();

            // Populate the type and name string dictionary
            resourceTable.TypeAndNameStrings = [];
            for (int i = 0; i < stringOffsets.Count; i++)
            {
                int stringOffset = (int)(stringOffsets[i] + initialOffset);
                data.Seek(stringOffset, SeekOrigin.Begin);

                var str = ParseResourceTypeAndNameString(data);
                if (str == null)
                    return null;

                resourceTable.TypeAndNameStrings[stringOffsets[i]] = str;
            }

            return resourceTable;
        }

        /// <summary>
        /// Parse a Stream into a resource entry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled resource entry on success, null on error</returns>
        public static ResourceTypeResourceEntry? ParseResourceTypeResourceEntry(Stream data)
        {
            // TODO: Should we read and store the resource data?
            return data.ReadType<ResourceTypeResourceEntry>();
        }

        /// <summary>
        /// Parse a Stream into a resource type and name string
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled resource type and name string on success, null on error</returns>
        public static ResourceTypeAndNameString? ParseResourceTypeAndNameString(Stream data)
        {
            // TODO: Use marshalling here instead of building
            var str = new ResourceTypeAndNameString();

            str.Length = data.ReadByteValue();
            str.Text = data.ReadBytes(str.Length);

            return str;
        }

        /// <summary>
        /// Parse a Stream into a resident-name table
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="endOffset">First address not part of the resident-name table</param>
        /// <returns>Filled resident-name table on success, null on error</returns>
        public static ResidentNameTableEntry[]? ParseResidentNameTable(Stream data, int endOffset)
        {
            // TODO: Use marshalling here instead of building
            var residentNameTable = new List<ResidentNameTableEntry>();

            while (data.Position < endOffset && data.Position < data.Length)
            {
                var entry = ParseResidentNameTableEntry(data);
                if (entry == null)
                    return null;

                residentNameTable.Add(entry);
            }

            return [.. residentNameTable];
        }

        /// <summary>
        /// Parse a Stream into a resident-name table entry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled resident-name table entry on success, null on error</returns>
        public static ResidentNameTableEntry? ParseResidentNameTableEntry(Stream data)
        {
            // TODO: Use marshalling here instead of building
            var entry = new ResidentNameTableEntry();

            entry.Length = data.ReadByteValue();
            entry.NameString = data.ReadBytes(entry.Length);
            entry.OrdinalNumber = data.ReadUInt16();

            return entry;
        }

        /// <summary>
        /// Parse a Stream into a module-reference table
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="count">Number of module-reference table entries to read</param>
        /// <returns>Filled module-reference table on success, null on error</returns>
        public static ModuleReferenceTableEntry[]? ParseModuleReferenceTable(Stream data, int count)
        {
            // TODO: Use marshalling here instead of building
            var moduleReferenceTable = new ModuleReferenceTableEntry[count];

            for (int i = 0; i < count; i++)
            {
                var entry = ParseModuleReferenceTableEntry(data);
                if (entry == null)
                    return null;

                moduleReferenceTable[i] = entry;
            }

            return moduleReferenceTable;
        }

        /// <summary>
        /// Parse a Stream into a module-reference table entry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled module-reference table entry on success, null on error</returns>
        public static ModuleReferenceTableEntry? ParseModuleReferenceTableEntry(Stream data)
        {
            return data.ReadType<ModuleReferenceTableEntry>();
        }

        /// <summary>
        /// Parse a Stream into an imported-name table
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="endOffset">First address not part of the imported-name table</param>
        /// <returns>Filled imported-name table on success, null on error</returns>
        public static Dictionary<ushort, ImportedNameTableEntry>? ParseImportedNameTable(Stream data, int endOffset)
        {
            // TODO: Use marshalling here instead of building
            var importedNameTable = new Dictionary<ushort, ImportedNameTableEntry>();

            while (data.Position < endOffset && data.Position < data.Length)
            {
                ushort currentOffset = (ushort)data.Position;
                var entry = ParseImportedNameTableEntry(data);
                if (entry == null)
                    return null;

                importedNameTable[currentOffset] = entry;
            }

            return importedNameTable;
        }

        /// <summary>
        /// Parse a Stream into an imported-name table entry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled imported-name table entry on success, null on error</returns>
        public static ImportedNameTableEntry? ParseImportedNameTableEntry(Stream data)
        {
            // TODO: Use marshalling here instead of building
            var entry = new ImportedNameTableEntry();

            entry.Length = data.ReadByteValue();
            entry.NameString = data.ReadBytes(entry.Length);

            return entry;
        }

        /// <summary>
        /// Parse a Stream into an entry table
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="endOffset">First address not part of the entry table</param>
        /// <returns>Filled entry table on success, null on error</returns>
        public static EntryTableBundle[] ParseEntryTable(Stream data, int endOffset)
        {
            // TODO: Use marshalling here instead of building
            var entryTable = new List<EntryTableBundle>();

            while (data.Position < endOffset && data.Position < data.Length)
            {
                var entry = new EntryTableBundle();
                entry.EntryCount = data.ReadByteValue();
                entry.SegmentIndicator = data.ReadByteValue();
                switch (entry.GetEntryType())
                {
                    case SegmentEntryType.Unused:
                        break;

                    case SegmentEntryType.FixedSegment:
                        entry.FixedFlagWord = (FixedSegmentEntryFlag)data.ReadByteValue();
                        entry.FixedOffset = data.ReadUInt16();
                        break;

                    case SegmentEntryType.MoveableSegment:
                        entry.MoveableFlagWord = (MoveableSegmentEntryFlag)data.ReadByteValue();
                        entry.MoveableReserved = data.ReadUInt16();
                        entry.MoveableSegmentNumber = data.ReadByteValue();
                        entry.MoveableOffset = data.ReadUInt16();
                        break;
                }
                entryTable.Add(entry);
            }

            return [.. entryTable];
        }

        /// <summary>
        /// Parse a Stream into a nonresident-name table
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="endOffset">First address not part of the nonresident-name table</param>
        /// <returns>Filled nonresident-name table on success, null on error</returns>
        public static NonResidentNameTableEntry[]? ParseNonResidentNameTable(Stream data, int endOffset)
        {
            // TODO: Use marshalling here instead of building
            var residentNameTable = new List<NonResidentNameTableEntry>();

            while (data.Position < endOffset && data.Position < data.Length)
            {
                var entry = ParseNonResidentNameTableEntry(data);
                if (entry == null)
                    return null;

                residentNameTable.Add(entry);
            }

            return [.. residentNameTable];
        }

        /// <summary>
        /// Parse a Stream into a nonresident-name table entry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled nonresident-name table entry on success, null on error</returns>
        public static NonResidentNameTableEntry? ParseNonResidentNameTableEntry(Stream data)
        {
            // TODO: Use marshalling here instead of building
            var entry = new NonResidentNameTableEntry();

            entry.Length = data.ReadByteValue();
            entry.NameString = data.ReadBytes(entry.Length);
            entry.OrdinalNumber = data.ReadUInt16();

            return entry;
        }
    }
}