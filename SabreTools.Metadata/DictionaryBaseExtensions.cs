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
            if (self is Adjuster selfAdjuster && other is Adjuster cloneAdjuster)
            {
                if (selfAdjuster.Default != cloneAdjuster.Default)
                    return false;
            }
            else if (self is Analog selfAnalog && other is Analog cloneAnalog)
            {
                if (selfAnalog.Mask != cloneAnalog.Mask)
                    return false;
            }
            else if (self is Archive selfArchive && other is Archive cloneArchive)
            {
                if (selfArchive.Description != cloneArchive.Description)
                    return false;
            }
            else if (self is BiosSet selfBiosSet && other is BiosSet cloneBiosSet)
            {
                if (selfBiosSet.Default != cloneBiosSet.Default)
                    return false;
                if (selfBiosSet.Description != cloneBiosSet.Description)
                    return false;
            }
            else if (self is Chip selfChip && other is Chip cloneChip)
            {
                if (selfChip.ChipType != cloneChip.ChipType)
                    return false;
                if (selfChip.SoundOnly != cloneChip.SoundOnly)
                    return false;
                if (selfChip.Tag != cloneChip.Tag)
                    return false;
            }
            else if (self is Condition selfCondition && other is Condition cloneCondition)
            {
                if (selfCondition.Mask != cloneCondition.Mask)
                    return false;
                if (selfCondition.Relation != cloneCondition.Relation)
                    return false;
                if (selfCondition.Tag != cloneCondition.Tag)
                    return false;
                if (selfCondition.Value != cloneCondition.Value)
                    return false;
            }
            else if (self is Configuration selfConfiguration && other is Configuration cloneConfiguration)
            {
                if (selfConfiguration.Mask != cloneConfiguration.Mask)
                    return false;
                if (selfConfiguration.Tag != cloneConfiguration.Tag)
                    return false;
            }
            else if (self is ConfLocation selfConfLocation && other is ConfLocation cloneConfLocation)
            {
                if (selfConfLocation.Inverted != cloneConfLocation.Inverted)
                    return false;
            }
            else if (self is ConfSetting selfConfSetting && other is ConfSetting cloneConfSetting)
            {
                if (selfConfSetting.Default != cloneConfSetting.Default)
                    return false;
                if (selfConfSetting.Value != cloneConfSetting.Value)
                    return false;
            }
            else if (self is Control selfControl && other is Control cloneControl)
            {
                if (selfControl.ControlType != cloneControl.ControlType)
                    return false;
                if (selfControl.Reverse != cloneControl.Reverse)
                    return false;
            }
            else if (self is DataArea selfDataArea && other is DataArea cloneDataArea)
            {
                if (selfDataArea.Endianness != cloneDataArea.Endianness)
                    return false;
            }
            else if (self is Device selfDevice && other is Device cloneDevice)
            {
                if (selfDevice.DeviceType != cloneDevice.DeviceType)
                    return false;
                if (selfDevice.FixedImage != cloneDevice.FixedImage)
                    return false;
                if (selfDevice.Interface != cloneDevice.Interface)
                    return false;
                if (selfDevice.Mandatory != cloneDevice.Mandatory)
                    return false;
                if (selfDevice.Tag != cloneDevice.Tag)
                    return false;
            }
            else if (self is DipLocation selfDipLocation && other is DipLocation cloneDipLocation)
            {
                if (selfDipLocation.Inverted != cloneDipLocation.Inverted)
                    return false;
            }
            else if (self is DipSwitch selfDipSwitch && other is DipSwitch cloneDipSwitch)
            {
                if (selfDipSwitch.Default != cloneDipSwitch.Default)
                    return false;
                if (selfDipSwitch.Mask != cloneDipSwitch.Mask)
                    return false;
                if (selfDipSwitch.Tag != cloneDipSwitch.Tag)
                    return false;
            }
            else if (self is DipValue selfDipValue && other is DipValue cloneDipValue)
            {
                if (selfDipValue.Default != cloneDipValue.Default)
                    return false;
                if (selfDipValue.Value != cloneDipValue.Value)
                    return false;
            }
            else if (self is Display selfDisplay && other is Display cloneDisplay)
            {
                if (selfDisplay.DisplayType != cloneDisplay.DisplayType)
                    return false;
                if (selfDisplay.FlipX != cloneDisplay.FlipX)
                    return false;
                if (selfDisplay.Tag != cloneDisplay.Tag)
                    return false;
            }
            else if (self is Driver selfDriver && other is Driver cloneDriver)
            {
                if (selfDriver.Blit != cloneDriver.Blit)
                    return false;
                if (selfDriver.Cocktail != cloneDriver.Cocktail)
                    return false;
                if (selfDriver.Color != cloneDriver.Color)
                    return false;
                if (selfDriver.Emulation != cloneDriver.Emulation)
                    return false;
                if (selfDriver.Incomplete != cloneDriver.Incomplete)
                    return false;
                if (selfDriver.NoSoundHardware != cloneDriver.NoSoundHardware)
                    return false;
                if (selfDriver.RequiresArtwork != cloneDriver.RequiresArtwork)
                    return false;
                if (selfDriver.SaveState != cloneDriver.SaveState)
                    return false;
                if (selfDriver.Sound != cloneDriver.Sound)
                    return false;
                if (selfDriver.Status != cloneDriver.Status)
                    return false;
                if (selfDriver.Unofficial != cloneDriver.Unofficial)
                    return false;
            }
            else if (self is Feature selfFeature && other is Feature cloneFeature)
            {
                if (selfFeature.FeatureType != cloneFeature.FeatureType)
                    return false;
                if (selfFeature.Overall != cloneFeature.Overall)
                    return false;
                if (selfFeature.Status != cloneFeature.Status)
                    return false;
                if (selfFeature.Value != cloneFeature.Value)
                    return false;
            }
            else if (self is Header selfHeader && other is Header cloneHeader)
            {
                if (selfHeader.BiosMode != cloneHeader.BiosMode)
                    return false;
                if (selfHeader.Debug != cloneHeader.Debug)
                    return false;
                if (selfHeader.Description != cloneHeader.Description)
                    return false;
                if (selfHeader.ForceMerging != cloneHeader.ForceMerging)
                    return false;
                if (selfHeader.ForceNodump != cloneHeader.ForceNodump)
                    return false;
                if (selfHeader.ForcePacking != cloneHeader.ForcePacking)
                    return false;
                if (selfHeader.ForceZipping != cloneHeader.ForceZipping)
                    return false;
                if (selfHeader.LockBiosMode != cloneHeader.LockBiosMode)
                    return false;
                if (selfHeader.LockRomMode != cloneHeader.LockRomMode)
                    return false;
                if (selfHeader.LockSampleMode != cloneHeader.LockSampleMode)
                    return false;
                if (selfHeader.RomMode != cloneHeader.RomMode)
                    return false;
                if (selfHeader.SampleMode != cloneHeader.SampleMode)
                    return false;
            }
            else if (self is Info selfInfo && other is Info cloneInfo)
            {
                if (selfInfo.Value != cloneInfo.Value)
                    return false;
            }
            else if (self is Input selfInput && other is Input cloneInput)
            {
                if (selfInput.Service != cloneInput.Service)
                    return false;
                if (selfInput.Tilt != cloneInput.Tilt)
                    return false;
            }
            else if (self is Instance selfInstance && other is Instance cloneInstance)
            {
                if (selfInstance.BriefName != cloneInstance.BriefName)
                    return false;
            }
            else if (self is Machine selfMachine && other is Machine cloneMachine)
            {
                if (selfMachine.Description != cloneMachine.Description)
                    return false;
                if (selfMachine.IsBios != cloneMachine.IsBios)
                    return false;
                if (selfMachine.IsDevice != cloneMachine.IsDevice)
                    return false;
                if (selfMachine.IsMechanical != cloneMachine.IsMechanical)
                    return false;
                if (selfMachine.Runnable != cloneMachine.Runnable)
                    return false;
                if (selfMachine.Supported != cloneMachine.Supported)
                    return false;
            }
            else if (self is Original selfOriginal && other is Original cloneOriginal)
            {
                if (selfOriginal.Content != cloneOriginal.Content)
                    return false;
            }
            else if (self is Part selfPart && other is Part clonePart)
            {
                if (selfPart.Interface != clonePart.Interface)
                    return false;
            }
            else if (self is Port selfPort && other is Port clonePort)
            {
                if (selfPort.Tag != clonePort.Tag)
                    return false;
            }
            else if (self is RamOption selfRamOption && other is RamOption cloneRamOption)
            {
                if (selfRamOption.Content != cloneRamOption.Content)
                    return false;
                if (selfRamOption.Default != cloneRamOption.Default)
                    return false;
            }
            else if (self is Release selfRelease && other is Release cloneRelease)
            {
                if (selfRelease.Default != cloneRelease.Default)
                    return false;
            }
            else if (self is SharedFeat selfSharedFeat && other is SharedFeat cloneSharedFeat)
            {
                if (selfSharedFeat.Value != cloneSharedFeat.Value)
                    return false;
            }
            else if (self is SlotOption selfSlotOption && other is SlotOption cloneSlotOption)
            {
                if (selfSlotOption.Default != cloneSlotOption.Default)
                    return false;
                if (selfSlotOption.DevName != cloneSlotOption.DevName)
                    return false;
            }
            else if (self is Software selfSoftware && other is Software cloneSoftware)
            {
                if (selfSoftware.Description != cloneSoftware.Description)
                    return false;
                if (selfSoftware.Supported != cloneSoftware.Supported)
                    return false;
            }
            else if (self is SoftwareList selfSoftwareList && other is SoftwareList cloneSoftwareList)
            {
                if (selfSoftwareList.Description != cloneSoftwareList.Description)
                    return false;
                if (selfSoftwareList.Status != cloneSoftwareList.Status)
                    return false;
                if (selfSoftwareList.Tag != cloneSoftwareList.Tag)
                    return false;
            }
            else if (self is Video selfVideo && other is Video cloneVideo)
            {
                if (selfVideo.Screen != cloneVideo.Screen)
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
