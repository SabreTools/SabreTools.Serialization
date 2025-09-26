using SabreTools.Data.Attributes;

namespace SabreTools.Serialization.Models.Metadata
{
    /// <summary>
    /// Format-agnostic representation of metadata header data
    /// </summary>
    public class Header : DictionaryBase
    {
        #region Keys

        /// <remarks>string</remarks>
        public const string AuthorKey = "author";

        /// <remarks>(none|split|merged|nonmerged|fullmerged|device|full) "split"</remarks>
        public const string BiosModeKey = "biosmode";

        /// <remarks>string</remarks>
        public const string BuildKey = "build";

        /// TODO: This needs an internal model OR mapping to fields
        /// <remarks>CanOpen</remarks>
        [NoFilter]
        public const string CanOpenKey = "canOpen";

        /// <remarks>string</remarks>
        public const string CategoryKey = "category";

        /// <remarks>string</remarks>
        public const string CommentKey = "comment";

        /// <remarks>string</remarks>
        public const string DateKey = "date";

        /// <remarks>string</remarks>
        public const string DatVersionKey = "datversion";

        /// <remarks>(yes|no) "no"</remarks>
        public const string DebugKey = "debug";

        /// <remarks>string</remarks>
        public const string DescriptionKey = "description";

        /// <remarks>string</remarks>
        public const string EmailKey = "email";

        /// <remarks>string</remarks>
        public const string EmulatorVersionKey = "emulatorversion";

        /// <remarks>(none|split|merged|nonmerged|fullmerged|device|full) "split"</remarks>
        public const string ForceMergingKey = "forcemerging";

        /// <remarks>(obsolete|required|ignore) "obsolete"</remarks>
        public const string ForceNodumpKey = "forcenodump";

        /// <remarks>(zip|unzip) "zip"</remarks>
        public const string ForcePackingKey = "forcepacking";

        /// <remarks>(yes|no) "yes"</remarks>
        public const string ForceZippingKey = "forcezipping";

        /// <remarks>string, string[]</remarks>
        public const string HeaderKey = "header";

        /// <remarks>string</remarks>
        public const string HomepageKey = "homepage";

        /// <remarks>string</remarks>
        public const string IdKey = "id";

        /// TODO: This needs an internal model OR mapping to fields
        /// <remarks>Search</remarks>
        [NoFilter]
        public const string ImagesKey = "images";

        /// <remarks>string</remarks>
        public const string ImFolderKey = "imFolder";

        /// TODO: This needs an internal model OR mapping to fields
        /// <remarks>Infos</remarks>
        [NoFilter]
        public const string InfosKey = "infos";

        /// <remarks>(yes|no) "no"</remarks>
        public const string LockBiosModeKey = "lockbiosmode";

        /// <remarks>(yes|no) "no"</remarks>
        public const string LockRomModeKey = "lockrommode";

        /// <remarks>(yes|no) "no"</remarks>
        public const string LockSampleModeKey = "locksamplemode";

        /// <remarks>string</remarks>
        public const string MameConfigKey = "mameconfig";

        /// <remarks>string</remarks>
        public const string NameKey = "name";

        /// TODO: This needs an internal model OR mapping to fields
        /// <remarks>NewDat</remarks>
        [NoFilter]
        public const string NewDatKey = "newDat";

        /// <remarks>string</remarks>
        public const string NotesKey = "notes";

        /// <remarks>string</remarks>
        public const string PluginKey = "plugin";

        /// <remarks>string</remarks>
        public const string RefNameKey = "refname";

        /// <remarks>(none|split|merged|nonmerged|fullmerged|device|full) "split"</remarks>
        public const string RomModeKey = "rommode";

        /// <remarks>string</remarks>
        public const string RomTitleKey = "romTitle";

        /// <remarks>string</remarks>
        public const string RootDirKey = "rootdir";

        /// <remarks>(none|split|merged|nonmerged|fullmerged|device|full) "split"</remarks>
        public const string SampleModeKey = "samplemode";

        /// <remarks>string</remarks>
        public const string SchemaLocationKey = "schemaLocation";

        /// <remarks>string</remarks>
        public const string ScreenshotsHeightKey = "screenshotsHeight";

        /// <remarks>string</remarks>
        public const string ScreenshotsWidthKey = "screenshotsWidth";

        /// TODO: This needs an internal model OR mapping to fields
        /// <remarks>Search</remarks>
        [NoFilter]
        public const string SearchKey = "search";

        /// <remarks>string</remarks>
        public const string SystemKey = "system";

        /// <remarks>string</remarks>
        public const string TimestampKey = "timestamp";

        /// <remarks>string</remarks>
        public const string TypeKey = "type";

        /// <remarks>string</remarks>
        public const string UrlKey = "url";

        /// <remarks>string</remarks>
        public const string VersionKey = "version";

        #endregion
    }
}