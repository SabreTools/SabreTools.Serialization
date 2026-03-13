using System;
using SabreTools.Data.Models.XboxExecutable;

namespace SabreTools.Data.Extensions
{
    public static class XboxExecutable
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
                return rva - matchingSection.VirtualAddress + matchingSection.RawAddress;

            // Loop through all of the sections
            uint maxVirtualAddress = 0, maxRawPointer = 0;
            for (int i = 0; i < sections.Length; i++)
            {
                // If the section "starts" at 0, just skip it
                var section = sections[i];
                if (section.RawAddress == 0)
                    continue;

                // If the virtual address is greater than the RVA
                if (rva < section.VirtualAddress)
                    continue;

                // Cache the maximum matching section data, in case of a miss
                if (rva >= section.VirtualAddress)
                {
                    maxVirtualAddress = section.VirtualAddress;
                    maxRawPointer = section.RawAddress;
                }

                // Attempt to derive the physical address from the current section
                if (section.VirtualSize != 0 && rva <= section.VirtualAddress + section.VirtualSize)
                    return rva - section.VirtualAddress + section.RawAddress;
                else if (section.RawSize != 0 && rva <= section.VirtualAddress + section.RawSize)
                    return rva - section.VirtualAddress + section.RawAddress;
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
                if (section.RawAddress == 0)
                    continue;

                // If the virtual address is greater than the RVA
                if (rva < section.VirtualAddress)
                    continue;

                // Attempt to derive the physical address from the current section
                if (section.VirtualSize != 0 && rva <= section.VirtualAddress + section.VirtualSize)
                    return i;
                else if (section.RawSize != 0 && rva <= section.VirtualAddress + section.RawSize)
                    return i;
            }

            return -1;
        }
    }
}
