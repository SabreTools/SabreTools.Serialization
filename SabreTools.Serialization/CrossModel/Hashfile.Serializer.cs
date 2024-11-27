using System;
using SabreTools.Models.Hashfile;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.CrossModel
{
    public partial class Hashfile : IModelSerializer<Models.Hashfile.Hashfile, Models.Metadata.MetadataFile>
    {
        /// <inheritdoc/>
        public Models.Metadata.MetadataFile? Serialize(Models.Hashfile.Hashfile? obj)
        {
            if (obj == null)
                return null;
            
            var metadataFile = new Models.Metadata.MetadataFile
            {
                [Models.Metadata.MetadataFile.HeaderKey] = ConvertHeaderToInternalModel(),
            };

            var machine = ConvertMachineToInternalModel(obj);
            metadataFile[Models.Metadata.MetadataFile.MachineKey] = new Models.Metadata.Machine[] { machine };

            return metadataFile;
        }

        /// <summary>
        /// Convert from <cref="Models.Hashfile.Hashfile"/> to <cref="Models.Metadata.Header"/>
        /// </summary>
        private static Models.Metadata.Header ConvertHeaderToInternalModel()
        {
            var header = new Models.Metadata.Header
            {
                [Models.Metadata.Header.NameKey] = "Hashfile",
            };
            return header;
        }

        /// <summary>
        /// Convert from <cref="Models.Hashfile.Hashfile"/> to <cref="Models.Metadata.Machine"/>
        /// </summary>
        private static Models.Metadata.Machine ConvertMachineToInternalModel(Models.Hashfile.Hashfile item)
        {
            var machine = new Models.Metadata.Machine();

            if (item.SFV != null && item.SFV.Length > 0)
                machine[Models.Metadata.Machine.RomKey] = Array.ConvertAll(item.SFV, ConvertToInternalModel);
            else if (item.MD2 != null && item.MD2.Length > 0)
                machine[Models.Metadata.Machine.RomKey] = Array.ConvertAll(item.MD2, ConvertToInternalModel);
            else if (item.MD4 != null && item.MD4.Length > 0)
                machine[Models.Metadata.Machine.RomKey] = Array.ConvertAll(item.MD4, ConvertToInternalModel);
            else if (item.MD5 != null && item.MD5.Length > 0)
                machine[Models.Metadata.Machine.RomKey] = Array.ConvertAll(item.MD5, ConvertToInternalModel);
            else if (item.SHA1 != null && item.SHA1.Length > 0)
                machine[Models.Metadata.Machine.RomKey] = Array.ConvertAll(item.SHA1, ConvertToInternalModel);
            else if (item.SHA256 != null && item.SHA256.Length > 0)
                machine[Models.Metadata.Machine.RomKey] = Array.ConvertAll(item.SHA256, ConvertToInternalModel);
            else if (item.SHA384 != null && item.SHA384.Length > 0)
                machine[Models.Metadata.Machine.RomKey] = Array.ConvertAll(item.SHA384, ConvertToInternalModel);
            else if (item.SHA512 != null && item.SHA512.Length > 0)
                machine[Models.Metadata.Machine.RomKey] = Array.ConvertAll(item.SHA512, ConvertToInternalModel);
            else if (item.SpamSum != null && item.SpamSum.Length > 0)
                machine[Models.Metadata.Machine.RomKey] = Array.ConvertAll(item.SpamSum, ConvertToInternalModel);

            return machine;
        }

        /// <summary>
        /// Convert from <cref="Models.Hashfile.MD2"/> to <cref="Models.Metadata.Rom"/>
        /// </summary>
        private static Models.Metadata.Rom ConvertToInternalModel(MD2 item)
        {
            var rom = new Models.Metadata.Rom
            {
                [Models.Metadata.Rom.MD2Key] = item.Hash,
                [Models.Metadata.Rom.NameKey] = item.File,
            };
            return rom;
        }

        /// <summary>
        /// Convert from <cref="Models.Hashfile.MD4"/> to <cref="Models.Metadata.Rom"/>
        /// </summary>
        private static Models.Metadata.Rom ConvertToInternalModel(MD4 item)
        {
            var rom = new Models.Metadata.Rom
            {
                [Models.Metadata.Rom.MD4Key] = item.Hash,
                [Models.Metadata.Rom.NameKey] = item.File,
            };
            return rom;
        }

        /// <summary>
        /// Convert from <cref="Models.Hashfile.MD5"/> to <cref="Models.Metadata.Rom"/>
        /// </summary>
        private static Models.Metadata.Rom ConvertToInternalModel(MD5 item)
        {
            var rom = new Models.Metadata.Rom
            {
                [Models.Metadata.Rom.MD5Key] = item.Hash,
                [Models.Metadata.Rom.NameKey] = item.File,
            };
            return rom;
        }

        /// <summary>
        /// Convert from <cref="Models.Hashfile.SFV"/> to <cref="Models.Metadata.Rom"/>
        /// </summary>
        private static Models.Metadata.Rom ConvertToInternalModel(SFV item)
        {
            var rom = new Models.Metadata.Rom
            {
                [Models.Metadata.Rom.NameKey] = item.File,
                [Models.Metadata.Rom.CRCKey] = item.Hash,
            };
            return rom;
        }

        /// <summary>
        /// Convert from <cref="Models.Hashfile.SHA1"/> to <cref="Models.Metadata.Rom"/>
        /// </summary>
        private static Models.Metadata.Rom ConvertToInternalModel(SHA1 item)
        {
            var rom = new Models.Metadata.Rom
            {
                [Models.Metadata.Rom.SHA1Key] = item.Hash,
                [Models.Metadata.Rom.NameKey] = item.File,
            };
            return rom;
        }

        /// <summary>
        /// Convert from <cref="Models.Hashfile.SHA256"/> to <cref="Models.Metadata.Rom"/>
        /// </summary>
        private static Models.Metadata.Rom ConvertToInternalModel(SHA256 item)
        {
            var rom = new Models.Metadata.Rom
            {
                [Models.Metadata.Rom.SHA256Key] = item.Hash,
                [Models.Metadata.Rom.NameKey] = item.File,
            };
            return rom;
        }

        /// <summary>
        /// Convert from <cref="Models.Hashfile.SHA384"/> to <cref="Models.Metadata.Rom"/>
        /// </summary>
        private static Models.Metadata.Rom ConvertToInternalModel(SHA384 item)
        {
            var rom = new Models.Metadata.Rom
            {
                [Models.Metadata.Rom.SHA384Key] = item.Hash,
                [Models.Metadata.Rom.NameKey] = item.File,
            };
            return rom;
        }

        /// <summary>
        /// Convert from <cref="Models.Hashfile.SHA512"/> to <cref="Models.Metadata.Rom"/>
        /// </summary>
        private static Models.Metadata.Rom ConvertToInternalModel(SHA512 item)
        {
            var rom = new Models.Metadata.Rom
            {
                [Models.Metadata.Rom.SHA512Key] = item.Hash,
                [Models.Metadata.Rom.NameKey] = item.File,
            };
            return rom;
        }

        /// <summary>
        /// Convert from <cref="Models.Hashfile.SpamSum"/> to <cref="Models.Metadata.Rom"/>
        /// </summary>
        private static Models.Metadata.Rom ConvertToInternalModel(SpamSum item)
        {
            var rom = new Models.Metadata.Rom
            {
                [Models.Metadata.Rom.SpamSumKey] = item.Hash,
                [Models.Metadata.Rom.NameKey] = item.File,
            };
            return rom;
        }
    }
}