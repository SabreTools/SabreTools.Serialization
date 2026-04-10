using System;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("adjuster"), XmlRoot("adjuster")]
    public class Adjuster : DatItem, ICloneable, IEquatable<Adjuster>
    {
        #region Properties

        /// <remarks>Condition subitem</remarks>
        public string? ConditionMask { get; set; }

        /// <remarks>Condition subitem, (eq|ne|gt|le|lt|ge)</remarks>
        public Relation? ConditionRelation { get; set; }

        /// <remarks>Condition subitem</remarks>
        public string? ConditionTag { get; set; }

        /// <remarks>Condition subitem</remarks>
        public string? ConditionValue { get; set; }

        /// <remarks>(yes|no) "no"</remarks>
        public bool? Default { get; set; }

        public string? Name { get; set; }

        #endregion

        public Adjuster() => ItemType = ItemType.Adjuster;

        /// <inheritdoc/>
        public object Clone()
        {
            var obj = new Adjuster();

            obj.ConditionMask = ConditionMask;
            obj.ConditionRelation = ConditionRelation;
            obj.ConditionTag = ConditionTag;
            obj.ConditionValue = ConditionValue;
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
            if ((ConditionMask is null) ^ (other.ConditionMask is null))
                return false;
            else if (ConditionMask is not null && !ConditionMask.Equals(other.ConditionMask, StringComparison.OrdinalIgnoreCase))
                return false;

            if (ConditionRelation != other.ConditionRelation)
                return false;

            if ((ConditionTag is null) ^ (other.ConditionTag is null))
                return false;
            else if (ConditionTag is not null && !ConditionTag.Equals(other.ConditionTag, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((ConditionValue is null) ^ (other.ConditionValue is null))
                return false;
            else if (ConditionValue is not null && !ConditionValue.Equals(other.ConditionValue, StringComparison.OrdinalIgnoreCase))
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
