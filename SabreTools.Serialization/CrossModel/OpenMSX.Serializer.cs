using System;
using SabreTools.Serialization.Interfaces;
using SabreTools.Serialization.Models.OpenMSX;

namespace SabreTools.Serialization.CrossModel
{
    public partial class OpenMSX : IModelSerializer<SoftwareDb, Serialization.Models.Metadata.MetadataFile>
    {
        /// <inheritdoc/>
        public Serialization.Models.Metadata.MetadataFile? Serialize(SoftwareDb? item)
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
        /// Convert from <see cref="SoftwareDb"/> to <see cref="Serialization.Models.Metadata.Header"/>
        /// </summary>
        public static Serialization.Models.Metadata.Header ConvertHeaderToInternalModel(SoftwareDb item)
        {
            var header = new Serialization.Models.Metadata.Header
            {
                [Serialization.Models.Metadata.Header.TimestampKey] = item.Timestamp,
            };
            return header;
        }

        /// <summary>
        /// Convert from <see cref="Software"/> to <see cref="Serialization.Models.Metadata.Machine"/>
        /// </summary>
        public static Serialization.Models.Metadata.Machine ConvertMachineToInternalModel(Software item)
        {
            var machine = new Serialization.Models.Metadata.Machine
            {
                [Serialization.Models.Metadata.Machine.NameKey] = item.Title,
                [Serialization.Models.Metadata.Machine.GenMSXIDKey] = item.GenMSXID,
                [Serialization.Models.Metadata.Machine.SystemKey] = item.System,
                [Serialization.Models.Metadata.Machine.CompanyKey] = item.Company,
                [Serialization.Models.Metadata.Machine.YearKey] = item.Year,
                [Serialization.Models.Metadata.Machine.CountryKey] = item.Country,
            };

            if (item.Dump != null && item.Dump.Length > 0)
            {
                machine[Serialization.Models.Metadata.Machine.DumpKey]
                    = Array.ConvertAll(item.Dump, ConvertToInternalModel);
            }

            return machine;
        }

        /// <summary>
        /// Convert from <see cref="Dump"/> to <see cref="Serialization.Models.Metadata.Dump"/>
        /// </summary>
        public static Serialization.Models.Metadata.Dump ConvertToInternalModel(Dump item)
        {
            var dump = new Serialization.Models.Metadata.Dump();

            if (item.Original != null)
                dump[Serialization.Models.Metadata.Dump.OriginalKey] = ConvertToInternalModel(item.Original);

            if (item.Rom != null)
            {
                switch (item.Rom)
                {
                    case Rom rom:
                        dump[Serialization.Models.Metadata.Dump.RomKey] = ConvertToInternalModel(rom);
                        break;

                    case MegaRom megaRom:
                        dump[Serialization.Models.Metadata.Dump.MegaRomKey] = ConvertToInternalModel(megaRom);
                        break;

                    case SCCPlusCart sccPlusCart:
                        dump[Serialization.Models.Metadata.Dump.SCCPlusCartKey] = ConvertToInternalModel(sccPlusCart);
                        break;
                }
            }

            return dump;
        }

        /// <summary>
        /// Convert from <see cref="Original"/> to <see cref="Serialization.Models.Metadata.Rom"/>
        /// </summary>
        public static Serialization.Models.Metadata.Original ConvertToInternalModel(Original item)
        {
            var original = new Serialization.Models.Metadata.Original
            {
                [Serialization.Models.Metadata.Original.ValueKey] = item.Value,
                [Serialization.Models.Metadata.Original.ContentKey] = item.Content,
            };
            return original;
        }

        /// <summary>
        /// Convert from <see cref="RomBase"/> to <see cref="Serialization.Models.Metadata.Rom"/>
        /// </summary>
        public static Serialization.Models.Metadata.Rom ConvertToInternalModel(RomBase item)
        {
            var rom = new Serialization.Models.Metadata.Rom
            {
                [Serialization.Models.Metadata.Rom.StartKey] = item.Start,
                [Serialization.Models.Metadata.Rom.OpenMSXType] = item.Type,
                [Serialization.Models.Metadata.Rom.SHA1Key] = item.Hash,
                [Serialization.Models.Metadata.Rom.RemarkKey] = item.Remark,
            };
            return rom;
        }
    }
}
