using System;
using SabreTools.Data.Models.Metadata;
using SabreTools.Hashing;
using SabreTools.IO.Extensions;
using SabreTools.Matching;

namespace SabreTools.Data.Extensions
{
    public static class MetadataExtensions
    {
        #region Accessors

        /// <summary>
        /// Gets the name to use for a DictionaryBase or null
        /// </summary>
        public static string? GetName(this DictionaryBase self)
        {
            if (self is null)
                return null;

            return self switch
            {
                Header header => header.Name,

                Machine machine => machine.Name,

                Adjuster adjuster => adjuster.Name,
                Analog => null,
                Archive archive => archive.Name,
                BiosSet biosSet => biosSet.Name,
                Chip chip => chip.Name,
                Condition => null,
                ConfLocation confLocation => confLocation.Name,
                ConfSetting confSetting => confSetting.Name,
                Configuration configuration => configuration.Name,
                Control => null,
                DataArea dataArea => dataArea.Name,
                Device => null,
                DeviceRef deviceRef => deviceRef.Name,
                DipLocation dipLocation => dipLocation.Name,
                DipSwitch dipSwitch => dipSwitch.Name,
                DipValue dipValue => dipValue.Name,
                Disk disk => disk.Name,
                DiskArea diskArea => diskArea.Name,
                Display => null,
                Driver => null,
                Extension extension => extension.Name,
                Feature feature => feature.Name,
                Info info => info.Name,
                Input => null,
                Instance instance => instance.Name,
                Media media => media.Name,
                Part part => part.Name,
                Port => null,
                RamOption ramOption => ramOption.Name,
                Release release => release.Name,
                ReleaseDetails => null,
                Rom rom => rom.Name,
                Sample sample => sample.Name,
                Serials => null,
                SharedFeat sharedFeat => sharedFeat.Name,
                Slot slot => slot.Name,
                SlotOption slotOption => slotOption.Name,
                SoftwareList softwareList => softwareList.Name,
                Sound => null,
                SourceDetails => null,
                Video => null,

                _ => null,
            };
        }

        /// <summary>
        /// Gets the name to use for a DictionaryBase or null
        /// </summary>
        public static void SetName(this DictionaryBase self, string? name)
        {
            if (self is null || string.IsNullOrEmpty(name))
                return;

            switch (self)
            {
                case Header header: header.Name = name; break;

                case Machine machine: machine.Name = name; break;

                case Adjuster adjuster: adjuster.Name = name; break;
                case Analog: break;
                case Archive archive: archive.Name = name; break;
                case BiosSet biosSet: biosSet.Name = name; break;
                case Chip chip: chip.Name = name; break;
                case Condition: break;
                case ConfLocation confLocation: confLocation.Name = name; break;
                case ConfSetting confSetting: confSetting.Name = name; break;
                case Configuration configuration: configuration.Name = name; break;
                case Control: break;
                case DataArea dataArea: dataArea.Name = name; break;
                case Device: break;
                case DeviceRef deviceRef: deviceRef.Name = name; break;
                case DipLocation dipLocation: dipLocation.Name = name; break;
                case DipSwitch dipSwitch: dipSwitch.Name = name; break;
                case DipValue dipValue: dipValue.Name = name; break;
                case Disk disk: disk.Name = name; break;
                case DiskArea diskArea: diskArea.Name = name; break;
                case Display: break;
                case Driver: break;
                case Extension extension: extension.Name = name; break;
                case Feature feature: feature.Name = name; break;
                case Info info: info.Name = name; break;
                case Input: break;
                case Instance instance: instance.Name = name; break;
                case Media media: media.Name = name; break;
                case Part part: part.Name = name; break;
                case Port: break;
                case RamOption ramOption: ramOption.Name = name; break;
                case Release release: release.Name = name; break;
                case ReleaseDetails: break;
                case Rom rom: rom.Name = name; break;
                case Sample sample: sample.Name = name; break;
                case Serials: break;
                case SharedFeat sharedFeat: sharedFeat.Name = name; break;
                case Slot slot: slot.Name = name; break;
                case SlotOption slotOption: slotOption.Name = name; break;
                case SoftwareList softwareList: softwareList.Name = name; break;
                case Sound: break;
                case SourceDetails: break;
                case Video: break;

                default: break;
            }
        }

        #endregion

        #region Cloning

        /// <summary>
        /// Deep clone a DictionaryBase object
        /// </summary>
        /// TODO: Move this to Clone methods in each file when keys go away entirely
        public static DictionaryBase? Clone(this DictionaryBase self)
        {
            // If construction failed, we can't do anything
            if (Activator.CreateInstance(self.GetType()) is not DictionaryBase clone)
                return null;

            // Handle individual type properties
            if (self is Adjuster selfAdjuster && clone is Adjuster cloneAdjuster)
            {
                cloneAdjuster.Default = selfAdjuster.Default;
            }
            else if (self is BiosSet selfBiosSet && clone is BiosSet cloneBiosSet)
            {
                cloneBiosSet.Default = selfBiosSet.Default;
            }
            else if (self is Chip selfChip && clone is Chip cloneChip)
            {
                cloneChip.SoundOnly = selfChip.SoundOnly;
            }
            else if (self is ConfLocation selfConfLocation && clone is ConfLocation cloneConfLocation)
            {
                cloneConfLocation.Inverted = selfConfLocation.Inverted;
            }
            else if (self is ConfSetting selfConfSetting && clone is ConfSetting cloneConfSetting)
            {
                cloneConfSetting.Default = selfConfSetting.Default;
            }
            else if (self is Control selfControl && clone is Control cloneControl)
            {
                cloneControl.Reverse = selfControl.Reverse;
            }
            else if (self is DipLocation selfDipLocation && clone is DipLocation cloneDipLocation)
            {
                cloneDipLocation.Inverted = selfDipLocation.Inverted;
            }
            else if (self is DipSwitch selfDipSwitch && clone is DipSwitch cloneDipSwitch)
            {
                cloneDipSwitch.Default = selfDipSwitch.Default;
            }
            else if (self is DipValue selfDipValue && clone is DipValue cloneDipValue)
            {
                cloneDipValue.Default = selfDipValue.Default;
            }
            else if (self is Disk selfDisk && clone is Disk cloneDisk)
            {
                cloneDisk.Optional = selfDisk.Optional;
                cloneDisk.Writable = selfDisk.Writable;
            }
            else if (self is Display selfDisplay && clone is Display cloneDisplay)
            {
                cloneDisplay.FlipX = selfDisplay.FlipX;
            }
            else if (self is Driver selfDriver && clone is Driver cloneDriver)
            {
                cloneDriver.Incomplete = selfDriver.Incomplete;
                cloneDriver.NoSoundHardware = selfDriver.NoSoundHardware;
                cloneDriver.RequiresArtwork = selfDriver.RequiresArtwork;
                cloneDriver.Unofficial = selfDriver.Unofficial;
            }
            else if (self is Header selfHeader && clone is Header cloneHeader)
            {
                cloneHeader.Debug = selfHeader.Debug;
                cloneHeader.ForceZipping = selfHeader.ForceZipping;
                cloneHeader.LockBiosMode = selfHeader.LockBiosMode;
                cloneHeader.LockRomMode = selfHeader.LockRomMode;
                cloneHeader.LockSampleMode = selfHeader.LockSampleMode;
            }
            else if (self is Input selfInput && clone is Input cloneInput)
            {
                cloneInput.Service = selfInput.Service;
                cloneInput.Tilt = selfInput.Tilt;
            }
            else if (self is Machine selfMachine && clone is Machine cloneMachine)
            {
                cloneMachine.IsBios = selfMachine.IsBios;
                cloneMachine.IsDevice = selfMachine.IsDevice;
                cloneMachine.IsMechanical = selfMachine.IsMechanical;
                cloneMachine.Runnable = selfMachine.Runnable;
            }
            else if (self is RamOption selfRamOption && clone is RamOption cloneRamOption)
            {
                cloneRamOption.Default = selfRamOption.Default;
            }
            else if (self is Release selfRelease && clone is Release cloneRelease)
            {
                cloneRelease.Default = selfRelease.Default;
            }
            else if (self is Rom selfRom && clone is Rom cloneRom)
            {
                cloneRom.Dispose = selfRom.Dispose;
                cloneRom.FileIsAvailable = selfRom.FileIsAvailable;
                cloneRom.Inverted = selfRom.Inverted;
                cloneRom.MIA = selfRom.MIA;
                cloneRom.Optional = selfRom.Optional;
                cloneRom.SoundOnly = selfRom.SoundOnly;
            }
            else if (self is SlotOption selfSlotOption && clone is SlotOption cloneSlotOption)
            {
                cloneSlotOption.Default = selfSlotOption.Default;
            }

            // Handle known properties
            clone.SetName(self.GetName());

            // Loop through and clone per type
            foreach (string key in self.Keys)
            {
                object? value = self[key];
                clone[key] = value switch
                {
                    // Primative types
                    bool or long or double or string => value,

                    // DictionaryBase types
                    DictionaryBase db => db.Clone(),

                    // Enumerable types
                    byte[] bytArr => bytArr.Clone(),
                    string[] strArr => strArr.Clone(),
                    DictionaryBase[] dbArr => Array.ConvertAll(dbArr, Clone),
                    ICloneable[] clArr => Array.ConvertAll(clArr, cl => cl.Clone()),

                    // Everything else just copies
                    _ => value,
                };
            }

            return clone;
        }

        #endregion

        #region Conversion

        /// <summary>
        /// Convert a DictionaryBase to a Rom
        /// </summary>
        public static Rom? ConvertToRom(this DictionaryBase? self)
        {
            // If the DatItem is missing, we can't do anything
            if (self is null)
                return null;

            return self switch
            {
                Disk diskSelf => ConvertToRom(diskSelf),
                Media mediaSelf => ConvertToRom(mediaSelf),
                _ => null,
            };
        }

        /// <summary>
        /// Convert a Disk to a Rom
        /// </summary>
        private static Rom? ConvertToRom(this Disk? disk)
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
                [Rom.MergeKey] = disk.ReadString(Disk.MergeKey),
                [Rom.RegionKey] = disk.ReadString(Disk.RegionKey),
                [Rom.StatusKey] = disk.ReadString(Disk.StatusKey),
                Optional = disk.Optional,
                [Rom.MD5Key] = disk.ReadString(Disk.MD5Key),
                [Rom.SHA1Key] = disk.ReadString(Disk.SHA1Key),
            };
        }

        /// <summary>
        /// Convert a Media to a Rom
        /// </summary>
        private static Rom? ConvertToRom(this Media? media)
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
                [Rom.MD5Key] = media.ReadString(Media.MD5Key),
                [Rom.SHA1Key] = media.ReadString(Media.SHA1Key),
                [Rom.SHA256Key] = media.ReadString(Media.SHA256Key),
                [Rom.SpamSumKey] = media.ReadString(Media.SpamSumKey),
            };
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
            string? selfMd5 = self.ReadString(Disk.MD5Key);
            string? otherMd5 = other.ReadString(Disk.MD5Key);
            bool conditionalMd5 = ConditionalHashEquals(selfMd5, otherMd5);

            string? selfSha1 = self.ReadString(Disk.SHA1Key);
            string? otherSha1 = other.ReadString(Disk.SHA1Key);
            bool conditionalSha1 = ConditionalHashEquals(selfSha1, otherSha1);

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
            string? selfMd5 = self.ReadString(Media.MD5Key);
            string? otherMd5 = other.ReadString(Media.MD5Key);
            bool conditionalMd5 = ConditionalHashEquals(selfMd5, otherMd5);

            string? selfSha1 = self.ReadString(Media.SHA1Key);
            string? otherSha1 = other.ReadString(Media.SHA1Key);
            bool conditionalSha1 = ConditionalHashEquals(selfSha1, otherSha1);

            string? selfSha256 = self.ReadString(Media.SHA256Key);
            string? otherSha256 = other.ReadString(Media.SHA256Key);
            bool conditionalSha256 = ConditionalHashEquals(selfSha256, otherSha256);

            string? selfSpamSum = self.ReadString(Media.SpamSumKey);
            string? otherSpamSum = other.ReadString(Media.SpamSumKey);
            bool conditionalSpamSum = ConditionalHashEquals(selfSpamSum, otherSpamSum);

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
            string? selfCrc16 = self.ReadString(Rom.CRC16Key);
            string? otherCrc16 = other.ReadString(Rom.CRC16Key);
            bool conditionalCrc16 = ConditionalHashEquals(selfCrc16, otherCrc16);

            string? selfCrc = self.ReadString(Rom.CRCKey);
            string? otherCrc = other.ReadString(Rom.CRCKey);
            bool conditionalCrc = ConditionalHashEquals(selfCrc, otherCrc);

            string? selfCrc64 = self.ReadString(Rom.CRC64Key);
            string? otherCrc64 = other.ReadString(Rom.CRC64Key);
            bool conditionalCrc64 = ConditionalHashEquals(selfCrc64, otherCrc64);

            string? selfMd2 = self.ReadString(Rom.MD2Key);
            string? otherMd2 = other.ReadString(Rom.MD2Key);
            bool conditionalMd2 = ConditionalHashEquals(selfMd2, otherMd2);

            string? selfMd4 = self.ReadString(Rom.MD4Key);
            string? otherMd4 = other.ReadString(Rom.MD4Key);
            bool conditionalMd4 = ConditionalHashEquals(selfMd4, otherMd4);

            string? selfMd5 = self.ReadString(Rom.MD5Key);
            string? otherMd5 = other.ReadString(Rom.MD5Key);
            bool conditionalMd5 = ConditionalHashEquals(selfMd5, otherMd5);

            string? selfRipeMD128 = self.ReadString(Rom.RIPEMD128Key);
            string? otherRipeMD128 = other.ReadString(Rom.RIPEMD128Key);
            bool conditionaRipeMD128 = ConditionalHashEquals(selfRipeMD128, otherRipeMD128);

            string? selfRipeMD160 = self.ReadString(Rom.RIPEMD160Key);
            string? otherRipeMD160 = other.ReadString(Rom.RIPEMD160Key);
            bool conditionaRipeMD160 = ConditionalHashEquals(selfRipeMD160, otherRipeMD160);

            string? selfSha1 = self.ReadString(Rom.SHA1Key);
            string? otherSha1 = other.ReadString(Rom.SHA1Key);
            bool conditionalSha1 = ConditionalHashEquals(selfSha1, otherSha1);

            string? selfSha256 = self.ReadString(Rom.SHA256Key);
            string? otherSha256 = other.ReadString(Rom.SHA256Key);
            bool conditionalSha256 = ConditionalHashEquals(selfSha256, otherSha256);

            string? selfSha384 = self.ReadString(Rom.SHA384Key);
            string? otherSha384 = other.ReadString(Rom.SHA384Key);
            bool conditionalSha384 = ConditionalHashEquals(selfSha384, otherSha384);

            string? selfSha512 = self.ReadString(Rom.SHA512Key);
            string? otherSha512 = other.ReadString(Rom.SHA512Key);
            bool conditionalSha512 = ConditionalHashEquals(selfSha512, otherSha512);

            string? selfSpamSum = self.ReadString(Rom.SpamSumKey);
            string? otherSpamSum = other.ReadString(Rom.SpamSumKey);
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
        public static bool HasHashes(this DictionaryBase self)
        {
            return self switch
            {
                Disk diskSelf => diskSelf.HasHashes(),
                Media mediaSelf => mediaSelf.HasHashes(),
                Rom romSelf => romSelf.HasHashes(),
                _ => false,
            };
        }

        /// <summary>
        /// Returns if all of the hashes are set to their 0-byte values or null
        /// </summary>
        public static bool HasZeroHash(this DictionaryBase self)
        {
            return self switch
            {
                Disk diskSelf => diskSelf.HasZeroHash(),
                Media mediaSelf => mediaSelf.HasZeroHash(),
                Rom romSelf => romSelf.HasZeroHash(),
                _ => false,
            };
        }

        /// <summary>
        /// Returns if there are no, non-empty hashes in common
        /// </summary>
        private static bool HasCommonHash(this Disk self, Disk other)
        {
            bool md5Null = string.IsNullOrEmpty(self.ReadString(Disk.MD5Key));
            md5Null ^= string.IsNullOrEmpty(other.ReadString(Disk.MD5Key));

            bool sha1Null = string.IsNullOrEmpty(self.ReadString(Disk.SHA1Key));
            sha1Null ^= string.IsNullOrEmpty(other.ReadString(Disk.SHA1Key));

            return !md5Null
                || !sha1Null;
        }

        /// <summary>
        /// Returns if there are no, non-empty hashes in common
        /// </summary>
        private static bool HasCommonHash(this Media self, Media other)
        {
            bool md5Null = string.IsNullOrEmpty(self.ReadString(Media.MD5Key));
            md5Null ^= string.IsNullOrEmpty(other.ReadString(Media.MD5Key));

            bool sha1Null = string.IsNullOrEmpty(self.ReadString(Media.SHA1Key));
            sha1Null ^= string.IsNullOrEmpty(other.ReadString(Media.SHA1Key));

            bool sha256Null = string.IsNullOrEmpty(self.ReadString(Media.SHA256Key));
            sha256Null ^= string.IsNullOrEmpty(other.ReadString(Media.SHA256Key));

            bool spamsumNull = string.IsNullOrEmpty(self.ReadString(Media.SpamSumKey));
            spamsumNull ^= string.IsNullOrEmpty(other.ReadString(Media.SpamSumKey));

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
            bool crc16Null = string.IsNullOrEmpty(self.ReadString(Rom.CRC16Key));
            crc16Null ^= string.IsNullOrEmpty(other.ReadString(Rom.CRC16Key));

            bool crcNull = string.IsNullOrEmpty(self.ReadString(Rom.CRCKey));
            crcNull ^= string.IsNullOrEmpty(other.ReadString(Rom.CRCKey));

            bool crc64Null = string.IsNullOrEmpty(self.ReadString(Rom.CRC64Key));
            crc64Null ^= string.IsNullOrEmpty(other.ReadString(Rom.CRC64Key));

            bool md2Null = string.IsNullOrEmpty(self.ReadString(Rom.MD2Key));
            md2Null ^= string.IsNullOrEmpty(other.ReadString(Rom.MD2Key));

            bool md4Null = string.IsNullOrEmpty(self.ReadString(Rom.MD4Key));
            md4Null ^= string.IsNullOrEmpty(other.ReadString(Rom.MD4Key));

            bool md5Null = string.IsNullOrEmpty(self.ReadString(Rom.MD5Key));
            md5Null ^= string.IsNullOrEmpty(other.ReadString(Rom.MD5Key));

            bool ripeMD128Null = string.IsNullOrEmpty(self.ReadString(Rom.RIPEMD128Key));
            ripeMD128Null ^= string.IsNullOrEmpty(other.ReadString(Rom.RIPEMD128Key));

            bool ripeMD160Null = string.IsNullOrEmpty(self.ReadString(Rom.RIPEMD160Key));
            ripeMD160Null ^= string.IsNullOrEmpty(other.ReadString(Rom.RIPEMD160Key));

            bool sha1Null = string.IsNullOrEmpty(self.ReadString(Rom.SHA1Key));
            sha1Null ^= string.IsNullOrEmpty(other.ReadString(Rom.SHA1Key));

            bool sha256Null = string.IsNullOrEmpty(self.ReadString(Rom.SHA256Key));
            sha256Null ^= string.IsNullOrEmpty(other.ReadString(Rom.SHA256Key));

            bool sha384Null = string.IsNullOrEmpty(self.ReadString(Rom.SHA384Key));
            sha384Null ^= string.IsNullOrEmpty(other.ReadString(Rom.SHA384Key));

            bool sha512Null = string.IsNullOrEmpty(self.ReadString(Rom.SHA512Key));
            sha512Null ^= string.IsNullOrEmpty(other.ReadString(Rom.SHA512Key));

            bool spamsumNull = string.IsNullOrEmpty(self.ReadString(Rom.SpamSumKey));
            spamsumNull ^= string.IsNullOrEmpty(other.ReadString(Rom.SpamSumKey));

            return !crc16Null
                || !crcNull
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
        /// Returns if any hashes exist
        /// </summary>
        private static bool HasHashes(this Disk disk)
        {
            bool md5Null = string.IsNullOrEmpty(disk.ReadString(Disk.MD5Key));
            bool sha1Null = string.IsNullOrEmpty(disk.ReadString(Disk.SHA1Key));

            return !md5Null
                || !sha1Null;
        }

        /// <summary>
        /// Returns if any hashes exist
        /// </summary>
        private static bool HasHashes(this Media media)
        {
            bool md5Null = string.IsNullOrEmpty(media.ReadString(Media.MD5Key));
            bool sha1Null = string.IsNullOrEmpty(media.ReadString(Media.SHA1Key));
            bool sha256Null = string.IsNullOrEmpty(media.ReadString(Media.SHA256Key));
            bool spamsumNull = string.IsNullOrEmpty(media.ReadString(Media.SpamSumKey));

            return !md5Null
                || !sha1Null
                || !sha256Null
                || !spamsumNull;
        }

        /// <summary>
        /// Returns if any hashes exist
        /// </summary>
        private static bool HasHashes(this Rom rom)
        {
            bool crc16Null = string.IsNullOrEmpty(rom.ReadString(Rom.CRC16Key));
            bool crcNull = string.IsNullOrEmpty(rom.ReadString(Rom.CRCKey));
            bool crc64Null = string.IsNullOrEmpty(rom.ReadString(Rom.CRC64Key));
            bool md2Null = string.IsNullOrEmpty(rom.ReadString(Rom.MD2Key));
            bool md4Null = string.IsNullOrEmpty(rom.ReadString(Rom.MD4Key));
            bool md5Null = string.IsNullOrEmpty(rom.ReadString(Rom.MD5Key));
            bool ripeMD128Null = string.IsNullOrEmpty(rom.ReadString(Rom.RIPEMD128Key));
            bool ripeMD160Null = string.IsNullOrEmpty(rom.ReadString(Rom.RIPEMD160Key));
            bool sha1Null = string.IsNullOrEmpty(rom.ReadString(Rom.SHA1Key));
            bool sha256Null = string.IsNullOrEmpty(rom.ReadString(Rom.SHA256Key));
            bool sha384Null = string.IsNullOrEmpty(rom.ReadString(Rom.SHA384Key));
            bool sha512Null = string.IsNullOrEmpty(rom.ReadString(Rom.SHA512Key));
            bool spamsumNull = string.IsNullOrEmpty(rom.ReadString(Rom.SpamSumKey));

            return !crc16Null
                || !crcNull
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
        private static bool HasZeroHash(this Disk disk)
        {
            string? md5 = disk.ReadString(Disk.MD5Key);
            bool md5Null = string.IsNullOrEmpty(md5) || string.Equals(md5, HashType.MD5.ZeroString, StringComparison.OrdinalIgnoreCase);

            string? sha1 = disk.ReadString(Disk.SHA1Key);
            bool sha1Null = string.IsNullOrEmpty(sha1) || string.Equals(sha1, HashType.SHA1.ZeroString, StringComparison.OrdinalIgnoreCase);

            return md5Null
                && sha1Null;
        }

        /// <summary>
        /// Returns if all of the hashes are set to their 0-byte values or null
        /// </summary>
        private static bool HasZeroHash(this Media media)
        {
            string? md5 = media.ReadString(Media.MD5Key);
            bool md5Null = string.IsNullOrEmpty(md5) || string.Equals(md5, HashType.MD5.ZeroString, StringComparison.OrdinalIgnoreCase);

            string? sha1 = media.ReadString(Media.SHA1Key);
            bool sha1Null = string.IsNullOrEmpty(sha1) || string.Equals(sha1, HashType.SHA1.ZeroString, StringComparison.OrdinalIgnoreCase);

            string? sha256 = media.ReadString(Media.SHA256Key);
            bool sha256Null = string.IsNullOrEmpty(sha256) || string.Equals(sha256, HashType.SHA256.ZeroString, StringComparison.OrdinalIgnoreCase);

            string? spamsum = media.ReadString(Media.SpamSumKey);
            bool spamsumNull = string.IsNullOrEmpty(spamsum) || string.Equals(spamsum, HashType.SpamSum.ZeroString, StringComparison.OrdinalIgnoreCase);

            return md5Null
                && sha1Null
                && sha256Null
                && spamsumNull;
        }

        /// <summary>
        /// Returns if all of the hashes are set to their 0-byte values or null
        /// </summary>
        private static bool HasZeroHash(this Rom rom)
        {
            string? crc16 = rom.ReadString(Rom.CRC16Key);
            bool crc16Null = string.IsNullOrEmpty(crc16) || string.Equals(crc16, HashType.CRC16.ZeroString, StringComparison.OrdinalIgnoreCase);

            string? crc = rom.ReadString(Rom.CRCKey);
            bool crcNull = string.IsNullOrEmpty(crc) || string.Equals(crc, HashType.CRC32.ZeroString, StringComparison.OrdinalIgnoreCase);

            string? crc64 = rom.ReadString(Rom.CRC64Key);
            bool crc64Null = string.IsNullOrEmpty(crc64) || string.Equals(crc64, HashType.CRC64.ZeroString, StringComparison.OrdinalIgnoreCase);

            string? md2 = rom.ReadString(Rom.MD2Key);
            bool md2Null = string.IsNullOrEmpty(md2) || string.Equals(md2, HashType.MD2.ZeroString, StringComparison.OrdinalIgnoreCase);

            string? md4 = rom.ReadString(Rom.MD4Key);
            bool md4Null = string.IsNullOrEmpty(md4) || string.Equals(md4, HashType.MD4.ZeroString, StringComparison.OrdinalIgnoreCase);

            string? md5 = rom.ReadString(Rom.MD5Key);
            bool md5Null = string.IsNullOrEmpty(md5) || string.Equals(md5, HashType.MD5.ZeroString, StringComparison.OrdinalIgnoreCase);

            string? ripeMD128 = rom.ReadString(Rom.RIPEMD128Key);
            bool ripeMD128Null = string.IsNullOrEmpty(value: ripeMD128) || string.Equals(ripeMD128, HashType.RIPEMD128.ZeroString, StringComparison.OrdinalIgnoreCase);

            string? ripeMD160 = rom.ReadString(Rom.RIPEMD160Key);
            bool ripeMD160Null = string.IsNullOrEmpty(ripeMD160) || string.Equals(ripeMD160, HashType.RIPEMD160.ZeroString, StringComparison.OrdinalIgnoreCase);

            string? sha1 = rom.ReadString(Rom.SHA1Key);
            bool sha1Null = string.IsNullOrEmpty(sha1) || string.Equals(sha1, HashType.SHA1.ZeroString, StringComparison.OrdinalIgnoreCase);

            string? sha256 = rom.ReadString(Rom.SHA256Key);
            bool sha256Null = string.IsNullOrEmpty(sha256) || string.Equals(sha256, HashType.SHA256.ZeroString, StringComparison.OrdinalIgnoreCase);

            string? sha384 = rom.ReadString(Rom.SHA384Key);
            bool sha384Null = string.IsNullOrEmpty(sha384) || string.Equals(sha384, HashType.SHA384.ZeroString, StringComparison.OrdinalIgnoreCase);

            string? sha512 = rom.ReadString(Rom.SHA512Key);
            bool sha512Null = string.IsNullOrEmpty(sha512) || string.Equals(sha512, HashType.SHA512.ZeroString, StringComparison.OrdinalIgnoreCase);

            string? spamsum = rom.ReadString(Rom.SpamSumKey);
            bool spamsumNull = string.IsNullOrEmpty(spamsum) || string.Equals(spamsum, HashType.SpamSum.ZeroString, StringComparison.OrdinalIgnoreCase);

            return crc16Null
                && crcNull
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

        #endregion

        #region Information Filling

        /// <summary>
        /// Fill any missing size and hash information from another DictionaryBase
        /// </summary>
        public static void FillMissingHashes(this DictionaryBase? self, DictionaryBase? other)
        {
            if (self is null || other is null)
                return;

#if NETCOREAPP || NETSTANDARD2_0_OR_GREATER
            switch (self, other)
            {
                case (Disk diskSelf, Disk diskOther):
                    diskSelf.FillMissingHashes(diskOther);
                    break;
                case (Media mediaSelf, Media mediaOther):
                    mediaSelf.FillMissingHashes(mediaOther);
                    break;
                case (Rom romSelf, Rom romOther):
                    romSelf.FillMissingHashes(romOther);
                    break;

                default:
                    break;
            }
#else
            if (self is Disk diskSelf && other is Disk diskOther)
                diskSelf.FillMissingHashes(diskOther);
            else if (self is Media mediaSelf && other is Media mediaOther)
                mediaSelf.FillMissingHashes(mediaOther);
            else if (self is Rom romSelf && other is Rom romOther)
                romSelf.FillMissingHashes(romOther);
#endif
        }

        /// <summary>
        /// Fill any missing size and hash information from another Disk
        /// </summary>
        private static void FillMissingHashes(this Disk? self, Disk? other)
        {
            if (self is null || other is null)
                return;

            string? selfMd5 = self.ReadString(Disk.MD5Key);
            string? otherMd5 = other.ReadString(Disk.MD5Key);
            if (string.IsNullOrEmpty(selfMd5) && !string.IsNullOrEmpty(otherMd5))
                self[Disk.MD5Key] = otherMd5;

            string? selfSha1 = self.ReadString(Disk.SHA1Key);
            string? otherSha1 = other.ReadString(Disk.SHA1Key);
            if (string.IsNullOrEmpty(selfSha1) && !string.IsNullOrEmpty(otherSha1))
                self[Disk.SHA1Key] = otherSha1;
        }

        /// <summary>
        /// Fill any missing size and hash information from another Media
        /// </summary>
        private static void FillMissingHashes(this Media? self, Media? other)
        {
            if (self is null || other is null)
                return;

            string? selfMd5 = self.ReadString(Media.MD5Key);
            string? otherMd5 = other.ReadString(Media.MD5Key);
            if (string.IsNullOrEmpty(selfMd5) && !string.IsNullOrEmpty(otherMd5))
                self[Media.MD5Key] = otherMd5;

            string? selfSha1 = self.ReadString(Media.SHA1Key);
            string? otherSha1 = other.ReadString(Media.SHA1Key);
            if (string.IsNullOrEmpty(selfSha1) && !string.IsNullOrEmpty(otherSha1))
                self[Media.SHA1Key] = otherSha1;

            string? selfSha256 = self.ReadString(Media.SHA256Key);
            string? otherSha256 = other.ReadString(Media.SHA256Key);
            if (string.IsNullOrEmpty(selfSha256) && !string.IsNullOrEmpty(otherSha256))
                self[Media.SHA256Key] = otherSha256;

            string? selfSpamSum = self.ReadString(Media.SpamSumKey);
            string? otherSpamSum = other.ReadString(Media.SpamSumKey);
            if (string.IsNullOrEmpty(selfSpamSum) && !string.IsNullOrEmpty(otherSpamSum))
                self[Media.SpamSumKey] = otherSpamSum;
        }

        /// <summary>
        /// Fill any missing size and hash information from another Rom
        /// </summary>
        private static void FillMissingHashes(this Rom? self, Rom? other)
        {
            if (self is null || other is null)
                return;

            long? selfSize = self.ReadLong(Rom.SizeKey);
            long? otherSize = other.ReadLong(Rom.SizeKey);
            if (selfSize is null && otherSize is not null)
                self[Rom.SizeKey] = otherSize;

            string? selfCrc16 = self.ReadString(Rom.CRC16Key);
            string? otherCrc16 = other.ReadString(Rom.CRC16Key);
            if (string.IsNullOrEmpty(selfCrc16) && !string.IsNullOrEmpty(otherCrc16))
                self[Rom.CRC16Key] = otherCrc16;

            string? selfCrc = self.ReadString(Rom.CRCKey);
            string? otherCrc = other.ReadString(Rom.CRCKey);
            if (string.IsNullOrEmpty(selfCrc) && !string.IsNullOrEmpty(otherCrc))
                self[Rom.CRCKey] = otherCrc;

            string? selfCrc64 = self.ReadString(Rom.CRC64Key);
            string? otherCrc64 = other.ReadString(Rom.CRC64Key);
            if (string.IsNullOrEmpty(selfCrc64) && !string.IsNullOrEmpty(otherCrc64))
                self[Rom.CRC64Key] = otherCrc64;

            string? selfMd2 = self.ReadString(Rom.MD2Key);
            string? otherMd2 = other.ReadString(Rom.MD2Key);
            if (string.IsNullOrEmpty(selfMd2) && !string.IsNullOrEmpty(otherMd2))
                self[Rom.MD2Key] = otherMd2;

            string? selfMd4 = self.ReadString(Rom.MD4Key);
            string? otherMd4 = other.ReadString(Rom.MD4Key);
            if (string.IsNullOrEmpty(selfMd4) && !string.IsNullOrEmpty(otherMd4))
                self[Rom.MD4Key] = otherMd4;

            string? selfMd5 = self.ReadString(Rom.MD5Key);
            string? otherMd5 = other.ReadString(Rom.MD5Key);
            if (string.IsNullOrEmpty(selfMd5) && !string.IsNullOrEmpty(otherMd5))
                self[Rom.MD5Key] = otherMd5;

            string? selfRipeMD128 = self.ReadString(Rom.RIPEMD128Key);
            string? otherRipeMD128 = other.ReadString(Rom.RIPEMD128Key);
            if (string.IsNullOrEmpty(selfRipeMD128) && !string.IsNullOrEmpty(otherRipeMD128))
                self[Rom.RIPEMD128Key] = otherRipeMD128;

            string? selfRipeMD160 = self.ReadString(Rom.RIPEMD160Key);
            string? otherRipeMD160 = other.ReadString(Rom.RIPEMD160Key);
            if (string.IsNullOrEmpty(selfRipeMD160) && !string.IsNullOrEmpty(otherRipeMD160))
                self[Rom.RIPEMD160Key] = otherRipeMD160;

            string? selfSha1 = self.ReadString(Rom.SHA1Key);
            string? otherSha1 = other.ReadString(Rom.SHA1Key);
            if (string.IsNullOrEmpty(selfSha1) && !string.IsNullOrEmpty(otherSha1))
                self[Rom.SHA1Key] = otherSha1;

            string? selfSha256 = self.ReadString(Rom.SHA256Key);
            string? otherSha256 = other.ReadString(Rom.SHA256Key);
            if (string.IsNullOrEmpty(selfSha256) && !string.IsNullOrEmpty(otherSha256))
                self[Rom.SHA256Key] = otherSha256;

            string? selfSha384 = self.ReadString(Rom.SHA384Key);
            string? otherSha384 = other.ReadString(Rom.SHA384Key);
            if (string.IsNullOrEmpty(selfSha384) && !string.IsNullOrEmpty(otherSha384))
                self[Rom.SHA384Key] = otherSha384;

            string? selfSha512 = self.ReadString(Rom.SHA512Key);
            string? otherSha512 = other.ReadString(Rom.SHA512Key);
            if (string.IsNullOrEmpty(selfSha512) && !string.IsNullOrEmpty(otherSha512))
                self[Rom.SHA512Key] = otherSha512;

            string? selfSpamSum = self.ReadString(Rom.SpamSumKey);
            string? otherSpamSum = other.ReadString(Rom.SpamSumKey);
            if (string.IsNullOrEmpty(selfSpamSum) && !string.IsNullOrEmpty(otherSpamSum))
                self[Rom.SpamSumKey] = otherSpamSum;
        }

        #endregion

        #region String to Enum

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
                _ => null,
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
        /// Get bool? value from input string
        /// </summary>
        /// <param name="yesno">String to get value from</param>
        /// <returns>bool? corresponding to the string</returns>
        public static bool? AsYesNo(this string? yesno)
        {
            return yesno?.ToLowerInvariant() switch
            {
                "yes" or "true" => true,
                "no" or "false" => false,
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
        public static string? AsStringValue(this SupportStatus value)
        {
            return value switch
            {
                SupportStatus.Good => "good",
                SupportStatus.Imperfect => "imperfect",
                SupportStatus.Preliminary => "preliminary",
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
