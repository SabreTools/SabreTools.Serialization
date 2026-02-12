using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using SabreTools.Data.Models.COFF;
using SabreTools.Data.Models.PortableExecutable;
using SabreTools.Data.Models.PortableExecutable.Resource.Entries;
using SabreTools.IO.Extensions;

#pragma warning disable IDE0017 // Simplify object initialization
namespace SabreTools.Data.Extensions
{
    public static class PortableExecutable
    {
        /// <summary>
        /// Convert a relative virtual address to a physical one
        /// </summary>
        /// <param name="rva">Relative virtual address to convert</param>
        /// <param name="sections">Array of sections to check against</param>
        /// <returns>Physical address, 0 on error</returns>
        public static uint ConvertVirtualAddress(this uint rva, SectionHeader[] sections)
        {
            // If we have an invalid section table, we can't do anything
            if (sections.Length == 0)
                return 0;

            // If the RVA is 0, we just return 0 because it's invalid
            if (rva == 0)
                return 0;

            // If the RVA matches a section start exactly, use that
            var matchingSection = Array.Find(sections, s => s.VirtualAddress == rva);
            if (matchingSection is not null)
                return rva - matchingSection.VirtualAddress + matchingSection.PointerToRawData;

            // Loop through all of the sections
            uint maxVirtualAddress = 0, maxRawPointer = 0;
            for (int i = 0; i < sections.Length; i++)
            {
                // If the section "starts" at 0, just skip it
                var section = sections[i];
                if (section.PointerToRawData == 0)
                    continue;

                // If the virtual address is greater than the RVA
                if (rva < section.VirtualAddress)
                    continue;

                // Cache the maximum matching section data, in case of a miss
                if (rva >= section.VirtualAddress)
                {
                    maxVirtualAddress = section.VirtualAddress;
                    maxRawPointer = section.PointerToRawData;
                }

                // Attempt to derive the physical address from the current section
                if (section.VirtualSize != 0 && rva <= section.VirtualAddress + section.VirtualSize)
                    return rva - section.VirtualAddress + section.PointerToRawData;
                else if (section.SizeOfRawData != 0 && rva <= section.VirtualAddress + section.SizeOfRawData)
                    return rva - section.VirtualAddress + section.PointerToRawData;
            }

            return maxRawPointer != 0 ? rva - maxVirtualAddress + maxRawPointer : 0;
        }

        /// <summary>
        /// Find the section a revlative virtual address lives in
        /// </summary>
        /// <param name="rva">Relative virtual address to convert</param>
        /// <param name="sections">Array of sections to check against</param>
        /// <returns>Section index, null on error</returns>
        public static int ContainingSectionIndex(this uint rva, SectionHeader[] sections)
        {
            // If we have an invalid section table, we can't do anything
            if (sections is null || sections.Length == 0)
                return -1;

            // If the RVA is 0, we just return -1 because it's invalid
            if (rva == 0)
                return -1;

            // Loop through all of the sections
            for (int i = 0; i < sections.Length; i++)
            {
                // If the section "starts" at 0, just skip it
                var section = sections[i];
                if (section.PointerToRawData == 0)
                    continue;

                // If the virtual address is greater than the RVA
                if (rva < section.VirtualAddress)
                    continue;

                // Attempt to derive the physical address from the current section
                if (section.VirtualSize != 0 && rva <= section.VirtualAddress + section.VirtualSize)
                    return i;
                else if (section.SizeOfRawData != 0 && rva <= section.VirtualAddress + section.SizeOfRawData)
                    return i;
            }

            return -1;
        }

        #region Debug

        /// <summary>
        /// Parse a byte array into a NB10ProgramDatabase
        /// </summary>
        /// <param name="data">Data to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>A filled NB10ProgramDatabase on success, null on error</returns>
        public static Models.PortableExecutable.DebugData.NB10ProgramDatabase? ParseNB10ProgramDatabase(this byte[] data, ref int offset)
        {
            var obj = new Models.PortableExecutable.DebugData.NB10ProgramDatabase();

            obj.Signature = data.ReadUInt32LittleEndian(ref offset);
            if (obj.Signature != 0x3031424E)
                return null;

            obj.Offset = data.ReadUInt32LittleEndian(ref offset);
            obj.Timestamp = data.ReadUInt32LittleEndian(ref offset);
            obj.Age = data.ReadUInt32LittleEndian(ref offset);
            obj.PdbFileName = data.ReadNullTerminatedAnsiString(ref offset) ?? string.Empty;

            return obj;
        }

        /// <summary>
        /// Parse a byte array into a RSDSProgramDatabase
        /// </summary>
        /// <param name="data">Data to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>A filled RSDSProgramDatabase on success, null on error</returns>
        public static Models.PortableExecutable.DebugData.RSDSProgramDatabase? ParseRSDSProgramDatabase(this byte[] data, ref int offset)
        {
            var obj = new Models.PortableExecutable.DebugData.RSDSProgramDatabase();

            obj.Signature = data.ReadUInt32LittleEndian(ref offset);
            if (obj.Signature != 0x53445352)
                return null;

            obj.GUID = data.ReadGuid(ref offset);
            obj.Age = data.ReadUInt32LittleEndian(ref offset);
            obj.PathAndFileName = data.ReadNullTerminatedUTF8String(ref offset) ?? string.Empty;

            return obj;
        }

        #endregion

        // TODO: Implement other resource types from https://learn.microsoft.com/en-us/windows/win32/menurc/resource-file-formats
        #region Resources

        /// <summary>
        /// Read resource data as a side-by-side assembly manifest
        /// </summary>
        /// <param name="entry">Resource data entry to parse into a side-by-side assembly manifest</param>
        /// <returns>A filled side-by-side assembly manifest on success, null on error</returns>
        public static AssemblyManifest? AsAssemblyManifest(this Models.PortableExecutable.Resource.DataEntry? entry)
        {
            // If we have an invalid entry, just skip
            if (entry?.Data is null)
                return null;

            try
            {
                var serializer = new XmlSerializer(typeof(AssemblyManifest));
                return serializer.Deserialize(new MemoryStream(entry.Data)) as AssemblyManifest;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Read resource data as a font group
        /// </summary>
        /// <param name="entry">Resource data entry to parse into a font group</param>
        /// <returns>A filled font group on success, null on error</returns>
        public static FontGroupHeader? AsFontGroup(this Models.PortableExecutable.Resource.DataEntry? entry)
        {
            // If we have an invalid entry, just skip
            if (entry?.Data is null)
                return null;

            // Initialize the iterator
            int offset = 0;

            // Create the output object
            var fontGroupHeader = new FontGroupHeader();

            fontGroupHeader.NumberOfFonts = entry.Data.ReadUInt16LittleEndian(ref offset);
            if (fontGroupHeader.NumberOfFonts > 0)
            {
                fontGroupHeader.DE = new DirEntry[fontGroupHeader.NumberOfFonts];
                for (int i = 0; i < fontGroupHeader.NumberOfFonts; i++)
                {
                    var dirEntry = new DirEntry();

                    dirEntry.FontOrdinal = entry.Data.ReadUInt16LittleEndian(ref offset);

                    dirEntry.Entry = new FontDirEntry();
                    dirEntry.Entry.Version = entry.Data.ReadUInt16LittleEndian(ref offset);
                    dirEntry.Entry.Size = entry.Data.ReadUInt32LittleEndian(ref offset);
                    dirEntry.Entry.Copyright = entry.Data.ReadBytes(ref offset, 60);
                    dirEntry.Entry.Type = entry.Data.ReadUInt16LittleEndian(ref offset);
                    dirEntry.Entry.Points = entry.Data.ReadUInt16LittleEndian(ref offset);
                    dirEntry.Entry.VertRes = entry.Data.ReadUInt16LittleEndian(ref offset);
                    dirEntry.Entry.HorizRes = entry.Data.ReadUInt16LittleEndian(ref offset);
                    dirEntry.Entry.Ascent = entry.Data.ReadUInt16LittleEndian(ref offset);
                    dirEntry.Entry.InternalLeading = entry.Data.ReadUInt16LittleEndian(ref offset);
                    dirEntry.Entry.ExternalLeading = entry.Data.ReadUInt16LittleEndian(ref offset);
                    dirEntry.Entry.Italic = entry.Data.ReadByte(ref offset);
                    dirEntry.Entry.Underline = entry.Data.ReadByte(ref offset);
                    dirEntry.Entry.StrikeOut = entry.Data.ReadByte(ref offset);
                    dirEntry.Entry.Weight = entry.Data.ReadUInt16LittleEndian(ref offset);
                    dirEntry.Entry.CharSet = entry.Data.ReadByte(ref offset);
                    dirEntry.Entry.PixWidth = entry.Data.ReadUInt16LittleEndian(ref offset);
                    dirEntry.Entry.PixHeight = entry.Data.ReadUInt16LittleEndian(ref offset);
                    dirEntry.Entry.PitchAndFamily = entry.Data.ReadByte(ref offset);
                    dirEntry.Entry.AvgWidth = entry.Data.ReadUInt16LittleEndian(ref offset);
                    dirEntry.Entry.MaxWidth = entry.Data.ReadUInt16LittleEndian(ref offset);
                    dirEntry.Entry.FirstChar = entry.Data.ReadByte(ref offset);
                    dirEntry.Entry.LastChar = entry.Data.ReadByte(ref offset);
                    dirEntry.Entry.DefaultChar = entry.Data.ReadByte(ref offset);
                    dirEntry.Entry.BreakChar = entry.Data.ReadByte(ref offset);
                    dirEntry.Entry.WidthBytes = entry.Data.ReadUInt16LittleEndian(ref offset);
                    dirEntry.Entry.Device = entry.Data.ReadUInt32LittleEndian(ref offset);
                    dirEntry.Entry.Face = entry.Data.ReadUInt32LittleEndian(ref offset);
                    dirEntry.Entry.Reserved = entry.Data.ReadUInt32LittleEndian(ref offset);

                    // TODO: Determine how to read these two? Immediately after?
                    dirEntry.Entry.DeviceName = entry.Data.ReadNullTerminatedAnsiString(ref offset) ?? string.Empty;
                    dirEntry.Entry.FaceName = entry.Data.ReadNullTerminatedAnsiString(ref offset) ?? string.Empty;

                    fontGroupHeader.DE[i] = dirEntry;
                }
            }

            // TODO: Implement entry parsing
            return fontGroupHeader;
        }

        /// <summary>
        ///  Read byte data as a string file info resource
        /// </summary>
        /// <param name="data">Data to parse into a string file info</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>A filled string file info resource on success, null on error</returns>
        public static StringFileInfo? AsStringFileInfo(byte[] data, ref int offset)
        {
            var stringFileInfo = new StringFileInfo();

            // Cache the initial offset
            int currentOffset = offset;

            stringFileInfo.Length = data.ReadUInt16LittleEndian(ref offset);
            stringFileInfo.ValueLength = data.ReadUInt16LittleEndian(ref offset);
            stringFileInfo.ResourceType = (VersionResourceType)data.ReadUInt16LittleEndian(ref offset);
            stringFileInfo.Key = data.ReadNullTerminatedUnicodeString(ref offset) ?? string.Empty;
            if (stringFileInfo.Key != "StringFileInfo")
            {
                offset -= 6 + (((stringFileInfo.Key?.Length ?? 0) + 1) * 2);
                return null;
            }

            // Align to the DWORD boundary if we're not at the end
            data.AlignToBoundary(ref offset, 4);

            var stringFileInfoChildren = new List<Models.PortableExecutable.Resource.Entries.StringTable>();
            while ((offset - currentOffset) < stringFileInfo.Length)
            {
                var stringTable = new Models.PortableExecutable.Resource.Entries.StringTable();

                stringTable.Length = data.ReadUInt16LittleEndian(ref offset);
                stringTable.ValueLength = data.ReadUInt16LittleEndian(ref offset);
                stringTable.ResourceType = (VersionResourceType)data.ReadUInt16LittleEndian(ref offset);
                stringTable.Key = data.ReadNullTerminatedUnicodeString(ref offset) ?? string.Empty;

                // Align to the DWORD boundary if we're not at the end
                data.AlignToBoundary(ref offset, 4);

                var stringTableChildren = new List<StringData>();
                while ((offset - currentOffset) < stringTable.Length)
                {
                    var stringData = new StringData();

                    int dataStartOffset = offset;
                    stringData.Length = data.ReadUInt16LittleEndian(ref offset);
                    stringData.ValueLength = data.ReadUInt16LittleEndian(ref offset);
                    stringData.ResourceType = (VersionResourceType)data.ReadUInt16LittleEndian(ref offset);
                    stringData.Key = data.ReadNullTerminatedUnicodeString(ref offset) ?? string.Empty;

                    // Align to the DWORD boundary if we're not at the end
                    data.AlignToBoundary(ref offset, 4);

                    if (stringData.ValueLength > 0)
                    {
                        int bytesReadable = Math.Min(stringData.ValueLength * sizeof(ushort), stringData.Length - (offset - dataStartOffset));
                        byte[] valueBytes = data.ReadBytes(ref offset, bytesReadable);
                        stringData.Value = Encoding.Unicode.GetString(valueBytes);
                    }

                    // Align to the DWORD boundary if we're not at the end
                    data.AlignToBoundary(ref offset, 4);

                    stringTableChildren.Add(stringData);
                    if (stringData.Length == 0 && stringData.ValueLength == 0)
                        break;
                }

                stringTable.Children = [.. stringTableChildren];

                stringFileInfoChildren.Add(stringTable);
            }

            stringFileInfo.Children = [.. stringFileInfoChildren];

            return stringFileInfo;
        }

        /// <summary>
        /// Read resource data as a string table resource
        /// </summary>
        /// <param name="entry">Resource data entry to parse into a string table resource</param>
        /// <returns>A filled string table resource on success, null on error</returns>
        /// TODO: Create concrete type for this and inherit from ResourceDataType
        public static Dictionary<int, string?>? AsStringTable(this Models.PortableExecutable.Resource.DataEntry? entry)
        {
            // If we have an invalid entry, just skip
            if (entry?.Data is null)
                return null;

            // Initialize the iterators
            int offset = 0, stringIndex = 0;

            // Create the output table
            var stringTable = new Dictionary<int, string?>();

            // Loop through and add
            while (offset < entry.Data.Length)
            {
                string? stringValue = entry.Data.ReadPrefixedUnicodeString(ref offset);
                if (stringValue is not null)
                {
                    stringValue = stringValue.Replace("\n", "\\n").Replace("\r", newValue: "\\r");
                    stringTable[stringIndex++] = stringValue;
                }
            }

            return stringTable;
        }

        /// <summary>
        ///  Read byte data as a var file info resource
        /// </summary>
        /// <param name="data">Data to parse into a var file info</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>A filled var file info resource on success, null on error</returns>
        public static VarFileInfo? AsVarFileInfo(byte[] data, ref int offset)
        {
            var varFileInfo = new VarFileInfo();

            // Cache the initial offset
            int initialOffset = offset;

            varFileInfo.Length = data.ReadUInt16LittleEndian(ref offset);
            varFileInfo.ValueLength = data.ReadUInt16LittleEndian(ref offset);
            varFileInfo.ResourceType = (VersionResourceType)data.ReadUInt16LittleEndian(ref offset);
            varFileInfo.Key = data.ReadNullTerminatedUnicodeString(ref offset) ?? string.Empty;
            if (varFileInfo.Key != "VarFileInfo")
                return null;

            // Align to the DWORD boundary if we're not at the end
            data.AlignToBoundary(ref offset, 4);

            var varFileInfoChildren = new List<VarData>();
            while ((offset - initialOffset) < varFileInfo.Length)
            {
                var varData = new VarData();

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
                data.AlignToBoundary(ref offset, 4);

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

            varFileInfo.Children = [.. varFileInfoChildren];

            return varFileInfo;
        }

        /// <summary>
        /// Read resource data as a version info resource
        /// </summary>
        /// <param name="entry">Resource data entry to parse into a version info resource</param>
        /// <returns>A filled version info resource on success, null on error</returns>
        public static VersionInfo? AsVersionInfo(this Models.PortableExecutable.Resource.DataEntry? entry)
        {
            // If we have an invalid entry, just skip
            if (entry?.Data is null)
                return null;

            // Initialize the iterator
            int offset = 0;

            // Create the output object
            var versionInfo = new VersionInfo();

            versionInfo.Length = entry.Data.ReadUInt16LittleEndian(ref offset);
            versionInfo.ValueLength = entry.Data.ReadUInt16LittleEndian(ref offset);
            versionInfo.ResourceType = (VersionResourceType)entry.Data.ReadUInt16LittleEndian(ref offset);
            versionInfo.Key = entry.Data.ReadNullTerminatedUnicodeString(ref offset) ?? string.Empty;
            if (versionInfo.Key != "VS_VERSION_INFO")
                return null;

            while (offset < entry.Data.Length && (offset % 4) != 0)
                versionInfo.Padding1 = entry.Data.ReadUInt16LittleEndian(ref offset);

            // Read fixed file info
            if (versionInfo.ValueLength > 0 && offset + versionInfo.ValueLength <= entry.Data.Length)
            {
                var fixedFileInfo = ParseFixedFileInfo(entry.Data, ref offset);
                if (fixedFileInfo?.Signature != 0xFEEF04BD)
                    return null;

                versionInfo.Value = fixedFileInfo;
            }

            while (offset < entry.Data.Length && (offset % 4) != 0)
                versionInfo.Padding2 = entry.Data.ReadUInt16LittleEndian(ref offset);

            // Determine if we have a StringFileInfo or VarFileInfo twice
            ReadInfoSection(entry.Data, ref offset, versionInfo);
            ReadInfoSection(entry.Data, ref offset, versionInfo);

            return versionInfo;
        }

        /// <summary>
        /// Parse a byte array into a FixedFileInfo
        /// </summary>
        /// <param name="data">Data to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>A filled FixedFileInfo on success, null on error</returns>
        public static FixedFileInfo ParseFixedFileInfo(this byte[] data, ref int offset)
        {
            var obj = new FixedFileInfo();

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
        /// Parse a byte array into a ResourceHeader
        /// </summary>
        /// <param name="data">Data to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>A filled ResourceHeader on success, null on error</returns>
        public static Models.PortableExecutable.Resource.ResourceHeader ParseResourceHeader(this byte[] data, ref int offset)
        {
            // Read in the table
            var obj = new Models.PortableExecutable.Resource.ResourceHeader();

            obj.DataSize = data.ReadUInt32LittleEndian(ref offset);
            obj.HeaderSize = data.ReadUInt32LittleEndian(ref offset);
            obj.ResourceType = (ResourceType)data.ReadUInt32LittleEndian(ref offset); // TODO: Could be a string too
            obj.Name = data.ReadUInt32LittleEndian(ref offset); // TODO: Could be a string too
            obj.DataVersion = data.ReadUInt32LittleEndian(ref offset);
            obj.MemoryFlags = (MemoryFlags)data.ReadUInt16LittleEndian(ref offset);
            obj.LanguageId = data.ReadUInt16LittleEndian(ref offset);
            obj.Version = data.ReadUInt32LittleEndian(ref offset);
            obj.Characteristics = data.ReadUInt32LittleEndian(ref offset);

            return obj;
        }

        /// <summary>
        /// Read either a `StringFileInfo` or `VarFileInfo` based on the key
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="offset"></param>
        /// <param name="versionInfo"></param>
        /// <returns></returns>
        private static void ReadInfoSection(byte[] data, ref int offset, VersionInfo versionInfo)
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
                var stringFileInfo = AsStringFileInfo(data, ref offset);
                versionInfo.StringFileInfo = stringFileInfo;
            }
            else if (nextKey == "VarFileInfo")
            {
                var varFileInfo = AsVarFileInfo(data, ref offset);
                versionInfo.VarFileInfo = varFileInfo;
            }
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Align the array position to a byte-size boundary
        /// </summary>
        /// <param name="input">Input array to try aligning</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <param name="alignment">Number of bytes to align on</param>
        /// <returns>True if the array could be aligned, false otherwise</returns>
        /// TODO: Remove when IO is updated
        internal static bool AlignToBoundary(this byte[]? input, ref int offset, byte alignment)
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
