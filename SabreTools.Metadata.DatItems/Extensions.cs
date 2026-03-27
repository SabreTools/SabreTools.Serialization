namespace SabreTools.Metadata.DatItems
{
    public static class Extensions
    {
        #region String to Enum

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
                "condition" => ItemType.Condition,
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
                "extension" => ItemType.Extension,
                "feature" => ItemType.Feature,
                "info" => ItemType.Info,
                "input" => ItemType.Input,
                "instance" => ItemType.Instance,
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
                "blank" => ItemType.Blank,
                _ => ItemType.NULL,
            };
        }

        /// <summary>
        /// Get the enum value for an input string, if possible
        /// </summary>
        /// <param name="value">String value to parse/param>
        /// <returns>Enum value representing the input, default on error</returns>
        public static MachineType AsMachineType(this string? value)
        {
            return value?.ToLowerInvariant() switch
            {
                "none" => MachineType.None,
                "bios" => MachineType.Bios,
                "device" or "dev" => MachineType.Device,
                "mechanical" or "mech" => MachineType.Mechanical,
                _ => MachineType.None,
            };
        }

        #endregion

        #region Enum to String

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
                ItemType.Condition => "condition",
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
                ItemType.Extension => "extension",
                ItemType.Feature => "feature",
                ItemType.Info => "info",
                ItemType.Input => "input",
                ItemType.Instance => "instance",
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
                ItemType.Blank => "blank",

                ItemType.NULL => null,
                _ => null,
            };
        }

        #endregion
    }
}
