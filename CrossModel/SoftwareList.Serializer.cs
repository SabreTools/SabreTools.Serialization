using System.Linq;
using SabreTools.Models.SoftwareList;

namespace SabreTools.Serialization.CrossModel
{
    public partial class SoftwareList : IModelSerializer<Models.SoftwareList.SoftwareList, Models.Metadata.MetadataFile>
    {
        /// <inheritdoc/>
#if NET48
        public Models.Metadata.MetadataFile Serialize(Models.SoftwareList.SoftwareList item)
#else
        public Models.Metadata.MetadataFile? Serialize(Models.SoftwareList.SoftwareList? item)
#endif
        {
            if (item == null)
                return null;

            var metadataFile = new Models.Metadata.MetadataFile
            {
                [Models.Metadata.MetadataFile.HeaderKey] = ConvertHeaderToInternalModel(item),
            };

            if (item?.Software != null && item.Software.Any())
            {
                metadataFile[Models.Metadata.MetadataFile.MachineKey] = item.Software
                    .Where(s => s != null)
                    .Select(ConvertMachineToInternalModel)
                    .ToArray();
            }

            return metadataFile;
        }

        /// <summary>
        /// Convert from <cref="Models.SoftwareList.SoftwareList"/> to <cref="Models.Metadata.Header"/>
        /// </summary>
        private static Models.Metadata.Header ConvertHeaderToInternalModel(Models.SoftwareList.SoftwareList item)
        {
            var header = new Models.Metadata.Header
            {
                [Models.Metadata.Header.NameKey] = item.Name,
                [Models.Metadata.Header.DescriptionKey] = item.Description,
                [Models.Metadata.Header.NotesKey] = item.Notes,
            };
            return header;
        }

        /// <summary>
        /// Convert from <cref="Models.SoftwareList.Software"/> to <cref="Models.Metadata.Machine"/>
        /// </summary>
        private static Models.Metadata.Machine ConvertMachineToInternalModel(Software item)
        {
            var machine = new Models.Metadata.Machine
            {
                [Models.Metadata.Machine.NameKey] = item.Name,
                [Models.Metadata.Machine.CloneOfKey] = item.CloneOf,
                [Models.Metadata.Machine.SupportedKey] = item.Supported,
                [Models.Metadata.Machine.DescriptionKey] = item.Description,
                [Models.Metadata.Machine.YearKey] = item.Year,
                [Models.Metadata.Machine.PublisherKey] = item.Publisher,
                [Models.Metadata.Machine.NotesKey] = item.Notes,
            };

            if (item.Info != null && item.Info.Any())
                machine[Models.Metadata.Machine.InfoKey] = item.Info.Select(ConvertToInternalModel).ToArray();

            if (item.SharedFeat != null && item.SharedFeat.Any())
                machine[Models.Metadata.Machine.SharedFeatKey] = item.SharedFeat.Select(ConvertToInternalModel).ToArray();

            if (item.Part != null && item.Part.Any())
                machine[Models.Metadata.Machine.PartKey] = item.Part.Select(ConvertToInternalModel).ToArray();

            return machine;
        }

        /// <summary>
        /// Convert from <cref="Models.SoftwareList.DataArea"/> to <cref="Models.Metadata.DataArea"/>
        /// </summary>
        private static Models.Metadata.DataArea ConvertToInternalModel(DataArea item)
        {
            var dataArea = new Models.Metadata.DataArea
            {
                [Models.Metadata.DataArea.NameKey] = item.Name,
                [Models.Metadata.DataArea.SizeKey] = item.Size,
                [Models.Metadata.DataArea.WidthKey] = item.Width,
                [Models.Metadata.DataArea.EndiannessKey] = item.Endianness,
            };

            if (item.Rom != null && item.Rom.Any())
                dataArea[Models.Metadata.DataArea.RomKey] = item.Rom.Select(ConvertToInternalModel).ToArray();

            return dataArea;
        }

        /// <summary>
        /// Convert from <cref="Models.SoftwareList.DipSwitch"/> to <cref="Models.Metadata.DipSwitch"/>
        /// </summary>
        private static Models.Metadata.DipSwitch ConvertToInternalModel(DipSwitch item)
        {
            var dipSwitch = new Models.Metadata.DipSwitch
            {
                [Models.Metadata.DipSwitch.NameKey] = item.Name,
                [Models.Metadata.DipSwitch.TagKey] = item.Tag,
                [Models.Metadata.DipSwitch.MaskKey] = item.Mask,
            };

            if (item.DipValue != null && item.DipValue.Any())
                dipSwitch[Models.Metadata.DipSwitch.DipValueKey] = item.DipValue.Select(ConvertToInternalModel).ToArray();

            return dipSwitch;
        }

        /// <summary>
        /// Convert from <cref="Models.SoftwareList.DipValue"/> to <cref="Models.Metadata.DipValue"/>
        /// </summary>
        private static Models.Metadata.DipValue ConvertToInternalModel(DipValue item)
        {
            var dipValue = new Models.Metadata.DipValue
            {
                [Models.Metadata.DipValue.NameKey] = item.Name,
                [Models.Metadata.DipValue.ValueKey] = item.Value,
                [Models.Metadata.DipValue.DefaultKey] = item.Default,
            };
            return dipValue;
        }

        /// <summary>
        /// Convert from <cref="Models.SoftwareList.Disk"/> to <cref="Models.Metadata.Disk"/>
        /// </summary>
        private static Models.Metadata.Disk ConvertToInternalModel(Disk item)
        {
            var disk = new Models.Metadata.Disk
            {
                [Models.Metadata.Disk.NameKey] = item.Name,
                [Models.Metadata.Disk.MD5Key] = item.MD5,
                [Models.Metadata.Disk.SHA1Key] = item.SHA1,
                [Models.Metadata.Disk.StatusKey] = item.Status,
                [Models.Metadata.Disk.WritableKey] = item.Writeable,
            };
            return disk;
        }

        /// <summary>
        /// Convert from <cref="Models.SoftwareList.DiskArea"/> to <cref="Models.Metadata.DiskArea"/>
        /// </summary>
        private static Models.Metadata.DiskArea ConvertToInternalModel(DiskArea item)
        {
            var diskArea = new Models.Metadata.DiskArea
            {
                [Models.Metadata.DiskArea.NameKey] = item.Name,
            };

            if (item.Disk != null && item.Disk.Any())
                diskArea[Models.Metadata.DiskArea.DiskKey] = item.Disk.Select(ConvertToInternalModel).ToArray();

            return diskArea;
        }

        /// <summary>
        /// Convert from <cref="Models.SoftwareList.Feature"/> to <cref="Models.Metadata.Feature"/>
        /// </summary>
        private static Models.Metadata.Feature ConvertToInternalModel(Feature item)
        {
            var feature = new Models.Metadata.Feature
            {
                [Models.Metadata.Feature.NameKey] = item.Name,
                [Models.Metadata.Feature.ValueKey] = item.Value,
            };
            return feature;
        }

        /// <summary>
        /// Convert from <cref="Models.SoftwareList.Info"/> to <cref="Models.Metadata.Info"/>
        /// </summary>
        private static Models.Metadata.Info ConvertToInternalModel(Info item)
        {
            var info = new Models.Metadata.Info
            {
                [Models.Metadata.Info.NameKey] = item.Name,
                [Models.Metadata.Info.ValueKey] = item.Value,
            };
            return info;
        }

        /// <summary>
        /// Convert from <cref="Models.SoftwareList.Part"/> to <cref="Models.Metadata.Part"/>
        /// </summary>
        private static Models.Metadata.Part ConvertToInternalModel(Part item)
        {
            var part = new Models.Metadata.Part
            {
                [Models.Metadata.Part.NameKey] = item.Name,
                [Models.Metadata.Part.InterfaceKey] = item.Interface,
            };

            if (item.Feature != null && item.Feature.Any())
                part[Models.Metadata.Part.FeatureKey] = item.Feature.Select(ConvertToInternalModel).ToArray();

            if (item.DataArea != null && item.DataArea.Any())
                part[Models.Metadata.Part.DataAreaKey] = item.DataArea.Select(ConvertToInternalModel).ToArray();

            if (item.DiskArea != null && item.DiskArea.Any())
                part[Models.Metadata.Part.DiskAreaKey] = item.DiskArea.Select(ConvertToInternalModel).ToArray();

            if (item.DipSwitch != null && item.DipSwitch.Any())
                part[Models.Metadata.Part.DipSwitchKey] = item.DipSwitch.Select(ConvertToInternalModel).ToArray();

            return part;
        }

        /// <summary>
        /// Convert from <cref="Models.SoftwareList.Rom"/> to <cref="Models.Metadata.Rom"/>
        /// </summary>
        private static Models.Metadata.Rom ConvertToInternalModel(Rom item)
        {
            var rom = new Models.Metadata.Rom
            {
                [Models.Metadata.Rom.NameKey] = item.Name,
                [Models.Metadata.Rom.SizeKey] = item.Size,
                [Models.Metadata.Rom.LengthKey] = item.Length,
                [Models.Metadata.Rom.CRCKey] = item.CRC,
                [Models.Metadata.Rom.SHA1Key] = item.SHA1,
                [Models.Metadata.Rom.OffsetKey] = item.Offset,
                [Models.Metadata.Rom.ValueKey] = item.Value,
                [Models.Metadata.Rom.StatusKey] = item.Status,
                [Models.Metadata.Rom.LoadFlagKey] = item.LoadFlag,
            };
            return rom;
        }

        /// <summary>
        /// Convert from <cref="Models.SoftwareList.SharedFeat"/> to <cref="Models.Metadata.SharedFeat"/>
        /// </summary>
        private static Models.Metadata.SharedFeat ConvertToInternalModel(SharedFeat item)
        {
            var sharedFeat = new Models.Metadata.SharedFeat
            {
                [Models.Metadata.SharedFeat.NameKey] = item.Name,
                [Models.Metadata.SharedFeat.ValueKey] = item.Value,
            };
            return sharedFeat;
        }
    }
}