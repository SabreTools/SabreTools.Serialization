using System;
using SabreTools.Data.Models.OpenMSX;

namespace SabreTools.Serialization.CrossModel
{
    public partial class OpenMSX : BaseMetadataSerializer<SoftwareDb>
    {
        /// <inheritdoc/>
        public override Data.Models.Metadata.MetadataFile? Serialize(SoftwareDb? item)
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
        /// Convert from <see cref="SoftwareDb"/> to <see cref="Models.Metadata.Header"/>
        /// </summary>
        public static Data.Models.Metadata.Header ConvertHeaderToInternalModel(SoftwareDb item)
        {
            var header = new Data.Models.Metadata.Header
            {
                [Data.Models.Metadata.Header.TimestampKey] = item.Timestamp,
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
                [Data.Models.Metadata.Machine.NameKey] = item.Title,
                [Data.Models.Metadata.Machine.GenMSXIDKey] = item.GenMSXID,
                [Data.Models.Metadata.Machine.SystemKey] = item.System,
                [Data.Models.Metadata.Machine.CompanyKey] = item.Company,
                [Data.Models.Metadata.Machine.YearKey] = item.Year,
                [Data.Models.Metadata.Machine.CountryKey] = item.Country,
            };

            if (item.Dump != null && item.Dump.Length > 0)
            {
                machine[Data.Models.Metadata.Machine.DumpKey]
                    = Array.ConvertAll(item.Dump, ConvertToInternalModel);
            }

            return machine;
        }

        /// <summary>
        /// Convert from <see cref="Dump"/> to <see cref="Models.Metadata.Dump"/>
        /// </summary>
        public static Data.Models.Metadata.Dump ConvertToInternalModel(Dump item)
        {
            var dump = new Data.Models.Metadata.Dump();

            if (item.Original != null)
                dump[Data.Models.Metadata.Dump.OriginalKey] = ConvertToInternalModel(item.Original);

            if (item.Rom != null)
            {
                switch (item.Rom)
                {
                    case Rom rom:
                        dump[Data.Models.Metadata.Dump.RomKey] = ConvertToInternalModel(rom);
                        break;

                    case MegaRom megaRom:
                        dump[Data.Models.Metadata.Dump.MegaRomKey] = ConvertToInternalModel(megaRom);
                        break;

                    case SCCPlusCart sccPlusCart:
                        dump[Data.Models.Metadata.Dump.SCCPlusCartKey] = ConvertToInternalModel(sccPlusCart);
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
                [Data.Models.Metadata.Original.ValueKey] = item.Value,
                [Data.Models.Metadata.Original.ContentKey] = item.Content,
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
                [Data.Models.Metadata.Rom.StartKey] = item.Start,
                [Data.Models.Metadata.Rom.OpenMSXType] = item.Type,
                [Data.Models.Metadata.Rom.SHA1Key] = item.Hash,
                [Data.Models.Metadata.Rom.RemarkKey] = item.Remark,
            };
            return rom;
        }
    }
}
