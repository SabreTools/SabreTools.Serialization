using Newtonsoft.Json;

namespace SabreTools.Serialization.Models.Xbox
{
    /// <summary>
    /// Package Title for each locale, for catalog.js
    /// </summary>
    public class Title
    {
        /// <summary>
        /// "locale":
        /// String representing locale that this title is in
        /// Known values: "default", "en", "de", "fr", "ar", "zh-hans",
        ///     "zh-hant", "zh-TW", "zh-HK", "zh-CN", "zh-SG", etc
        /// </summary>
        [JsonProperty("locale", NullValueHandling = NullValueHandling.Ignore)]
        public string? Locale { get; set; }

        /// <summary>
        /// "title":
        /// Package title
        /// </summary>
        [JsonProperty("title", NullValueHandling = NullValueHandling.Ignore)]
        public string? Name { get; set; }
    }
}
