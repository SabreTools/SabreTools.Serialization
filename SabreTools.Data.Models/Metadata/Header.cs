
namespace SabreTools.Data.Models.Metadata
{
    /// <summary>
    /// Format-agnostic representation of metadata header data
    /// </summary>
    public class Header : DictionaryBase
    {
        #region Properties

        /// <remarks>(none|split|merged|nonmerged|fullmerged|device|full) "split"</remarks>
        public MergingFlag BiosMode { get; set; }

        /// <remarks>(yes|no) "no"</remarks>
        public bool? Debug { get; set; }

        /// <remarks>(none|split|merged|nonmerged|fullmerged|device|full) "split"</remarks>
        public MergingFlag ForceMerging { get; set; }

        /// <remarks>(obsolete|required|ignore) "obsolete"</remarks>
        public NodumpFlag ForceNodump { get; set; }

        /// <remarks>(zip|unzip) "zip"</remarks>
        public PackingFlag ForcePacking { get; set; }

        /// <remarks>(yes|no) "yes"</remarks>
        public bool? ForceZipping { get; set; }

        /// <remarks>(yes|no) "no"</remarks>
        public bool? LockBiosMode { get; set; }

        /// <remarks>(yes|no) "no"</remarks>
        public bool? LockRomMode { get; set; }

        /// <remarks>(yes|no) "no"</remarks>
        public bool? LockSampleMode { get; set; }

        public string? Name { get; set; }

        /// <remarks>(none|split|merged|nonmerged|fullmerged|device|full) "split"</remarks>
        public MergingFlag RomMode { get; set; }

        /// <remarks>(none|split|merged|nonmerged|fullmerged|device|full) "split"</remarks>
        public MergingFlag SampleMode { get; set; }

        #endregion

        #region Keys

        /// <remarks>string</remarks>
        public const string AuthorKey = "author";

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

        /// <remarks>string</remarks>
        public const string DescriptionKey = "description";

        /// <remarks>string</remarks>
        public const string EmailKey = "email";

        /// <remarks>string</remarks>
        public const string EmulatorVersionKey = "emulatorversion";

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

        /// <remarks>string</remarks>
        public const string MameConfigKey = "mameconfig";

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

        /// <remarks>string</remarks>
        public const string RomTitleKey = "romTitle";

        /// <remarks>string</remarks>
        public const string RootDirKey = "rootdir";

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
