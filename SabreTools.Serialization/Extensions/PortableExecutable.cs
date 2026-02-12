using System;
using System.Collections.Generic;
using SabreTools.Data.Models.COFF;
using SabreTools.Data.Models.PortableExecutable;
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

        #endregion
    }
}
