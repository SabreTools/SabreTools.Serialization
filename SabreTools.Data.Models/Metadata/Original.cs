using System;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("original"), XmlRoot("original")]
    public class Original : DatItem, ICloneable, IEquatable<Original>
    {
        #region Properties

        public string? Content { get; set; }

        public bool? Value { get; set; }

        #endregion

        public Original() => ItemType = ItemType.Original;

        /// <inheritdoc/>
        public object Clone()
        {
            var obj = new Original();

            obj.Content = Content;
            obj.Value = Value;

            return obj;
        }

        /// <inheritdoc/>
        public bool Equals(Original? other)
        {
            // Null never matches
            if (other is null)
                return false;

            // Properties
            if ((Content is null) ^ (other.Content is null))
                return false;
            else if (Content is not null && !Content.Equals(other.Content, StringComparison.OrdinalIgnoreCase))
                return false;

            if (Value != other.Value)
                return false;

            return true;
        }
    }
}
