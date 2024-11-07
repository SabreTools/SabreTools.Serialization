using System.Collections.Generic;
using System.Linq;
using SabreTools.Hashing;
using SabreTools.Models.Hashfile;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.CrossModel
{
    public partial class Hashfile : IModelSerializer<Models.Hashfile.Hashfile, Models.Metadata.MetadataFile>
    {
        /// <inheritdoc/>
        public Models.Hashfile.Hashfile? Deserialize(Models.Metadata.MetadataFile? obj) => Deserialize(obj, HashType.CRC32);

        /// <inheritdoc/>
        public Models.Hashfile.Hashfile? Deserialize(Models.Metadata.MetadataFile? obj, HashType hash)
        {
            if (obj == null)
                return null;

            var machines = obj.Read<Models.Metadata.Machine[]>(Models.Metadata.MetadataFile.MachineKey);
            if (machines == null || !machines.Any())
                return null;

            var hashfiles = machines
                .Where(m => m != null)
                .Select(machine => ConvertMachineFromInternalModel(machine, hash));

            var sfvs = new List<SFV>();
            var md5s = new List<MD5>();
            var sha1s = new List<SHA1>();
            var sha256s = new List<SHA256>();
            var sha384s = new List<SHA384>();
            var sha512s = new List<SHA512>();
            var spamsums = new List<SpamSum>();

            foreach (var hashfile in hashfiles)
            {
                if (hashfile.SFV != null && hashfile.SFV.Any())
                    sfvs.AddRange(hashfile.SFV);
                if (hashfile.MD5 != null && hashfile.MD5.Any())
                    md5s.AddRange(hashfile.MD5);
                if (hashfile.SHA1 != null && hashfile.SHA1.Any())
                    sha1s.AddRange(hashfile.SHA1);
                if (hashfile.SHA256 != null && hashfile.SHA256.Any())
                    sha256s.AddRange(hashfile.SHA256);
                if (hashfile.SHA384 != null && hashfile.SHA384.Any())
                    sha384s.AddRange(hashfile.SHA384);
                if (hashfile.SHA512 != null && hashfile.SHA512.Any())
                    sha512s.AddRange(hashfile.SHA512);
                if (hashfile.SpamSum != null && hashfile.SpamSum.Any())
                    spamsums.AddRange(hashfile.SpamSum);
            }

            var hashfileItem = new Models.Hashfile.Hashfile();

            if (sfvs.Any())
                hashfileItem.SFV = sfvs.ToArray();
            if (md5s.Any())
                hashfileItem.MD5 = md5s.ToArray();
            if (sha1s.Any())
                hashfileItem.SHA1 = sha1s.ToArray();
            if (sha256s.Any())
                hashfileItem.SHA256 = sha256s.ToArray();
            if (sha384s.Any())
                hashfileItem.SHA384 = sha384s.ToArray();
            if (sha512s.Any())
                hashfileItem.SHA512 = sha512s.ToArray();
            if (spamsums.Any())
                hashfileItem.SpamSum = spamsums.ToArray();

            return hashfileItem;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.MetadataFile"/> to an array of <cref="Models.Hashfile.Hashfile"/>
        /// </summary>
        public static Models.Hashfile.Hashfile[]? ConvertArrayFromInternalModel(Models.Metadata.MetadataFile? item, HashType hash)
        {
            if (item == null)
                return null;

            var machines = item.Read<Models.Metadata.Machine[]>(Models.Metadata.MetadataFile.MachineKey);
            if (machines != null && machines.Any())
            {
                return machines
                    .Where(m => m != null)
                    .Select(machine => ConvertMachineFromInternalModel(machine, hash))
                    .ToArray();
            }

            return null;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Machine"/> to <cref="Models.Hashfile.Hashfile"/>
        /// </summary>
        private static Models.Hashfile.Hashfile ConvertMachineFromInternalModel(Models.Metadata.Machine item, HashType hash)
        {
            var roms = item.Read<Models.Metadata.Rom[]>(Models.Metadata.Machine.RomKey);
            if (roms == null)
                return new Models.Hashfile.Hashfile();

            return new Models.Hashfile.Hashfile
            {
                SFV = hash == HashType.CRC32
                    || hash == HashType.CRC32_AIXM
                    || hash == HashType.CRC32_AUTOSAR
                    || hash == HashType.CRC32_BASE91D
                    || hash == HashType.CRC32_BZIP2
                    || hash == HashType.CRC32_CDROMEDC
                    || hash == HashType.CRC32_CKSUM
                    || hash == HashType.CRC32_ISCSI
                    || hash == HashType.CRC32_ISOHDLC
                    || hash == HashType.CRC32_JAMCRC
                    || hash == HashType.CRC32_MEF
                    || hash == HashType.CRC32_MPEG2
                    || hash == HashType.CRC32_XFER
                        ? roms
                            .Where(r => r != null)
                            .Select(ConvertToSFV)
                            .ToArray()
                        : null,
                MD5 = hash == HashType.MD5
                    ? roms
                        .Where(r => r != null)
                        .Select(ConvertToMD5)
                        .ToArray()
                    : null,
                SHA1 = hash == HashType.SHA1
                    ? roms
                        .Where(r => r != null)
                        .Select(ConvertToSHA1)
                        .ToArray()
                    : null,
                SHA256 = hash == HashType.SHA256
                    ? roms
                        .Where(r => r != null)
                        .Select(ConvertToSHA256)
                        .ToArray()
                    : null,
                SHA384 = hash == HashType.SHA384
                    ? roms
                        .Where(r => r != null)
                        .Select(ConvertToSHA384)
                        .ToArray()
                    : null,
                SHA512 = hash == HashType.SHA512
                    ? roms
                        .Where(r => r != null)
                        .Select(ConvertToSHA512)
                        .ToArray()
                    : null,
                SpamSum = hash == HashType.SpamSum
                    ? roms
                        .Where(r => r != null)
                        .Select(ConvertToSpamSum)
                        .ToArray()
                    : null,
            };
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Rom"/> to <cref="Models.Hashfile.MD5"/>
        /// </summary>
        private static MD5 ConvertToMD5(Models.Metadata.Rom item)
        {
            var md5 = new MD5
            {
                Hash = item.ReadString(Models.Metadata.Rom.MD5Key),
                File = item.ReadString(Models.Metadata.Rom.NameKey),
            };
            return md5;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Rom"/> to <cref="Models.Hashfile.SFV"/>
        /// </summary>
        private static SFV ConvertToSFV(Models.Metadata.Rom item)
        {
            var sfv = new SFV
            {
                File = item.ReadString(Models.Metadata.Rom.NameKey),
                Hash = item.ReadString(Models.Metadata.Rom.CRCKey),
            };
            return sfv;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Rom"/> to <cref="Models.Hashfile.SHA1"/>
        /// </summary>
        private static SHA1 ConvertToSHA1(Models.Metadata.Rom item)
        {
            var sha1 = new SHA1
            {
                Hash = item.ReadString(Models.Metadata.Rom.SHA1Key),
                File = item.ReadString(Models.Metadata.Rom.NameKey),
            };
            return sha1;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Rom"/> to <cref="Models.Hashfile.SHA256"/>
        /// </summary>
        private static SHA256 ConvertToSHA256(Models.Metadata.Rom item)
        {
            var sha256 = new SHA256
            {
                Hash = item.ReadString(Models.Metadata.Rom.SHA256Key),
                File = item.ReadString(Models.Metadata.Rom.NameKey),
            };
            return sha256;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Rom"/> to <cref="Models.Hashfile.SHA384"/>
        /// </summary>
        private static SHA384 ConvertToSHA384(Models.Metadata.Rom item)
        {
            var sha384 = new SHA384
            {
                Hash = item.ReadString(Models.Metadata.Rom.SHA384Key),
                File = item.ReadString(Models.Metadata.Rom.NameKey),
            };
            return sha384;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Rom"/> to <cref="Models.Hashfile.SHA512"/>
        /// </summary>
        private static SHA512 ConvertToSHA512(Models.Metadata.Rom item)
        {
            var sha512 = new SHA512
            {
                Hash = item.ReadString(Models.Metadata.Rom.SHA512Key),
                File = item.ReadString(Models.Metadata.Rom.NameKey),
            };
            return sha512;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Rom"/> to <cref="Models.Hashfile.SpamSum"/>
        /// </summary>
        private static SpamSum ConvertToSpamSum(Models.Metadata.Rom item)
        {
            var spamsum = new SpamSum
            {
                Hash = item.ReadString(Models.Metadata.Rom.SpamSumKey),
                File = item.ReadString(Models.Metadata.Rom.NameKey),
            };
            return spamsum;
        }
    }
}