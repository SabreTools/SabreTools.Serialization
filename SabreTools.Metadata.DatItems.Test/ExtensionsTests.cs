using Xunit;

namespace SabreTools.Metadata.DatItems.Test
{
    public class ExtensionsTests
    {
        #region String to Enum

        [Theory]
        [InlineData(null, Data.Models.Metadata.ItemType.NULL)]
        [InlineData("adjuster", Data.Models.Metadata.ItemType.Adjuster)]
        [InlineData("analog", Data.Models.Metadata.ItemType.Analog)]
        [InlineData("archive", Data.Models.Metadata.ItemType.Archive)]
        [InlineData("biosset", Data.Models.Metadata.ItemType.BiosSet)]
        [InlineData("blank", Data.Models.Metadata.ItemType.Blank)]
        [InlineData("chip", Data.Models.Metadata.ItemType.Chip)]
        [InlineData("condition", Data.Models.Metadata.ItemType.Condition)]
        [InlineData("configuration", Data.Models.Metadata.ItemType.Configuration)]
        [InlineData("conflocation", Data.Models.Metadata.ItemType.ConfLocation)]
        [InlineData("confsetting", Data.Models.Metadata.ItemType.ConfSetting)]
        [InlineData("control", Data.Models.Metadata.ItemType.Control)]
        [InlineData("dataarea", Data.Models.Metadata.ItemType.DataArea)]
        [InlineData("device", Data.Models.Metadata.ItemType.Device)]
        [InlineData("deviceref", Data.Models.Metadata.ItemType.DeviceRef)]
        [InlineData("device_ref", Data.Models.Metadata.ItemType.DeviceRef)]
        [InlineData("diplocation", Data.Models.Metadata.ItemType.DipLocation)]
        [InlineData("dipswitch", Data.Models.Metadata.ItemType.DipSwitch)]
        [InlineData("dipvalue", Data.Models.Metadata.ItemType.DipValue)]
        [InlineData("disk", Data.Models.Metadata.ItemType.Disk)]
        [InlineData("diskarea", Data.Models.Metadata.ItemType.DiskArea)]
        [InlineData("display", Data.Models.Metadata.ItemType.Display)]
        [InlineData("driver", Data.Models.Metadata.ItemType.Driver)]
        [InlineData("extension", Data.Models.Metadata.ItemType.Extension)]
        [InlineData("feature", Data.Models.Metadata.ItemType.Feature)]
        [InlineData("file", Data.Models.Metadata.ItemType.File)]
        [InlineData("info", Data.Models.Metadata.ItemType.Info)]
        [InlineData("input", Data.Models.Metadata.ItemType.Input)]
        [InlineData("instance", Data.Models.Metadata.ItemType.Instance)]
        [InlineData("media", Data.Models.Metadata.ItemType.Media)]
        [InlineData("part", Data.Models.Metadata.ItemType.Part)]
        [InlineData("partfeature", Data.Models.Metadata.ItemType.PartFeature)]
        [InlineData("part_feature", Data.Models.Metadata.ItemType.PartFeature)]
        [InlineData("port", Data.Models.Metadata.ItemType.Port)]
        [InlineData("ramoption", Data.Models.Metadata.ItemType.RamOption)]
        [InlineData("ram_option", Data.Models.Metadata.ItemType.RamOption)]
        [InlineData("release", Data.Models.Metadata.ItemType.Release)]
        [InlineData("releasedetails", Data.Models.Metadata.ItemType.ReleaseDetails)]
        [InlineData("release_details", Data.Models.Metadata.ItemType.ReleaseDetails)]
        [InlineData("rom", Data.Models.Metadata.ItemType.Rom)]
        [InlineData("sample", Data.Models.Metadata.ItemType.Sample)]
        [InlineData("serials", Data.Models.Metadata.ItemType.Serials)]
        [InlineData("sharedfeat", Data.Models.Metadata.ItemType.SharedFeat)]
        [InlineData("shared_feat", Data.Models.Metadata.ItemType.SharedFeat)]
        [InlineData("sharedfeature", Data.Models.Metadata.ItemType.SharedFeat)]
        [InlineData("shared_feature", Data.Models.Metadata.ItemType.SharedFeat)]
        [InlineData("slot", Data.Models.Metadata.ItemType.Slot)]
        [InlineData("slotoption", Data.Models.Metadata.ItemType.SlotOption)]
        [InlineData("slot_option", Data.Models.Metadata.ItemType.SlotOption)]
        [InlineData("softwarelist", Data.Models.Metadata.ItemType.SoftwareList)]
        [InlineData("software_list", Data.Models.Metadata.ItemType.SoftwareList)]
        [InlineData("sound", Data.Models.Metadata.ItemType.Sound)]
        [InlineData("sourcedetails", Data.Models.Metadata.ItemType.SourceDetails)]
        [InlineData("source_details", Data.Models.Metadata.ItemType.SourceDetails)]
        public void AsItemTypeTest(string? field, Data.Models.Metadata.ItemType expected)
        {
            Data.Models.Metadata.ItemType actual = field.AsItemType();
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
        [InlineData(Data.Models.Metadata.ItemType.NULL, null)]
        [InlineData(Data.Models.Metadata.ItemType.Adjuster, "adjuster")]
        [InlineData(Data.Models.Metadata.ItemType.Analog, "analog")]
        [InlineData(Data.Models.Metadata.ItemType.Archive, "archive")]
        [InlineData(Data.Models.Metadata.ItemType.BiosSet, "biosset")]
        [InlineData(Data.Models.Metadata.ItemType.Blank, "blank")]
        [InlineData(Data.Models.Metadata.ItemType.Chip, "chip")]
        [InlineData(Data.Models.Metadata.ItemType.Condition, "condition")]
        [InlineData(Data.Models.Metadata.ItemType.Configuration, "configuration")]
        [InlineData(Data.Models.Metadata.ItemType.ConfLocation, "conflocation")]
        [InlineData(Data.Models.Metadata.ItemType.ConfSetting, "confsetting")]
        [InlineData(Data.Models.Metadata.ItemType.Control, "control")]
        [InlineData(Data.Models.Metadata.ItemType.DataArea, "dataarea")]
        [InlineData(Data.Models.Metadata.ItemType.Device, "device")]
        [InlineData(Data.Models.Metadata.ItemType.DeviceRef, "device_ref")]
        [InlineData(Data.Models.Metadata.ItemType.DipLocation, "diplocation")]
        [InlineData(Data.Models.Metadata.ItemType.DipSwitch, "dipswitch")]
        [InlineData(Data.Models.Metadata.ItemType.DipValue, "dipvalue")]
        [InlineData(Data.Models.Metadata.ItemType.Disk, "disk")]
        [InlineData(Data.Models.Metadata.ItemType.DiskArea, "diskarea")]
        [InlineData(Data.Models.Metadata.ItemType.Display, "display")]
        [InlineData(Data.Models.Metadata.ItemType.Driver, "driver")]
        [InlineData(Data.Models.Metadata.ItemType.Extension, "extension")]
        [InlineData(Data.Models.Metadata.ItemType.Feature, "feature")]
        [InlineData(Data.Models.Metadata.ItemType.File, "file")]
        [InlineData(Data.Models.Metadata.ItemType.Info, "info")]
        [InlineData(Data.Models.Metadata.ItemType.Input, "input")]
        [InlineData(Data.Models.Metadata.ItemType.Instance, "instance")]
        [InlineData(Data.Models.Metadata.ItemType.Media, "media")]
        [InlineData(Data.Models.Metadata.ItemType.Part, "part")]
        [InlineData(Data.Models.Metadata.ItemType.PartFeature, "part_feature")]
        [InlineData(Data.Models.Metadata.ItemType.Port, "port")]
        [InlineData(Data.Models.Metadata.ItemType.RamOption, "ramoption")]
        [InlineData(Data.Models.Metadata.ItemType.Release, "release")]
        [InlineData(Data.Models.Metadata.ItemType.ReleaseDetails, "release_details")]
        [InlineData(Data.Models.Metadata.ItemType.Rom, "rom")]
        [InlineData(Data.Models.Metadata.ItemType.Sample, "sample")]
        [InlineData(Data.Models.Metadata.ItemType.Serials, "serials")]
        [InlineData(Data.Models.Metadata.ItemType.SharedFeat, "sharedfeat")]
        [InlineData(Data.Models.Metadata.ItemType.Slot, "slot")]
        [InlineData(Data.Models.Metadata.ItemType.SlotOption, "slotoption")]
        [InlineData(Data.Models.Metadata.ItemType.SoftwareList, "softwarelist")]
        [InlineData(Data.Models.Metadata.ItemType.Sound, "sound")]
        [InlineData(Data.Models.Metadata.ItemType.SourceDetails, "source_details")]
        public void FromItemTypeTest(Data.Models.Metadata.ItemType field, string? expected)
        {
            string? actual = field.AsStringValue();
            Assert.Equal(expected, actual);
        }

        #endregion
    }
}
