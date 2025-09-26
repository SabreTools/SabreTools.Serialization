using System;
using SabreTools.Serialization.Interfaces;
using SabreTools.Serialization.Models.SoftwareList;

namespace SabreTools.Serialization.CrossModel
{
    public partial class SoftwareList : IModelSerializer<SabreTools.Serialization.Models.SoftwareList.SoftwareList, Serialization.Models.Metadata.MetadataFile>
    {
        /// <inheritdoc/>
        public SabreTools.Serialization.Models.SoftwareList.SoftwareList? Deserialize(Serialization.Models.Metadata.MetadataFile? obj)
        {
            if (obj == null)
                return null;

            var header = obj.Read<Serialization.Models.Metadata.Header>(Serialization.Models.Metadata.MetadataFile.HeaderKey);
            var metadataFile = header != null ? ConvertHeaderFromInternalModel(header) : new SabreTools.Serialization.Models.SoftwareList.SoftwareList();

            var machines = obj.Read<Serialization.Models.Metadata.Machine[]>(Serialization.Models.Metadata.MetadataFile.MachineKey);
            if (machines != null && machines.Length > 0)
                metadataFile.Software = Array.ConvertAll(machines, ConvertMachineFromInternalModel);

            return metadataFile;
        }

        /// <summary>
        /// Convert from <see cref="Serialization.Models.Metadata.Header"/> to <see cref="SabreTools.Serialization.Models.SoftwareList.SoftwareList"/>
        /// </summary>
        private static SabreTools.Serialization.Models.SoftwareList.SoftwareList ConvertHeaderFromInternalModel(Serialization.Models.Metadata.Header item)
        {
            var softwareList = new SabreTools.Serialization.Models.SoftwareList.SoftwareList
            {
                Name = item.ReadString(Serialization.Models.Metadata.Header.NameKey),
                Description = item.ReadString(Serialization.Models.Metadata.Header.DescriptionKey),
                Notes = item.ReadString(Serialization.Models.Metadata.Header.NotesKey),
            };
            return softwareList;
        }

        /// <summary>
        /// Convert from <see cref="Serialization.Models.Metadata.Machine"/> to <see cref="SSoftware"/>
        /// </summary>
        private static Software ConvertMachineFromInternalModel(Serialization.Models.Metadata.Machine item)
        {
            var software = new Software
            {
                Name = item.ReadString(Serialization.Models.Metadata.Machine.NameKey),
                CloneOf = item.ReadString(Serialization.Models.Metadata.Machine.CloneOfKey),
                Supported = item.ReadString(Serialization.Models.Metadata.Machine.SupportedKey),
                Description = item.ReadString(Serialization.Models.Metadata.Machine.DescriptionKey),
                Year = item.ReadString(Serialization.Models.Metadata.Machine.YearKey),
                Publisher = item.ReadString(Serialization.Models.Metadata.Machine.PublisherKey),
                Notes = item.ReadString(Serialization.Models.Metadata.Machine.NotesKey),
            };

            var infos = item.Read<Serialization.Models.Metadata.Info[]>(Serialization.Models.Metadata.Machine.InfoKey);
            if (infos != null && infos.Length > 0)
                software.Info = Array.ConvertAll(infos, ConvertFromInternalModel);

            var sharedFeats = item.Read<Serialization.Models.Metadata.SharedFeat[]>(Serialization.Models.Metadata.Machine.SharedFeatKey);
            if (sharedFeats != null && sharedFeats.Length > 0)
                software.SharedFeat = Array.ConvertAll(sharedFeats, ConvertFromInternalModel);

            var parts = item.Read<Serialization.Models.Metadata.Part[]>(Serialization.Models.Metadata.Machine.PartKey);
            if (parts != null && parts.Length > 0)
                software.Part = Array.ConvertAll(parts, ConvertFromInternalModel);

            return software;
        }

        /// <summary>
        /// Convert from <see cref="Serialization.Models.Metadata.DataArea"/> to <see cref="SDataArea"/>
        /// </summary>
        private static DataArea ConvertFromInternalModel(Serialization.Models.Metadata.DataArea item)
        {
            var dataArea = new DataArea
            {
                Name = item.ReadString(Serialization.Models.Metadata.DataArea.NameKey),
                Size = item.ReadString(Serialization.Models.Metadata.DataArea.SizeKey),
                Width = item.ReadString(Serialization.Models.Metadata.DataArea.WidthKey),
                Endianness = item.ReadString(Serialization.Models.Metadata.DataArea.EndiannessKey),
            };

            var roms = item.Read<Serialization.Models.Metadata.Rom[]>(Serialization.Models.Metadata.DataArea.RomKey);
            if (roms != null && roms.Length > 0)
                dataArea.Rom = Array.ConvertAll(roms, ConvertFromInternalModel);

            return dataArea;
        }

        /// <summary>
        /// Convert from <see cref="Serialization.Models.Metadata.DipSwitch"/> to <see cref="SDipSwitch"/>
        /// </summary>
        private static DipSwitch ConvertFromInternalModel(Serialization.Models.Metadata.DipSwitch item)
        {
            var dipSwitch = new DipSwitch
            {
                Name = item.ReadString(Serialization.Models.Metadata.DipSwitch.NameKey),
                Tag = item.ReadString(Serialization.Models.Metadata.DipSwitch.TagKey),
                Mask = item.ReadString(Serialization.Models.Metadata.DipSwitch.MaskKey),
            };

            var dipValues = item.Read<Serialization.Models.Metadata.DipValue[]>(Serialization.Models.Metadata.DipSwitch.DipValueKey);
            if (dipValues != null && dipValues.Length > 0)
                dipSwitch.DipValue = Array.ConvertAll(dipValues, ConvertFromInternalModel);

            return dipSwitch;
        }

        /// <summary>
        /// Convert from <see cref="Serialization.Models.Metadata.DipValue"/> to <see cref="SDipValue"/>
        /// </summary>
        private static DipValue ConvertFromInternalModel(Serialization.Models.Metadata.DipValue item)
        {
            var dipValue = new DipValue
            {
                Name = item.ReadString(Serialization.Models.Metadata.DipValue.NameKey),
                Value = item.ReadString(Serialization.Models.Metadata.DipValue.ValueKey),
                Default = item.ReadString(Serialization.Models.Metadata.DipValue.DefaultKey),
            };
            return dipValue;
        }

        /// <summary>
        /// Convert from <see cref="Serialization.Models.Metadata.Disk"/> to <see cref="SDisk"/>
        /// </summary>
        private static Disk ConvertFromInternalModel(Serialization.Models.Metadata.Disk item)
        {
            var disk = new Disk
            {
                Name = item.ReadString(Serialization.Models.Metadata.Disk.NameKey),
                MD5 = item.ReadString(Serialization.Models.Metadata.Disk.MD5Key),
                SHA1 = item.ReadString(Serialization.Models.Metadata.Disk.SHA1Key),
                Status = item.ReadString(Serialization.Models.Metadata.Disk.StatusKey),
                Writeable = item.ReadString(Serialization.Models.Metadata.Disk.WritableKey),
            };
            return disk;
        }

        /// <summary>
        /// Convert from <see cref="Serialization.Models.Metadata.DiskArea"/> to <see cref="SDiskArea"/>
        /// </summary>
        private static DiskArea ConvertFromInternalModel(Serialization.Models.Metadata.DiskArea item)
        {
            var diskArea = new DiskArea
            {
                Name = item.ReadString(Serialization.Models.Metadata.DiskArea.NameKey),
            };

            var disks = item.Read<Serialization.Models.Metadata.Disk[]>(Serialization.Models.Metadata.DiskArea.DiskKey);
            if (disks != null && disks.Length > 0)
                diskArea.Disk = Array.ConvertAll(disks, ConvertFromInternalModel);

            return diskArea;
        }

        /// <summary>
        /// Convert from <see cref="Serialization.Models.Metadata.Feature"/> to <see cref="SFeature"/>
        /// </summary>
        private static Feature ConvertFromInternalModel(Serialization.Models.Metadata.Feature item)
        {
            var feature = new Feature
            {
                Name = item.ReadString(Serialization.Models.Metadata.Feature.NameKey),
                Value = item.ReadString(Serialization.Models.Metadata.Feature.ValueKey),
            };
            return feature;
        }

        /// <summary>
        /// Convert from <see cref="Serialization.Models.Metadata.Info"/> to <see cref="SInfo"/>
        /// </summary>
        private static Info ConvertFromInternalModel(Serialization.Models.Metadata.Info item)
        {
            var info = new Info
            {
                Name = item.ReadString(Serialization.Models.Metadata.Info.NameKey),
                Value = item.ReadString(Serialization.Models.Metadata.Info.ValueKey),
            };
            return info;
        }

        /// <summary>
        /// Convert from <see cref="Serialization.Models.Metadata.Part"/> to <see cref="SPart"/>
        /// </summary>
        private static Part ConvertFromInternalModel(Serialization.Models.Metadata.Part item)
        {
            var part = new Part
            {
                Name = item.ReadString(Serialization.Models.Metadata.Part.NameKey),
                Interface = item.ReadString(Serialization.Models.Metadata.Part.InterfaceKey),
            };

            var features = item.Read<Serialization.Models.Metadata.Feature[]>(Serialization.Models.Metadata.Part.FeatureKey);
            if (features != null && features.Length > 0)
                part.Feature = Array.ConvertAll(features, ConvertFromInternalModel);

            var dataAreas = item.Read<Serialization.Models.Metadata.DataArea[]>(Serialization.Models.Metadata.Part.DataAreaKey);
            if (dataAreas != null && dataAreas.Length > 0)
                part.DataArea = Array.ConvertAll(dataAreas, ConvertFromInternalModel);

            var diskAreas = item.Read<Serialization.Models.Metadata.DiskArea[]>(Serialization.Models.Metadata.Part.DiskAreaKey);
            if (diskAreas != null && diskAreas.Length > 0)
                part.DiskArea = Array.ConvertAll(diskAreas, ConvertFromInternalModel);

            var dipSwitches = item.Read<Serialization.Models.Metadata.DipSwitch[]>(Serialization.Models.Metadata.Part.DipSwitchKey);
            if (dipSwitches != null && dipSwitches.Length > 0)
                part.DipSwitch = Array.ConvertAll(dipSwitches, ConvertFromInternalModel);

            return part;
        }

        /// <summary>
        /// Convert from <see cref="Serialization.Models.Metadata.Rom"/> to <see cref="SRom"/>
        /// </summary>
        private static Rom ConvertFromInternalModel(Serialization.Models.Metadata.Rom item)
        {
            var rom = new Rom
            {
                Name = item.ReadString(Serialization.Models.Metadata.Rom.NameKey),
                Size = item.ReadString(Serialization.Models.Metadata.Rom.SizeKey),
                Length = item.ReadString(Serialization.Models.Metadata.Rom.LengthKey),
                CRC = item.ReadString(Serialization.Models.Metadata.Rom.CRCKey),
                SHA1 = item.ReadString(Serialization.Models.Metadata.Rom.SHA1Key),
                Offset = item.ReadString(Serialization.Models.Metadata.Rom.OffsetKey),
                Value = item.ReadString(Serialization.Models.Metadata.Rom.ValueKey),
                Status = item.ReadString(Serialization.Models.Metadata.Rom.StatusKey),
                LoadFlag = item.ReadString(Serialization.Models.Metadata.Rom.LoadFlagKey),
            };
            return rom;
        }

        /// <summary>
        /// Convert from <see cref="Serialization.Models.Metadata.SharedFeat"/> to <see cref="SSharedFeat"/>
        /// </summary>
        private static SharedFeat ConvertFromInternalModel(Serialization.Models.Metadata.SharedFeat item)
        {
            var sharedFeat = new SharedFeat
            {
                Name = item.ReadString(Serialization.Models.Metadata.SharedFeat.NameKey),
                Value = item.ReadString(Serialization.Models.Metadata.SharedFeat.ValueKey),
            };
            return sharedFeat;
        }
    }
}
