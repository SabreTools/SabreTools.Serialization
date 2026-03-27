using Xunit;

namespace SabreTools.Metadata.DatItems.Test
{
    public class ExtensionsTests
    {
        #region String to Enum

        [Theory]
        [InlineData(null, ItemType.NULL)]
        [InlineData("adjuster", ItemType.Adjuster)]
        [InlineData("analog", ItemType.Analog)]
        [InlineData("archive", ItemType.Archive)]
        [InlineData("biosset", ItemType.BiosSet)]
        [InlineData("blank", ItemType.Blank)]
        [InlineData("chip", ItemType.Chip)]
        [InlineData("condition", ItemType.Condition)]
        [InlineData("configuration", ItemType.Configuration)]
        [InlineData("conflocation", ItemType.ConfLocation)]
        [InlineData("confsetting", ItemType.ConfSetting)]
        [InlineData("control", ItemType.Control)]
        [InlineData("dataarea", ItemType.DataArea)]
        [InlineData("device", ItemType.Device)]
        [InlineData("deviceref", ItemType.DeviceRef)]
        [InlineData("device_ref", ItemType.DeviceRef)]
        [InlineData("diplocation", ItemType.DipLocation)]
        [InlineData("dipswitch", ItemType.DipSwitch)]
        [InlineData("dipvalue", ItemType.DipValue)]
        [InlineData("disk", ItemType.Disk)]
        [InlineData("diskarea", ItemType.DiskArea)]
        [InlineData("display", ItemType.Display)]
        [InlineData("driver", ItemType.Driver)]
        [InlineData("extension", ItemType.Extension)]
        [InlineData("feature", ItemType.Feature)]
        [InlineData("file", ItemType.File)]
        [InlineData("info", ItemType.Info)]
        [InlineData("input", ItemType.Input)]
        [InlineData("instance", ItemType.Instance)]
        [InlineData("media", ItemType.Media)]
        [InlineData("part", ItemType.Part)]
        [InlineData("partfeature", ItemType.PartFeature)]
        [InlineData("part_feature", ItemType.PartFeature)]
        [InlineData("port", ItemType.Port)]
        [InlineData("ramoption", ItemType.RamOption)]
        [InlineData("ram_option", ItemType.RamOption)]
        [InlineData("release", ItemType.Release)]
        [InlineData("releasedetails", ItemType.ReleaseDetails)]
        [InlineData("release_details", ItemType.ReleaseDetails)]
        [InlineData("rom", ItemType.Rom)]
        [InlineData("sample", ItemType.Sample)]
        [InlineData("serials", ItemType.Serials)]
        [InlineData("sharedfeat", ItemType.SharedFeat)]
        [InlineData("shared_feat", ItemType.SharedFeat)]
        [InlineData("sharedfeature", ItemType.SharedFeat)]
        [InlineData("shared_feature", ItemType.SharedFeat)]
        [InlineData("slot", ItemType.Slot)]
        [InlineData("slotoption", ItemType.SlotOption)]
        [InlineData("slot_option", ItemType.SlotOption)]
        [InlineData("softwarelist", ItemType.SoftwareList)]
        [InlineData("software_list", ItemType.SoftwareList)]
        [InlineData("sound", ItemType.Sound)]
        [InlineData("sourcedetails", ItemType.SourceDetails)]
        [InlineData("source_details", ItemType.SourceDetails)]
        public void AsItemTypeTest(string? field, ItemType expected)
        {
            ItemType actual = field.AsItemType();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(null, MachineType.None)]
        [InlineData("none", MachineType.None)]
        [InlineData("bios", MachineType.Bios)]
        [InlineData("dev", MachineType.Device)]
        [InlineData("device", MachineType.Device)]
        [InlineData("mech", MachineType.Mechanical)]
        [InlineData("mechanical", MachineType.Mechanical)]
        public void AsMachineTypeTest(string? field, MachineType expected)
        {
            MachineType actual = field.AsMachineType();
            Assert.Equal(expected, actual);
        }

        #endregion

        #region Enum to String

        [Theory]
        [InlineData(ItemType.NULL, null)]
        [InlineData(ItemType.Adjuster, "adjuster")]
        [InlineData(ItemType.Analog, "analog")]
        [InlineData(ItemType.Archive, "archive")]
        [InlineData(ItemType.BiosSet, "biosset")]
        [InlineData(ItemType.Blank, "blank")]
        [InlineData(ItemType.Chip, "chip")]
        [InlineData(ItemType.Condition, "condition")]
        [InlineData(ItemType.Configuration, "configuration")]
        [InlineData(ItemType.ConfLocation, "conflocation")]
        [InlineData(ItemType.ConfSetting, "confsetting")]
        [InlineData(ItemType.Control, "control")]
        [InlineData(ItemType.DataArea, "dataarea")]
        [InlineData(ItemType.Device, "device")]
        [InlineData(ItemType.DeviceRef, "device_ref")]
        [InlineData(ItemType.DipLocation, "diplocation")]
        [InlineData(ItemType.DipSwitch, "dipswitch")]
        [InlineData(ItemType.DipValue, "dipvalue")]
        [InlineData(ItemType.Disk, "disk")]
        [InlineData(ItemType.DiskArea, "diskarea")]
        [InlineData(ItemType.Display, "display")]
        [InlineData(ItemType.Driver, "driver")]
        [InlineData(ItemType.Extension, "extension")]
        [InlineData(ItemType.Feature, "feature")]
        [InlineData(ItemType.File, "file")]
        [InlineData(ItemType.Info, "info")]
        [InlineData(ItemType.Input, "input")]
        [InlineData(ItemType.Instance, "instance")]
        [InlineData(ItemType.Media, "media")]
        [InlineData(ItemType.Part, "part")]
        [InlineData(ItemType.PartFeature, "part_feature")]
        [InlineData(ItemType.Port, "port")]
        [InlineData(ItemType.RamOption, "ramoption")]
        [InlineData(ItemType.Release, "release")]
        [InlineData(ItemType.ReleaseDetails, "release_details")]
        [InlineData(ItemType.Rom, "rom")]
        [InlineData(ItemType.Sample, "sample")]
        [InlineData(ItemType.Serials, "serials")]
        [InlineData(ItemType.SharedFeat, "sharedfeat")]
        [InlineData(ItemType.Slot, "slot")]
        [InlineData(ItemType.SlotOption, "slotoption")]
        [InlineData(ItemType.SoftwareList, "softwarelist")]
        [InlineData(ItemType.Sound, "sound")]
        [InlineData(ItemType.SourceDetails, "source_details")]
        public void FromItemTypeTest(ItemType field, string? expected)
        {
            string? actual = field.AsStringValue();
            Assert.Equal(expected, actual);
        }

        #endregion
    }
}
