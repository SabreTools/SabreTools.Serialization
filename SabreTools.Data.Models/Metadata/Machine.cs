
namespace SabreTools.Data.Models.Metadata
{
    /// <summary>
    /// Format-agnostic representation of game, machine, and set data
    /// </summary>
    public class Machine : DictionaryBase
    {
        #region Properties

        public string? Board { get; set; }

        public string? Buttons { get; set; }

        public string? CloneOf { get; set; }

        public string? CloneOfId { get; set; }

        public string? Company { get; set; }

        public string? Control { get; set; }

        public string? Country { get; set; }

        public string? Description { get; set; }

        public string? DirName { get; set; }

        public string? DisplayCount { get; set; }

        public string? DisplayType { get; set; }

        public string? DuplicateID { get; set; }

        public string? Emulator { get; set; }

        public string? Extra { get; set; }

        public string? Favorite { get; set; }

        public string? GenMSXID { get; set; }

        public string? Hash { get; set; }

        public string? History { get; set; }

        public string? Id { get; set; }

        public string? Im1CRC { get; set; }

        public string? Im2CRC { get; set; }

        public string? ImageNumber { get; set; }

        /// <remarks>(yes|no) "no"</remarks>
        public bool? IsBios { get; set; }

        /// <remarks>(yes|no) "no"</remarks>
        public bool? IsDevice { get; set; }

        /// <remarks>(yes|no) "no"</remarks>
        public bool? IsMechanical { get; set; }

        public string? Language { get; set; }

        public string? Location { get; set; }

        public string? Manufacturer { get; set; }

        public string? Name { get; set; }

        public string? Notes { get; set; }

        public string? PlayedCount { get; set; }

        public string? PlayedTime { get; set; }

        public string? Players { get; set; }

        public string? Publisher { get; set; }

        public string? RebuildTo { get; set; }

        public string? ReleaseNumber { get; set; }

        public string? RomOf { get; set; }

        public string? Rotation { get; set; }

        /// <remarks>(yes|partial|no) "no"</remarks>
        public Runnable? Runnable { get; set; }

        public string? SampleOf { get; set; }

        public string? SaveType { get; set; }

        public string? SourceFile { get; set; }

        public string? SourceRom { get; set; }

        public string? Status { get; set; }

        /// <remarks>(yes|partial|no) "yes"</remarks>
        public Supported? Supported { get; set; }

        public string? System { get; set; }

        public string? Tags { get; set; }

        public string? Url { get; set; }

        public string? Year { get; set; }

        #endregion

        #region Keys

        /// <remarks>Adjuster[]</remarks>
        [NoFilter]
        public const string AdjusterKey = "adjuster";

        /// <remarks>Archive[]</remarks>
        [NoFilter]
        public const string ArchiveKey = "archive";

        /// <remarks>BiosSet[]</remarks>
        [NoFilter]
        public const string BiosSetKey = "biosset";

        /// <remarks>string, string[]</remarks>
        public const string CategoryKey = "category";

        /// <remarks>Chip[]</remarks>
        [NoFilter]
        public const string ChipKey = "chip";

        /// <remarks>string, string[]</remarks>
        public const string CommentKey = "comment";

        /// <remarks>Configuration[]</remarks>
        [NoFilter]
        public const string ConfigurationKey = "configuration";

        /// <remarks>Device[]</remarks>
        [NoFilter]
        public const string DeviceKey = "device";

        /// <remarks>DeviceRef[]</remarks>
        [NoFilter]
        public const string DeviceRefKey = "device_ref";

        /// <remarks>DipSwitch[]</remarks>
        [NoFilter]
        public const string DipSwitchKey = "dipswitch";

        /// <remarks>Disk[]</remarks>
        [NoFilter]
        public const string DiskKey = "disk";

        /// <remarks>Display[]</remarks>
        [NoFilter]
        public const string DisplayKey = "display";

        /// <remarks>Driver</remarks>
        [NoFilter]
        public const string DriverKey = "driver";

        /// <remarks>Dump[]</remarks>
        [NoFilter]
        public const string DumpKey = "dump";

        /// <remarks>Feature[]</remarks>
        [NoFilter]
        public const string FeatureKey = "feature";

        /// <remarks>Info[]</remarks>
        [NoFilter]
        public const string InfoKey = "info";

        /// <remarks>Input</remarks>
        [NoFilter]
        public const string InputKey = "input";

        /// <remarks>Media[]</remarks>
        [NoFilter]
        public const string MediaKey = "media";

        /// <remarks>Part[]</remarks>
        [NoFilter]
        public const string PartKey = "part";

        /// <remarks>Port[]</remarks>
        [NoFilter]
        public const string PortKey = "port";

        /// <remarks>RamOption[]</remarks>
        [NoFilter]
        public const string RamOptionKey = "ramoption";

        /// <remarks>Release[]</remarks>
        [NoFilter]
        public const string ReleaseKey = "release";

        /// <remarks>Rom[]</remarks>
        [NoFilter]
        public const string RomKey = "rom";

        /// <remarks>Sample[]</remarks>
        [NoFilter]
        public const string SampleKey = "sample";

        /// <remarks>SharedFeat[]</remarks>
        [NoFilter]
        public const string SharedFeatKey = "sharedfeat";

        /// <remarks>Slot[]</remarks>
        [NoFilter]
        public const string SlotKey = "slot";

        /// <remarks>SoftwareList[]</remarks>
        [NoFilter]
        public const string SoftwareListKey = "softwarelist";

        /// <remarks>Sound</remarks>
        [NoFilter]
        public const string SoundKey = "sound";

        /// TODO: This needs an internal model OR mapping to fields
        /// <remarks>Trurip</remarks>
        [NoFilter]
        public const string TruripKey = "trurip";

        /// <remarks>Video[]</remarks>
        [NoFilter]
        public const string VideoKey = "video";

        #endregion
    }
}
