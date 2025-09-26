using System;
using SabreTools.Serialization.Models.OpenMSX;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.CrossModel
{
    public partial class OpenMSX : IModelSerializer<SoftwareDb, Models.Metadata.MetadataFile>
    {
        /// <inheritdoc/>
        public SoftwareDb? Deserialize(Models.Metadata.MetadataFile? obj)
        {
            if (obj == null)
                return null;

            var header = obj.Read<Models.Metadata.Header>(Models.Metadata.MetadataFile.HeaderKey);
            var softwareDb = header != null ? ConvertHeaderFromInternalModel(header) : new SoftwareDb();

            var machines = obj.Read<Models.Metadata.Machine[]>(Models.Metadata.MetadataFile.MachineKey);
            if (machines != null && machines.Length > 0)
                softwareDb.Software = Array.ConvertAll(machines, ConvertMachineFromInternalModel);

            return softwareDb;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Header"/> to <see cref="SoftwareDb"/>
        /// </summary>
        private static SoftwareDb ConvertHeaderFromInternalModel(Models.Metadata.Header item)
        {
            var softwareDb = new SoftwareDb
            {
                Timestamp = item.ReadString(Models.Metadata.Header.TimestampKey),
            };
            return softwareDb;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Machine"/> to <see cref="Software"/>
        /// </summary>
        private static Software ConvertMachineFromInternalModel(Models.Metadata.Machine item)
        {
            var game = new Software
            {
                Title = item.ReadString(Models.Metadata.Machine.NameKey),
                GenMSXID = item.ReadString(Models.Metadata.Machine.GenMSXIDKey),
                System = item.ReadString(Models.Metadata.Machine.SystemKey),
                Company = item.ReadString(Models.Metadata.Machine.CompanyKey),
                Year = item.ReadString(Models.Metadata.Machine.YearKey),
                Country = item.ReadString(Models.Metadata.Machine.CountryKey),
            };

            var dumps = item.Read<Models.Metadata.Dump[]>(Models.Metadata.Machine.DumpKey);
            if (dumps != null && dumps.Length > 0)
                game.Dump = Array.ConvertAll(dumps, ConvertFromInternalModel);

            return game;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Dump"/> to <see cref="Dump"/>
        /// </summary>
        private static Dump ConvertFromInternalModel(Models.Metadata.Dump item)
        {
            var dump = new Dump();

            var original = item.Read<Models.Metadata.Original>(Models.Metadata.Dump.OriginalKey);
            if (original != null)
                dump.Original = ConvertFromInternalModel(original);

            var rom = item.Read<Models.Metadata.Rom>(Models.Metadata.Dump.RomKey);
            if (rom != null)
                dump.Rom = ConvertRomFromInternalModel(rom);

            var megaRom = item.Read<Models.Metadata.Rom>(Models.Metadata.Dump.MegaRomKey);
            if (megaRom != null)
                dump.Rom = ConvertMegaRomFromInternalModel(megaRom);

            var sccPlusCart = item.Read<Models.Metadata.Rom>(Models.Metadata.Dump.SCCPlusCartKey);
            if (sccPlusCart != null)
                dump.Rom = ConvertSCCPlusCartFromInternalModel(sccPlusCart);

            return dump;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Rom"/> to <see cref="MegaRom"/>
        /// </summary>
        private static MegaRom ConvertMegaRomFromInternalModel(Models.Metadata.Rom item)
        {
            var megaRom = new MegaRom
            {
                Start = item.ReadString(Models.Metadata.Rom.StartKey),
                Type = item.ReadString(Models.Metadata.Rom.OpenMSXType),
                Hash = item.ReadString(Models.Metadata.Rom.SHA1Key),
                Remark = item.ReadString(Models.Metadata.Rom.RemarkKey),
            };
            return megaRom;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Original"/> to <see cref="Original"/>
        /// </summary>
        private static Original ConvertFromInternalModel(Models.Metadata.Original item)
        {
            var original = new Original
            {
                Value = item.ReadString(Models.Metadata.Original.ValueKey),
                Content = item.ReadString(Models.Metadata.Original.ContentKey),
            };
            return original;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Rom"/> to <see cref="Rom"/>
        /// </summary>
        private static Rom ConvertRomFromInternalModel(Models.Metadata.Rom item)
        {
            var rom = new Rom
            {
                Start = item.ReadString(Models.Metadata.Rom.StartKey),
                Type = item.ReadString(Models.Metadata.Rom.OpenMSXType),
                Hash = item.ReadString(Models.Metadata.Rom.SHA1Key),
                Remark = item.ReadString(Models.Metadata.Rom.RemarkKey),
            };
            return rom;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Rom"/> to <see cref="SCCPlusCart"/>
        /// </summary>
        private static SCCPlusCart ConvertSCCPlusCartFromInternalModel(Models.Metadata.Rom item)
        {
            var sccPlusCart = new SCCPlusCart
            {
                Start = item.ReadString(Models.Metadata.Rom.StartKey),
                Type = item.ReadString(Models.Metadata.Rom.OpenMSXType),
                Hash = item.ReadString(Models.Metadata.Rom.SHA1Key),
                Remark = item.ReadString(Models.Metadata.Rom.RemarkKey),
            };
            return sccPlusCart;
        }
    }
}
