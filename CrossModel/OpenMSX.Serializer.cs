using System.Linq;
using SabreTools.Models.OpenMSX;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.CrossModel
{
    public partial class OpenMSX : IModelSerializer<SoftwareDb, Models.Metadata.MetadataFile>
    {
        /// <inheritdoc/>
#if NET48
        public Models.Metadata.MetadataFile Serialize(SoftwareDb item)
#else
        public Models.Metadata.MetadataFile? Serialize(SoftwareDb? item)
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
        /// Convert from <cref="Models.OpenMSX.SoftwareDb"/> to <cref="Models.Metadata.Header"/>
        /// </summary>
        public static Models.Metadata.Header ConvertHeaderToInternalModel(SoftwareDb item)
        {
            var header = new Models.Metadata.Header
            {
                [Models.Metadata.Header.TimestampKey] = item.Timestamp,
            };
            return header;
        }

        /// <summary>
        /// Convert from <cref="Models.OpenMSX.Software"/> to <cref="Models.Metadata.Machine"/>
        /// </summary>
        public static Models.Metadata.Machine ConvertMachineToInternalModel(Software item)
        {
            var machine = new Models.Metadata.Machine
            {
                [Models.Metadata.Machine.NameKey] = item.Title,
                [Models.Metadata.Machine.GenMSXIDKey] = item.GenMSXID,
                [Models.Metadata.Machine.SystemKey] = item.System,
                [Models.Metadata.Machine.CompanyKey] = item.Company,
                [Models.Metadata.Machine.YearKey] = item.Year,
                [Models.Metadata.Machine.CountryKey] = item.Country,
            };

            if (item.Dump != null && item.Dump.Any())
            {
                machine[Models.Metadata.Machine.DumpKey] = item.Dump
                    .Where(d => d != null)
                    .Select(ConvertToInternalModel)
                    .ToArray();
            }

            return machine;
        }

        /// <summary>
        /// Convert from <cref="Models.OpenMSX.Dump"/> to <cref="Models.Metadata.Dump"/>
        /// </summary>
        public static Models.Metadata.Dump ConvertToInternalModel(Dump item)
        {
            var dump = new Models.Metadata.Dump();

            if (item.Original != null)
                dump[Models.Metadata.Dump.OriginalKey] = ConvertToInternalModel(item.Original);

            if (item.Rom != null)
            {
                switch (item.Rom)
                {
                    case Rom rom:
                        dump[Models.Metadata.Dump.RomKey] = ConvertToInternalModel(rom);
                        break;

                    case MegaRom megaRom:
                        dump[Models.Metadata.Dump.MegaRomKey] = ConvertToInternalModel(megaRom);
                        break;

                    case SCCPlusCart sccPlusCart:
                        dump[Models.Metadata.Dump.SCCPlusCartKey] = ConvertToInternalModel(sccPlusCart);
                        break;
                }
            }

            return dump;
        }

        /// <summary>
        /// Convert from <cref="Models.OpenMSX.Original"/> to <cref="Models.Metadata.Rom"/>
        /// </summary>
        public static Models.Metadata.Original ConvertToInternalModel(Original item)
        {
            var original = new Models.Metadata.Original
            {
                [Models.Metadata.Original.ValueKey] = item.Value,
                [Models.Metadata.Original.ContentKey] = item.Content,
            };
            return original;
        }

        /// <summary>
        /// Convert from <cref="Models.OpenMSX.RomBase"/> to <cref="Models.Metadata.Rom"/>
        /// </summary>
        public static Models.Metadata.Rom ConvertToInternalModel(RomBase item)
        {
            var rom = new Models.Metadata.Rom
            {
                [Models.Metadata.Rom.StartKey] = item.Start,
                [Models.Metadata.Rom.TypeKey] = item.Type,
                [Models.Metadata.Rom.SHA1Key] = item.Hash,
                [Models.Metadata.Rom.RemarkKey] = item.Remark,
            };
            return rom;
        }
    }
}