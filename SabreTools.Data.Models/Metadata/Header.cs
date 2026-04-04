
namespace SabreTools.Data.Models.Metadata
{
    /// <summary>
    /// Format-agnostic representation of metadata header data
    /// </summary>
    public class Header : DictionaryBase
    {
        #region Properties

        public string? Author { get; set; }

        /// <remarks>(none|split|merged|nonmerged|fullmerged|device|full) "split"</remarks>
        public MergingFlag BiosMode { get; set; }

        public string? Build { get; set; }

        /// TODO: This needs an internal model OR mapping to fields
        public OfflineList.CanOpen? CanOpen { get; set; }

        public string? Category { get; set; }

        public string? Comment { get; set; }

        public string? Date { get; set; }

        public string? DatVersion { get; set; }

        /// <remarks>(yes|no) "no"</remarks>
        public bool? Debug { get; set; }

        public string? Description { get; set; }

        public string? Email { get; set; }

        public string? EmulatorVersion { get; set; }

        /// <remarks>(none|split|merged|nonmerged|fullmerged|device|full) "split"</remarks>
        public MergingFlag ForceMerging { get; set; }

        /// <remarks>(obsolete|required|ignore) "obsolete"</remarks>
        public NodumpFlag ForceNodump { get; set; }

        /// <remarks>(zip|unzip) "zip"</remarks>
        public PackingFlag ForcePacking { get; set; }

        /// <remarks>(yes|no) "yes"</remarks>
        public bool? ForceZipping { get; set; }

        public string[]? HeaderRow { get; set; }

        public string? HeaderSkipper { get; set; }

        public string? Homepage { get; set; }

        public string? Id { get; set; }

        /// TODO: This needs an internal model OR mapping to fields
        public OfflineList.Images? Images { get; set; }

        public string? ImFolder { get; set; }

        /// TODO: This needs an internal model OR mapping to fields
        public OfflineList.Infos? Infos { get; set; }

        /// <remarks>(yes|no) "no"</remarks>
        public bool? LockBiosMode { get; set; }

        /// <remarks>(yes|no) "no"</remarks>
        public bool? LockRomMode { get; set; }

        /// <remarks>(yes|no) "no"</remarks>
        public bool? LockSampleMode { get; set; }

        public string? MameConfig { get; set; }

        public string? Name { get; set; }

        /// TODO: This needs an internal model OR mapping to fields
        public OfflineList.NewDat? NewDat { get; set; }

        public string? Notes { get; set; }

        public string? Plugin { get; set; }

        public string? RefName { get; set; }

        /// <remarks>(none|split|merged|nonmerged|fullmerged|device|full) "split"</remarks>
        public MergingFlag RomMode { get; set; }

        public string? RomTitle { get; set; }

        public string? RootDir { get; set; }

        /// <remarks>(none|split|merged|nonmerged|fullmerged|device|full) "split"</remarks>
        public MergingFlag SampleMode { get; set; }

        public string? ScreenshotsHeight { get; set; }

        public string? ScreenshotsWidth { get; set; }

        /// TODO: This needs an internal model OR mapping to fields
        public OfflineList.Search? Search { get; set; }

        public string? System { get; set; }

        public string? Timestamp { get; set; }

        public string? Type { get; set; }

        public string? Url { get; set; }

        public string? Version { get; set; }

        #endregion

        #region Keys

        /// <remarks>string</remarks>
        public const string SchemaLocationKey = "schemaLocation";

        #endregion
    }
}
