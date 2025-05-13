using System;
using SabreTools.Models.SoftwareList;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.CrossModel
{
    public partial class SoftwareList : IModelSerializer<Models.SoftwareList.SoftwareList, Models.Metadata.MetadataFile>
    {
        /// <inheritdoc/>
        public Models.SoftwareList.SoftwareList? Deserialize(Models.Metadata.MetadataFile? obj)
        {
            if (obj == null)
                return null;

            var header = obj.Read<Models.Metadata.Header>(Models.Metadata.MetadataFile.HeaderKey);
            var metadataFile = header != null ? ConvertHeaderFromInternalModel(header) : new Models.SoftwareList.SoftwareList();

            var machines = obj.Read<Models.Metadata.Machine[]>(Models.Metadata.MetadataFile.MachineKey);
            if (machines != null && machines.Length > 0)
                metadataFile.Software = Array.ConvertAll(machines, ConvertMachineFromInternalModel);

            return metadataFile;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Header"/> to <see cref="Models.SoftwareList.SoftwareList"/>
        /// </summary>
        private static Models.SoftwareList.SoftwareList ConvertHeaderFromInternalModel(Models.Metadata.Header item)
        {
            var softwareList = new Models.SoftwareList.SoftwareList
            {
                Name = item.ReadString(Models.Metadata.Header.NameKey),
                Description = item.ReadString(Models.Metadata.Header.DescriptionKey),
                Notes = item.ReadString(Models.Metadata.Header.NotesKey),
            };
            return softwareList;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Machine"/> to <see cref="Models.SoftwareList.Software"/>
        /// </summary>
        private static Software ConvertMachineFromInternalModel(Models.Metadata.Machine item)
        {
            var software = new Software
            {
                Name = item.ReadString(Models.Metadata.Machine.NameKey),
                CloneOf = item.ReadString(Models.Metadata.Machine.CloneOfKey),
                Supported = item.ReadString(Models.Metadata.Machine.SupportedKey),
                Description = item.ReadString(Models.Metadata.Machine.DescriptionKey),
                Year = item.ReadString(Models.Metadata.Machine.YearKey),
                Publisher = item.ReadString(Models.Metadata.Machine.PublisherKey),
                Notes = item.ReadString(Models.Metadata.Machine.NotesKey),
            };

            var infos = item.Read<Models.Metadata.Info[]>(Models.Metadata.Machine.InfoKey);
            if (infos != null && infos.Length > 0)
                software.Info = Array.ConvertAll(infos, ConvertFromInternalModel);

            var sharedFeats = item.Read<Models.Metadata.SharedFeat[]>(Models.Metadata.Machine.SharedFeatKey);
            if (sharedFeats != null && sharedFeats.Length > 0)
                software.SharedFeat = Array.ConvertAll(sharedFeats, ConvertFromInternalModel);

            var parts = item.Read<Models.Metadata.Part[]>(Models.Metadata.Machine.PartKey);
            if (parts != null && parts.Length > 0)
                software.Part = Array.ConvertAll(parts, ConvertFromInternalModel);

            return software;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.DataArea"/> to <see cref="Models.SoftwareList.DataArea"/>
        /// </summary>
        private static DataArea ConvertFromInternalModel(Models.Metadata.DataArea item)
        {
            var dataArea = new DataArea
            {
                Name = item.ReadString(Models.Metadata.DataArea.NameKey),
                Size = item.ReadString(Models.Metadata.DataArea.SizeKey),
                Width = item.ReadString(Models.Metadata.DataArea.WidthKey),
                Endianness = item.ReadString(Models.Metadata.DataArea.EndiannessKey),
            };

            var roms = item.Read<Models.Metadata.Rom[]>(Models.Metadata.DataArea.RomKey);
            if (roms != null && roms.Length > 0)
                dataArea.Rom = Array.ConvertAll(roms,ConvertFromInternalModel);

            return dataArea;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.DipSwitch"/> to <see cref="Models.SoftwareList.DipSwitch"/>
        /// </summary>
        private static DipSwitch ConvertFromInternalModel(Models.Metadata.DipSwitch item)
        {
            var dipSwitch = new DipSwitch
            {
                Name = item.ReadString(Models.Metadata.DipSwitch.NameKey),
                Tag = item.ReadString(Models.Metadata.DipSwitch.TagKey),
                Mask = item.ReadString(Models.Metadata.DipSwitch.MaskKey),
            };

            var dipValues = item.Read<Models.Metadata.DipValue[]>(Models.Metadata.DipSwitch.DipValueKey);
            if (dipValues != null && dipValues.Length > 0)
                dipSwitch.DipValue = Array.ConvertAll(dipValues, ConvertFromInternalModel);

            return dipSwitch;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.DipValue"/> to <see cref="Models.SoftwareList.DipValue"/>
        /// </summary>
        private static DipValue ConvertFromInternalModel(Models.Metadata.DipValue item)
        {
            var dipValue = new DipValue
            {
                Name = item.ReadString(Models.Metadata.DipValue.NameKey),
                Value = item.ReadString(Models.Metadata.DipValue.ValueKey),
                Default = item.ReadString(Models.Metadata.DipValue.DefaultKey),
            };
            return dipValue;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Disk"/> to <see cref="Models.SoftwareList.Disk"/>
        /// </summary>
        private static Disk ConvertFromInternalModel(Models.Metadata.Disk item)
        {
            var disk = new Disk
            {
                Name = item.ReadString(Models.Metadata.Disk.NameKey),
                MD5 = item.ReadString(Models.Metadata.Disk.MD5Key),
                SHA1 = item.ReadString(Models.Metadata.Disk.SHA1Key),
                Status = item.ReadString(Models.Metadata.Disk.StatusKey),
                Writeable = item.ReadString(Models.Metadata.Disk.WritableKey),
            };
            return disk;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.DiskArea"/> to <see cref="Models.SoftwareList.DiskArea"/>
        /// </summary>
        private static DiskArea ConvertFromInternalModel(Models.Metadata.DiskArea item)
        {
            var diskArea = new DiskArea
            {
                Name = item.ReadString(Models.Metadata.DiskArea.NameKey),
            };

            var disks = item.Read<Models.Metadata.Disk[]>(Models.Metadata.DiskArea.DiskKey);
            if (disks != null && disks.Length > 0)
                diskArea.Disk = Array.ConvertAll(disks, ConvertFromInternalModel);

            return diskArea;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Feature"/> to <see cref="Models.SoftwareList.Feature"/>
        /// </summary>
        private static Feature ConvertFromInternalModel(Models.Metadata.Feature item)
        {
            var feature = new Feature
            {
                Name = item.ReadString(Models.Metadata.Feature.NameKey),
                Value = item.ReadString(Models.Metadata.Feature.ValueKey),
            };
            return feature;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Info"/> to <see cref="Models.SoftwareList.Info"/>
        /// </summary>
        private static Info ConvertFromInternalModel(Models.Metadata.Info item)
        {
            var info = new Info
            {
                Name = item.ReadString(Models.Metadata.Info.NameKey),
                Value = item.ReadString(Models.Metadata.Info.ValueKey),
            };
            return info;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Part"/> to <see cref="Models.SoftwareList.Part"/>
        /// </summary>
        private static Part ConvertFromInternalModel(Models.Metadata.Part item)
        {
            var part = new Part
            {
                Name = item.ReadString(Models.Metadata.Part.NameKey),
                Interface = item.ReadString(Models.Metadata.Part.InterfaceKey),
            };

            var features = item.Read<Models.Metadata.Feature[]>(Models.Metadata.Part.FeatureKey);
            if (features != null && features.Length > 0)
                part.Feature = Array.ConvertAll(features, ConvertFromInternalModel);

            var dataAreas = item.Read<Models.Metadata.DataArea[]>(Models.Metadata.Part.DataAreaKey);
            if (dataAreas != null && dataAreas.Length > 0)
                part.DataArea = Array.ConvertAll(dataAreas, ConvertFromInternalModel);

            var diskAreas = item.Read<Models.Metadata.DiskArea[]>(Models.Metadata.Part.DiskAreaKey);
            if (diskAreas != null && diskAreas.Length > 0)
                part.DiskArea = Array.ConvertAll(diskAreas, ConvertFromInternalModel);

            var dipSwitches = item.Read<Models.Metadata.DipSwitch[]>(Models.Metadata.Part.DipSwitchKey);
            if (dipSwitches != null && dipSwitches.Length > 0)
                part.DipSwitch = Array.ConvertAll(dipSwitches, ConvertFromInternalModel);

            return part;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Rom"/> to <see cref="Models.SoftwareList.Rom"/>
        /// </summary>
        private static Rom ConvertFromInternalModel(Models.Metadata.Rom item)
        {
            var rom = new Rom
            {
                Name = item.ReadString(Models.Metadata.Rom.NameKey),
                Size = item.ReadString(Models.Metadata.Rom.SizeKey),
                Length = item.ReadString(Models.Metadata.Rom.LengthKey),
                CRC = item.ReadString(Models.Metadata.Rom.CRCKey),
                SHA1 = item.ReadString(Models.Metadata.Rom.SHA1Key),
                Offset = item.ReadString(Models.Metadata.Rom.OffsetKey),
                Value = item.ReadString(Models.Metadata.Rom.ValueKey),
                Status = item.ReadString(Models.Metadata.Rom.StatusKey),
                LoadFlag = item.ReadString(Models.Metadata.Rom.LoadFlagKey),
            };
            return rom;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.SharedFeat"/> to <see cref="Models.SoftwareList.SharedFeat"/>
        /// </summary>
        private static SharedFeat ConvertFromInternalModel(Models.Metadata.SharedFeat item)
        {
            var sharedFeat = new SharedFeat
            {
                Name = item.ReadString(Models.Metadata.SharedFeat.NameKey),
                Value = item.ReadString(Models.Metadata.SharedFeat.ValueKey),
            };
            return sharedFeat;
        }
    }
}