using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    /// <summary>
    /// Format-agnostic representation of item data
    /// </summary>
    public abstract class DatItem
    {
        /// <summary>
        /// Quick accessor to item type
        /// </summary>
        [JsonProperty("itemtype", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("itemtype")]
        public ItemType ItemType { get; protected set; }
    }
}
