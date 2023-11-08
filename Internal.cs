using System;
using System.Collections.Generic;
using System.Linq;
using SabreTools.Models.Metadata;

namespace SabreTools.Serialization
{
    public static class Internal
    {
        /// <summary>
        /// Extract nested items from a Dump
        /// </summary>
        public static DatItem[]? ExtractItems(Dump? item)
        {
            if (item == null)
                return null;

            var datItems = new List<DatItem>();

            var rom = item.Read<Rom>(Dump.RomKey);
            if (rom != null)
                datItems.Add(rom);

            var megaRom = item.Read<Rom>(Dump.MegaRomKey);
            if (megaRom != null)
                datItems.Add(megaRom);

            var sccPlusCart = item.Read<Rom>(Dump.SCCPlusCartKey);
            if (sccPlusCart != null)
                datItems.Add(sccPlusCart);

            return datItems.ToArray();
        }

        /// <summary>
        /// Extract nested items from a Part
        /// </summary>
        public static DatItem[]? ExtractItems(Part? item)
        {
            if (item == null)
                return null;

            var datItems = new List<DatItem>();

            var features = item.Read<Feature[]>(Part.FeatureKey);
            if (features != null && features.Any())
                datItems.AddRange(features);

            var dataAreas = item.Read<DataArea[]>(Part.DataAreaKey);
            if (dataAreas != null && dataAreas.Any())
            {
                datItems.AddRange(dataAreas
                    .Where(d => d != null)
                    .SelectMany(ExtractItems));
            }

            var diskAreas = item.Read<DiskArea[]>(Part.DiskAreaKey);
            if (diskAreas != null && diskAreas.Any())
            {
                datItems.AddRange(diskAreas
                    .Where(d => d != null)
                    .SelectMany(ExtractItems));
            }

            var dipSwitches = item.Read<DipSwitch[]>(Part.DipSwitchKey);
            if (dipSwitches != null && dipSwitches.Any())
            {
                datItems.AddRange(dipSwitches
                    .Where(d => d != null));
            }

            return datItems.ToArray();
        }

        /// <summary>
        /// Extract nested items from a DataArea
        /// </summary>
        private static Rom[] ExtractItems(DataArea item)
        {
            var roms = item.Read<Rom[]>(DataArea.RomKey);
            if (roms == null || !roms.Any())
                return Array.Empty<Rom>();

            return roms.ToArray();
        }

        /// <summary>
        /// Extract nested items from a DiskArea
        /// </summary>
        private static Disk[] ExtractItems(DiskArea item)
        {
            var roms = item.Read<Disk[]>(DiskArea.DiskKey);
            if (roms == null || !roms.Any())
                return Array.Empty<Disk>();

            return roms.ToArray();
        }
    }
}