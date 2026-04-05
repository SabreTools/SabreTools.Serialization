using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    /// <summary>
    /// Format-agnostic representation of item data
    /// </summary>
    public class DatItem
    {
        /// <summary>
        /// Quick accessor to item type, if it exists
        /// </summary>
        [JsonProperty("itemtype", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("itemtype")]
        public ItemType ItemType { get; protected set; }
    }
}
