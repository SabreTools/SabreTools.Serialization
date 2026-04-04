using System;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("adjuster"), XmlRoot("adjuster")]
    public class Adjuster : DatItem, ICloneable, IEquatable<Adjuster>
    {
        #region Properties

        public Condition? Condition { get; set; }

        /// <remarks>(yes|no) "no"</remarks>
        public bool? Default { get; set; }

        public string? Name { get; set; }

        #endregion

        public Adjuster() => ItemType = ItemType.Adjuster;

        /// <inheritdoc/>
        public object Clone()
        {
            var obj = new Adjuster();

            obj.Condition = Condition?.Clone() as Condition;
            obj.Default = Default;
            obj.Name = Name;

            return obj;
        }

        /// <inheritdoc/>
        public bool Equals(Adjuster? other)
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

            // Sub-items
            if ((Condition is null) ^ (other.Condition is null))
                return false;
            else if (Condition is not null && other.Condition is not null && Condition.Equals(other.Condition))
                return false;

            return true;
        }
    }
}
