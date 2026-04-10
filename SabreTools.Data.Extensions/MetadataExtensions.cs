using System;
using SabreTools.Data.Models.Metadata;
using SabreTools.Hashing;
using SabreTools.IO.Extensions;
using SabreTools.Matching;

namespace SabreTools.Data.Extensions
{
    // TODO: Add proper enum for YesNo
    // TODO: Combine yes/partial/no enums
    public static class MetadataExtensions
    {
        #region Conversion

        /// <summary>
        /// Convert a Disk to a Rom
        /// </summary>
        public static Rom? ConvertToRom(this Disk? disk)
        {
            // If the Disk is missing, we can't do anything
            if (disk is null)
                return null;

            // Append a suffix to the name
            string? name = disk.Name;
            if (name is not null)
                name += ".chd";

            return new Rom
            {
                Name = name,
                Merge = disk.Merge,
                Region = disk.Region,
                Status = disk.Status,
                Optional = disk.Optional,
                MD5 = disk.MD5,
                SHA1 = disk.SHA1,
            };
        }

        /// <summary>
        /// Convert a Media to a Rom
        /// </summary>
        public static Rom? ConvertToRom(this Media? media)
        {
            // If the Media is missing, we can't do anything
            if (media is null)
                return null;

            // Append a suffix to the name
            string? name = media.Name;
            if (name is not null)
                name += ".aaruf";

            return new Rom
            {
                Name = name,
                MD5 = media.MD5,
                SHA1 = media.SHA1,
                SHA256 = media.SHA256,
                SpamSum = media.SpamSum,
            };
        }

        #endregion

        #region Equality Checking

        /// <summary>
        /// Check equality of two Disk objects
        /// </summary>
        public static bool PartialEquals(this Disk self, Disk other)
        {
            ItemStatus? selfStatus = self.Status;
            ItemStatus? otherStatus = other.Status;

            string? selfName = self.Name;
            string? otherName = other.Name;

            // If all hashes are empty but they're both nodump and the names match, then they're dupes
            if (selfStatus == ItemStatus.Nodump
                && otherStatus == ItemStatus.Nodump
                && string.Equals(selfName, otherName, StringComparison.OrdinalIgnoreCase)
                && !self.HasHashes()
                && !other.HasHashes())
            {
                return true;
            }

            // If we get a partial match
            if (self.HashMatch(other))
                return true;

            // All other cases fail
            return false;
        }

        /// <summary>
        /// Check equality of two Media objects
        /// </summary>
        public static bool PartialEquals(this Media self, Media other)
        {
            // If we get a partial match
            if (self.HashMatch(other))
                return true;

            // All other cases fail
            return false;
        }

        /// <summary>
        /// Check equality of two Rom objects
        /// </summary>
        public static bool PartialEquals(this Rom self, Rom other)
        {
            ItemStatus? selfStatus = self.Status;
            ItemStatus? otherStatus = other.Status;

            string? selfName = self.Name;
            string? otherName = other.Name;

            long? selfSize = self.Size;
            long? otherSize = other.Size;

            // If all hashes are empty but they're both nodump and the names match, then they're dupes
            if (selfStatus == ItemStatus.Nodump
                && otherStatus == ItemStatus.Nodump
                && string.Equals(selfName, otherName, StringComparison.OrdinalIgnoreCase)
                && !self.HasHashes()
                && !other.HasHashes())
            {
                return true;
            }

            // If we have a file that has no known size, rely on the hashes only
            if (selfSize is null && self.HashMatch(other))
                return true;
            else if (otherSize is null && self.HashMatch(other))
                return true;

            // If we get a partial match
            if (selfSize == otherSize && self.HashMatch(other))
                return true;

            // All other cases fail
            return false;
        }

        #endregion

        #region Hash Checking

        /// <summary>
        /// Determine if two hashes are equal for the purposes of merging
        /// </summary>
        public static bool ConditionalHashEquals(byte[]? firstHash, byte[]? secondHash)
        {
            // If either hash is empty, we say they're equal for merging
            if (firstHash.IsNullOrEmpty() || secondHash.IsNullOrEmpty())
                return true;

            // If they're different sizes, they can't match
            if (firstHash!.Length != secondHash!.Length)
                return false;

            // Otherwise, they need to match exactly
            return firstHash.EqualsExactly(secondHash);
        }

        /// <summary>
        /// Determine if two hashes are equal for the purposes of merging
        /// </summary>
        public static bool ConditionalHashEquals(string? firstHash, string? secondHash)
        {
            // If either hash is empty, we say they're equal for merging
            if (string.IsNullOrEmpty(firstHash) || string.IsNullOrEmpty(secondHash))
                return true;

            // If they're different sizes, they can't match
            if (firstHash!.Length != secondHash!.Length)
                return false;

            // Otherwise, they need to match exactly
            return string.Equals(firstHash, secondHash, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Returns if any hashes are common
        /// </summary>
        public static bool HashMatch(this Disk self, Disk other)
        {
            // If either have no hashes, we return false, otherwise this would be a false positive
            if (!self.HasHashes() || !other.HasHashes())
                return false;

            // If neither have hashes in common, we return false, otherwise this would be a false positive
            if (!self.HasCommonHash(other))
                return false;

            // Return if all hashes match according to merge rules
            bool conditionalMd5 = ConditionalHashEquals(self.MD5, other.MD5);
            bool conditionalSha1 = ConditionalHashEquals(self.SHA1, other.SHA1);

            return conditionalMd5
                && conditionalSha1;
        }

        /// <summary>
        /// Returns if any hashes are common
        /// </summary>
        public static bool HashMatch(this Media self, Media other)
        {
            // If either have no hashes, we return false, otherwise this would be a false positive
            if (!self.HasHashes() || !other.HasHashes())
                return false;

            // If neither have hashes in common, we return false, otherwise this would be a false positive
            if (!self.HasCommonHash(other))
                return false;

            // Return if all hashes match according to merge rules
            bool conditionalMd5 = ConditionalHashEquals(self.MD5, other.MD5);
            bool conditionalSha1 = ConditionalHashEquals(self.SHA1, other.SHA1);
            bool conditionalSha256 = ConditionalHashEquals(self.SHA256, other.SHA256);
            bool conditionalSpamSum = ConditionalHashEquals(self.SpamSum, other.SpamSum);

            return conditionalMd5
                && conditionalSha1
                && conditionalSha256
                && conditionalSpamSum;
        }

        /// <summary>
        /// Returns if any hashes are common
        /// </summary>
        public static bool HashMatch(this Rom self, Rom other)
        {
            // If either have no hashes, we return false, otherwise this would be a false positive
            if (!self.HasHashes() || !other.HasHashes())
                return false;

            // If neither have hashes in common, we return false, otherwise this would be a false positive
            if (!self.HasCommonHash(other))
                return false;

            // Return if all hashes match according to merge rules
            string? selfCrc16 = self.CRC16;
            string? otherCrc16 = other.CRC16;
            bool conditionalCrc16 = ConditionalHashEquals(selfCrc16, otherCrc16);

            string? selfCrc = self.CRC32;
            string? otherCrc = other.CRC32;
            bool conditionalCrc = ConditionalHashEquals(selfCrc, otherCrc);

            string? selfCrc64 = self.CRC64;
            string? otherCrc64 = other.CRC64;
            bool conditionalCrc64 = ConditionalHashEquals(selfCrc64, otherCrc64);

            string? selfMd2 = self.MD2;
            string? otherMd2 = other.MD2;
            bool conditionalMd2 = ConditionalHashEquals(selfMd2, otherMd2);

            string? selfMd4 = self.MD4;
            string? otherMd4 = other.MD4;
            bool conditionalMd4 = ConditionalHashEquals(selfMd4, otherMd4);

            string? selfMd5 = self.MD5;
            string? otherMd5 = other.MD5;
            bool conditionalMd5 = ConditionalHashEquals(selfMd5, otherMd5);

            string? selfRipeMD128 = self.RIPEMD128;
            string? otherRipeMD128 = other.RIPEMD128;
            bool conditionaRipeMD128 = ConditionalHashEquals(selfRipeMD128, otherRipeMD128);

            string? selfRipeMD160 = self.RIPEMD160;
            string? otherRipeMD160 = other.RIPEMD160;
            bool conditionaRipeMD160 = ConditionalHashEquals(selfRipeMD160, otherRipeMD160);

            string? selfSha1 = self.SHA1;
            string? otherSha1 = other.SHA1;
            bool conditionalSha1 = ConditionalHashEquals(selfSha1, otherSha1);

            string? selfSha256 = self.SHA256;
            string? otherSha256 = other.SHA256;
            bool conditionalSha256 = ConditionalHashEquals(selfSha256, otherSha256);

            string? selfSha384 = self.SHA384;
            string? otherSha384 = other.SHA384;
            bool conditionalSha384 = ConditionalHashEquals(selfSha384, otherSha384);

            string? selfSha512 = self.SHA512;
            string? otherSha512 = other.SHA512;
            bool conditionalSha512 = ConditionalHashEquals(selfSha512, otherSha512);

            string? selfSpamSum = self.SpamSum;
            string? otherSpamSum = other.SpamSum;
            bool conditionalSpamSum = ConditionalHashEquals(selfSpamSum, otherSpamSum);

            return conditionalCrc16
                && conditionalCrc
                && conditionalCrc64
                && conditionalMd2
                && conditionalMd4
                && conditionalMd5
                && conditionaRipeMD128
                && conditionaRipeMD160
                && conditionalSha1
                && conditionalSha256
                && conditionalSha384
                && conditionalSha512
                && conditionalSpamSum;
        }

        /// <summary>
        /// Returns if any hashes exist
        /// </summary>
        public static bool HasHashes(this Disk disk)
        {
            bool md5Null = string.IsNullOrEmpty(disk.MD5);
            bool sha1Null = string.IsNullOrEmpty(disk.SHA1);

            return !md5Null
                || !sha1Null;
        }

        /// <summary>
        /// Returns if any hashes exist
        /// </summary>
        public static bool HasHashes(this Media media)
        {
            bool md5Null = string.IsNullOrEmpty(media.MD5);
            bool sha1Null = string.IsNullOrEmpty(media.SHA1);
            bool sha256Null = string.IsNullOrEmpty(media.SHA256);
            bool spamsumNull = string.IsNullOrEmpty(media.SpamSum);

            return !md5Null
                || !sha1Null
                || !sha256Null
                || !spamsumNull;
        }

        /// <summary>
        /// Returns if any hashes exist
        /// </summary>
        public static bool HasHashes(this Rom rom)
        {
            bool crc16Null = string.IsNullOrEmpty(rom.CRC16);
            bool crc32Null = string.IsNullOrEmpty(rom.CRC32);
            bool crc64Null = string.IsNullOrEmpty(rom.CRC64);
            bool md2Null = string.IsNullOrEmpty(rom.MD2);
            bool md4Null = string.IsNullOrEmpty(rom.MD4);
            bool md5Null = string.IsNullOrEmpty(rom.MD5);
            bool ripeMD128Null = string.IsNullOrEmpty(rom.RIPEMD128);
            bool ripeMD160Null = string.IsNullOrEmpty(rom.RIPEMD160);
            bool sha1Null = string.IsNullOrEmpty(rom.SHA1);
            bool sha256Null = string.IsNullOrEmpty(rom.SHA256);
            bool sha384Null = string.IsNullOrEmpty(rom.SHA384);
            bool sha512Null = string.IsNullOrEmpty(rom.SHA512);
            bool spamsumNull = string.IsNullOrEmpty(rom.SpamSum);

            return !crc16Null
                || !crc32Null
                || !crc64Null
                || !md2Null
                || !md4Null
                || !md5Null
                || !ripeMD128Null
                || !ripeMD160Null
                || !sha1Null
                || !sha256Null
                || !sha384Null
                || !sha512Null
                || !spamsumNull;
        }

        /// <summary>
        /// Returns if all of the hashes are set to their 0-byte values or null
        /// </summary>
        public static bool HasZeroHash(this Disk disk)
        {
            string? md5 = disk.MD5;
            bool md5Null = string.IsNullOrEmpty(md5) || string.Equals(md5, HashType.MD5.ZeroString, StringComparison.OrdinalIgnoreCase);

            string? sha1 = disk.SHA1;
            bool sha1Null = string.IsNullOrEmpty(sha1) || string.Equals(sha1, HashType.SHA1.ZeroString, StringComparison.OrdinalIgnoreCase);

            return md5Null
                && sha1Null;
        }

        /// <summary>
        /// Returns if all of the hashes are set to their 0-byte values or null
        /// </summary>
        public static bool HasZeroHash(this Media media)
        {
            string? md5 = media.MD5;
            bool md5Null = string.IsNullOrEmpty(md5) || string.Equals(md5, HashType.MD5.ZeroString, StringComparison.OrdinalIgnoreCase);

            string? sha1 = media.SHA1;
            bool sha1Null = string.IsNullOrEmpty(sha1) || string.Equals(sha1, HashType.SHA1.ZeroString, StringComparison.OrdinalIgnoreCase);

            string? sha256 = media.SHA256;
            bool sha256Null = string.IsNullOrEmpty(sha256) || string.Equals(sha256, HashType.SHA256.ZeroString, StringComparison.OrdinalIgnoreCase);

            string? spamsum = media.SpamSum;
            bool spamsumNull = string.IsNullOrEmpty(spamsum) || string.Equals(spamsum, HashType.SpamSum.ZeroString, StringComparison.OrdinalIgnoreCase);

            return md5Null
                && sha1Null
                && sha256Null
                && spamsumNull;
        }

        /// <summary>
        /// Returns if all of the hashes are set to their 0-byte values or null
        /// </summary>
        public static bool HasZeroHash(this Rom rom)
        {
            string? crc16 = rom.CRC16;
            bool crc16Null = string.IsNullOrEmpty(crc16) || string.Equals(crc16, HashType.CRC16.ZeroString, StringComparison.OrdinalIgnoreCase);

            string? crc32 = rom.CRC32;
            bool crc32Null = string.IsNullOrEmpty(crc32) || string.Equals(crc32, HashType.CRC32.ZeroString, StringComparison.OrdinalIgnoreCase);

            string? crc64 = rom.CRC64;
            bool crc64Null = string.IsNullOrEmpty(crc64) || string.Equals(crc64, HashType.CRC64.ZeroString, StringComparison.OrdinalIgnoreCase);

            string? md2 = rom.MD2;
            bool md2Null = string.IsNullOrEmpty(md2) || string.Equals(md2, HashType.MD2.ZeroString, StringComparison.OrdinalIgnoreCase);

            string? md4 = rom.MD4;
            bool md4Null = string.IsNullOrEmpty(md4) || string.Equals(md4, HashType.MD4.ZeroString, StringComparison.OrdinalIgnoreCase);

            string? md5 = rom.MD5;
            bool md5Null = string.IsNullOrEmpty(md5) || string.Equals(md5, HashType.MD5.ZeroString, StringComparison.OrdinalIgnoreCase);

            string? ripeMD128 = rom.RIPEMD128;
            bool ripeMD128Null = string.IsNullOrEmpty(value: ripeMD128) || string.Equals(ripeMD128, HashType.RIPEMD128.ZeroString, StringComparison.OrdinalIgnoreCase);

            string? ripeMD160 = rom.RIPEMD160;
            bool ripeMD160Null = string.IsNullOrEmpty(ripeMD160) || string.Equals(ripeMD160, HashType.RIPEMD160.ZeroString, StringComparison.OrdinalIgnoreCase);

            string? sha1 = rom.SHA1;
            bool sha1Null = string.IsNullOrEmpty(sha1) || string.Equals(sha1, HashType.SHA1.ZeroString, StringComparison.OrdinalIgnoreCase);

            string? sha256 = rom.SHA256;
            bool sha256Null = string.IsNullOrEmpty(sha256) || string.Equals(sha256, HashType.SHA256.ZeroString, StringComparison.OrdinalIgnoreCase);

            string? sha384 = rom.SHA384;
            bool sha384Null = string.IsNullOrEmpty(sha384) || string.Equals(sha384, HashType.SHA384.ZeroString, StringComparison.OrdinalIgnoreCase);

            string? sha512 = rom.SHA512;
            bool sha512Null = string.IsNullOrEmpty(sha512) || string.Equals(sha512, HashType.SHA512.ZeroString, StringComparison.OrdinalIgnoreCase);

            string? spamsum = rom.SpamSum;
            bool spamsumNull = string.IsNullOrEmpty(spamsum) || string.Equals(spamsum, HashType.SpamSum.ZeroString, StringComparison.OrdinalIgnoreCase);

            return crc16Null
                && crc32Null
                && crc64Null
                && md2Null
                && md4Null
                && md5Null
                && ripeMD128Null
                && ripeMD160Null
                && sha1Null
                && sha256Null
                && sha384Null
                && sha512Null
                && spamsumNull;
        }

        /// <summary>
        /// Returns if there are no, non-empty hashes in common
        /// </summary>
        private static bool HasCommonHash(this Disk self, Disk other)
        {
            bool md5Null = string.IsNullOrEmpty(self.MD5);
            md5Null ^= string.IsNullOrEmpty(other.MD5);

            bool sha1Null = string.IsNullOrEmpty(self.SHA1);
            sha1Null ^= string.IsNullOrEmpty(other.SHA1);

            return !md5Null
                || !sha1Null;
        }

        /// <summary>
        /// Returns if there are no, non-empty hashes in common
        /// </summary>
        private static bool HasCommonHash(this Media self, Media other)
        {
            bool md5Null = string.IsNullOrEmpty(self.MD5);
            md5Null ^= string.IsNullOrEmpty(other.MD5);

            bool sha1Null = string.IsNullOrEmpty(self.SHA1);
            sha1Null ^= string.IsNullOrEmpty(other.SHA1);

            bool sha256Null = string.IsNullOrEmpty(self.SHA256);
            sha256Null ^= string.IsNullOrEmpty(other.SHA256);

            bool spamsumNull = string.IsNullOrEmpty(self.SpamSum);
            spamsumNull ^= string.IsNullOrEmpty(other.SpamSum);

            return !md5Null
                || !sha1Null
                || !sha256Null
                || !spamsumNull;
        }

        /// <summary>
        /// Returns if there are no, non-empty hashes in common
        /// </summary>
        private static bool HasCommonHash(this Rom self, Rom other)
        {
            bool crc16Null = string.IsNullOrEmpty(self.CRC16);
            crc16Null ^= string.IsNullOrEmpty(other.CRC16);

            bool crc32Null = string.IsNullOrEmpty(self.CRC32);
            crc32Null ^= string.IsNullOrEmpty(other.CRC32);

            bool crc64Null = string.IsNullOrEmpty(self.CRC64);
            crc64Null ^= string.IsNullOrEmpty(other.CRC64);

            bool md2Null = string.IsNullOrEmpty(self.MD2);
            md2Null ^= string.IsNullOrEmpty(other.MD2);

            bool md4Null = string.IsNullOrEmpty(self.MD4);
            md4Null ^= string.IsNullOrEmpty(other.MD4);

            bool md5Null = string.IsNullOrEmpty(self.MD5);
            md5Null ^= string.IsNullOrEmpty(other.MD5);

            bool ripeMD128Null = string.IsNullOrEmpty(self.RIPEMD128);
            ripeMD128Null ^= string.IsNullOrEmpty(other.RIPEMD128);

            bool ripeMD160Null = string.IsNullOrEmpty(self.RIPEMD160);
            ripeMD160Null ^= string.IsNullOrEmpty(other.RIPEMD160);

            bool sha1Null = string.IsNullOrEmpty(self.SHA1);
            sha1Null ^= string.IsNullOrEmpty(other.SHA1);

            bool sha256Null = string.IsNullOrEmpty(self.SHA256);
            sha256Null ^= string.IsNullOrEmpty(other.SHA256);

            bool sha384Null = string.IsNullOrEmpty(self.SHA384);
            sha384Null ^= string.IsNullOrEmpty(other.SHA384);

            bool sha512Null = string.IsNullOrEmpty(self.SHA512);
            sha512Null ^= string.IsNullOrEmpty(other.SHA512);

            bool spamsumNull = string.IsNullOrEmpty(self.SpamSum);
            spamsumNull ^= string.IsNullOrEmpty(other.SpamSum);

            return !crc16Null
                || !crc32Null
                || !crc64Null
                || !md2Null
                || !md4Null
                || !md5Null
                || !ripeMD128Null
                || !ripeMD160Null
                || !sha1Null
                || !sha256Null
                || !sha384Null
                || !sha512Null
                || !spamsumNull;
        }

        #endregion

        #region Information Filling

        /// <summary>
        /// Fill any missing size and hash information from another Disk
        /// </summary>
        public static void FillMissingHashes(this Disk? self, Disk? other)
        {
            if (self is null || other is null)
                return;

            string? selfMd5 = self.MD5;
            string? otherMd5 = other.MD5;
            if (string.IsNullOrEmpty(selfMd5) && !string.IsNullOrEmpty(otherMd5))
                self.MD5 = otherMd5;

            string? selfSha1 = self.SHA1;
            string? otherSha1 = other.SHA1;
            if (string.IsNullOrEmpty(selfSha1) && !string.IsNullOrEmpty(otherSha1))
                self.SHA1 = otherSha1;
        }

        /// <summary>
        /// Fill any missing size and hash information from another Media
        /// </summary>
        public static void FillMissingHashes(this Media? self, Media? other)
        {
            if (self is null || other is null)
                return;

            string? selfMd5 = self.MD5;
            string? otherMd5 = other.MD5;
            if (string.IsNullOrEmpty(selfMd5) && !string.IsNullOrEmpty(otherMd5))
                self.MD5 = otherMd5;

            string? selfSha1 = self.SHA1;
            string? otherSha1 = other.SHA1;
            if (string.IsNullOrEmpty(selfSha1) && !string.IsNullOrEmpty(otherSha1))
                self.SHA1 = otherSha1;

            string? selfSha256 = self.SHA256;
            string? otherSha256 = other.SHA256;
            if (string.IsNullOrEmpty(selfSha256) && !string.IsNullOrEmpty(otherSha256))
                self.SHA256 = otherSha256;

            string? selfSpamSum = self.SpamSum;
            string? otherSpamSum = other.SpamSum;
            if (string.IsNullOrEmpty(selfSpamSum) && !string.IsNullOrEmpty(otherSpamSum))
                self.SpamSum = otherSpamSum;
        }

        /// <summary>
        /// Fill any missing size and hash information from another Rom
        /// </summary>
        public static void FillMissingHashes(this Rom? self, Rom? other)
        {
            if (self is null || other is null)
                return;

            long? selfSize = self.Size;
            long? otherSize = other.Size;
            if (selfSize is null && otherSize is not null)
                self.Size = otherSize;

            string? selfCrc16 = self.CRC16;
            string? otherCrc16 = other.CRC16;
            if (string.IsNullOrEmpty(selfCrc16) && !string.IsNullOrEmpty(otherCrc16))
                self.CRC16 = otherCrc16;

            string? selfCrc = self.CRC32;
            string? otherCrc = other.CRC32;
            if (string.IsNullOrEmpty(selfCrc) && !string.IsNullOrEmpty(otherCrc))
                self.CRC32 = otherCrc;

            string? selfCrc64 = self.CRC64;
            string? otherCrc64 = other.CRC64;
            if (string.IsNullOrEmpty(selfCrc64) && !string.IsNullOrEmpty(otherCrc64))
                self.CRC64 = otherCrc64;

            string? selfMd2 = self.MD2;
            string? otherMd2 = other.MD2;
            if (string.IsNullOrEmpty(selfMd2) && !string.IsNullOrEmpty(otherMd2))
                self.MD2 = otherMd2;

            string? selfMd4 = self.MD4;
            string? otherMd4 = other.MD4;
            if (string.IsNullOrEmpty(selfMd4) && !string.IsNullOrEmpty(otherMd4))
                self.MD4 = otherMd4;

            string? selfMd5 = self.MD5;
            string? otherMd5 = other.MD5;
            if (string.IsNullOrEmpty(selfMd5) && !string.IsNullOrEmpty(otherMd5))
                self.MD5 = otherMd5;

            string? selfRipeMD128 = self.RIPEMD128;
            string? otherRipeMD128 = other.RIPEMD128;
            if (string.IsNullOrEmpty(selfRipeMD128) && !string.IsNullOrEmpty(otherRipeMD128))
                self.RIPEMD128 = otherRipeMD128;

            string? selfRipeMD160 = self.RIPEMD160;
            string? otherRipeMD160 = other.RIPEMD160;
            if (string.IsNullOrEmpty(selfRipeMD160) && !string.IsNullOrEmpty(otherRipeMD160))
                self.RIPEMD160 = otherRipeMD160;

            string? selfSha1 = self.SHA1;
            string? otherSha1 = other.SHA1;
            if (string.IsNullOrEmpty(selfSha1) && !string.IsNullOrEmpty(otherSha1))
                self.SHA1 = otherSha1;

            string? selfSha256 = self.SHA256;
            string? otherSha256 = other.SHA256;
            if (string.IsNullOrEmpty(selfSha256) && !string.IsNullOrEmpty(otherSha256))
                self.SHA256 = otherSha256;

            string? selfSha384 = self.SHA384;
            string? otherSha384 = other.SHA384;
            if (string.IsNullOrEmpty(selfSha384) && !string.IsNullOrEmpty(otherSha384))
                self.SHA384 = otherSha384;

            string? selfSha512 = self.SHA512;
            string? otherSha512 = other.SHA512;
            if (string.IsNullOrEmpty(selfSha512) && !string.IsNullOrEmpty(otherSha512))
                self.SHA512 = otherSha512;

            string? selfSpamSum = self.SpamSum;
            string? otherSpamSum = other.SpamSum;
            if (string.IsNullOrEmpty(selfSpamSum) && !string.IsNullOrEmpty(otherSpamSum))
                self.SpamSum = otherSpamSum;
        }

        #endregion

        #region String to Enum

        /// <summary>
        /// Get the enum value for an input string, if possible
        /// </summary>
        /// <param name="value">String value to parse/param>
        /// <returns>Enum value representing the input, null on error</returns>
        public static Blit? AsBlit(this string? value)
        {
            return value?.ToLowerInvariant() switch
            {
                "plain" => Blit.Plain,
                "dirty" => Blit.Dirty,
                _ => null,
            };
        }

        /// <summary>
        /// Get the enum value for an input string, if possible
        /// </summary>
        /// <param name="value">String value to parse/param>
        /// <returns>Enum value representing the input, null on error</returns>
        public static ChipType? AsChipType(this string? value)
        {
            return value?.ToLowerInvariant() switch
            {
                "cpu" => ChipType.CPU,
                "audio" => ChipType.Audio,
                _ => null,
            };
        }

        /// <summary>
        /// Get the enum value for an input string, if possible
        /// </summary>
        /// <param name="value">String value to parse/param>
        /// <returns>Enum value representing the input, null on error</returns>
        public static ControlType? AsControlType(this string? value)
        {
            return value?.ToLowerInvariant() switch
            {
                "joy" => ControlType.Joy,
                "stick" => ControlType.Stick,
                "paddle" => ControlType.Paddle,
                "pedal" => ControlType.Pedal,
                "lightgun" => ControlType.Lightgun,
                "positional" => ControlType.Positional,
                "dial" => ControlType.Dial,
                "trackball" => ControlType.Trackball,
                "mouse" => ControlType.Mouse,
                "only_buttons" => ControlType.OnlyButtons,
                "keypad" => ControlType.Keypad,
                "keyboard" => ControlType.Keyboard,
                "mahjong" => ControlType.Mahjong,
                "hanafuda" => ControlType.Hanafuda,
                "gambling" => ControlType.Gambling,
                _ => null,
            };
        }

        /// <summary>
        /// Get the enum value for an input string, if possible
        /// </summary>
        /// <param name="value">String value to parse/param>
        /// <returns>Enum value representing the input, null on error</returns>
        public static DeviceType? AsDeviceType(this string? value)
        {
            return value?.ToLowerInvariant() switch
            {
                "unknown" => DeviceType.Unknown,
                "cartridge" => DeviceType.Cartridge,
                "floppydisk" => DeviceType.FloppyDisk,
                "harddisk" => DeviceType.HardDisk,
                "cylinder" => DeviceType.Cylinder,
                "cassette" => DeviceType.Cassette,
                "punchcard" => DeviceType.PunchCard,
                "punchtape" => DeviceType.PunchTape,
                "printout" => DeviceType.Printout,
                "serial" => DeviceType.Serial,
                "parallel" => DeviceType.Parallel,
                "snapshot" => DeviceType.Snapshot,
                "quickload" => DeviceType.QuickLoad,
                "memcard" => DeviceType.MemCard,
                "cdrom" => DeviceType.CDROM,
                "magtape" => DeviceType.MagTape,
                "romimage" => DeviceType.ROMImage,
                "midiin" => DeviceType.MIDIIn,
                "midiout" => DeviceType.MIDIOut,
                "picture" => DeviceType.Picture,
                "vidfile" => DeviceType.VidFile,
                _ => null,
            };
        }

        /// <summary>
        /// Get the enum value for an input string, if possible
        /// </summary>
        /// <param name="value">String value to parse/param>
        /// <returns>Enum value representing the input, null on error</returns>
        public static DisplayType? AsDisplayType(this string? value)
        {
            return value?.ToLowerInvariant() switch
            {
                "raster" => DisplayType.Raster,
                "vector" => DisplayType.Vector,
                "lcd" => DisplayType.LCD,
                "svg" => DisplayType.SVG,
                "unknown" => DisplayType.Unknown,
                _ => null,
            };
        }

        /// <summary>
        /// Get the enum value for an input string, if possible
        /// </summary>
        /// <param name="value">String value to parse/param>
        /// <returns>Enum value representing the input, null on error</returns>
        public static Endianness? AsEndianness(this string? value)
        {
            return value?.ToLowerInvariant() switch
            {
                "big" => Endianness.Big,
                "little" => Endianness.Little,
                _ => null,
            };
        }

        /// <summary>
        /// Get the enum value for an input string, if possible
        /// </summary>
        /// <param name="value">String value to parse/param>
        /// <returns>Enum value representing the input, null on error</returns>
        public static FeatureStatus? AsFeatureStatus(this string? value)
        {
            return value?.ToLowerInvariant() switch
            {
                "unemulated" => FeatureStatus.Unemulated,
                "imperfect" => FeatureStatus.Imperfect,
                _ => null,
            };
        }

        /// <summary>
        /// Get the enum value for an input string, if possible
        /// </summary>
        /// <param name="value">String value to parse/param>
        /// <returns>Enum value representing the input, null on error</returns>
        public static FeatureType? AsFeatureType(this string? value)
        {
            return value?.ToLowerInvariant() switch
            {
                "protection" => FeatureType.Protection,
                "palette" => FeatureType.Palette,
                "graphics" => FeatureType.Graphics,
                "sound" => FeatureType.Sound,
                "controls" => FeatureType.Controls,
                "keyboard" => FeatureType.Keyboard,
                "mouse" => FeatureType.Mouse,
                "microphone" => FeatureType.Microphone,
                "camera" => FeatureType.Camera,
                "disk" => FeatureType.Disk,
                "printer" => FeatureType.Printer,
                "lan" => FeatureType.Lan,
                "wan" => FeatureType.Wan,
                "timing" => FeatureType.Timing,
                _ => null,
            };
        }

        /// <summary>
        /// Get the enum value for an input string, if possible
        /// </summary>
        /// <param name="value">String value to parse/param>
        /// <returns>Enum value representing the input, null on error</returns>
        public static ItemStatus? AsItemStatus(this string? value)
        {
            return value?.ToLowerInvariant() switch
            {
                "none" or "no" => ItemStatus.None,
                "good" => ItemStatus.Good,
                "baddump" => ItemStatus.BadDump,
                "nodump" or "yes" => ItemStatus.Nodump,
                "verified" => ItemStatus.Verified,
                "deduped" => ItemStatus.Deduped,
                _ => null,
            };
        }

        /// <summary>
        /// Get the enum value for an input string, if possible
        /// </summary>
        /// <param name="value">String value to parse/param>
        /// <returns>Enum value representing the input, default on error</returns>
        public static ItemType AsItemType(this string? value)
        {
            return value?.ToLowerInvariant() switch
            {
                // "Actionable" item types
                "rom" => ItemType.Rom,
                "disk" => ItemType.Disk,
                "file" => ItemType.File,
                "media" => ItemType.Media,

                // "Auxiliary" item types
                "adjuster" => ItemType.Adjuster,
                "analog" => ItemType.Analog,
                "archive" => ItemType.Archive,
                "biosset" => ItemType.BiosSet,
                "chip" => ItemType.Chip,
                "configuration" => ItemType.Configuration,
                "conflocation" => ItemType.ConfLocation,
                "confsetting" => ItemType.ConfSetting,
                "control" => ItemType.Control,
                "dataarea" => ItemType.DataArea,
                "device" => ItemType.Device,
                "device_ref" or "deviceref" => ItemType.DeviceRef,
                "diplocation" => ItemType.DipLocation,
                "dipswitch" => ItemType.DipSwitch,
                "dipvalue" => ItemType.DipValue,
                "diskarea" => ItemType.DiskArea,
                "display" => ItemType.Display,
                "driver" => ItemType.Driver,
                "dump" => ItemType.Dump,
                "feature" => ItemType.Feature,
                "info" => ItemType.Info,
                "input" => ItemType.Input,
                "original" => ItemType.Original,
                "part" => ItemType.Part,
                "part_feature" or "partfeature" => ItemType.PartFeature,
                "port" => ItemType.Port,
                "ramoption" or "ram_option" => ItemType.RamOption,
                "release" => ItemType.Release,
                "release_details" or "releasedetails" => ItemType.ReleaseDetails,
                "sample" => ItemType.Sample,
                "serials" => ItemType.Serials,
                "sharedfeat" or "shared_feat" or "sharedfeature" or "shared_feature" => ItemType.SharedFeat,
                "slot" => ItemType.Slot,
                "slotoption" or "slot_option" => ItemType.SlotOption,
                "softwarelist" or "software_list" => ItemType.SoftwareList,
                "sound" => ItemType.Sound,
                "source_details" or "sourcedetails" => ItemType.SourceDetails,
                "video" => ItemType.Video,
                "blank" => ItemType.Blank,
                _ => ItemType.NULL,
            };
        }

        /// <summary>
        /// Get the enum value for an input string, if possible
        /// </summary>
        /// <param name="value">String value to parse/param>
        /// <returns>Enum value representing the input, null on error</returns>
        public static LoadFlag? AsLoadFlag(this string? value)
        {
            return value?.ToLowerInvariant() switch
            {
                "load16_byte" => LoadFlag.Load16Byte,
                "load16_word" => LoadFlag.Load16Word,
                "load16_word_swap" => LoadFlag.Load16WordSwap,
                "load32_byte" => LoadFlag.Load32Byte,
                "load32_word" => LoadFlag.Load32Word,
                "load32_word_swap" => LoadFlag.Load32WordSwap,
                "load32_dword" => LoadFlag.Load32DWord,
                "load64_word" => LoadFlag.Load64Word,
                "load64_word_swap" => LoadFlag.Load64WordSwap,
                "reload" => LoadFlag.Reload,
                "fill" => LoadFlag.Fill,
                "continue" => LoadFlag.Continue,
                "reload_plain" => LoadFlag.ReloadPlain,
                "ignore" => LoadFlag.Ignore,
                _ => null,
            };
        }

        /// <summary>
        /// Get the enum value for an input string, if possible
        /// </summary>
        /// <param name="value">String value to parse/param>
        /// <returns>Enum value representing the input, default on error</returns>
        public static MergingFlag AsMergingFlag(this string? value)
        {
            return value?.ToLowerInvariant() switch
            {
                "none" => MergingFlag.None,
                "split" => MergingFlag.Split,
                "merged" => MergingFlag.Merged,
                "nonmerged" or "unmerged" => MergingFlag.NonMerged,
                "fullmerged" => MergingFlag.FullMerged,
                "device" or "deviceunmerged" or "devicenonmerged" => MergingFlag.DeviceNonMerged,
                "full" or "fullunmerged" or "fullnonmerged" => MergingFlag.FullNonMerged,
                _ => MergingFlag.None,
            };
        }

        /// <summary>
        /// Get the enum value for an input string, if possible
        /// </summary>
        /// <param name="value">String value to parse/param>
        /// <returns>Enum value representing the input, default on error</returns>
        public static NodumpFlag AsNodumpFlag(this string? value)
        {
            return value?.ToLowerInvariant() switch
            {
                "none" => NodumpFlag.None,
                "obsolete" => NodumpFlag.Obsolete,
                "required" => NodumpFlag.Required,
                "ignore" => NodumpFlag.Ignore,
                _ => NodumpFlag.None,
            };
        }

        /// <summary>
        /// Get the enum value for an input string, if possible
        /// </summary>
        /// <param name="value">String value to parse/param>
        /// <returns>Enum value representing the input, null on error</returns>
        public static OpenMSXSubType? AsOpenMSXSubType(this string? value)
        {
            return value?.ToLowerInvariant() switch
            {
                "rom" => OpenMSXSubType.Rom,
                "megarom" => OpenMSXSubType.MegaRom,
                "sccpluscart" => OpenMSXSubType.SCCPlusCart,
                _ => null,
            };
        }

        /// <summary>
        /// Get the enum value for an input string, if possible
        /// </summary>
        /// <param name="value">String value to parse/param>
        /// <returns>Enum value representing the input, default on error</returns>
        public static PackingFlag AsPackingFlag(this string? value)
        {
            return value?.ToLowerInvariant() switch
            {
                "none" => PackingFlag.None,
                "zip" or "yes" => PackingFlag.Zip,
                "unzip" or "no" => PackingFlag.Unzip,
                "partial" => PackingFlag.Partial,
                "flat" => PackingFlag.Flat,
                "fileonly" => PackingFlag.FileOnly,
                _ => PackingFlag.None,
            };
        }

        /// <summary>
        /// Get the enum value for an input string, if possible
        /// </summary>
        /// <param name="value">String value to parse/param>
        /// <returns>Enum value representing the input, null on error</returns>
        public static Relation? AsRelation(this string? value)
        {
            return value?.ToLowerInvariant() switch
            {
                "eq" => Relation.Equal,
                "ne" => Relation.NotEqual,
                "gt" => Relation.GreaterThan,
                "le" => Relation.LessThanOrEqual,
                "lt" => Relation.LessThan,
                "ge" => Relation.GreaterThanOrEqual,
                _ => null,
            };
        }

        /// <summary>
        /// Get the enum value for an input string, if possible
        /// </summary>
        /// <param name="value">String value to parse/param>
        /// <returns>Enum value representing the input, null on error</returns>
        public static Rotation? AsRotation(this string? value)
        {
            return value?.ToLowerInvariant() switch
            {
                "0" or "north" or "vertical" => Rotation.North,
                "90" or "east" or "horizontal" => Rotation.East,
                "180" or "south" => Rotation.South,
                "270" or "west" => Rotation.West,
                _ => null,
            };
        }

        /// <summary>
        /// Get the enum value for an input string, if possible
        /// </summary>
        /// <param name="value">String value to parse/param>
        /// <returns>Enum value representing the input, null on error</returns>
        public static Runnable? AsRunnable(this string? value)
        {
            return value?.ToLowerInvariant() switch
            {
                "no" => Runnable.No,
                "partial" => Runnable.Partial,
                "yes" => Runnable.Yes,
                _ => null,
            };
        }

        /// <summary>
        /// Get the enum value for an input string, if possible
        /// </summary>
        /// <param name="value">String value to parse/param>
        /// <returns>Enum value representing the input, null on error</returns>
        public static SoftwareListStatus? AsSoftwareListStatus(this string? value)
        {
            return value?.ToLowerInvariant() switch
            {
                "none" => SoftwareListStatus.None,
                "original" => SoftwareListStatus.Original,
                "compatible" => SoftwareListStatus.Compatible,
                _ => null,
            };
        }

        /// <summary>
        /// Get the enum value for an input string, if possible
        /// </summary>
        /// <param name="value">String value to parse/param>
        /// <returns>Enum value representing the input, null on error</returns>
        public static Supported? AsSupported(this string? value)
        {
            return value?.ToLowerInvariant() switch
            {
                "no" or "unsupported" => Supported.No,
                "partial" => Supported.Partial,
                "yes" or "supported" => Supported.Yes,
                _ => null,
            };
        }

        /// <summary>
        /// Get the enum value for an input string, if possible
        /// </summary>
        /// <param name="value">String value to parse/param>
        /// <returns>Enum value representing the input, null on error</returns>
        public static SupportStatus? AsSupportStatus(this string? value)
        {
            return value?.ToLowerInvariant() switch
            {
                "good" => SupportStatus.Good,
                "imperfect" => SupportStatus.Imperfect,
                "preliminary" => SupportStatus.Preliminary,
                _ => null,
            };
        }

        /// <summary>
        /// Get the enum value for an input string, if possible
        /// </summary>
        /// <param name="value">String value to parse/param>
        /// <returns>Enum value representing the input, null on error</returns>
        public static Width? AsWidth(this string? value)
        {
            return value?.ToLowerInvariant() switch
            {
                "8" or "byte" => Width.Byte,
                "16" or "short" => Width.Short,
                "32" or "int" => Width.Int,
                "64" or "long" => Width.Long,
                _ => null,
            };
        }

        /// <summary>
        /// Get bool? value from input string
        /// </summary>
        /// <param name="yesno">String to get value from</param>
        /// <returns>bool? corresponding to the string</returns>
        public static bool? AsYesNo(this string? yesno)
        {
            return yesno?.ToLowerInvariant() switch
            {
                "1" or "yes" or "true" => true,
                "0" or "no" or "false" => false,
                _ => null,
            };
        }

        #endregion

        #region Enum to String

        /// <summary>
        /// Get the string value for an input enum, if possible
        /// </summary>
        /// <param name="value">Enum value to parse/param>
        /// <returns>String value representing the input, null on error</returns>
        public static string? AsStringValue(this Blit value)
        {
            return value switch
            {
                Blit.Plain => "plain",
                Blit.Dirty => "dirty",
                _ => null,
            };
        }

        /// <summary>
        /// Get the string value for an input enum, if possible
        /// </summary>
        /// <param name="value">Enum value to parse/param>
        /// <returns>String value representing the input, null on error</returns>
        public static string? AsStringValue(this ChipType value)
        {
            return value switch
            {
                ChipType.CPU => "cpu",
                ChipType.Audio => "audio",
                _ => null,
            };
        }

        /// <summary>
        /// Get the string value for an input enum, if possible
        /// </summary>
        /// <param name="value">Enum value to parse/param>
        /// <returns>String value representing the input, null on error</returns>
        public static string? AsStringValue(this ControlType value)
        {
            return value switch
            {
                ControlType.Joy => "joy",
                ControlType.Stick => "stick",
                ControlType.Paddle => "paddle",
                ControlType.Pedal => "pedal",
                ControlType.Lightgun => "lightgun",
                ControlType.Positional => "positional",
                ControlType.Dial => "dial",
                ControlType.Trackball => "trackball",
                ControlType.Mouse => "mouse",
                ControlType.OnlyButtons => "only_buttons",
                ControlType.Keypad => "keypad",
                ControlType.Keyboard => "keyboard",
                ControlType.Mahjong => "mahjong",
                ControlType.Hanafuda => "hanafuda",
                ControlType.Gambling => "gambling",
                _ => null,
            };
        }

        /// <summary>
        /// Get the string value for an input enum, if possible
        /// </summary>
        /// <param name="value">Enum value to parse/param>
        /// <returns>String value representing the input, null on error</returns>
        public static string? AsStringValue(this DeviceType value)
        {
            return value switch
            {
                DeviceType.Unknown => "unknown",
                DeviceType.Cartridge => "cartridge",
                DeviceType.FloppyDisk => "floppydisk",
                DeviceType.HardDisk => "harddisk",
                DeviceType.Cylinder => "cylinder",
                DeviceType.Cassette => "cassette",
                DeviceType.PunchCard => "punchcard",
                DeviceType.PunchTape => "punchtape",
                DeviceType.Printout => "printout",
                DeviceType.Serial => "serial",
                DeviceType.Parallel => "parallel",
                DeviceType.Snapshot => "snapshot",
                DeviceType.QuickLoad => "quickload",
                DeviceType.MemCard => "memcard",
                DeviceType.CDROM => "cdrom",
                DeviceType.MagTape => "magtape",
                DeviceType.ROMImage => "romimage",
                DeviceType.MIDIIn => "midiin",
                DeviceType.MIDIOut => "midiout",
                DeviceType.Picture => "picture",
                DeviceType.VidFile => "vidfile",
                _ => null,
            };
        }

        /// <summary>
        /// Get the string value for an input enum, if possible
        /// </summary>
        /// <param name="value">Enum value to parse/param>
        /// <returns>String value representing the input, null on error</returns>
        public static string? AsStringValue(this DisplayType value)
        {
            return value switch
            {
                DisplayType.Raster => "raster",
                DisplayType.Vector => "vector",
                DisplayType.LCD => "lcd",
                DisplayType.SVG => "svg",
                DisplayType.Unknown => "unknown",
                _ => null,
            };
        }

        /// <summary>
        /// Get the string value for an input enum, if possible
        /// </summary>
        /// <param name="value">Enum value to parse/param>
        /// <returns>String value representing the input, null on error</returns>
        public static string? AsStringValue(this Endianness value)
        {
            return value switch
            {
                Endianness.Big => "big",
                Endianness.Little => "little",
                _ => null,
            };
        }

        /// <summary>
        /// Get the string value for an input enum, if possible
        /// </summary>
        /// <param name="value">Enum value to parse/param>
        /// <returns>String value representing the input, null on error</returns>
        public static string? AsStringValue(this FeatureStatus value)
        {
            return value switch
            {
                FeatureStatus.Unemulated => "unemulated",
                FeatureStatus.Imperfect => "imperfect",
                _ => null,
            };
        }

        /// <summary>
        /// Get the string value for an input enum, if possible
        /// </summary>
        /// <param name="value">Enum value to parse/param>
        /// <returns>String value representing the input, null on error</returns>
        public static string? AsStringValue(this FeatureType value)
        {
            return value switch
            {
                FeatureType.Protection => "protection",
                FeatureType.Palette => "palette",
                FeatureType.Graphics => "graphics",
                FeatureType.Sound => "sound",
                FeatureType.Controls => "controls",
                FeatureType.Keyboard => "keyboard",
                FeatureType.Mouse => "mouse",
                FeatureType.Microphone => "microphone",
                FeatureType.Camera => "camera",
                FeatureType.Disk => "disk",
                FeatureType.Printer => "printer",
                FeatureType.Lan => "lan",
                FeatureType.Wan => "wan",
                FeatureType.Timing => "timing",
                _ => null,
            };
        }

        /// <summary>
        /// Get the string value for an input enum, if possible
        /// </summary>
        /// <param name="value">Enum value to parse/param>
        /// <param name="useSecond">True to use the second mapping option, if it exists</param>
        /// <returns>String value representing the input, null on error</returns>
        public static string? AsStringValue(this ItemStatus value, bool useSecond = false)
        {
            return value switch
            {
                ItemStatus.None => useSecond ? "no" : "none",
                ItemStatus.Good => "good",
                ItemStatus.BadDump => "baddump",
                ItemStatus.Nodump => useSecond ? "yes" : "nodump",
                ItemStatus.Verified => "verified",
                ItemStatus.Deduped => "deduped",
                _ => null,
            };
        }

        /// <summary>
        /// Get the string value for an input enum, if possible
        /// </summary>
        /// <param name="value">Enum value to parse/param>
        /// <returns>String value representing the input, default on error</returns>
        public static string? AsStringValue(this ItemType value)
        {
            return value switch
            {
                // "Actionable" item types
                ItemType.Rom => "rom",
                ItemType.Disk => "disk",
                ItemType.File => "file",
                ItemType.Media => "media",

                // "Auxiliary" item types
                ItemType.Adjuster => "adjuster",
                ItemType.Analog => "analog",
                ItemType.Archive => "archive",
                ItemType.BiosSet => "biosset",
                ItemType.Chip => "chip",
                ItemType.Configuration => "configuration",
                ItemType.ConfLocation => "conflocation",
                ItemType.ConfSetting => "confsetting",
                ItemType.Control => "control",
                ItemType.DataArea => "dataarea",
                ItemType.Device => "device",
                ItemType.DeviceRef => "device_ref",
                ItemType.DipLocation => "diplocation",
                ItemType.DipSwitch => "dipswitch",
                ItemType.DipValue => "dipvalue",
                ItemType.DiskArea => "diskarea",
                ItemType.Display => "display",
                ItemType.Driver => "driver",
                ItemType.Dump => "dump",
                ItemType.Feature => "feature",
                ItemType.Info => "info",
                ItemType.Input => "input",
                ItemType.Original => "original",
                ItemType.Part => "part",
                ItemType.PartFeature => "part_feature",
                ItemType.Port => "port",
                ItemType.RamOption => "ramoption",
                ItemType.Release => "release",
                ItemType.ReleaseDetails => "release_details",
                ItemType.Sample => "sample",
                ItemType.Serials => "serials",
                ItemType.SharedFeat => "sharedfeat",
                ItemType.Slot => "slot",
                ItemType.SlotOption => "slotoption",
                ItemType.SoftwareList => "softwarelist",
                ItemType.Sound => "sound",
                ItemType.SourceDetails => "source_details",
                ItemType.Video => "video",
                ItemType.Blank => "blank",

                ItemType.NULL => null,
                _ => null,
            };
        }

        /// <summary>
        /// Get the string value for an input enum, if possible
        /// </summary>
        /// <param name="value">Enum value to parse/param>
        /// <returns>String value representing the input, null on error</returns>
        public static string? AsStringValue(this LoadFlag value)
        {
            return value switch
            {
                LoadFlag.Load16Byte => "load16_byte",
                LoadFlag.Load16Word => "load16_word",
                LoadFlag.Load16WordSwap => "load16_word_swap",
                LoadFlag.Load32Byte => "load32_byte",
                LoadFlag.Load32Word => "load32_word",
                LoadFlag.Load32WordSwap => "load32_word_swap",
                LoadFlag.Load32DWord => "load32_dword",
                LoadFlag.Load64Word => "load64_word",
                LoadFlag.Load64WordSwap => "load64_word_swap",
                LoadFlag.Reload => "reload",
                LoadFlag.Fill => "fill",
                LoadFlag.Continue => "continue",
                LoadFlag.ReloadPlain => "reload_plain",
                LoadFlag.Ignore => "ignore",
                _ => null,
            };
        }

        /// <summary>
        /// Get the string value for an input enum, if possible
        /// </summary>
        /// <param name="value">Enum value to parse/param>
        /// <param name="useSecond">True to use the second mapping option, if it exists</param>
        /// <returns>String value representing the input, default on error</returns>
        public static string? AsStringValue(this MergingFlag value, bool useSecond = false)
        {
            return value switch
            {
                MergingFlag.None => "none",
                MergingFlag.Split => "split",
                MergingFlag.Merged => "merged",
                MergingFlag.NonMerged => useSecond ? "unmerged" : "nonmerged",
                MergingFlag.FullMerged => "fullmerged",
                MergingFlag.DeviceNonMerged => useSecond ? "devicenonmerged" : "device",
                MergingFlag.FullNonMerged => useSecond ? "fullnonmerged" : "full",
                _ => null,
            };
        }

        /// <summary>
        /// Get the string value for an input enum, if possible
        /// </summary>
        /// <param name="value">Enum value to parse/param>
        /// <returns>String value representing the input, default on error</returns>
        public static string? AsStringValue(this NodumpFlag value)
        {
            return value switch
            {
                NodumpFlag.None => "none",
                NodumpFlag.Obsolete => "obsolete",
                NodumpFlag.Required => "required",
                NodumpFlag.Ignore => "ignore",
                _ => null,
            };
        }

        /// <summary>
        /// Get the string value for an input enum, if possible
        /// </summary>
        /// <param name="value">Enum value to parse/param>
        /// <returns>String value representing the input, null on error</returns>
        public static string? AsStringValue(this OpenMSXSubType value)
        {
            return value switch
            {
                OpenMSXSubType.Rom => "rom",
                OpenMSXSubType.MegaRom => "megarom",
                OpenMSXSubType.SCCPlusCart => "sccpluscart",
                _ => null,
            };
        }

        /// <summary>
        /// Get the string value for an input enum, if possible
        /// </summary>
        /// <param name="value">Enum value to parse/param>
        /// <param name="useSecond">True to use the second mapping option, if it exists</param>
        /// <returns>String value representing the input, default on error</returns>
        public static string? AsStringValue(this PackingFlag value, bool useSecond = false)
        {
            return value switch
            {
                PackingFlag.None => "none",
                PackingFlag.Zip => useSecond ? "yes" : "zip",
                PackingFlag.Unzip => useSecond ? "no" : "unzip",
                PackingFlag.Partial => "partial",
                PackingFlag.Flat => "flat",
                PackingFlag.FileOnly => "fileonly",
                _ => null,
            };
        }

        /// <summary>
        /// Get the string value for an input enum, if possible
        /// </summary>
        /// <param name="value">Enum value to parse/param>
        /// <returns>String value representing the input, null on error</returns>
        public static string? AsStringValue(this Relation value)
        {
            return value switch
            {
                Relation.Equal => "eq",
                Relation.NotEqual => "ne",
                Relation.GreaterThan => "gt",
                Relation.LessThanOrEqual => "le",
                Relation.LessThan => "lt",
                Relation.GreaterThanOrEqual => "ge",
                _ => null,
            };
        }

        /// <summary>
        /// Get the string value for an input enum, if possible
        /// </summary>
        /// <param name="value">Enum value to parse/param>
        /// <returns>String value representing the input, null on error</returns>
        public static string? AsStringValue(this Rotation value, bool useSecond = false)
        {
            return value switch
            {
                Rotation.North => useSecond ? "vertical" : "0",
                Rotation.East => useSecond ? "horizontal" : "90",
                Rotation.South => useSecond ? "vertical" : "180",
                Rotation.West => useSecond ? "horizontal" : "270",
                _ => null,
            };
        }

        /// <summary>
        /// Get the string value for an input enum, if possible
        /// </summary>
        /// <param name="value">Enum value to parse/param>
        /// <returns>String value representing the input, null on error</returns>
        public static string? AsStringValue(this Runnable value)
        {
            return value switch
            {
                Runnable.No => "no",
                Runnable.Partial => "partial",
                Runnable.Yes => "yes",
                _ => null,
            };
        }

        /// <summary>
        /// Get the string value for an input enum, if possible
        /// </summary>
        /// <param name="value">Enum value to parse/param>
        /// <returns>String value representing the input, null on error</returns>
        public static string? AsStringValue(this SoftwareListStatus value)
        {
            return value switch
            {
                SoftwareListStatus.None => "none",
                SoftwareListStatus.Original => "original",
                SoftwareListStatus.Compatible => "compatible",
                _ => null,
            };
        }

        /// <summary>
        /// Get the string value for an input enum, if possible
        /// </summary>
        /// <param name="value">Enum value to parse/param>
        /// <param name="useSecond">True to use the second mapping option, if it exists</param>
        /// <returns>String value representing the input, null on error</returns>
        public static string? AsStringValue(this Supported value, bool useSecond = false)
        {
            return value switch
            {
                Supported.No => useSecond ? "unsupported" : "no",
                Supported.Partial => "partial",
                Supported.Yes => useSecond ? "supported" : "yes",
                _ => null,
            };
        }

        /// <summary>
        /// Get the string value for an input enum, if possible
        /// </summary>
        /// <param name="value">Enum value to parse/param>
        /// <returns>String value representing the input, null on error</returns>
        public static string? AsStringValue(this Width value)
        {
            return value switch
            {
                Width.Byte => "8",
                Width.Short => "16",
                Width.Int => "32",
                Width.Long => "64",
                _ => null,
            };
        }

        /// <summary>
        /// Get the string value for an input enum, if possible
        /// </summary>
        /// <param name="value">Enum value to parse/param>
        /// <returns>String value representing the input, null on error</returns>
        public static string? AsStringValue(this SupportStatus value)
        {
            return value switch
            {
                SupportStatus.Good => "good",
                SupportStatus.Imperfect => "imperfect",
                SupportStatus.Preliminary => "preliminary",
                SupportStatus.Test => "test",
                _ => null,
            };
        }

        /// <summary>
        /// Get string value from input bool?
        /// </summary>
        /// <param name="yesno">bool? to get value from</param>
        /// <returns>String corresponding to the bool?</returns>
        public static string? FromYesNo(this bool? yesno)
        {
            return yesno switch
            {
                true => "yes",
                false => "no",
                _ => null,
            };
        }

        #endregion
    }
}
