using SabreTools.Data.Attributes;

namespace SabreTools.Data.Models.Metadata
{
    /// <summary>
    /// Format-agnostic representation of game, machine, and set data
    /// </summary>
    public class Machine : DictionaryBase
    {
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

        /// <remarks>string</remarks>
        public const string BoardKey = "board";

        /// <remarks>string</remarks>
        public const string ButtonsKey = "buttons";

        /// <remarks>string, string[]</remarks>
        public const string CategoryKey = "category";

        /// <remarks>Chip[]</remarks>
        [NoFilter]
        public const string ChipKey = "chip";

        /// <remarks>string</remarks>
        public const string CloneOfKey = "cloneof";

        /// <remarks>string</remarks>
        public const string CloneOfIdKey = "cloneofid";

        /// <remarks>string, string[]</remarks>
        public const string CommentKey = "comment";

        /// <remarks>string</remarks>
        public const string CompanyKey = "company";

        /// <remarks>Configuration[]</remarks>
        [NoFilter]
        public const string ConfigurationKey = "configuration";

        /// <remarks>string</remarks>
        public const string ControlKey = "control";

        /// <remarks>string</remarks>
        public const string CountryKey = "country";

        /// <remarks>string</remarks>
        public const string DescriptionKey = "description";

        /// <remarks>Device[]</remarks>
        [NoFilter]
        public const string DeviceKey = "device";

        /// <remarks>DeviceRef[]</remarks>
        [NoFilter]
        public const string DeviceRefKey = "device_ref";

        /// <remarks>DipSwitch[]</remarks>
        [NoFilter]
        public const string DipSwitchKey = "dipswitch";

        /// <remarks>string</remarks>
        public const string DirNameKey = "dirName";

        /// <remarks>Disk[]</remarks>
        [NoFilter]
        public const string DiskKey = "disk";

        /// <remarks>string</remarks>
        public const string DisplayCountKey = "displaycount";

        /// <remarks>Display[]</remarks>
        [NoFilter]
        public const string DisplayKey = "display";

        /// <remarks>string</remarks>
        public const string DisplayTypeKey = "displaytype";

        /// <remarks>Driver</remarks>
        [NoFilter]
        public const string DriverKey = "driver";

        /// <remarks>Dump[]</remarks>
        [NoFilter]
        public const string DumpKey = "dump";

        /// <remarks>string</remarks>
        public const string DuplicateIDKey = "duplicateID";

        /// <remarks>string</remarks>
        public const string EmulatorKey = "emulator";

        /// <remarks>string</remarks>
        public const string ExtraKey = "extra";

        /// <remarks>string</remarks>
        public const string FavoriteKey = "favorite";

        /// <remarks>Feature[]</remarks>
        [NoFilter]
        public const string FeatureKey = "feature";

        /// <remarks>string</remarks>
        public const string GenMSXIDKey = "genmsxid";

        /// <remarks>string</remarks>
        public const string HashKey = "hash";

        /// <remarks>string</remarks>
        public const string HistoryKey = "history";

        /// <remarks>string</remarks>
        public const string IdKey = "id";

        /// <remarks>string</remarks>
        public const string Im1CRCKey = "im1CRC";

        /// <remarks>string</remarks>
        public const string Im2CRCKey = "im2CRC";

        /// <remarks>string</remarks>
        public const string ImageNumberKey = "imageNumber";

        /// <remarks>Info[]</remarks>
        [NoFilter]
        public const string InfoKey = "info";

        /// <remarks>Input</remarks>
        [NoFilter]
        public const string InputKey = "input";

        /// <remarks>(yes|no) "no"</remarks>
        public const string IsBiosKey = "isbios";

        /// <remarks>(yes|no) "no"</remarks>
        public const string IsDeviceKey = "isdevice";

        /// <remarks>(yes|no) "no"</remarks>
        public const string IsMechanicalKey = "ismechanical";

        /// <remarks>string</remarks>
        public const string LanguageKey = "language";

        /// <remarks>string</remarks>
        public const string LocationKey = "location";

        /// <remarks>string</remarks>
        public const string ManufacturerKey = "manufacturer";

        /// <remarks>Media[]</remarks>
        [NoFilter]
        public const string MediaKey = "media";

        /// <remarks>string</remarks>
        public const string NameKey = "name";

        /// <remarks>string</remarks>
        public const string NotesKey = "notes";

        /// <remarks>Part[]</remarks>
        [NoFilter]
        public const string PartKey = "part";

        /// <remarks>string</remarks>
        public const string PlayedCountKey = "playedcount";

        /// <remarks>string</remarks>
        public const string PlayedTimeKey = "playedtime";

        /// <remarks>string</remarks>
        public const string PlayersKey = "players";

        /// <remarks>Port[]</remarks>
        [NoFilter]
        public const string PortKey = "port";

        /// <remarks>string</remarks>
        public const string PublisherKey = "publisher";

        /// <remarks>RamOption[]</remarks>
        [NoFilter]
        public const string RamOptionKey = "ramoption";

        /// <remarks>string</remarks>
        public const string RebuildToKey = "rebuildto";

        /// <remarks>Release[]</remarks>
        [NoFilter]
        public const string ReleaseKey = "release";

        /// <remarks>string</remarks>
        public const string ReleaseNumberKey = "releaseNumber";

        /// <remarks>Rom[]</remarks>
        [NoFilter]
        public const string RomKey = "rom";

        /// <remarks>string</remarks>
        public const string RomOfKey = "romof";

        /// <remarks>string</remarks>
        public const string RotationKey = "rotation";

        /// <remarks>(yes|no) "no"</remarks>
        public const string RunnableKey = "runnable";

        /// <remarks>Sample[]</remarks>
        [NoFilter]
        public const string SampleKey = "sample";

        /// <remarks>string</remarks>
        public const string SampleOfKey = "sampleof";

        /// <remarks>string</remarks>
        public const string SaveTypeKey = "saveType";

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

        /// <remarks>string</remarks>
        public const string SourceFileKey = "sourcefile";

        /// <remarks>string</remarks>
        public const string SourceRomKey = "sourceRom";

        /// <remarks>string</remarks>
        public const string StatusKey = "status";

        /// <remarks>(yes|partial|no) "yes"</remarks>
        public const string SupportedKey = "supported";

        /// <remarks>string</remarks>
        public const string SystemKey = "system";

        /// <remarks>string</remarks>
        public const string TagsKey = "tags";

        /// TODO: This needs an internal model OR mapping to fields
        /// <remarks>Trurip</remarks>
        [NoFilter]
        public const string TruripKey = "trurip";

        /// <remarks>string</remarks>
        public const string UrlKey = "url";

        /// <remarks>Video[]</remarks>
        [NoFilter]
        public const string VideoKey = "video";

        /// <remarks>string</remarks>
        public const string YearKey = "year";

        #endregion
    }
}