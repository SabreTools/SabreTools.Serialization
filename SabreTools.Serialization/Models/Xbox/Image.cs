using Newtonsoft.Json;

namespace SabreTools.Serialization.Models.Xbox
{
    /// <summary>
    /// List of image files associated with a package in catalog.js
    /// </summary>
    public class Image
    {
        /// <summary>
        /// "size":
        /// String representing image size
        /// Known values: "100x100", "208x208", "480x480"
        /// </summary>
        [JsonProperty("size", NullValueHandling = NullValueHandling.Ignore)]
        public string? Size { get; set; }

        /// <summary>
        /// "image":
        /// File name of image within MSXC/Metadata/<PackageName>/
        /// </summary>
        [JsonProperty("image", NullValueHandling = NullValueHandling.Ignore)]
        public string? Name { get; set; }
    }
}
