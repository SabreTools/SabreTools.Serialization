using System;
using SabreTools.Data.Models.OpenMSX;

namespace SabreTools.Serialization.CrossModel
{
    public partial class OpenMSX : BaseMetadataSerializer<SoftwareDb>
    {
        /// <inheritdoc/>
        public override Data.Models.Metadata.MetadataFile? Serialize(SoftwareDb? item)
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
        /// Convert from <see cref="SoftwareDb"/> to <see cref="Models.Metadata.Header"/>
        /// </summary>
        public static Data.Models.Metadata.Header ConvertHeaderToInternalModel(SoftwareDb item)
        {
            var header = new Data.Models.Metadata.Header
            {
                Timestamp = item.Timestamp,
            };
            return header;
        }

        /// <summary>
        /// Convert from <see cref="Software"/> to <see cref="Models.Metadata.Machine"/>
        /// </summary>
        public static Data.Models.Metadata.Machine ConvertMachineToInternalModel(Software item)
        {
            var machine = new Data.Models.Metadata.Machine
            {
                Name = item.Title,
                GenMSXID = item.GenMSXID,
                System = item.System,
                Company = item.Company,
                Year = item.Year,
                Country = item.Country,
            };

            if (item.Dump is not null && item.Dump.Length > 0)
                machine.Dump = Array.ConvertAll(item.Dump, ConvertToInternalModel);

            return machine;
        }

        /// <summary>
        /// Convert from <see cref="Dump"/> to <see cref="Models.Metadata.Dump"/>
        /// </summary>
        public static Data.Models.Metadata.Dump ConvertToInternalModel(Dump item)
        {
            var dump = new Data.Models.Metadata.Dump();

            if (item.Original is not null)
                dump.Original = ConvertToInternalModel(item.Original);

            if (item.Rom is not null)
            {
                switch (item.Rom)
                {
                    case Rom rom:
                        dump.Rom = ConvertToInternalModel(rom);
                        break;

                    case MegaRom megaRom:
                        dump.MegaRom = ConvertToInternalModel(megaRom);
                        break;

                    case SCCPlusCart sccPlusCart:
                        dump.SCCPlusCart = ConvertToInternalModel(sccPlusCart);
                        break;

                    default:
                        // TODO: Log invalid values
                        break;
                }
            }

            return dump;
        }

        /// <summary>
        /// Convert from <see cref="Original"/> to <see cref="Models.Metadata.Rom"/>
        /// </summary>
        public static Data.Models.Metadata.Original ConvertToInternalModel(Original item)
        {
            var original = new Data.Models.Metadata.Original
            {
                Value = item.Value,
                Content = item.Content,
            };
            return original;
        }

        /// <summary>
        /// Convert from <see cref="RomBase"/> to <see cref="Models.Metadata.Rom"/>
        /// </summary>
        public static Data.Models.Metadata.Rom ConvertToInternalModel(RomBase item)
        {
            var rom = new Data.Models.Metadata.Rom
            {
                Start = item.Start,
                OpenMSXType = item.Type,
                SHA1 = item.Hash,
                Remark = item.Remark,
            };
            return rom;
        }
    }
}
