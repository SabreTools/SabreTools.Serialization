using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Metadata.DatItems.Formats
{
    /// <summary>
    /// Represents the OpenMSX original value
    /// </summary>
    [JsonObject("original"), XmlRoot("original")]
    public sealed class Original
    {
        #region Properties

        [JsonProperty("content"), XmlElement("content")]
        public string? Content
        {
            get => _internal.Content;
            set => _internal.Content = value;
        }

        /// <inheritdoc cref="DatItem.ItemType">/>
        public Data.Models.Metadata.ItemType ItemType
            => Data.Models.Metadata.ItemType.Original;

        [JsonProperty("value"), XmlElement("value")]
        public bool? Value
        {
            get => _internal.Value;
            set => _internal.Value = value;
        }

        #endregion

        /// <summary>
        /// Internal Original model
        /// </summary>
        [JsonIgnore]
        private readonly Data.Models.Metadata.Original _internal = new();
    }
}
