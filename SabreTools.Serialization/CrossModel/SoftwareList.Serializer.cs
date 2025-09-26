using System;
using SabreTools.Serialization.Interfaces;
using SabreTools.Serialization.Models.SoftwareList;

namespace SabreTools.Serialization.CrossModel
{
    public partial class SoftwareList : IModelSerializer<SabreTools.Serialization.Models.SoftwareList.SoftwareList, Serialization.Models.Metadata.MetadataFile>
    {
        /// <inheritdoc/>
        public Serialization.Models.Metadata.MetadataFile? Serialize(SabreTools.Serialization.Models.SoftwareList.SoftwareList? item)
        {
            if (item == null)
                return null;

            var metadataFile = new Serialization.Models.Metadata.MetadataFile
            {
                [Serialization.Models.Metadata.MetadataFile.HeaderKey] = ConvertHeaderToInternalModel(item),
            };

            if (item?.Software != null && item.Software.Length > 0)
            {
                metadataFile[Serialization.Models.Metadata.MetadataFile.MachineKey]
                    = Array.ConvertAll(item.Software, ConvertMachineToInternalModel);
            }

            return metadataFile;
        }

        /// <summary>
        /// Convert from <see cref="SabreTools.Serialization.Models.SoftwareList.SoftwareList"/> to <see cref="Serialization.Models.Metadata.Header"/>
        /// </summary>
        private static Serialization.Models.Metadata.Header ConvertHeaderToInternalModel(SabreTools.Serialization.Models.SoftwareList.SoftwareList item)
        {
            var header = new Serialization.Models.Metadata.Header
            {
                [Serialization.Models.Metadata.Header.NameKey] = item.Name,
                [Serialization.Models.Metadata.Header.DescriptionKey] = item.Description,
                [Serialization.Models.Metadata.Header.NotesKey] = item.Notes,
            };
            return header;
        }

        /// <summary>
        /// Convert from <see cref="SabreTools.Serialization.Models.SoftwareList.Software"/> to <see cref="Serialization.Models.Metadata.Machine"/>
        /// </summary>
        private static Serialization.Models.Metadata.Machine ConvertMachineToInternalModel(Software item)
        {
            var machine = new Serialization.Models.Metadata.Machine
            {
                [Serialization.Models.Metadata.Machine.NameKey] = item.Name,
                [Serialization.Models.Metadata.Machine.CloneOfKey] = item.CloneOf,
                [Serialization.Models.Metadata.Machine.SupportedKey] = item.Supported,
                [Serialization.Models.Metadata.Machine.DescriptionKey] = item.Description,
                [Serialization.Models.Metadata.Machine.YearKey] = item.Year,
                [Serialization.Models.Metadata.Machine.PublisherKey] = item.Publisher,
                [Serialization.Models.Metadata.Machine.NotesKey] = item.Notes,
            };

            if (item.Info != null && item.Info.Length > 0)
                machine[Serialization.Models.Metadata.Machine.InfoKey] = Array.ConvertAll(item.Info, ConvertToInternalModel);

            if (item.SharedFeat != null && item.SharedFeat.Length > 0)
                machine[Serialization.Models.Metadata.Machine.SharedFeatKey] = Array.ConvertAll(item.SharedFeat, ConvertToInternalModel);

            if (item.Part != null && item.Part.Length > 0)
                machine[Serialization.Models.Metadata.Machine.PartKey] = Array.ConvertAll(item.Part, ConvertToInternalModel);

            return machine;
        }

        /// <summary>
        /// Convert from <see cref="DataArea"/> to <see cref="Serialization.Models.Metadata.DataArea"/>
        /// </summary>
        private static Serialization.Models.Metadata.DataArea ConvertToInternalModel(DataArea item)
        {
            var dataArea = new Serialization.Models.Metadata.DataArea
            {
                [Serialization.Models.Metadata.DataArea.NameKey] = item.Name,
                [Serialization.Models.Metadata.DataArea.SizeKey] = item.Size,
                [Serialization.Models.Metadata.DataArea.WidthKey] = item.Width,
                [Serialization.Models.Metadata.DataArea.EndiannessKey] = item.Endianness,
            };

            if (item.Rom != null && item.Rom.Length > 0)
                dataArea[Serialization.Models.Metadata.DataArea.RomKey] = Array.ConvertAll(item.Rom, ConvertToInternalModel);

            return dataArea;
        }

        /// <summary>
        /// Convert from <see cref="DipSwitch"/> to <see cref="Serialization.Models.Metadata.DipSwitch"/>
        /// </summary>
        private static Serialization.Models.Metadata.DipSwitch ConvertToInternalModel(DipSwitch item)
        {
            var dipSwitch = new Serialization.Models.Metadata.DipSwitch
            {
                [Serialization.Models.Metadata.DipSwitch.NameKey] = item.Name,
                [Serialization.Models.Metadata.DipSwitch.TagKey] = item.Tag,
                [Serialization.Models.Metadata.DipSwitch.MaskKey] = item.Mask,
            };

            if (item.DipValue != null && item.DipValue.Length > 0)
                dipSwitch[Serialization.Models.Metadata.DipSwitch.DipValueKey] = Array.ConvertAll(item.DipValue, ConvertToInternalModel);

            return dipSwitch;
        }

        /// <summary>
        /// Convert from <see cref="DipValue"/> to <see cref="Serialization.Models.Metadata.DipValue"/>
        /// </summary>
        private static Serialization.Models.Metadata.DipValue ConvertToInternalModel(DipValue item)
        {
            var dipValue = new Serialization.Models.Metadata.DipValue
            {
                [Serialization.Models.Metadata.DipValue.NameKey] = item.Name,
                [Serialization.Models.Metadata.DipValue.ValueKey] = item.Value,
                [Serialization.Models.Metadata.DipValue.DefaultKey] = item.Default,
            };
            return dipValue;
        }

        /// <summary>
        /// Convert from <see cref="Disk"/> to <see cref="Serialization.Models.Metadata.Disk"/>
        /// </summary>
        private static Serialization.Models.Metadata.Disk ConvertToInternalModel(Disk item)
        {
            var disk = new Serialization.Models.Metadata.Disk
            {
                [Serialization.Models.Metadata.Disk.NameKey] = item.Name,
                [Serialization.Models.Metadata.Disk.MD5Key] = item.MD5,
                [Serialization.Models.Metadata.Disk.SHA1Key] = item.SHA1,
                [Serialization.Models.Metadata.Disk.StatusKey] = item.Status,
                [Serialization.Models.Metadata.Disk.WritableKey] = item.Writeable,
            };
            return disk;
        }

        /// <summary>
        /// Convert from <see cref="DiskArea"/> to <see cref="Serialization.Models.Metadata.DiskArea"/>
        /// </summary>
        private static Serialization.Models.Metadata.DiskArea ConvertToInternalModel(DiskArea item)
        {
            var diskArea = new Serialization.Models.Metadata.DiskArea
            {
                [Serialization.Models.Metadata.DiskArea.NameKey] = item.Name,
            };

            if (item.Disk != null && item.Disk.Length > 0)
                diskArea[Serialization.Models.Metadata.DiskArea.DiskKey] = Array.ConvertAll(item.Disk, ConvertToInternalModel);

            return diskArea;
        }

        /// <summary>
        /// Convert from <see cref="Feature"/> to <see cref="Serialization.Models.Metadata.Feature"/>
        /// </summary>
        private static Serialization.Models.Metadata.Feature ConvertToInternalModel(Feature item)
        {
            var feature = new Serialization.Models.Metadata.Feature
            {
                [Serialization.Models.Metadata.Feature.NameKey] = item.Name,
                [Serialization.Models.Metadata.Feature.ValueKey] = item.Value,
            };
            return feature;
        }

        /// <summary>
        /// Convert from <see cref="Info"/> to <see cref="Serialization.Models.Metadata.Info"/>
        /// </summary>
        private static Serialization.Models.Metadata.Info ConvertToInternalModel(Info item)
        {
            var info = new Serialization.Models.Metadata.Info
            {
                [Serialization.Models.Metadata.Info.NameKey] = item.Name,
                [Serialization.Models.Metadata.Info.ValueKey] = item.Value,
            };
            return info;
        }

        /// <summary>
        /// Convert from <see cref="Part"/> to <see cref="Serialization.Models.Metadata.Part"/>
        /// </summary>
        private static Serialization.Models.Metadata.Part ConvertToInternalModel(Part item)
        {
            var part = new Serialization.Models.Metadata.Part
            {
                [Serialization.Models.Metadata.Part.NameKey] = item.Name,
                [Serialization.Models.Metadata.Part.InterfaceKey] = item.Interface,
            };

            if (item.Feature != null && item.Feature.Length > 0)
                part[Serialization.Models.Metadata.Part.FeatureKey] = Array.ConvertAll(item.Feature, ConvertToInternalModel);

            if (item.DataArea != null && item.DataArea.Length > 0)
                part[Serialization.Models.Metadata.Part.DataAreaKey] = Array.ConvertAll(item.DataArea, ConvertToInternalModel);

            if (item.DiskArea != null && item.DiskArea.Length > 0)
                part[Serialization.Models.Metadata.Part.DiskAreaKey] = Array.ConvertAll(item.DiskArea, ConvertToInternalModel);

            if (item.DipSwitch != null && item.DipSwitch.Length > 0)
                part[Serialization.Models.Metadata.Part.DipSwitchKey] = Array.ConvertAll(item.DipSwitch, ConvertToInternalModel);

            return part;
        }

        /// <summary>
        /// Convert from <see cref="Rom"/> to <see cref="Serialization.Models.Metadata.Rom"/>
        /// </summary>
        private static Serialization.Models.Metadata.Rom ConvertToInternalModel(Rom item)
        {
            var rom = new Serialization.Models.Metadata.Rom
            {
                [Serialization.Models.Metadata.Rom.NameKey] = item.Name,
                [Serialization.Models.Metadata.Rom.SizeKey] = item.Size,
                [Serialization.Models.Metadata.Rom.LengthKey] = item.Length,
                [Serialization.Models.Metadata.Rom.CRCKey] = item.CRC,
                [Serialization.Models.Metadata.Rom.SHA1Key] = item.SHA1,
                [Serialization.Models.Metadata.Rom.OffsetKey] = item.Offset,
                [Serialization.Models.Metadata.Rom.ValueKey] = item.Value,
                [Serialization.Models.Metadata.Rom.StatusKey] = item.Status,
                [Serialization.Models.Metadata.Rom.LoadFlagKey] = item.LoadFlag,
            };
            return rom;
        }

        /// <summary>
        /// Convert from <see cref="SharedFeat"/> to <see cref="Serialization.Models.Metadata.SharedFeat"/>
        /// </summary>
        private static Serialization.Models.Metadata.SharedFeat ConvertToInternalModel(SharedFeat item)
        {
            var sharedFeat = new Serialization.Models.Metadata.SharedFeat
            {
                [Serialization.Models.Metadata.SharedFeat.NameKey] = item.Name,
                [Serialization.Models.Metadata.SharedFeat.ValueKey] = item.Value,
            };
            return sharedFeat;
        }
    }
}
