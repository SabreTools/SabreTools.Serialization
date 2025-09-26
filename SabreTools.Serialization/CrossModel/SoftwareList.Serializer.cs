using System;
using SabreTools.Data.Models.SoftwareList;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.CrossModel
{
    public partial class SoftwareList : ICrossModel<Data.Models.SoftwareList.SoftwareList, Data.Models.Metadata.MetadataFile>
    {
        /// <inheritdoc/>
        public Data.Models.Metadata.MetadataFile? Serialize(Data.Models.SoftwareList.SoftwareList? item)
        {
            if (item == null)
                return null;

            var metadataFile = new Data.Models.Metadata.MetadataFile
            {
                [Data.Models.Metadata.MetadataFile.HeaderKey] = ConvertHeaderToInternalModel(item),
            };

            if (item?.Software != null && item.Software.Length > 0)
            {
                metadataFile[Data.Models.Metadata.MetadataFile.MachineKey]
                    = Array.ConvertAll(item.Software, ConvertMachineToInternalModel);
            }

            return metadataFile;
        }

        /// <summary>
        /// Convert from <see cref="Models.SoftwareList.SoftwareList"/> to <see cref="Models.Metadata.Header"/>
        /// </summary>
        private static Data.Models.Metadata.Header ConvertHeaderToInternalModel(Data.Models.SoftwareList.SoftwareList item)
        {
            var header = new Data.Models.Metadata.Header
            {
                [Data.Models.Metadata.Header.NameKey] = item.Name,
                [Data.Models.Metadata.Header.DescriptionKey] = item.Description,
                [Data.Models.Metadata.Header.NotesKey] = item.Notes,
            };
            return header;
        }

        /// <summary>
        /// Convert from <see cref="Models.SoftwareList.Software"/> to <see cref="Models.Metadata.Machine"/>
        /// </summary>
        private static Data.Models.Metadata.Machine ConvertMachineToInternalModel(Software item)
        {
            var machine = new Data.Models.Metadata.Machine
            {
                [Data.Models.Metadata.Machine.NameKey] = item.Name,
                [Data.Models.Metadata.Machine.CloneOfKey] = item.CloneOf,
                [Data.Models.Metadata.Machine.SupportedKey] = item.Supported,
                [Data.Models.Metadata.Machine.DescriptionKey] = item.Description,
                [Data.Models.Metadata.Machine.YearKey] = item.Year,
                [Data.Models.Metadata.Machine.PublisherKey] = item.Publisher,
                [Data.Models.Metadata.Machine.NotesKey] = item.Notes,
            };

            if (item.Info != null && item.Info.Length > 0)
                machine[Data.Models.Metadata.Machine.InfoKey] = Array.ConvertAll(item.Info, ConvertToInternalModel);

            if (item.SharedFeat != null && item.SharedFeat.Length > 0)
                machine[Data.Models.Metadata.Machine.SharedFeatKey] = Array.ConvertAll(item.SharedFeat, ConvertToInternalModel);

            if (item.Part != null && item.Part.Length > 0)
                machine[Data.Models.Metadata.Machine.PartKey] = Array.ConvertAll(item.Part, ConvertToInternalModel);

            return machine;
        }

        /// <summary>
        /// Convert from <see cref="DataArea"/> to <see cref="Models.Metadata.DataArea"/>
        /// </summary>
        private static Data.Models.Metadata.DataArea ConvertToInternalModel(DataArea item)
        {
            var dataArea = new Data.Models.Metadata.DataArea
            {
                [Data.Models.Metadata.DataArea.NameKey] = item.Name,
                [Data.Models.Metadata.DataArea.SizeKey] = item.Size,
                [Data.Models.Metadata.DataArea.WidthKey] = item.Width,
                [Data.Models.Metadata.DataArea.EndiannessKey] = item.Endianness,
            };

            if (item.Rom != null && item.Rom.Length > 0)
                dataArea[Data.Models.Metadata.DataArea.RomKey] = Array.ConvertAll(item.Rom, ConvertToInternalModel);

            return dataArea;
        }

        /// <summary>
        /// Convert from <see cref="DipSwitch"/> to <see cref="Models.Metadata.DipSwitch"/>
        /// </summary>
        private static Data.Models.Metadata.DipSwitch ConvertToInternalModel(DipSwitch item)
        {
            var dipSwitch = new Data.Models.Metadata.DipSwitch
            {
                [Data.Models.Metadata.DipSwitch.NameKey] = item.Name,
                [Data.Models.Metadata.DipSwitch.TagKey] = item.Tag,
                [Data.Models.Metadata.DipSwitch.MaskKey] = item.Mask,
            };

            if (item.DipValue != null && item.DipValue.Length > 0)
                dipSwitch[Data.Models.Metadata.DipSwitch.DipValueKey] = Array.ConvertAll(item.DipValue, ConvertToInternalModel);

            return dipSwitch;
        }

        /// <summary>
        /// Convert from <see cref="DipValue"/> to <see cref="Models.Metadata.DipValue"/>
        /// </summary>
        private static Data.Models.Metadata.DipValue ConvertToInternalModel(DipValue item)
        {
            var dipValue = new Data.Models.Metadata.DipValue
            {
                [Data.Models.Metadata.DipValue.NameKey] = item.Name,
                [Data.Models.Metadata.DipValue.ValueKey] = item.Value,
                [Data.Models.Metadata.DipValue.DefaultKey] = item.Default,
            };
            return dipValue;
        }

        /// <summary>
        /// Convert from <see cref="Disk"/> to <see cref="Models.Metadata.Disk"/>
        /// </summary>
        private static Data.Models.Metadata.Disk ConvertToInternalModel(Disk item)
        {
            var disk = new Data.Models.Metadata.Disk
            {
                [Data.Models.Metadata.Disk.NameKey] = item.Name,
                [Data.Models.Metadata.Disk.MD5Key] = item.MD5,
                [Data.Models.Metadata.Disk.SHA1Key] = item.SHA1,
                [Data.Models.Metadata.Disk.StatusKey] = item.Status,
                [Data.Models.Metadata.Disk.WritableKey] = item.Writeable,
            };
            return disk;
        }

        /// <summary>
        /// Convert from <see cref="DiskArea"/> to <see cref="Models.Metadata.DiskArea"/>
        /// </summary>
        private static Data.Models.Metadata.DiskArea ConvertToInternalModel(DiskArea item)
        {
            var diskArea = new Data.Models.Metadata.DiskArea
            {
                [Data.Models.Metadata.DiskArea.NameKey] = item.Name,
            };

            if (item.Disk != null && item.Disk.Length > 0)
                diskArea[Data.Models.Metadata.DiskArea.DiskKey] = Array.ConvertAll(item.Disk, ConvertToInternalModel);

            return diskArea;
        }

        /// <summary>
        /// Convert from <see cref="Feature"/> to <see cref="Models.Metadata.Feature"/>
        /// </summary>
        private static Data.Models.Metadata.Feature ConvertToInternalModel(Feature item)
        {
            var feature = new Data.Models.Metadata.Feature
            {
                [Data.Models.Metadata.Feature.NameKey] = item.Name,
                [Data.Models.Metadata.Feature.ValueKey] = item.Value,
            };
            return feature;
        }

        /// <summary>
        /// Convert from <see cref="Info"/> to <see cref="Models.Metadata.Info"/>
        /// </summary>
        private static Data.Models.Metadata.Info ConvertToInternalModel(Info item)
        {
            var info = new Data.Models.Metadata.Info
            {
                [Data.Models.Metadata.Info.NameKey] = item.Name,
                [Data.Models.Metadata.Info.ValueKey] = item.Value,
            };
            return info;
        }

        /// <summary>
        /// Convert from <see cref="Part"/> to <see cref="Models.Metadata.Part"/>
        /// </summary>
        private static Data.Models.Metadata.Part ConvertToInternalModel(Part item)
        {
            var part = new Data.Models.Metadata.Part
            {
                [Data.Models.Metadata.Part.NameKey] = item.Name,
                [Data.Models.Metadata.Part.InterfaceKey] = item.Interface,
            };

            if (item.Feature != null && item.Feature.Length > 0)
                part[Data.Models.Metadata.Part.FeatureKey] = Array.ConvertAll(item.Feature, ConvertToInternalModel);

            if (item.DataArea != null && item.DataArea.Length > 0)
                part[Data.Models.Metadata.Part.DataAreaKey] = Array.ConvertAll(item.DataArea, ConvertToInternalModel);

            if (item.DiskArea != null && item.DiskArea.Length > 0)
                part[Data.Models.Metadata.Part.DiskAreaKey] = Array.ConvertAll(item.DiskArea, ConvertToInternalModel);

            if (item.DipSwitch != null && item.DipSwitch.Length > 0)
                part[Data.Models.Metadata.Part.DipSwitchKey] = Array.ConvertAll(item.DipSwitch, ConvertToInternalModel);

            return part;
        }

        /// <summary>
        /// Convert from <see cref="Rom"/> to <see cref="Models.Metadata.Rom"/>
        /// </summary>
        private static Data.Models.Metadata.Rom ConvertToInternalModel(Rom item)
        {
            var rom = new Data.Models.Metadata.Rom
            {
                [Data.Models.Metadata.Rom.NameKey] = item.Name,
                [Data.Models.Metadata.Rom.SizeKey] = item.Size,
                [Data.Models.Metadata.Rom.LengthKey] = item.Length,
                [Data.Models.Metadata.Rom.CRCKey] = item.CRC,
                [Data.Models.Metadata.Rom.SHA1Key] = item.SHA1,
                [Data.Models.Metadata.Rom.OffsetKey] = item.Offset,
                [Data.Models.Metadata.Rom.ValueKey] = item.Value,
                [Data.Models.Metadata.Rom.StatusKey] = item.Status,
                [Data.Models.Metadata.Rom.LoadFlagKey] = item.LoadFlag,
            };
            return rom;
        }

        /// <summary>
        /// Convert from <see cref="SharedFeat"/> to <see cref="Models.Metadata.SharedFeat"/>
        /// </summary>
        private static Data.Models.Metadata.SharedFeat ConvertToInternalModel(SharedFeat item)
        {
            var sharedFeat = new Data.Models.Metadata.SharedFeat
            {
                [Data.Models.Metadata.SharedFeat.NameKey] = item.Name,
                [Data.Models.Metadata.SharedFeat.ValueKey] = item.Value,
            };
            return sharedFeat;
        }
    }
}
