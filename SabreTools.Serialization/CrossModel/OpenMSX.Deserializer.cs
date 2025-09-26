using System;
using SabreTools.Data.Models.OpenMSX;

namespace SabreTools.Serialization.CrossModel
{
    public partial class OpenMSX : BaseMetadataSerializer<SoftwareDb>
    {
        /// <inheritdoc/>
        public override SoftwareDb? Deserialize(Data.Models.Metadata.MetadataFile? obj)
        {
            if (obj == null)
                return null;

            var header = obj.Read<Data.Models.Metadata.Header>(Data.Models.Metadata.MetadataFile.HeaderKey);
            var softwareDb = header != null ? ConvertHeaderFromInternalModel(header) : new SoftwareDb();

            var machines = obj.Read<Data.Models.Metadata.Machine[]>(Data.Models.Metadata.MetadataFile.MachineKey);
            if (machines != null && machines.Length > 0)
                softwareDb.Software = Array.ConvertAll(machines, ConvertMachineFromInternalModel);

            return softwareDb;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Header"/> to <see cref="SoftwareDb"/>
        /// </summary>
        private static SoftwareDb ConvertHeaderFromInternalModel(Data.Models.Metadata.Header item)
        {
            var softwareDb = new SoftwareDb
            {
                Timestamp = item.ReadString(Data.Models.Metadata.Header.TimestampKey),
            };
            return softwareDb;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Machine"/> to <see cref="Software"/>
        /// </summary>
        private static Software ConvertMachineFromInternalModel(Data.Models.Metadata.Machine item)
        {
            var game = new Software
            {
                Title = item.ReadString(Data.Models.Metadata.Machine.NameKey),
                GenMSXID = item.ReadString(Data.Models.Metadata.Machine.GenMSXIDKey),
                System = item.ReadString(Data.Models.Metadata.Machine.SystemKey),
                Company = item.ReadString(Data.Models.Metadata.Machine.CompanyKey),
                Year = item.ReadString(Data.Models.Metadata.Machine.YearKey),
                Country = item.ReadString(Data.Models.Metadata.Machine.CountryKey),
            };

            var dumps = item.Read<Data.Models.Metadata.Dump[]>(Data.Models.Metadata.Machine.DumpKey);
            if (dumps != null && dumps.Length > 0)
                game.Dump = Array.ConvertAll(dumps, ConvertFromInternalModel);

            return game;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Dump"/> to <see cref="Dump"/>
        /// </summary>
        private static Dump ConvertFromInternalModel(Data.Models.Metadata.Dump item)
        {
            var dump = new Dump();

            var original = item.Read<Data.Models.Metadata.Original>(Data.Models.Metadata.Dump.OriginalKey);
            if (original != null)
                dump.Original = ConvertFromInternalModel(original);

            var rom = item.Read<Data.Models.Metadata.Rom>(Data.Models.Metadata.Dump.RomKey);
            if (rom != null)
                dump.Rom = ConvertRomFromInternalModel(rom);

            var megaRom = item.Read<Data.Models.Metadata.Rom>(Data.Models.Metadata.Dump.MegaRomKey);
            if (megaRom != null)
                dump.Rom = ConvertMegaRomFromInternalModel(megaRom);

            var sccPlusCart = item.Read<Data.Models.Metadata.Rom>(Data.Models.Metadata.Dump.SCCPlusCartKey);
            if (sccPlusCart != null)
                dump.Rom = ConvertSCCPlusCartFromInternalModel(sccPlusCart);

            return dump;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Rom"/> to <see cref="MegaRom"/>
        /// </summary>
        private static MegaRom ConvertMegaRomFromInternalModel(Data.Models.Metadata.Rom item)
        {
            var megaRom = new MegaRom
            {
                Start = item.ReadString(Data.Models.Metadata.Rom.StartKey),
                Type = item.ReadString(Data.Models.Metadata.Rom.OpenMSXType),
                Hash = item.ReadString(Data.Models.Metadata.Rom.SHA1Key),
                Remark = item.ReadString(Data.Models.Metadata.Rom.RemarkKey),
            };
            return megaRom;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Original"/> to <see cref="Original"/>
        /// </summary>
        private static Original ConvertFromInternalModel(Data.Models.Metadata.Original item)
        {
            var original = new Original
            {
                Value = item.ReadString(Data.Models.Metadata.Original.ValueKey),
                Content = item.ReadString(Data.Models.Metadata.Original.ContentKey),
            };
            return original;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Rom"/> to <see cref="Rom"/>
        /// </summary>
        private static Rom ConvertRomFromInternalModel(Data.Models.Metadata.Rom item)
        {
            var rom = new Rom
            {
                Start = item.ReadString(Data.Models.Metadata.Rom.StartKey),
                Type = item.ReadString(Data.Models.Metadata.Rom.OpenMSXType),
                Hash = item.ReadString(Data.Models.Metadata.Rom.SHA1Key),
                Remark = item.ReadString(Data.Models.Metadata.Rom.RemarkKey),
            };
            return rom;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Rom"/> to <see cref="SCCPlusCart"/>
        /// </summary>
        private static SCCPlusCart ConvertSCCPlusCartFromInternalModel(Data.Models.Metadata.Rom item)
        {
            var sccPlusCart = new SCCPlusCart
            {
                Start = item.ReadString(Data.Models.Metadata.Rom.StartKey),
                Type = item.ReadString(Data.Models.Metadata.Rom.OpenMSXType),
                Hash = item.ReadString(Data.Models.Metadata.Rom.SHA1Key),
                Remark = item.ReadString(Data.Models.Metadata.Rom.RemarkKey),
            };
            return sccPlusCart;
        }
    }
}
