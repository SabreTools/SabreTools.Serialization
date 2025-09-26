using System;
using System.Collections.Generic;
using SabreTools.Data.Models.Hashfile;
using SabreTools.Hashing;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.CrossModel
{
    public partial class Hashfile : IModelSerializer<Data.Models.Hashfile.Hashfile, Data.Models.Metadata.MetadataFile>
    {
        /// <inheritdoc/>
        public Data.Models.Hashfile.Hashfile? Deserialize(Data.Models.Metadata.MetadataFile? obj) => Deserialize(obj, HashType.CRC32);

        /// <inheritdoc/>
        public Data.Models.Hashfile.Hashfile? Deserialize(Data.Models.Metadata.MetadataFile? obj, HashType hash)
        {
            if (obj == null)
                return null;

            var machines = obj.Read<Data.Models.Metadata.Machine[]>(Data.Models.Metadata.MetadataFile.MachineKey);
            if (machines == null || machines.Length == 0)
                return null;

            var hashfiles = Array.ConvertAll(machines,
                m => ConvertMachineFromInternalModel(m, hash));

            var sfvs = new List<SFV>();
            var md2s = new List<MD2>();
            var md4s = new List<MD4>();
            var md5s = new List<MD5>();
            var ripemd128s = new List<RIPEMD128>();
            var ripemd160s = new List<RIPEMD160>();
            var sha1s = new List<SHA1>();
            var sha256s = new List<SHA256>();
            var sha384s = new List<SHA384>();
            var sha512s = new List<SHA512>();
            var spamsums = new List<SpamSum>();

            foreach (var hashfile in hashfiles)
            {
                if (hashfile.SFV != null && hashfile.SFV.Length > 0)
                    sfvs.AddRange(hashfile.SFV);
                if (hashfile.MD2 != null && hashfile.MD2.Length > 0)
                    md2s.AddRange(hashfile.MD2);
                if (hashfile.MD4 != null && hashfile.MD4.Length > 0)
                    md4s.AddRange(hashfile.MD4);
                if (hashfile.MD5 != null && hashfile.MD5.Length > 0)
                    md5s.AddRange(hashfile.MD5);
                if (hashfile.RIPEMD128 != null && hashfile.RIPEMD128.Length > 0)
                    ripemd128s.AddRange(hashfile.RIPEMD128);
                if (hashfile.RIPEMD160 != null && hashfile.RIPEMD160.Length > 0)
                    ripemd160s.AddRange(hashfile.RIPEMD160);
                if (hashfile.SHA1 != null && hashfile.SHA1.Length > 0)
                    sha1s.AddRange(hashfile.SHA1);
                if (hashfile.SHA256 != null && hashfile.SHA256.Length > 0)
                    sha256s.AddRange(hashfile.SHA256);
                if (hashfile.SHA384 != null && hashfile.SHA384.Length > 0)
                    sha384s.AddRange(hashfile.SHA384);
                if (hashfile.SHA512 != null && hashfile.SHA512.Length > 0)
                    sha512s.AddRange(hashfile.SHA512);
                if (hashfile.SpamSum != null && hashfile.SpamSum.Length > 0)
                    spamsums.AddRange(hashfile.SpamSum);
            }

            var hashfileItem = new Data.Models.Hashfile.Hashfile();

            if (sfvs.Count > 0)
                hashfileItem.SFV = [.. sfvs];
            if (md2s.Count > 0)
                hashfileItem.MD2 = [.. md2s];
            if (md4s.Count > 0)
                hashfileItem.MD4 = [.. md4s];
            if (md5s.Count > 0)
                hashfileItem.MD5 = [.. md5s];
            if (ripemd128s.Count > 0)
                hashfileItem.RIPEMD128 = [.. ripemd128s];
            if (ripemd160s.Count > 0)
                hashfileItem.RIPEMD160 = [.. ripemd160s];
            if (sha1s.Count > 0)
                hashfileItem.SHA1 = [.. sha1s];
            if (sha256s.Count > 0)
                hashfileItem.SHA256 = [.. sha256s];
            if (sha384s.Count > 0)
                hashfileItem.SHA384 = [.. sha384s];
            if (sha512s.Count > 0)
                hashfileItem.SHA512 = [.. sha512s];
            if (spamsums.Count > 0)
                hashfileItem.SpamSum = [.. spamsums];

            return hashfileItem;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Machine"/> to <see cref="Models.Hashfile.Hashfile"/>
        /// </summary>
        private static Data.Models.Hashfile.Hashfile ConvertMachineFromInternalModel(Data.Models.Metadata.Machine item, HashType hash)
        {
            var roms = item.Read<Data.Models.Metadata.Rom[]>(Data.Models.Metadata.Machine.RomKey);
            if (roms == null)
                return new Data.Models.Hashfile.Hashfile();

            return new Data.Models.Hashfile.Hashfile
            {
                SFV = hash == HashType.CRC32
                    ? Array.ConvertAll(roms, ConvertToSFV)
                    : null,
                MD2 = hash == HashType.MD2
                    ? Array.ConvertAll(roms, ConvertToMD2)
                    : null,
                MD4 = hash == HashType.MD4
                    ? Array.ConvertAll(roms, ConvertToMD4)
                    : null,
                MD5 = hash == HashType.MD5
                    ? Array.ConvertAll(roms, ConvertToMD5)
                    : null,
                RIPEMD128 = hash == HashType.RIPEMD128
                    ? Array.ConvertAll(roms, ConvertToRIPEMD128)
                    : null,
                RIPEMD160 = hash == HashType.RIPEMD160
                    ? Array.ConvertAll(roms, ConvertToRIPEMD160)
                    : null,
                SHA1 = hash == HashType.SHA1
                    ? Array.ConvertAll(roms, ConvertToSHA1)
                    : null,
                SHA256 = hash == HashType.SHA256
                    ? Array.ConvertAll(roms, ConvertToSHA256)
                    : null,
                SHA384 = hash == HashType.SHA384
                    ? Array.ConvertAll(roms, ConvertToSHA384)
                    : null,
                SHA512 = hash == HashType.SHA512
                    ? Array.ConvertAll(roms, ConvertToSHA512)
                    : null,
                SpamSum = hash == HashType.SpamSum
                    ? Array.ConvertAll(roms, ConvertToSpamSum)
                    : null,
            };
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Rom"/> to <see cref="MD2"/>
        /// </summary>
        private static MD2 ConvertToMD2(Data.Models.Metadata.Rom item)
        {
            var md2 = new MD2
            {
                Hash = item.ReadString(Data.Models.Metadata.Rom.MD2Key),
                File = item.ReadString(Data.Models.Metadata.Rom.NameKey),
            };
            return md2;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Rom"/> to <see cref="MD4"/>
        /// </summary>
        private static MD4 ConvertToMD4(Data.Models.Metadata.Rom item)
        {
            var md4 = new MD4
            {
                Hash = item.ReadString(Data.Models.Metadata.Rom.MD4Key),
                File = item.ReadString(Data.Models.Metadata.Rom.NameKey),
            };
            return md4;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Rom"/> to <see cref="MD5"/>
        /// </summary>
        private static MD5 ConvertToMD5(Data.Models.Metadata.Rom item)
        {
            var md5 = new MD5
            {
                Hash = item.ReadString(Data.Models.Metadata.Rom.MD5Key),
                File = item.ReadString(Data.Models.Metadata.Rom.NameKey),
            };
            return md5;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Rom"/> to <see cref="RIPEMD128"/>
        /// </summary>
        private static RIPEMD128 ConvertToRIPEMD128(Data.Models.Metadata.Rom item)
        {
            var ripemd128 = new RIPEMD128
            {
                Hash = item.ReadString(Data.Models.Metadata.Rom.RIPEMD128Key),
                File = item.ReadString(Data.Models.Metadata.Rom.NameKey),
            };
            return ripemd128;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Rom"/> to <see cref="RIPEMD160"/>
        /// </summary>
        private static RIPEMD160 ConvertToRIPEMD160(Data.Models.Metadata.Rom item)
        {
            var ripemd160 = new RIPEMD160
            {
                Hash = item.ReadString(Data.Models.Metadata.Rom.RIPEMD160Key),
                File = item.ReadString(Data.Models.Metadata.Rom.NameKey),
            };
            return ripemd160;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Rom"/> to <see cref="SFV"/>
        /// </summary>
        private static SFV ConvertToSFV(Data.Models.Metadata.Rom item)
        {
            var sfv = new SFV
            {
                File = item.ReadString(Data.Models.Metadata.Rom.NameKey),
                Hash = item.ReadString(Data.Models.Metadata.Rom.CRCKey),
            };
            return sfv;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Rom"/> to <see cref="SHA1"/>
        /// </summary>
        private static SHA1 ConvertToSHA1(Data.Models.Metadata.Rom item)
        {
            var sha1 = new SHA1
            {
                Hash = item.ReadString(Data.Models.Metadata.Rom.SHA1Key),
                File = item.ReadString(Data.Models.Metadata.Rom.NameKey),
            };
            return sha1;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Rom"/> to <see cref="SHA256"/>
        /// </summary>
        private static SHA256 ConvertToSHA256(Data.Models.Metadata.Rom item)
        {
            var sha256 = new SHA256
            {
                Hash = item.ReadString(Data.Models.Metadata.Rom.SHA256Key),
                File = item.ReadString(Data.Models.Metadata.Rom.NameKey),
            };
            return sha256;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Rom"/> to <see cref="SHA384"/>
        /// </summary>
        private static SHA384 ConvertToSHA384(Data.Models.Metadata.Rom item)
        {
            var sha384 = new SHA384
            {
                Hash = item.ReadString(Data.Models.Metadata.Rom.SHA384Key),
                File = item.ReadString(Data.Models.Metadata.Rom.NameKey),
            };
            return sha384;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Rom"/> to <see cref="SHA512"/>
        /// </summary>
        private static SHA512 ConvertToSHA512(Data.Models.Metadata.Rom item)
        {
            var sha512 = new SHA512
            {
                Hash = item.ReadString(Data.Models.Metadata.Rom.SHA512Key),
                File = item.ReadString(Data.Models.Metadata.Rom.NameKey),
            };
            return sha512;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Rom"/> to <see cref="SpamSum"/>
        /// </summary>
        private static SpamSum ConvertToSpamSum(Data.Models.Metadata.Rom item)
        {
            var spamsum = new SpamSum
            {
                Hash = item.ReadString(Data.Models.Metadata.Rom.SpamSumKey),
                File = item.ReadString(Data.Models.Metadata.Rom.NameKey),
            };
            return spamsum;
        }
    }
}
