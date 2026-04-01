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
        public static Data.Models.Metadata.ItemType AsItemType(this string? value)
        {
            return value?.ToLowerInvariant() switch
            {
                // "Actionable" item types
                "rom" => Data.Models.Metadata.ItemType.Rom,
                "disk" => Data.Models.Metadata.ItemType.Disk,
                "file" => Data.Models.Metadata.ItemType.File,
                "media" => Data.Models.Metadata.ItemType.Media,

                // "Auxiliary" item types
                "adjuster" => Data.Models.Metadata.ItemType.Adjuster,
                "analog" => Data.Models.Metadata.ItemType.Analog,
                "archive" => Data.Models.Metadata.ItemType.Archive,
                "biosset" => Data.Models.Metadata.ItemType.BiosSet,
                "chip" => Data.Models.Metadata.ItemType.Chip,
                "condition" => Data.Models.Metadata.ItemType.Condition,
                "configuration" => Data.Models.Metadata.ItemType.Configuration,
                "conflocation" => Data.Models.Metadata.ItemType.ConfLocation,
                "confsetting" => Data.Models.Metadata.ItemType.ConfSetting,
                "control" => Data.Models.Metadata.ItemType.Control,
                "dataarea" => Data.Models.Metadata.ItemType.DataArea,
                "device" => Data.Models.Metadata.ItemType.Device,
                "device_ref" or "deviceref" => Data.Models.Metadata.ItemType.DeviceRef,
                "diplocation" => Data.Models.Metadata.ItemType.DipLocation,
                "dipswitch" => Data.Models.Metadata.ItemType.DipSwitch,
                "dipvalue" => Data.Models.Metadata.ItemType.DipValue,
                "diskarea" => Data.Models.Metadata.ItemType.DiskArea,
                "display" => Data.Models.Metadata.ItemType.Display,
                "driver" => Data.Models.Metadata.ItemType.Driver,
                "dump" => Data.Models.Metadata.ItemType.Dump,
                "extension" => Data.Models.Metadata.ItemType.Extension,
                "feature" => Data.Models.Metadata.ItemType.Feature,
                "info" => Data.Models.Metadata.ItemType.Info,
                "input" => Data.Models.Metadata.ItemType.Input,
                "instance" => Data.Models.Metadata.ItemType.Instance,
                "original" => Data.Models.Metadata.ItemType.Original,
                "part" => Data.Models.Metadata.ItemType.Part,
                "part_feature" or "partfeature" => Data.Models.Metadata.ItemType.PartFeature,
                "port" => Data.Models.Metadata.ItemType.Port,
                "ramoption" or "ram_option" => Data.Models.Metadata.ItemType.RamOption,
                "release" => Data.Models.Metadata.ItemType.Release,
                "release_details" or "releasedetails" => Data.Models.Metadata.ItemType.ReleaseDetails,
                "sample" => Data.Models.Metadata.ItemType.Sample,
                "serials" => Data.Models.Metadata.ItemType.Serials,
                "sharedfeat" or "shared_feat" or "sharedfeature" or "shared_feature" => Data.Models.Metadata.ItemType.SharedFeat,
                "slot" => Data.Models.Metadata.ItemType.Slot,
                "slotoption" or "slot_option" => Data.Models.Metadata.ItemType.SlotOption,
                "software" => Data.Models.Metadata.ItemType.Software,
                "softwarelist" or "software_list" => Data.Models.Metadata.ItemType.SoftwareList,
                "sound" => Data.Models.Metadata.ItemType.Sound,
                "source_details" or "sourcedetails" => Data.Models.Metadata.ItemType.SourceDetails,
                "video" => Data.Models.Metadata.ItemType.Video,
                "blank" => Data.Models.Metadata.ItemType.Blank,
                _ => Data.Models.Metadata.ItemType.NULL,
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
        public static string? AsStringValue(this Data.Models.Metadata.ItemType value)
        {
            return value switch
            {
                // "Actionable" item types
                Data.Models.Metadata.ItemType.Rom => "rom",
                Data.Models.Metadata.ItemType.Disk => "disk",
                Data.Models.Metadata.ItemType.File => "file",
                Data.Models.Metadata.ItemType.Media => "media",

                // "Auxiliary" item types
                Data.Models.Metadata.ItemType.Adjuster => "adjuster",
                Data.Models.Metadata.ItemType.Analog => "analog",
                Data.Models.Metadata.ItemType.Archive => "archive",
                Data.Models.Metadata.ItemType.BiosSet => "biosset",
                Data.Models.Metadata.ItemType.Chip => "chip",
                Data.Models.Metadata.ItemType.Condition => "condition",
                Data.Models.Metadata.ItemType.Configuration => "configuration",
                Data.Models.Metadata.ItemType.ConfLocation => "conflocation",
                Data.Models.Metadata.ItemType.ConfSetting => "confsetting",
                Data.Models.Metadata.ItemType.Control => "control",
                Data.Models.Metadata.ItemType.DataArea => "dataarea",
                Data.Models.Metadata.ItemType.Device => "device",
                Data.Models.Metadata.ItemType.DeviceRef => "device_ref",
                Data.Models.Metadata.ItemType.DipLocation => "diplocation",
                Data.Models.Metadata.ItemType.DipSwitch => "dipswitch",
                Data.Models.Metadata.ItemType.DipValue => "dipvalue",
                Data.Models.Metadata.ItemType.DiskArea => "diskarea",
                Data.Models.Metadata.ItemType.Display => "display",
                Data.Models.Metadata.ItemType.Driver => "driver",
                Data.Models.Metadata.ItemType.Dump => "dump",
                Data.Models.Metadata.ItemType.Extension => "extension",
                Data.Models.Metadata.ItemType.Feature => "feature",
                Data.Models.Metadata.ItemType.Info => "info",
                Data.Models.Metadata.ItemType.Input => "input",
                Data.Models.Metadata.ItemType.Instance => "instance",
                Data.Models.Metadata.ItemType.Original => "original",
                Data.Models.Metadata.ItemType.Part => "part",
                Data.Models.Metadata.ItemType.PartFeature => "part_feature",
                Data.Models.Metadata.ItemType.Port => "port",
                Data.Models.Metadata.ItemType.RamOption => "ramoption",
                Data.Models.Metadata.ItemType.Release => "release",
                Data.Models.Metadata.ItemType.ReleaseDetails => "release_details",
                Data.Models.Metadata.ItemType.Sample => "sample",
                Data.Models.Metadata.ItemType.Serials => "serials",
                Data.Models.Metadata.ItemType.SharedFeat => "sharedfeat",
                Data.Models.Metadata.ItemType.Slot => "slot",
                Data.Models.Metadata.ItemType.SlotOption => "slotoption",
                Data.Models.Metadata.ItemType.Software => "software",
                Data.Models.Metadata.ItemType.SoftwareList => "softwarelist",
                Data.Models.Metadata.ItemType.Sound => "sound",
                Data.Models.Metadata.ItemType.SourceDetails => "source_details",
                Data.Models.Metadata.ItemType.Video => "video",
                Data.Models.Metadata.ItemType.Blank => "blank",

                Data.Models.Metadata.ItemType.NULL => null,
                _ => null,
            };
        }

        #endregion
    }
}
