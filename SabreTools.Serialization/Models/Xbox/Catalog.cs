using Newtonsoft.Json;

namespace SabreTools.Serialization.Models.Xbox
{
    /// <summary>
    /// Contains metadata information about XboxOne and XboxSX discs
    /// Stored in a JSON file on the disc at /MSXC/Metadata/catalog.js
    /// </summary>
    [JsonObject]
    public class Catalog
    {
        /// <summary>
        /// "version":
        /// Version of this catalog.js file
        /// Known values: 1.0, 2.0, 2.1, 4.0, 4.1 (4.1 not confirmed on a disc)
        /// </summary>
        [JsonProperty("version")]
        public string? Version { get; set; }

        /// <summary>
        /// "discNumber":
        /// Varies for each disc in set
        /// 0 is reserved and shouldnt be used
        /// Known Versions Present: 4.0
        /// </summary>
        [JsonProperty("discNumber", NullValueHandling = NullValueHandling.Ignore)]
        public int? DiscNumber { get; set; }

        /// <summary>
        /// "discCount":
        /// Total number of discs in set
        /// Same value for each disc in the set
        /// Known Versions Present: 4.0
        /// </summary>
        [JsonProperty("discCount", NullValueHandling = NullValueHandling.Ignore)]
        public int? DiscCount { get; set; }

        /// <summary>
        /// "discSetId":
        /// 8 hex character ID for the set itself
        /// Same value for each disc in the set
        /// Known Versions Present: 4.0
        /// </summary>
        [JsonProperty("discSetId", NullValueHandling = NullValueHandling.Ignore)]
        public string? DiscSetID { get; set; }

        /// <summary>
        /// "bundle":
        /// Package details for the bundle itself
        /// Known fields used: ProductID, XboxProductID,
        ///     OneStoreProductID, Titles, VUI, Images
        /// Known Versions Present: 2.0, 2.1, 4.0
        /// </summary>
        [JsonProperty("bundle", NullValueHandling = NullValueHandling.Ignore)]
        public Package? Bundle { get; set; }

        /// <summary>
        /// "launchPackage":
        /// Package name to use as launch package
        /// Before 4.0, object=Package with only ContentID filled
        /// For 4.0 onwards, object=String, representing filename
        /// Known Versions Present: 2.0, 2.1, 4.0
        /// </summary>
        [JsonProperty("launchPackage", NullValueHandling = NullValueHandling.Ignore)]
        public object? LaunchPackage { get; set; }

        /// <summary>
        /// "packages":
        /// Package details for each package on disc
        /// Known Versions Present: 2.0, 2.1, 4.0
        /// </summary>
        [JsonProperty("packages")]
        public Package[]? Packages { get; set; }

        /// <summary>
        /// "siblings":
        /// List of Package Names that are related to this disc
        /// The console picks the correct one to use
        /// Known Versions Present: 4.0
        /// </summary>
        [JsonProperty("siblings", NullValueHandling = NullValueHandling.Ignore)]
        public string[][]? Siblings { get; set; }

        #region v1.0 only

        // The below fields are usually present in a Package sub-field
        // but for v1.0 catalog.js files, they are at the root Catalog object

        /// <summary>
        /// "productId":
        /// Hex identifier for package Product ID
        /// Known Versions Present: 1.0
        /// Exists within Packages[].ProductID for v2.0 onwards
        /// </summary>
        [JsonProperty("productId", NullValueHandling = NullValueHandling.Ignore)]
        public string? ProductID { get; set; }

        /// <summary>
        /// "contentId":
        /// Hex content identifier
        /// Known Versions present: 1.0
        /// Exists within Packages[].ContentID for v2.0 onwards
        /// </summary>
        [JsonProperty("contentId", NullValueHandling = NullValueHandling.Ignore)]
        public string? ContentID { get; set; }

        /// <summary>
        /// "titleId":
        /// 8 hex character package Title ID
        /// Known Versions Present: 1.0
        /// Exists within Packages[].TitleID for v2.0 onwards
        /// </summary>
        [JsonProperty("titleId", NullValueHandling = NullValueHandling.Ignore)]
        public string? TitleID { get; set; }

        /// <summary>
        /// "titles"
        /// List of name of package for each locale
        /// Known Versions Present: 1.0
        /// Exists within Packages[].Titles for v2.0 onwards
        /// </summary>
        [JsonProperty("titles", NullValueHandling = NullValueHandling.Ignore)]
        public Title[]? Titles { get; set; }

        /// <summary>
        /// "vui":
        /// List of Voice User Interface packages titles for each locale
        /// Known Versions Present: 1.0
        /// Exists within Packages[].VUI for v2.0 onwards
        /// </summary>
        [JsonProperty("vui", NullValueHandling = NullValueHandling.Ignore)]
        public Title[]? VUI { get; set; }

        /// <summary>
        /// "images":
        /// List of paths to each image in MSXC/Metadata/<PackageName>/
        /// Known Versions Present: 1.0
        /// Exists within Packages[].Images for v2.0 onwards
        /// </summary>
        [JsonProperty("images", NullValueHandling = NullValueHandling.Ignore)]
        public Image[]? Images { get; set; }

        /// <summary>
        /// "ratings":
        /// List of package age ratings for each relevant rating system
        /// Known Versions Present: 1.0
        /// Exists within Packages[].Ratings for v2.0 onwards
        /// </summary>
        [JsonProperty("ratings", NullValueHandling = NullValueHandling.Ignore)]
        public Rating[]? Ratings { get; set; }

        /// <summary>
        /// "size":
        /// Size of package in bytes
        /// Known Versions Present: 1.0
        /// Exists within Packages[].Size for v2.0 onwards
        /// </summary>
        [JsonProperty("size", NullValueHandling = NullValueHandling.Ignore)]
        public long? Size { get; set; }

        /// <summary>
        /// "type":
        /// Package Type
        /// Known values: "Game" (Game package), "Durable" (DLC package)
        /// Known Versions Present: 1.0
        /// Exists within Packages[].Type for v2.0 onwards
        /// </summary>
        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public string? Type { get; set; }

        #endregion
    }
}
