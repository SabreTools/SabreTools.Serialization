using Newtonsoft.Json;

namespace SabreTools.Serialization.Models.Xbox
{
    /// <summary>
    /// Package rating for each rating system, in catalog.js
    /// </summary>
    public class Rating
    {
        /// <summary>
        /// "system":
        /// Name of rating system
        /// Known values: COB-AU, PEGI, PCBP, USK, China, CERO, ESRB, GCAM, CSRR,
        ///     COB, DJCTQ, GRB, OFLC, OFLC-NZ, PEGIPortugal, FPB, Microsoft
        /// </summary>
        [JsonProperty("system", NullValueHandling = NullValueHandling.Ignore)]
        public string? System { get; set; }

        /// <summary>
        /// "value":
        /// String representing rating value, depends on rating system
        /// </summary>
        [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
        public string? Value { get; set; }
    }
}
