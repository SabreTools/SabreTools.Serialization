using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SabreTools.Data.Models.VDF
{
    /// <summary>
    /// Contains metadata information about retail Steam discs
    /// Stored in a VDF file on the disc
    /// </summary>
    /// <remarks>Stored in the order it appears in the sku sis file, as it is always the same order.</remarks>
    [JsonObject]
    public class Sku
    {
        // At the moment, the only keys that matter for anything in SabreTools are sku, apps, depots, and manifests
        // TODO: check case sensitivity
        #region Non-Arrays

        /// <summary>
        /// "name"
        /// Name of the disc/app
        /// Known values: Arbitrary string
        /// </summary>
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string? Name { get; set; }

        /// <summary>
        /// "productname"
        /// productname of the retail installer
        /// Known values: Arbitrary string
        /// </summary>
        /// <remarks>sim/sid only</remarks>
        [JsonProperty("productname", NullValueHandling = NullValueHandling.Ignore)]
        public string? ProductName { get; set; }

        /// <summary>
        /// "subscriptionID"
        /// subscriptionID of the retail installer
        /// Known values: Arbitrary number
        /// </summary>
        /// <remarks>sim/sid only</remarks>
        [JsonProperty("subscriptionID", NullValueHandling = NullValueHandling.Ignore)]
        public long? SubscriptionId { get; set; }

        /// <summary>
        /// "appID"
        /// AppID of the retail installer
        /// Known values: Arbitrary number
        /// </summary>
        /// <remarks>sim/sid only. Both appID and AppID seem to be used in practice.</remarks>
        [JsonProperty("appID", NullValueHandling = NullValueHandling.Ignore)]
        public long? AppId { get; set; }

        /// <summary>
        /// "disks"
        /// Number of discs of the retail installer set
        /// Known values: 1-5? 10? Unsure what the most discs in a steam retail installer is currently known to be
        /// </summary>
        [JsonProperty("disks", NullValueHandling = NullValueHandling.Ignore)]
        public uint? Disks { get; set; }

        /// <summary>
        /// "language"
        /// language of the retail installer
        /// Known values: english, russian
        /// </summary>
        /// <remarks>sim/sid only</remarks>
        [JsonProperty("language", NullValueHandling = NullValueHandling.Ignore)]
        public string? Language { get; set; }

        /// <summary>
        /// "disk"
        /// Numbered disk of the retail installer set
        /// Known values: 1-5? 10? Unsure what the most discs in a steam retail installer is currently known to be
        /// </summary>
        /// <remarks>csm/csd only</remarks>
        [JsonProperty("disk", NullValueHandling = NullValueHandling.Ignore)]
        public uint? Disk { get; set; }

        /// <summary>
        /// "backup"
        /// Unknown. This is probably a boolean?
        /// Known values: 0
        /// </summary>
        [JsonProperty("backup", NullValueHandling = NullValueHandling.Ignore)]
        public uint? Backup { get; set; }

        /// <summary>
        /// "contenttype"
        /// Unknown.
        /// Known values: 3
        /// </summary>
        [JsonProperty("contenttype", NullValueHandling = NullValueHandling.Ignore)]
        public uint? ContentType { get; set; }

        #endregion

        // When VDF has an array, they represent it like this, with the left numbers being indexes:
        /// "1"		"1056577072"
        /// "2"		"1056702256"
        /// "3"		"1056203136"
        /// etc.
        /// The following format is also used like this, although this isn't one that needs to be parsed right now.
        /// Currently unsure what the first number means. Maybe this is a two dimensional array?
        /// "1 0"		"1493324560"
        /// "1 1"		"1492884912"
        /// "1 2"		"1492755784"
        /// "1 3"		"28749920"
        #region Arrays

        /// <summary>
        /// "apps"
        /// AppIDs contained on the disc.
        /// Known values: arbitrary
        /// </summary>
        /// <remarks>On csm/csd discs, both are used interchangeably, but never at the same time. It's usually still lowercase though.
        /// It always seems to be lowercase on sim/sid discs</remarks>
        [JsonProperty("apps", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<long, long>? Apps { get; set; }

        /// <summary>
        /// "depots"
        /// DepotIDs contained on the disc.
        /// Known values: arbitrary
        /// </summary>
        [JsonProperty("depots", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<long, long>? Depots { get; set; }

        // The "packages" property should go here, but it uses the second array format mentioned above, so it's more
        // difficult to adapt. Since it's not needed at the moment anyways, it's left out for now.

        /// <summary>
        /// "manifests"
        /// DepotIDs contained on the disc.
        /// Known values: arbitrary pairs of DepotID - Manifest
        /// </summary>
        [JsonProperty("manifests", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<long, long>? Manifests { get; set; }

        /// <summary>
        /// "chunkstores"
        /// chunkstores contained on the disc.
        /// Known values: DepotIDs containing arrays of chunkstores.
        /// </summary>
        /// <remarks>These are indexed from 1 instead of 0 for some reason.</remarks>
        /// TODO: not that it really matters, but will this parse the internal values recursively properly?
        [JsonProperty("chunkstores", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<long, Dictionary<long, long>?>? Chunkstores { get; set; }

        /// <summary>
        /// All remaining data not matched above.
        /// </summary>
        [JsonExtensionData]
        public IDictionary<string, JToken>? EverythingElse { get; set; }

        #endregion
    }
}
