using System;
using SabreTools.Data.Models.SoftwareList;

namespace SabreTools.Serialization.CrossModel
{
    public partial class SoftwareList : BaseMetadataSerializer<Data.Models.SoftwareList.SoftwareList>
    {
        /// <inheritdoc/>
        public override Data.Models.Metadata.MetadataFile? Serialize(Data.Models.SoftwareList.SoftwareList? item)
        {
            if (item is null)
                return null;

            var metadataFile = new Data.Models.Metadata.MetadataFile
            {
                Header = ConvertHeaderToInternalModel(item),
            };

            if (item?.Software is not null && item.Software.Length > 0)
                metadataFile.Machine = Array.ConvertAll(item.Software, ConvertMachineToInternalModel);

            return metadataFile;
        }

        /// <summary>
        /// Convert from <see cref="Models.SoftwareList.SoftwareList"/> to <see cref="Models.Metadata.Header"/>
        /// </summary>
        private static Data.Models.Metadata.Header ConvertHeaderToInternalModel(Data.Models.SoftwareList.SoftwareList item)
        {
            var header = new Data.Models.Metadata.Header
            {
                Name = item.Name,
                Description = item.Description,
                Notes = item.Notes,
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
                Name = item.Name,
                CloneOf = item.CloneOf,
                Supported = item.Supported,
                Description = item.Description,
                Year = item.Year,
                Publisher = item.Publisher,
                Notes = item.Notes,
            };

            if (item.Info is not null && item.Info.Length > 0)
                machine.Info = Array.ConvertAll(item.Info, ConvertToInternalModel);

            if (item.SharedFeat is not null && item.SharedFeat.Length > 0)
                machine.SharedFeat = Array.ConvertAll(item.SharedFeat, ConvertToInternalModel);

            if (item.Part is not null && item.Part.Length > 0)
                machine.Part = Array.ConvertAll(item.Part, ConvertToInternalModel);

            return machine;
        }

        /// <summary>
        /// Convert from <see cref="DataArea"/> to <see cref="Models.Metadata.DataArea"/>
        /// </summary>
        private static Data.Models.Metadata.DataArea ConvertToInternalModel(DataArea item)
        {
            var dataArea = new Data.Models.Metadata.DataArea
            {
                Name = item.Name,
                Size = item.Size,
                Width = item.Width,
                Endianness = item.Endianness,
            };

            if (item.Rom is not null && item.Rom.Length > 0)
                dataArea.Rom = Array.ConvertAll(item.Rom, ConvertToInternalModel);

            return dataArea;
        }

        /// <summary>
        /// Convert from <see cref="DipSwitch"/> to <see cref="Models.Metadata.DipSwitch"/>
        /// </summary>
        private static Data.Models.Metadata.DipSwitch ConvertToInternalModel(DipSwitch item)
        {
            var dipSwitch = new Data.Models.Metadata.DipSwitch
            {
                Name = item.Name,
                Tag = item.Tag,
                Mask = item.Mask,
            };

            if (item.DipValue is not null && item.DipValue.Length > 0)
                dipSwitch.DipValue = Array.ConvertAll(item.DipValue, ConvertToInternalModel);

            return dipSwitch;
        }

        /// <summary>
        /// Convert from <see cref="DipValue"/> to <see cref="Models.Metadata.DipValue"/>
        /// </summary>
        private static Data.Models.Metadata.DipValue ConvertToInternalModel(DipValue item)
        {
            var dipValue = new Data.Models.Metadata.DipValue
            {
                Name = item.Name,
                Value = item.Value,
                Default = item.Default,
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
                Name = item.Name,
                MD5 = item.MD5,
                SHA1 = item.SHA1,
                Status = item.Status,
                Writable = item.Writeable,
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
                Name = item.Name,
            };

            if (item.Disk is not null && item.Disk.Length > 0)
                diskArea.Disk = Array.ConvertAll(item.Disk, ConvertToInternalModel);

            return diskArea;
        }

        /// <summary>
        /// Convert from <see cref="Feature"/> to <see cref="Models.Metadata.Feature"/>
        /// </summary>
        private static Data.Models.Metadata.Feature ConvertToInternalModel(Feature item)
        {
            var feature = new Data.Models.Metadata.Feature
            {
                Name = item.Name,
                Value = item.Value,
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
                Name = item.Name,
                Value = item.Value,
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
                Name = item.Name,
                Interface = item.Interface,
            };

            if (item.Feature is not null && item.Feature.Length > 0)
                part.Feature = Array.ConvertAll(item.Feature, ConvertToInternalModel);

            if (item.DataArea is not null && item.DataArea.Length > 0)
                part.DataArea = Array.ConvertAll(item.DataArea, ConvertToInternalModel);

            if (item.DiskArea is not null && item.DiskArea.Length > 0)
                part.DiskArea = Array.ConvertAll(item.DiskArea, ConvertToInternalModel);

            if (item.DipSwitch is not null && item.DipSwitch.Length > 0)
                part.DipSwitch = Array.ConvertAll(item.DipSwitch, ConvertToInternalModel);

            return part;
        }

        /// <summary>
        /// Convert from <see cref="Rom"/> to <see cref="Models.Metadata.Rom"/>
        /// </summary>
        private static Data.Models.Metadata.Rom ConvertToInternalModel(Rom item)
        {
            var rom = new Data.Models.Metadata.Rom
            {
                Name = item.Name,
                Size = item.Size,
                [Data.Models.Metadata.Rom.LengthKey] = item.Length,
                [Data.Models.Metadata.Rom.CRCKey] = item.CRC,
                [Data.Models.Metadata.Rom.SHA1Key] = item.SHA1,
                [Data.Models.Metadata.Rom.OffsetKey] = item.Offset,
                Value = item.Value,
                Status = item.Status,
                LoadFlag = item.LoadFlag,
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
                Name = item.Name,
                Value = item.Value,
            };
            return sharedFeat;
        }
    }
}
