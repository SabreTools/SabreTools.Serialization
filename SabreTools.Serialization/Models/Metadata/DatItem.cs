using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    /// <summary>
    /// Format-agnostic representation of item data
    /// </summary>
    public class DatItem : DictionaryBase
    {
        #region Common Keys

        public const string TypeKey = "_type";

        #endregion

        /// <summary>
        /// Quick accessor to item type, if it exists
        /// </summary>
        [JsonProperty("itemtype", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("itemtype")]
        public ItemType? Type
        {
            get => ContainsKey(TypeKey) ? this[TypeKey] as ItemType? : null;
            set => this[TypeKey] = value;
        }
    }
}