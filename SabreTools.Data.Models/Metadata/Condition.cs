using System;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("condition"), XmlRoot("condition")]
    public class Condition : DatItem, ICloneable, IEquatable<Condition>
    {
        #region Properties

        public string? Mask { get; set; }

        /// <remarks>(eq|ne|gt|le|lt|ge)</remarks>
        public Relation? Relation { get; set; }

        public string? Tag { get; set; }

        public string? Value { get; set; }

        #endregion

        public Condition() => ItemType = ItemType.Condition;

        /// <inheritdoc/>
        public object Clone()
        {
            var obj = new Condition();

            obj.Mask = Mask;
            obj.Relation = Relation;
            obj.Tag = Tag;
            obj.Value = Value;

            return obj;
        }

        /// <inheritdoc/>
        public bool Equals(Condition? other)
        {
            // Null never matches
            if (other is null)
                return false;

            // Properties
            if ((Mask is null) ^ (other.Mask is null))
                return false;
            else if (Mask is not null && !Mask.Equals(other.Mask, StringComparison.OrdinalIgnoreCase))
                return false;

            if (Relation != other.Relation)
                return false;

            if ((Tag is null) ^ (other.Tag is null))
                return false;
            else if (Tag is not null && !Tag.Equals(other.Tag, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((Value is null) ^ (other.Value is null))
                return false;
            else if (Value is not null && !Value.Equals(other.Value, StringComparison.OrdinalIgnoreCase))
                return false;

            return true;
        }
    }
}
