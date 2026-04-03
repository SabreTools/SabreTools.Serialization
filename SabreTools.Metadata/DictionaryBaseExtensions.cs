using System;
using System.Collections.Generic;
using System.Linq;
using SabreTools.Data.Extensions;
using SabreTools.Data.Models.Metadata;

namespace SabreTools.Metadata
{
    public static class DictionaryBaseExtensions
    {
        #region Equality Checking

        /// <summary>
        /// Check equality of two DictionaryBase objects
        /// </summary>
        /// TODO: Fix equality checking with case sensitivity of string properties
        public static bool EqualTo(this DictionaryBase self, DictionaryBase other)
        {
            // Check types first
            if (self.GetType() != other.GetType())
                return false;

            // Check based on the item type
#if NETCOREAPP || NETSTANDARD2_0_OR_GREATER
            return (self, other) switch
            {
                (Disk diskSelf, Disk diskOther) => EqualsImpl(diskSelf, diskOther),
                (Media mediaSelf, Media mediaOther) => EqualsImpl(mediaSelf, mediaOther),
                (Rom romSelf, Rom romOther) => EqualsImpl(romSelf, romOther),
                _ => EqualsImpl(self, other),
            };
#else
            if (self is Disk diskSelf && other is Disk diskOther)
                return EqualsImpl(diskSelf, diskOther);
            else if (self is Media mediaSelf && other is Media mediaOther)
                return EqualsImpl(mediaSelf, mediaOther);
            else if (self is Rom romSelf && other is Rom romOther)
                return EqualsImpl(romSelf, romOther);
            else
                return EqualsImpl(self, other);
#endif
        }

        /// <summary>
        /// Check equality of two DictionaryBase objects
        /// </summary>
        /// TODO: Fix equality checking with case sensitivity of string properties
        private static bool EqualsImpl(this DictionaryBase self, DictionaryBase other)
        {
            // If the number of key-value pairs doesn't match, they can't match
            if (self.Count != other.Count)
                return false;

            // If any keys are missing on either side, they can't match
            var selfKeys = new HashSet<string>(self.Keys);
            var otherKeys = new HashSet<string>(other.Keys);
            if (!selfKeys.SetEquals(otherKeys))
                return false;

            // Check names
            if (self.GetName() != other.GetName())
                return false;

            // Handle individual type properties
            if (self is Adjuster selfAdjuster && other is Adjuster otherAdjuster)
            {
                if (selfAdjuster.Default != otherAdjuster.Default)
                    return false;
            }
            else if (self is Analog selfAnalog && other is Analog otherAnalog)
            {
                if (!selfAnalog.Equals(otherAnalog))
                    return false;
            }
            else if (self is Archive selfArchive && other is Archive otherArchive)
            {
                if (selfArchive.Description != otherArchive.Description)
                    return false;
            }
            else if (self is BiosSet selfBiosSet && other is BiosSet otherBiosSet)
            {
                if (!selfBiosSet.Equals(otherBiosSet))
                    return false;
            }
            else if (self is Chip selfChip && other is Chip otherChip)
            {
                if (!selfChip.Equals(otherChip))
                    return false;
            }
            else if (self is Condition selfCondition && other is Condition otherCondition)
            {
                if (!selfCondition.Equals(otherCondition))
                    return false;
            }
            else if (self is Configuration selfConfiguration && other is Configuration otherConfiguration)
            {
                if (selfConfiguration.Mask != otherConfiguration.Mask)
                    return false;
                if (selfConfiguration.Tag != otherConfiguration.Tag)
                    return false;
            }
            else if (self is ConfLocation selfConfLocation && other is ConfLocation otherConfLocation)
            {
                if (!selfConfLocation.Equals(otherConfLocation))
                    return false;
            }
            else if (self is ConfSetting selfConfSetting && other is ConfSetting otherConfSetting)
            {
                if (selfConfSetting.Default != otherConfSetting.Default)
                    return false;
                if (selfConfSetting.Value != otherConfSetting.Value)
                    return false;
            }
            else if (self is Control selfControl && other is Control otherControl)
            {
                if (!selfControl.Equals(otherControl))
                    return false;
            }
            else if (self is DataArea selfDataArea && other is DataArea otherDataArea)
            {
                if (selfDataArea.Endianness != otherDataArea.Endianness)
                    return false;
                if (selfDataArea.Size != otherDataArea.Size)
                    return false;
            }
            else if (self is Device selfDevice && other is Device otherDevice)
            {
                if (selfDevice.DeviceType != otherDevice.DeviceType)
                    return false;
                if (selfDevice.FixedImage != otherDevice.FixedImage)
                    return false;
                if (selfDevice.Interface != otherDevice.Interface)
                    return false;
                if (selfDevice.Mandatory != otherDevice.Mandatory)
                    return false;
                if (selfDevice.Tag != otherDevice.Tag)
                    return false;
            }
            else if (self is DipLocation selfDipLocation && other is DipLocation otherDipLocation)
            {
                if (!selfDipLocation.Equals(otherDipLocation))
                    return false;
            }
            else if (self is DipSwitch selfDipSwitch && other is DipSwitch otherDipSwitch)
            {
                if (selfDipSwitch.Default != otherDipSwitch.Default)
                    return false;
                if (selfDipSwitch.Mask != otherDipSwitch.Mask)
                    return false;
                if (selfDipSwitch.Tag != otherDipSwitch.Tag)
                    return false;
            }
            else if (self is DipValue selfDipValue && other is DipValue otherDipValue)
            {
                if (selfDipValue.Default != otherDipValue.Default)
                    return false;
                if (selfDipValue.Value != otherDipValue.Value)
                    return false;
            }
            else if (self is Disk selfDisk && other is Disk otherDisk)
            {
                // Intentionally skipped here
            }
            else if (self is Display selfDisplay && other is Display otherDisplay)
            {
                if (!selfDisplay.Equals(otherDisplay))
                    return false;
            }
            else if (self is Driver selfDriver && other is Driver otherDriver)
            {
                if (selfDriver.Blit != otherDriver.Blit)
                    return false;
                if (selfDriver.Cocktail != otherDriver.Cocktail)
                    return false;
                if (selfDriver.Color != otherDriver.Color)
                    return false;
                if (selfDriver.Emulation != otherDriver.Emulation)
                    return false;
                if (selfDriver.Incomplete != otherDriver.Incomplete)
                    return false;
                if (selfDriver.NoSoundHardware != otherDriver.NoSoundHardware)
                    return false;
                if (selfDriver.RequiresArtwork != otherDriver.RequiresArtwork)
                    return false;
                if (selfDriver.SaveState != otherDriver.SaveState)
                    return false;
                if (selfDriver.Sound != otherDriver.Sound)
                    return false;
                if (selfDriver.Status != otherDriver.Status)
                    return false;
                if (selfDriver.Unofficial != otherDriver.Unofficial)
                    return false;
            }
            else if (self is Feature selfFeature && other is Feature otherFeature)
            {
                if (!selfFeature.Equals(otherFeature))
                    return false;
            }
            else if (self is Header selfHeader && other is Header otherHeader)
            {
                if (selfHeader.Author != otherHeader.Author)
                    return false;
                if (selfHeader.BiosMode != otherHeader.BiosMode)
                    return false;
                if (selfHeader.Build != otherHeader.Build)
                    return false;
                if (selfHeader.Category != otherHeader.Category)
                    return false;
                if (selfHeader.Comment != otherHeader.Comment)
                    return false;
                if (selfHeader.Date != otherHeader.Date)
                    return false;
                if (selfHeader.DatVersion != otherHeader.DatVersion)
                    return false;
                if (selfHeader.Debug != otherHeader.Debug)
                    return false;
                if (selfHeader.Description != otherHeader.Description)
                    return false;
                if (selfHeader.Email != otherHeader.Email)
                    return false;
                if (selfHeader.EmulatorVersion != otherHeader.EmulatorVersion)
                    return false;
                if (selfHeader.ForceMerging != otherHeader.ForceMerging)
                    return false;
                if (selfHeader.ForceNodump != otherHeader.ForceNodump)
                    return false;
                if (selfHeader.ForcePacking != otherHeader.ForcePacking)
                    return false;
                if (selfHeader.ForceZipping != otherHeader.ForceZipping)
                    return false;
                if (selfHeader.Homepage != otherHeader.Homepage)
                    return false;
                if (selfHeader.Id != otherHeader.Id)
                    return false;
                if (selfHeader.LockBiosMode != otherHeader.LockBiosMode)
                    return false;
                if (selfHeader.LockRomMode != otherHeader.LockRomMode)
                    return false;
                if (selfHeader.LockSampleMode != otherHeader.LockSampleMode)
                    return false;
                if (selfHeader.MameConfig != otherHeader.MameConfig)
                    return false;
                if (selfHeader.Name != otherHeader.Name)
                    return false;
                if (selfHeader.Notes != otherHeader.Notes)
                    return false;
                if (selfHeader.Plugin != otherHeader.Plugin)
                    return false;
                if (selfHeader.RefName != otherHeader.RefName)
                    return false;
                if (selfHeader.RomMode != otherHeader.RomMode)
                    return false;
                if (selfHeader.RomTitle != otherHeader.RomTitle)
                    return false;
                if (selfHeader.RootDir != otherHeader.RootDir)
                    return false;
                if (selfHeader.SampleMode != otherHeader.SampleMode)
                    return false;
                if (selfHeader.System != otherHeader.System)
                    return false;
                if (selfHeader.Timestamp != otherHeader.Timestamp)
                    return false;
                if (selfHeader.Type != otherHeader.Type)
                    return false;
                if (selfHeader.Url != otherHeader.Url)
                    return false;
                if (selfHeader.Version != otherHeader.Version)
                    return false;
            }
            else if (self is Info selfInfo && other is Info otherInfo)
            {
                if (!selfInfo.Equals(otherInfo))
                    return false;
            }
            else if (self is Input selfInput && other is Input otherInput)
            {
                if (selfInput.Service != otherInput.Service)
                    return false;
                if (selfInput.Tilt != otherInput.Tilt)
                    return false;
            }
            else if (self is Instance selfInstance && other is Instance otherInstance)
            {
                if (!selfInstance.Equals(otherInstance))
                    return false;
            }
            else if (self is Machine selfMachine && other is Machine otherMachine)
            {
                if (selfMachine.Description != otherMachine.Description)
                    return false;
                if (selfMachine.IsBios != otherMachine.IsBios)
                    return false;
                if (selfMachine.IsDevice != otherMachine.IsDevice)
                    return false;
                if (selfMachine.IsMechanical != otherMachine.IsMechanical)
                    return false;
                if (selfMachine.Runnable != otherMachine.Runnable)
                    return false;
                if (selfMachine.Supported != otherMachine.Supported)
                    return false;
            }
            else if (self is Media selfMedia && other is Media otherMedia)
            {
                // Intentionally skipped here
            }
            else if (self is Original selfOriginal && other is Original otherOriginal)
            {
                if (selfOriginal.Content != otherOriginal.Content)
                    return false;
            }
            else if (self is Part selfPart && other is Part otherPart)
            {
                if (selfPart.Interface != otherPart.Interface)
                    return false;
            }
            else if (self is Port selfPort && other is Port otherPort)
            {
                if (selfPort.Tag != otherPort.Tag)
                    return false;
            }
            else if (self is RamOption selfRamOption && other is RamOption otherRamOption)
            {
                if (!selfRamOption.Equals(otherRamOption))
                    return false;
            }
            else if (self is Release selfRelease && other is Release otherRelease)
            {
                if (selfRelease.Default != otherRelease.Default)
                    return false;
            }
            else if (self is Rom selfRom && other is Rom otherRom)
            {
                // Intentionally skipped here
            }
            else if (self is SharedFeat selfSharedFeat && other is SharedFeat otherSharedFeat)
            {
                if (!selfSharedFeat.Equals(otherSharedFeat))
                    return false;
            }
            else if (self is SlotOption selfSlotOption && other is SlotOption otherSlotOption)
            {
                if (!selfSlotOption.Equals(otherSlotOption))
                    return false;
            }
            else if (self is Software selfSoftware && other is Software otherSoftware)
            {
                if (selfSoftware.Description != otherSoftware.Description)
                    return false;
                if (selfSoftware.Supported != otherSoftware.Supported)
                    return false;
            }
            else if (self is SoftwareList selfSoftwareList && other is SoftwareList otherSoftwareList)
            {
                if (selfSoftwareList.Description != otherSoftwareList.Description)
                    return false;
                if (selfSoftwareList.Status != otherSoftwareList.Status)
                    return false;
                if (selfSoftwareList.Tag != otherSoftwareList.Tag)
                    return false;
            }
            else if (self is Sound selfSound && other is Sound otherSound)
            {
                if (!selfSound.Equals(otherSound))
                    return false;
            }
            else if (self is Video selfVideo && other is Video otherVideo)
            {
                if (!selfVideo.Equals(otherVideo))
                    return false;
            }

            // Check all pairs to see if they're equal
            foreach (var kvp in self)
            {
#if NETCOREAPP || NETSTANDARD2_0_OR_GREATER
                switch (kvp.Value, other[kvp.Key])
                {
                    case (string selfString, string otherString):
                        if (!string.Equals(selfString, otherString, StringComparison.OrdinalIgnoreCase))
                            return false;
                        break;

                    case (ModelBackedItem selfMbi, ModelBackedItem otherMbi):
                        if (!selfMbi.Equals(otherMbi))
                            return false;
                        break;

                    case (DictionaryBase selfDb, DictionaryBase otherDb):
                        if (!selfDb.Equals(otherDb))
                            return false;
                        break;

                    // TODO: Make this case-insensitive
                    case (string[] selfStrArr, string[] otherStrArr):
                        if (selfStrArr.Length != otherStrArr.Length)
                            return false;
                        if (selfStrArr.Except(otherStrArr).Any())
                            return false;
                        if (otherStrArr.Except(selfStrArr).Any())
                            return false;
                        break;

                    // TODO: Fix the logic here for real equality
                    case (DictionaryBase[] selfDbArr, DictionaryBase[] otherDbArr):
                        if (selfDbArr.Length != otherDbArr.Length)
                            return false;
                        if (selfDbArr.Except(otherDbArr).Any())
                            return false;
                        if (otherDbArr.Except(selfDbArr).Any())
                            return false;
                        break;

                    default:
                        // Handle cases where a null is involved
                        if (kvp.Value is null && other[kvp.Key] is null)
                            return true;
                        else if (kvp.Value is null && other[kvp.Key] is not null)
                            return false;
                        else if (kvp.Value is not null && other[kvp.Key] is null)
                            return false;

                        // Try to rely on type hashes
                        else if (kvp.Value!.GetHashCode() != other[kvp.Key]!.GetHashCode())
                            return false;

                        break;
                }
#else
                if (kvp.Value is string selfString && other[kvp.Key] is string otherString)
                {
                    if (!string.Equals(selfString, otherString, StringComparison.OrdinalIgnoreCase))
                        return false;
                }
                else if (kvp.Value is ModelBackedItem selfMbi && other[kvp.Key] is ModelBackedItem otherMbi)
                {
                    if (!selfMbi.Equals(otherMbi))
                        return false;
                }
                else if (kvp.Value is DictionaryBase selfDb && other[kvp.Key] is DictionaryBase otherDb)
                {
                    if (!selfDb.Equals(otherDb))
                        return false;
                }
                else if (kvp.Value is string[] selfStrArr && other[kvp.Key] is string[] otherStrArr)
                {
                    // TODO: Make this case-insensitive
                    if (selfStrArr.Length != otherStrArr.Length)
                        return false;
                    if (selfStrArr.Except(otherStrArr).Any())
                        return false;
                    if (otherStrArr.Except(selfStrArr).Any())
                        return false;
                }
                else if (kvp.Value is DictionaryBase[] selfDbArr && other[kvp.Key] is DictionaryBase[] otherDbArr)
                {
                    // TODO: Fix the logic here for real equality
                    if (selfDbArr.Length != otherDbArr.Length)
                        return false;
                    if (selfDbArr.Except(otherDbArr).Any())
                        return false;
                    if (otherDbArr.Except(selfDbArr).Any())
                        return false;
                }
                else
                {
                    // Handle cases where a null is involved
                    if (kvp.Value is null && other[kvp.Key] is null)
                        return true;
                    else if (kvp.Value is null && other[kvp.Key] is not null)
                        return false;
                    else if (kvp.Value is not null && other[kvp.Key] is null)
                        return false;

                    // Try to rely on type hashes
                    else if (kvp.Value!.GetHashCode() != other[kvp.Key]!.GetHashCode())
                        return false;
                }
#endif
            }

            return true;
        }

        /// <summary>
        /// Check equality of two Disk objects
        /// </summary>
        private static bool EqualsImpl(this Disk self, Disk other)
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
        private static bool EqualsImpl(this Media self, Media other)
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
        private static bool EqualsImpl(this Rom self, Rom other)
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
    }
}
