using System;
using SabreTools.Data.Models.Hashfile;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.CrossModel
{
    public partial class Hashfile : IModelSerializer<Data.Models.Hashfile.Hashfile, Data.Models.Metadata.MetadataFile>
    {
        /// <inheritdoc/>
        public Data.Models.Metadata.MetadataFile? Serialize(Data.Models.Hashfile.Hashfile? obj)
        {
            if (obj == null)
                return null;

            var metadataFile = new Data.Models.Metadata.MetadataFile
            {
                [Data.Models.Metadata.MetadataFile.HeaderKey] = ConvertHeaderToInternalModel(),
            };

            var machine = ConvertMachineToInternalModel(obj);
            metadataFile[Data.Models.Metadata.MetadataFile.MachineKey] = new Data.Models.Metadata.Machine[] { machine };

            return metadataFile;
        }

        /// <summary>
        /// Convert from <see cref="Models.Hashfile.Hashfile"/> to <see cref="Models.Metadata.Header"/>
        /// </summary>
        private static Data.Models.Metadata.Header ConvertHeaderToInternalModel()
        {
            var header = new Data.Models.Metadata.Header
            {
                [Data.Models.Metadata.Header.NameKey] = "Hashfile",
            };
            return header;
        }

        /// <summary>
        /// Convert from <see cref="Models.Hashfile.Hashfile"/> to <see cref="Models.Metadata.Machine"/>
        /// </summary>
        private static Data.Models.Metadata.Machine ConvertMachineToInternalModel(Data.Models.Hashfile.Hashfile item)
        {
            var machine = new Data.Models.Metadata.Machine();

            if (item.SFV != null && item.SFV.Length > 0)
                machine[Data.Models.Metadata.Machine.RomKey] = Array.ConvertAll(item.SFV, ConvertToInternalModel);
            else if (item.MD2 != null && item.MD2.Length > 0)
                machine[Data.Models.Metadata.Machine.RomKey] = Array.ConvertAll(item.MD2, ConvertToInternalModel);
            else if (item.MD4 != null && item.MD4.Length > 0)
                machine[Data.Models.Metadata.Machine.RomKey] = Array.ConvertAll(item.MD4, ConvertToInternalModel);
            else if (item.MD5 != null && item.MD5.Length > 0)
                machine[Data.Models.Metadata.Machine.RomKey] = Array.ConvertAll(item.MD5, ConvertToInternalModel);
            else if (item.RIPEMD128 != null && item.RIPEMD128.Length > 0)
                machine[Data.Models.Metadata.Machine.RomKey] = Array.ConvertAll(item.RIPEMD128, ConvertToInternalModel);
            else if (item.RIPEMD160 != null && item.RIPEMD160.Length > 0)
                machine[Data.Models.Metadata.Machine.RomKey] = Array.ConvertAll(item.RIPEMD160, ConvertToInternalModel);
            else if (item.SHA1 != null && item.SHA1.Length > 0)
                machine[Data.Models.Metadata.Machine.RomKey] = Array.ConvertAll(item.SHA1, ConvertToInternalModel);
            else if (item.SHA256 != null && item.SHA256.Length > 0)
                machine[Data.Models.Metadata.Machine.RomKey] = Array.ConvertAll(item.SHA256, ConvertToInternalModel);
            else if (item.SHA384 != null && item.SHA384.Length > 0)
                machine[Data.Models.Metadata.Machine.RomKey] = Array.ConvertAll(item.SHA384, ConvertToInternalModel);
            else if (item.SHA512 != null && item.SHA512.Length > 0)
                machine[Data.Models.Metadata.Machine.RomKey] = Array.ConvertAll(item.SHA512, ConvertToInternalModel);
            else if (item.SpamSum != null && item.SpamSum.Length > 0)
                machine[Data.Models.Metadata.Machine.RomKey] = Array.ConvertAll(item.SpamSum, ConvertToInternalModel);

            return machine;
        }

        /// <summary>
        /// Convert from <see cref="MD2"/> to <see cref="Models.Metadata.Rom"/>
        /// </summary>
        private static Data.Models.Metadata.Rom ConvertToInternalModel(MD2 item)
        {
            var rom = new Data.Models.Metadata.Rom
            {
                [Data.Models.Metadata.Rom.MD2Key] = item.Hash,
                [Data.Models.Metadata.Rom.NameKey] = item.File,
            };
            return rom;
        }

        /// <summary>
        /// Convert from <see cref="MD4"/> to <see cref="Models.Metadata.Rom"/>
        /// </summary>
        private static Data.Models.Metadata.Rom ConvertToInternalModel(MD4 item)
        {
            var rom = new Data.Models.Metadata.Rom
            {
                [Data.Models.Metadata.Rom.MD4Key] = item.Hash,
                [Data.Models.Metadata.Rom.NameKey] = item.File,
            };
            return rom;
        }

        /// <summary>
        /// Convert from <see cref="MD5"/> to <see cref="Models.Metadata.Rom"/>
        /// </summary>
        private static Data.Models.Metadata.Rom ConvertToInternalModel(MD5 item)
        {
            var rom = new Data.Models.Metadata.Rom
            {
                [Data.Models.Metadata.Rom.MD5Key] = item.Hash,
                [Data.Models.Metadata.Rom.NameKey] = item.File,
            };
            return rom;
        }

        /// <summary>
        /// Convert from <see cref="RIPEMD128"/> to <see cref="Models.Metadata.Rom"/>
        /// </summary>
        private static Data.Models.Metadata.Rom ConvertToInternalModel(RIPEMD128 item)
        {
            var rom = new Data.Models.Metadata.Rom
            {
                [Data.Models.Metadata.Rom.RIPEMD128Key] = item.Hash,
                [Data.Models.Metadata.Rom.NameKey] = item.File,
            };
            return rom;
        }

        /// <summary>
        /// Convert from <see cref="RIPEMD160"/> to <see cref="Models.Metadata.Rom"/>
        /// </summary>
        private static Data.Models.Metadata.Rom ConvertToInternalModel(RIPEMD160 item)
        {
            var rom = new Data.Models.Metadata.Rom
            {
                [Data.Models.Metadata.Rom.RIPEMD160Key] = item.Hash,
                [Data.Models.Metadata.Rom.NameKey] = item.File,
            };
            return rom;
        }

        /// <summary>
        /// Convert from <see cref="SFV"/> to <see cref="Models.Metadata.Rom"/>
        /// </summary>
        private static Data.Models.Metadata.Rom ConvertToInternalModel(SFV item)
        {
            var rom = new Data.Models.Metadata.Rom
            {
                [Data.Models.Metadata.Rom.NameKey] = item.File,
                [Data.Models.Metadata.Rom.CRCKey] = item.Hash,
            };
            return rom;
        }

        /// <summary>
        /// Convert from <see cref="SHA1"/> to <see cref="Models.Metadata.Rom"/>
        /// </summary>
        private static Data.Models.Metadata.Rom ConvertToInternalModel(SHA1 item)
        {
            var rom = new Data.Models.Metadata.Rom
            {
                [Data.Models.Metadata.Rom.SHA1Key] = item.Hash,
                [Data.Models.Metadata.Rom.NameKey] = item.File,
            };
            return rom;
        }

        /// <summary>
        /// Convert from <see cref="SHA256"/> to <see cref="Models.Metadata.Rom"/>
        /// </summary>
        private static Data.Models.Metadata.Rom ConvertToInternalModel(SHA256 item)
        {
            var rom = new Data.Models.Metadata.Rom
            {
                [Data.Models.Metadata.Rom.SHA256Key] = item.Hash,
                [Data.Models.Metadata.Rom.NameKey] = item.File,
            };
            return rom;
        }

        /// <summary>
        /// Convert from <see cref="SHA384"/> to <see cref="Models.Metadata.Rom"/>
        /// </summary>
        private static Data.Models.Metadata.Rom ConvertToInternalModel(SHA384 item)
        {
            var rom = new Data.Models.Metadata.Rom
            {
                [Data.Models.Metadata.Rom.SHA384Key] = item.Hash,
                [Data.Models.Metadata.Rom.NameKey] = item.File,
            };
            return rom;
        }

        /// <summary>
        /// Convert from <see cref="SHA512"/> to <see cref="Models.Metadata.Rom"/>
        /// </summary>
        private static Data.Models.Metadata.Rom ConvertToInternalModel(SHA512 item)
        {
            var rom = new Data.Models.Metadata.Rom
            {
                [Data.Models.Metadata.Rom.SHA512Key] = item.Hash,
                [Data.Models.Metadata.Rom.NameKey] = item.File,
            };
            return rom;
        }

        /// <summary>
        /// Convert from <see cref="SpamSum"/> to <see cref="Models.Metadata.Rom"/>
        /// </summary>
        private static Data.Models.Metadata.Rom ConvertToInternalModel(SpamSum item)
        {
            var rom = new Data.Models.Metadata.Rom
            {
                [Data.Models.Metadata.Rom.SpamSumKey] = item.Hash,
                [Data.Models.Metadata.Rom.NameKey] = item.File,
            };
            return rom;
        }
    }
}
