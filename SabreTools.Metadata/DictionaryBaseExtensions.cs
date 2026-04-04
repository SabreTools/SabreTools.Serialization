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
                (Adjuster selfAdjuster, Adjuster otherAdjuster) => selfAdjuster.Equals(otherAdjuster),
                (Analog selfAnalog, Analog otherAnalog) => selfAnalog.Equals(otherAnalog),
                (Archive selfArchive, Archive otherArchive) => selfArchive.Equals(otherArchive),
                (BiosSet selfBiosSet, BiosSet otherBiosSet) => selfBiosSet.Equals(otherBiosSet),
                (Chip selfChip, Chip otherChip) => selfChip.Equals(otherChip),
                (Condition selfCondition, Condition otherCondition) => selfCondition.Equals(otherCondition),
                (Configuration selfConfiguration, Configuration otherConfiguration) => selfConfiguration.Equals(otherConfiguration),
                (ConfLocation selfConfLocation, ConfLocation otherConfLocation) => selfConfLocation.Equals(otherConfLocation),
                (ConfSetting selfConfSetting, ConfSetting otherConfSetting) => selfConfSetting.Equals(otherConfSetting),
                (Control selfControl, Control otherControl) => selfControl.Equals(otherControl),
                (DataArea selfDataArea, DataArea otherDataArea) => selfDataArea.Equals(otherDataArea),
                (Device selfDevice, Device otherDevice) => selfDevice.Equals(otherDevice),
                (DipLocation selfDipLocation, DipLocation otherDipLocation) => selfDipLocation.Equals(otherDipLocation),
                (DipSwitch selfDipSwitch, DipSwitch otherDipSwitch) => selfDipSwitch.Equals(otherDipSwitch),
                (DipValue selfDipValue, DipValue otherDipValue) => selfDipValue.Equals(otherDipValue),
                (Disk diskSelf, Disk diskOther) => EqualsImpl(diskSelf, diskOther),
                (DiskArea selfDiskArea, DiskArea otherDiskArea) => selfDiskArea.Equals(otherDiskArea),
                (Display selfDisplay, Display otherDisplay) => selfDisplay.Equals(otherDisplay),
                (Driver selfDriver, Driver otherDriver) => selfDriver.Equals(otherDriver),
                (Dump selfDump, Dump otherDump) => selfDump.Equals(otherDump),
                (Feature selfFeature, Feature otherFeature) => selfFeature.Equals(otherFeature),
                (Info selfInfo, Info otherInfo) => selfInfo.Equals(otherInfo),
                (Input selfInput, Input otherInput) => selfInput.Equals(otherInput),
                (Instance selfInstance, Instance otherInstance) => selfInstance.Equals(otherInstance),
                (Media mediaSelf, Media mediaOther) => EqualsImpl(mediaSelf, mediaOther),
                (Original selfOriginal, Original otherOriginal) => selfOriginal.Equals(otherOriginal),
                (Part selfPart, Part otherPart) => selfPart.Equals(otherPart),
                (Port selfPort, Port otherPort) => selfPort.Equals(otherPort),
                (RamOption selfRamOption, RamOption otherRamOption) => selfRamOption.Equals(otherRamOption),
                (Release selfRelease, Release otherRelease) => selfRelease.Equals(otherRelease),
                (ReleaseDetails selfReleaseDetails, ReleaseDetails otherReleaseDetails) => selfReleaseDetails.Equals(otherReleaseDetails),
                (Rom romSelf, Rom romOther) => EqualsImpl(romSelf, romOther),
                (Serials selfSerials, Serials otherSerials) => selfSerials.Equals(otherSerials),
                (SharedFeat selfSharedFeat, SharedFeat otherSharedFeat) => selfSharedFeat.Equals(otherSharedFeat),
                (Slot selfSlot, Slot otherSlot) => selfSlot.Equals(otherSlot),
                (SlotOption selfSlotOption, SlotOption otherSlotOption) => selfSlotOption.Equals(otherSlotOption),
                (SoftwareList selfSoftwareList, SoftwareList otherSoftwareList) => selfSoftwareList.Equals(otherSoftwareList),
                (Sound selfSound, Sound otherSound) => selfSound.Equals(otherSound),
                (SourceDetails selfSourceDetails, SourceDetails otherSourceDetails) => selfSourceDetails.Equals(otherSourceDetails),
                (Video videoSelf, Video videoOther) => EqualsImpl(videoSelf, videoOther),
                _ => self.EqualsImpl(other),
            };
#else
            if (self is Adjuster selfAdjuster && other is Adjuster otherAdjuster)
                return selfAdjuster.Equals(otherAdjuster);
            else if (self is Analog selfAnalog && other is Analog otherAnalog)
                return selfAnalog.Equals(otherAnalog);
            else if (self is Archive selfArchive && other is Archive otherArchive)
                return selfArchive.Equals(otherArchive);
            else if (self is BiosSet selfBiosSet && other is BiosSet otherBiosSet)
                return selfBiosSet.Equals(otherBiosSet);
            else if (self is Chip selfChip && other is Chip otherChip)
                return selfChip.Equals(otherChip);
            else if (self is Condition selfCondition && other is Condition otherCondition)
                return selfCondition.Equals(otherCondition);
            else if (self is Configuration selfConfiguration && other is Configuration otherConfiguration)
                return selfConfiguration.Equals(otherConfiguration);
            else if (self is ConfLocation selfConfLocation && other is ConfLocation otherConfLocation)
                return selfConfLocation.Equals(otherConfLocation);
            else if (self is ConfSetting selfConfSetting && other is ConfSetting otherConfSetting)
                return selfConfSetting.Equals(otherConfSetting);
            else if (self is Control selfControl && other is Control otherControl)
                return selfControl.Equals(otherControl);
            else if (self is DataArea selfDataArea && other is DataArea otherDataArea)
                return selfDataArea.Equals(otherDataArea);
            else if (self is Device selfDevice && other is Device otherDevice)
                return selfDevice.Equals(otherDevice);
            else if (self is DipLocation selfDipLocation && other is DipLocation otherDipLocation)
                return selfDipLocation.Equals(otherDipLocation);
            else if (self is DipSwitch selfDipSwitch && other is DipSwitch otherDipSwitch)
                return selfDipSwitch.Equals(otherDipSwitch);
            else if (self is DipValue selfDipValue && other is DipValue otherDipValue)
                return selfDipValue.Equals(otherDipValue);
            else if (self is Disk diskSelf && other is Disk diskOther)
                return EqualsImpl(diskSelf, diskOther);
            else if (self is DiskArea selfDiskArea && other is DiskArea otherDiskArea)
                return selfDiskArea.Equals(otherDiskArea);
            else if (self is Display selfDisplay && other is Display otherDisplay)
                return selfDisplay.Equals(otherDisplay);
            else if (self is Driver selfDriver && other is Driver otherDriver)
                return selfDriver.Equals(otherDriver);
            else if (self is Dump selfDump && other is Dump otherDump)
                return selfDump.Equals(otherDump);
            else if (self is Feature selfFeature && other is Feature otherFeature)
                return selfFeature.Equals(otherFeature);
            else if (self is Info selfInfo && other is Info otherInfo)
                return selfInfo.Equals(otherInfo);
            else if (self is Input selfInput && other is Input otherInput)
                return selfInput.Equals(otherInput);
            else if (self is Instance selfInstance && other is Instance otherInstance)
                return selfInstance.Equals(otherInstance);
            else if (self is Media mediaSelf && other is Media mediaOther)
                return EqualsImpl(mediaSelf, mediaOther);
            else if (self is Original selfOriginal && other is Original otherOriginal)
                return selfOriginal.Equals(otherOriginal);
            else if (self is Part selfPart && other is Part otherPart)
                return selfPart.Equals(otherPart);
            else if (self is Port selfPort && other is Port otherPort)
                return selfPort.Equals(otherPort);
            else if (self is RamOption selfRamOption && other is RamOption otherRamOption)
                return selfRamOption.Equals(otherRamOption);
            else if (self is Release selfRelease && other is Release otherRelease)
                return selfRelease.Equals(otherRelease);
            else if (self is ReleaseDetails selfReleaseDetails && other is ReleaseDetails otherReleaseDetails)
                return selfReleaseDetails.Equals(otherReleaseDetails);
            else if (self is Rom romSelf && other is Rom romOther)
                return EqualsImpl(romSelf, romOther);
            else if (self is Serials selfSerials && other is Serials otherSerials)
                return selfSerials.Equals(otherSerials);
            else if (self is SharedFeat selfSharedFeat && other is SharedFeat otherSharedFeat)
                return selfSharedFeat.Equals(otherSharedFeat);
            else if (self is Slot selfSlot && other is Slot otherSlot)
                return selfSlot.Equals(otherSlot);
            else if (self is SlotOption selfSlotOption && other is SlotOption otherSlotOption)
                return selfSlotOption.Equals(otherSlotOption);
            else if (self is SoftwareList selfSoftwareList && other is SoftwareList otherSoftwareList)
                return selfSoftwareList.Equals(otherSoftwareList);
            else if (self is Sound selfSound && other is Sound otherSound)
                return selfSound.Equals(otherSound);
            else if (self is SourceDetails selfSourceDetails && other is SourceDetails otherSourceDetails)
                return selfSourceDetails.Equals(otherSourceDetails);
            else if (self is Video selfVideo && other is Video otherVideo)
                return selfVideo.Equals(otherVideo);
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

            // Handle individual type properties
            if (self is Header selfHeader && other is Header otherHeader)
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
            else if (self is Machine selfMachine && other is Machine otherMachine)
            {
                if (selfMachine.Board != otherMachine.Board)
                    return false;
                if (selfMachine.Buttons != otherMachine.Buttons)
                    return false;
                if (selfMachine.CloneOf != otherMachine.CloneOf)
                    return false;
                if (selfMachine.CloneOfId != otherMachine.CloneOfId)
                    return false;
                if (selfMachine.Company != otherMachine.Company)
                    return false;
                if (selfMachine.Control != otherMachine.Control)
                    return false;
                if (selfMachine.Country != otherMachine.Country)
                    return false;
                if (selfMachine.Description != otherMachine.Description)
                    return false;
                if (selfMachine.DirName != otherMachine.DirName)
                    return false;
                if (selfMachine.DisplayCount != otherMachine.DisplayCount)
                    return false;
                if (selfMachine.DisplayType != otherMachine.DisplayType)
                    return false;
                if (selfMachine.DuplicateID != otherMachine.DuplicateID)
                    return false;
                if (selfMachine.Emulator != otherMachine.Emulator)
                    return false;
                if (selfMachine.Extra != otherMachine.Extra)
                    return false;
                if (selfMachine.Favorite != otherMachine.Favorite)
                    return false;
                if (selfMachine.GenMSXID != otherMachine.GenMSXID)
                    return false;
                if (selfMachine.Hash != otherMachine.Hash)
                    return false;
                if (selfMachine.History != otherMachine.History)
                    return false;
                if (selfMachine.Id != otherMachine.Id)
                    return false;
                if (selfMachine.Im1CRC != otherMachine.Im1CRC)
                    return false;
                if (selfMachine.Im2CRC != otherMachine.Im2CRC)
                    return false;
                if (selfMachine.ImageNumber != otherMachine.ImageNumber)
                    return false;
                if (selfMachine.IsBios != otherMachine.IsBios)
                    return false;
                if (selfMachine.IsDevice != otherMachine.IsDevice)
                    return false;
                if (selfMachine.IsMechanical != otherMachine.IsMechanical)
                    return false;
                if (selfMachine.Language != otherMachine.Language)
                    return false;
                if (selfMachine.Location != otherMachine.Location)
                    return false;
                if (selfMachine.Manufacturer != otherMachine.Manufacturer)
                    return false;
                if (selfMachine.Notes != otherMachine.Notes)
                    return false;
                if (selfMachine.PlayedCount != otherMachine.PlayedCount)
                    return false;
                if (selfMachine.PlayedTime != otherMachine.PlayedTime)
                    return false;
                if (selfMachine.Players != otherMachine.Players)
                    return false;
                if (selfMachine.Publisher != otherMachine.Publisher)
                    return false;
                if (selfMachine.RebuildTo != otherMachine.RebuildTo)
                    return false;
                if (selfMachine.ReleaseNumber != otherMachine.ReleaseNumber)
                    return false;
                if (selfMachine.RomOf != otherMachine.RomOf)
                    return false;
                if (selfMachine.Rotation != otherMachine.Rotation)
                    return false;
                if (selfMachine.Runnable != otherMachine.Runnable)
                    return false;
                if (selfMachine.SampleOf != otherMachine.SampleOf)
                    return false;
                if (selfMachine.SaveType != otherMachine.SaveType)
                    return false;
                if (selfMachine.SourceFile != otherMachine.SourceFile)
                    return false;
                if (selfMachine.SourceRom != otherMachine.SourceRom)
                    return false;
                if (selfMachine.Status != otherMachine.Status)
                    return false;
                if (selfMachine.Supported != otherMachine.Supported)
                    return false;
                if (selfMachine.System != otherMachine.System)
                    return false;
                if (selfMachine.Tags != otherMachine.Tags)
                    return false;
                if (selfMachine.Url != otherMachine.Url)
                    return false;
                if (selfMachine.Year != otherMachine.Year)
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
