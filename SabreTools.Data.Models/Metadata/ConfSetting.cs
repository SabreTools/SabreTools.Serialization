using System;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("confsetting"), XmlRoot("confsetting")]
    public class ConfSetting : DatItem, ICloneable, IEquatable<ConfSetting>
    {
        #region Properties

        public Condition? Condition { get; set; }

        /// <remarks>(yes|no) "no"</remarks>
        public bool? Default { get; set; }

        public string? Name { get; set; }

        public string? Value { get; set; }

        #endregion

        public ConfSetting() => ItemType = ItemType.ConfSetting;

        /// <inheritdoc/>
        public object Clone()
        {
            var obj = new ConfSetting();

            obj.Condition = Condition?.Clone() as Condition;
            obj.Default = Default;
            obj.Name = Name;
            obj.Value = Value;

            return obj;
        }

        /// <inheritdoc/>
        public bool Equals(ConfSetting? other)
        {
            // Null never matches
            if (other is null)
                return false;

            // Properties
            if (Default != other.Default)
                return false;

            if ((Name is null) ^ (other.Name is null))
                return false;
            else if (Name is not null && !Name.Equals(other.Name, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((Value is null) ^ (other.Value is null))
                return false;
            else if (Value is not null && !Value.Equals(other.Value, StringComparison.OrdinalIgnoreCase))
                return false;

            // Sub-items
            if ((Condition is null) ^ (other.Condition is null))
                return false;
            else if (Condition is not null && other.Condition is not null && Condition.Equals(other.Condition))
                return false;

            return true;
        }
    }
}
