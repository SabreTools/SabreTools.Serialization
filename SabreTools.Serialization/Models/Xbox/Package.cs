using Newtonsoft.Json;

namespace SabreTools.Serialization.Models.Xbox
{
    /// <summary>
    /// Metadata about each package on disc, in catalog.js
    /// Packages are stored within /MSXC/
    /// </summary>
    public class Package
    {
        /// <summary>
        /// "packageName":
        /// Package name of variant
        /// Matches MSXC/<PackageName> and MSXC/Metdata/<PackageName>
        /// Known Versions Present: 2.0, 2.1, 4.0
        /// </summary>
        [JsonProperty("packageName", NullValueHandling = NullValueHandling.Ignore)]
        public string? PackageName { get; set; }

        /// <summary>
        /// "productId":
        /// Hex identifier for package Product ID
        /// Known Versions Present: 2.0, 2.1
        /// </summary>
        [JsonProperty("productId", NullValueHandling = NullValueHandling.Ignore)]
        public string? ProductID { get; set; }

        /// <summary>
        /// "contentId":
        /// Hex content identifier
        /// Known Versions present: 2.0, 2.1
        /// </summary>
        [JsonProperty("contentId", NullValueHandling = NullValueHandling.Ignore)]
        public string? ContentID { get; set; }

        /// <summary>
        /// "xboxProductId":
        /// Hex product identifier
        /// Known Versions Present: 4.0
        /// </summary>
        [JsonProperty("xboxProductId", NullValueHandling = NullValueHandling.Ignore)]
        public string? XboxProductID { get; set; }

        /// <summary>
        /// "oneStoreProductId":
        /// Partner Center Product ID
        /// 12 character uppercase alphanumeric
        /// Known Versions Present: 4.0
        /// </summary>
        [JsonProperty("oneStoreProductId", NullValueHandling = NullValueHandling.Ignore)]
        public string? OneStoreProductID { get; set; }

        /// <summary>
        /// "allowedOneStoreProductIds":
        /// List of OneStoreProductID that this package is associated with
        /// Used for DLC packages only (Type = "Durable")
        /// Known Versions Present: 4.0
        /// </summary>
        [JsonProperty("allowedOneStoreProductIds", NullValueHandling = NullValueHandling.Ignore)]
        public string[]? AllowedOneStoreProductIDs { get; set; }

        /// <summary>
        /// "franchiseGameHubId":
        /// Hex identifier
        /// Optionally used to mark package as game hub
        /// Known Versions Present: 4.1
        /// </summary>
        [JsonProperty("franchiseGameHubId", NullValueHandling = NullValueHandling.Ignore)]
        public string? FranchiseGameHubID { get; set; }

        /// <summary>
        /// "associatedFranchiseGameHubId":
        /// Hex identifier
        /// Marks corresponding FranchiseGameHubID that this package is launched with
        /// Known Versions Present: 4.1
        /// </summary>
        [JsonProperty("associatedFranchiseGameHubId", NullValueHandling = NullValueHandling.Ignore)]
        public string? AssociatedFranchiseGameHubID { get; set; }

        /// <summary>
        /// "titleId":
        /// 8 hex character package Title ID
        /// Known Versions Present: 2.0, 2.1, 4.0
        /// </summary>
        [JsonProperty("titleId", NullValueHandling = NullValueHandling.Ignore)]
        public string? TitleID { get; set; }

        /// <summary>
        /// "titles"
        /// List of name of package for each locale
        /// Known Versions Present: 2.0, 2.1, 4.0
        /// </summary>
        [JsonProperty("titles", NullValueHandling = NullValueHandling.Ignore)]
        public Title[]? Titles { get; set; }

        /// <summary>
        /// "vui":
        /// List of Voice User Interface packages titles for each locale
        /// Known Versions Present: 2.0, 2.1, 4.0
        /// </summary>
        [JsonProperty("vui", NullValueHandling = NullValueHandling.Ignore)]
        public Title[]? VUI { get; set; }

        /// <summary>
        /// "images":
        /// List of paths to each image in MSXC/Metadata/<PackageName>/
        /// Known Versions Present: 2.0, 2.1, 4.0
        /// </summary>
        [JsonProperty("images", NullValueHandling = NullValueHandling.Ignore)]
        public Image[]? Images { get; set; }

        /// <summary>
        /// "ratings":
        /// List of package age ratings for each relevant rating system
        /// Known Versions Present: 2.0, 2.1, 4.0
        /// </summary>
        [JsonProperty("ratings", NullValueHandling = NullValueHandling.Ignore)]
        public Rating[]? Ratings { get; set; }

        /// <summary>
        /// "attributes":
        /// Extra attributes associated with this package
        /// Known Versions Present: 2.1, 4.0
        /// </summary>
        [JsonProperty("attributes", NullValueHandling = NullValueHandling.Ignore)]
        public Attribute[]? Attributes { get; set; }

        /// <summary>
        /// "variants":
        /// Alternative packages
        /// Known Versions Present: 4.0
        /// </summary>
        [JsonProperty("variants", NullValueHandling = NullValueHandling.Ignore)]
        public Package[]? Variants { get; set; }

        /// <summary>
        /// "generation":
        /// Console generation the package is for
        /// Known values: "8" (XboxOne), "9" (Xbox Series X|S)
        /// Known Versions Present: 4.0
        /// </summary>
        [JsonProperty("generation", NullValueHandling = NullValueHandling.Ignore)]
        public string? Generation { get; set; }

        /// <summary>
        /// "size":
        /// Size of package in bytes
        /// Known Versions Present: 2.0, 2.1
        /// </summary>
        [JsonProperty("size", NullValueHandling = NullValueHandling.Ignore)]
        public long? Size { get; set; }

        /// <summary>
        /// "type":
        /// Package Type
        /// Known values: "Game" (Game package), "Durable" (DLC package)
        /// Known Versions Present: 2.0, 2.1, 4.0
        /// </summary>
        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public string? Type { get; set; }
    }
}
