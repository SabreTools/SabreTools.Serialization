using Newtonsoft.Json;

namespace SabreTools.Data.Models.Xbox
{
    /// <summary>
    /// Extra attributes relating to package, in catalog.js
    /// </summary>
    public class Attribute
    {
        /// <summary>
        /// "supports4k":
        /// True if package supports 4K, false otherwise
        /// </summary>
        [JsonProperty("supports4k", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Supports4K { get; set; }

        /// <summary>
        /// "supportsHdr":
        /// True if package supports HDR, false otherwise
        /// </summary>
        [JsonProperty("supportsHdr", NullValueHandling = NullValueHandling.Ignore)]
        public bool? SupportsHDR { get; set; }

        /// <summary>
        /// "isXboxOneXEnhanced":
        /// True if package is XboxOne X enhanced, false otherwise
        /// </summary>
        [JsonProperty("isXboxOneXEnhanced", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsXboxOneXEnhanced { get; set; }

    }
}
