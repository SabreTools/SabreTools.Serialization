using Newtonsoft.Json;

namespace SabreTools.Data.Models.VDF
{
    /// <summary>
    /// Contains metadata information about retail Steam discs
    /// Stored in a VDF file on the disc
    /// </summary>
    /// <remarks>Stored in the order it appears in the sku sis file, as it is always the same order.</remarks>
    [JsonObject]
    public class SkuSis
    {
        // At the moment, the only keys that matter for anything in SabreTools are sku, apps, depots, and manifests
        // TODO: check case sensitivity
        #region Non-Arrays

        /// <summary>
        /// "sku"
        /// Top-level value for sku.sis files.
        /// Known values: the entire sku.sis object
        /// </summary>
        /// <remarks>capital SKU on sim/sid, lowercase sku on csm/csd</remarks>
        [JsonProperty("sku", NullValueHandling = NullValueHandling.Ignore)]
        public Sku? Sku { get; set; }

        #endregion
    }
}
