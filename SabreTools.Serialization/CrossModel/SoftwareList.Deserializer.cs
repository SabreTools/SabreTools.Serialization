using System;
using SabreTools.Data.Models.SoftwareList;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.CrossModel
{
    public partial class SoftwareList : IModelSerializer<Data.Models.SoftwareList.SoftwareList, Data.Models.Metadata.MetadataFile>
    {
        /// <inheritdoc/>
        public Data.Models.SoftwareList.SoftwareList? Deserialize(Data.Models.Metadata.MetadataFile? obj)
        {
            if (obj == null)
                return null;

            var header = obj.Read<Data.Models.Metadata.Header>(Data.Models.Metadata.MetadataFile.HeaderKey);
            var metadataFile = header != null ? ConvertHeaderFromInternalModel(header) : new Data.Models.SoftwareList.SoftwareList();

            var machines = obj.Read<Data.Models.Metadata.Machine[]>(Data.Models.Metadata.MetadataFile.MachineKey);
            if (machines != null && machines.Length > 0)
                metadataFile.Software = Array.ConvertAll(machines, ConvertMachineFromInternalModel);

            return metadataFile;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Header"/> to <see cref="Models.SoftwareList.SoftwareList"/>
        /// </summary>
        private static Data.Models.SoftwareList.SoftwareList ConvertHeaderFromInternalModel(Data.Models.Metadata.Header item)
        {
            var softwareList = new Data.Models.SoftwareList.SoftwareList
            {
                Name = item.ReadString(Data.Models.Metadata.Header.NameKey),
                Description = item.ReadString(Data.Models.Metadata.Header.DescriptionKey),
                Notes = item.ReadString(Data.Models.Metadata.Header.NotesKey),
            };
            return softwareList;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Machine"/> to <see cref="SSoftware"/>
        /// </summary>
        private static Software ConvertMachineFromInternalModel(Data.Models.Metadata.Machine item)
        {
            var software = new Software
            {
                Name = item.ReadString(Data.Models.Metadata.Machine.NameKey),
                CloneOf = item.ReadString(Data.Models.Metadata.Machine.CloneOfKey),
                Supported = item.ReadString(Data.Models.Metadata.Machine.SupportedKey),
                Description = item.ReadString(Data.Models.Metadata.Machine.DescriptionKey),
                Year = item.ReadString(Data.Models.Metadata.Machine.YearKey),
                Publisher = item.ReadString(Data.Models.Metadata.Machine.PublisherKey),
                Notes = item.ReadString(Data.Models.Metadata.Machine.NotesKey),
            };

            var infos = item.Read<Data.Models.Metadata.Info[]>(Data.Models.Metadata.Machine.InfoKey);
            if (infos != null && infos.Length > 0)
                software.Info = Array.ConvertAll(infos, ConvertFromInternalModel);

            var sharedFeats = item.Read<Data.Models.Metadata.SharedFeat[]>(Data.Models.Metadata.Machine.SharedFeatKey);
            if (sharedFeats != null && sharedFeats.Length > 0)
                software.SharedFeat = Array.ConvertAll(sharedFeats, ConvertFromInternalModel);

            var parts = item.Read<Data.Models.Metadata.Part[]>(Data.Models.Metadata.Machine.PartKey);
            if (parts != null && parts.Length > 0)
                software.Part = Array.ConvertAll(parts, ConvertFromInternalModel);

            return software;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.DataArea"/> to <see cref="SDataArea"/>
        /// </summary>
        private static DataArea ConvertFromInternalModel(Data.Models.Metadata.DataArea item)
        {
            var dataArea = new DataArea
            {
                Name = item.ReadString(Data.Models.Metadata.DataArea.NameKey),
                Size = item.ReadString(Data.Models.Metadata.DataArea.SizeKey),
                Width = item.ReadString(Data.Models.Metadata.DataArea.WidthKey),
                Endianness = item.ReadString(Data.Models.Metadata.DataArea.EndiannessKey),
            };

            var roms = item.Read<Data.Models.Metadata.Rom[]>(Data.Models.Metadata.DataArea.RomKey);
            if (roms != null && roms.Length > 0)
                dataArea.Rom = Array.ConvertAll(roms, ConvertFromInternalModel);

            return dataArea;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.DipSwitch"/> to <see cref="SDipSwitch"/>
        /// </summary>
        private static DipSwitch ConvertFromInternalModel(Data.Models.Metadata.DipSwitch item)
        {
            var dipSwitch = new DipSwitch
            {
                Name = item.ReadString(Data.Models.Metadata.DipSwitch.NameKey),
                Tag = item.ReadString(Data.Models.Metadata.DipSwitch.TagKey),
                Mask = item.ReadString(Data.Models.Metadata.DipSwitch.MaskKey),
            };

            var dipValues = item.Read<Data.Models.Metadata.DipValue[]>(Data.Models.Metadata.DipSwitch.DipValueKey);
            if (dipValues != null && dipValues.Length > 0)
                dipSwitch.DipValue = Array.ConvertAll(dipValues, ConvertFromInternalModel);

            return dipSwitch;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.DipValue"/> to <see cref="SDipValue"/>
        /// </summary>
        private static DipValue ConvertFromInternalModel(Data.Models.Metadata.DipValue item)
        {
            var dipValue = new DipValue
            {
                Name = item.ReadString(Data.Models.Metadata.DipValue.NameKey),
                Value = item.ReadString(Data.Models.Metadata.DipValue.ValueKey),
                Default = item.ReadString(Data.Models.Metadata.DipValue.DefaultKey),
            };
            return dipValue;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Disk"/> to <see cref="SDisk"/>
        /// </summary>
        private static Disk ConvertFromInternalModel(Data.Models.Metadata.Disk item)
        {
            var disk = new Disk
            {
                Name = item.ReadString(Data.Models.Metadata.Disk.NameKey),
                MD5 = item.ReadString(Data.Models.Metadata.Disk.MD5Key),
                SHA1 = item.ReadString(Data.Models.Metadata.Disk.SHA1Key),
                Status = item.ReadString(Data.Models.Metadata.Disk.StatusKey),
                Writeable = item.ReadString(Data.Models.Metadata.Disk.WritableKey),
            };
            return disk;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.DiskArea"/> to <see cref="SDiskArea"/>
        /// </summary>
        private static DiskArea ConvertFromInternalModel(Data.Models.Metadata.DiskArea item)
        {
            var diskArea = new DiskArea
            {
                Name = item.ReadString(Data.Models.Metadata.DiskArea.NameKey),
            };

            var disks = item.Read<Data.Models.Metadata.Disk[]>(Data.Models.Metadata.DiskArea.DiskKey);
            if (disks != null && disks.Length > 0)
                diskArea.Disk = Array.ConvertAll(disks, ConvertFromInternalModel);

            return diskArea;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Feature"/> to <see cref="SFeature"/>
        /// </summary>
        private static Feature ConvertFromInternalModel(Data.Models.Metadata.Feature item)
        {
            var feature = new Feature
            {
                Name = item.ReadString(Data.Models.Metadata.Feature.NameKey),
                Value = item.ReadString(Data.Models.Metadata.Feature.ValueKey),
            };
            return feature;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Info"/> to <see cref="SInfo"/>
        /// </summary>
        private static Info ConvertFromInternalModel(Data.Models.Metadata.Info item)
        {
            var info = new Info
            {
                Name = item.ReadString(Data.Models.Metadata.Info.NameKey),
                Value = item.ReadString(Data.Models.Metadata.Info.ValueKey),
            };
            return info;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Part"/> to <see cref="SPart"/>
        /// </summary>
        private static Part ConvertFromInternalModel(Data.Models.Metadata.Part item)
        {
            var part = new Part
            {
                Name = item.ReadString(Data.Models.Metadata.Part.NameKey),
                Interface = item.ReadString(Data.Models.Metadata.Part.InterfaceKey),
            };

            var features = item.Read<Data.Models.Metadata.Feature[]>(Data.Models.Metadata.Part.FeatureKey);
            if (features != null && features.Length > 0)
                part.Feature = Array.ConvertAll(features, ConvertFromInternalModel);

            var dataAreas = item.Read<Data.Models.Metadata.DataArea[]>(Data.Models.Metadata.Part.DataAreaKey);
            if (dataAreas != null && dataAreas.Length > 0)
                part.DataArea = Array.ConvertAll(dataAreas, ConvertFromInternalModel);

            var diskAreas = item.Read<Data.Models.Metadata.DiskArea[]>(Data.Models.Metadata.Part.DiskAreaKey);
            if (diskAreas != null && diskAreas.Length > 0)
                part.DiskArea = Array.ConvertAll(diskAreas, ConvertFromInternalModel);

            var dipSwitches = item.Read<Data.Models.Metadata.DipSwitch[]>(Data.Models.Metadata.Part.DipSwitchKey);
            if (dipSwitches != null && dipSwitches.Length > 0)
                part.DipSwitch = Array.ConvertAll(dipSwitches, ConvertFromInternalModel);

            return part;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Rom"/> to <see cref="SRom"/>
        /// </summary>
        private static Rom ConvertFromInternalModel(Data.Models.Metadata.Rom item)
        {
            var rom = new Rom
            {
                Name = item.ReadString(Data.Models.Metadata.Rom.NameKey),
                Size = item.ReadString(Data.Models.Metadata.Rom.SizeKey),
                Length = item.ReadString(Data.Models.Metadata.Rom.LengthKey),
                CRC = item.ReadString(Data.Models.Metadata.Rom.CRCKey),
                SHA1 = item.ReadString(Data.Models.Metadata.Rom.SHA1Key),
                Offset = item.ReadString(Data.Models.Metadata.Rom.OffsetKey),
                Value = item.ReadString(Data.Models.Metadata.Rom.ValueKey),
                Status = item.ReadString(Data.Models.Metadata.Rom.StatusKey),
                LoadFlag = item.ReadString(Data.Models.Metadata.Rom.LoadFlagKey),
            };
            return rom;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.SharedFeat"/> to <see cref="SSharedFeat"/>
        /// </summary>
        private static SharedFeat ConvertFromInternalModel(Data.Models.Metadata.SharedFeat item)
        {
            var sharedFeat = new SharedFeat
            {
                Name = item.ReadString(Data.Models.Metadata.SharedFeat.NameKey),
                Value = item.ReadString(Data.Models.Metadata.SharedFeat.ValueKey),
            };
            return sharedFeat;
        }
    }
}
