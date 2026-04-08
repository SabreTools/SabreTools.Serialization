using System;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("ramoption"), XmlRoot("ramoption")]
    public class RamOption : DatItem, ICloneable, IEquatable<RamOption>
    {
        #region Properties

        public string? Content { get; set; }

        /// <remarks>(yes|no) "no"</remarks>
        public bool? Default { get; set; }

        public string? Name { get; set; }

        #endregion

        public RamOption() => ItemType = ItemType.RamOption;

        /// <inheritdoc/>
        public object Clone()
        {
            var obj = new RamOption();

            obj.Content = Content;
            obj.Default = Default;
            obj.Name = Name;

            return obj;
        }

        /// <inheritdoc/>
        public bool Equals(RamOption? other)
        {
            // Null never matches
            if (other is null)
                return false;

            // Properties
            if ((Content is null) ^ (other.Content is null))
                return false;
            else if (Content is not null && !Content.Equals(other.Content, StringComparison.OrdinalIgnoreCase))
                return false;

            if (Default != other.Default)
                return false;

            if ((Name is null) ^ (other.Name is null))
                return false;
            else if (Name is not null && !Name.Equals(other.Name, StringComparison.OrdinalIgnoreCase))
                return false;

            return true;
        }
    }
}
