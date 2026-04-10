using System;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("configuration"), XmlRoot("configuration")]
    public class Configuration : DatItem, ICloneable, IEquatable<Configuration>
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

        public ConfLocation[]? ConfLocation { get; set; }

        public ConfSetting[]? ConfSetting { get; set; }

        public string? Mask { get; set; }

        public string? Name { get; set; }

        public string? Tag { get; set; }

        #endregion

        public Configuration() => ItemType = ItemType.Configuration;

        /// <inheritdoc/>
        public object Clone()
        {
            var obj = new Configuration();

            obj.ConditionMask = ConditionMask;
            obj.ConditionRelation = ConditionRelation;
            obj.ConditionTag = ConditionTag;
            obj.ConditionValue = ConditionValue;
            if (ConfLocation is not null)
                obj.ConfLocation = Array.ConvertAll(ConfLocation, i => (ConfLocation)i.Clone());
            if (ConfSetting is not null)
                obj.ConfSetting = Array.ConvertAll(ConfSetting, i => (ConfSetting)i.Clone());
            obj.Mask = Mask;
            obj.Name = Name;
            obj.Tag = Tag;

            return obj;
        }

        /// <inheritdoc/>
        public bool Equals(Configuration? other)
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

            if ((Mask is null) ^ (other.Mask is null))
                return false;
            else if (Mask is not null && !Mask.Equals(other.Mask, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((Name is null) ^ (other.Name is null))
                return false;
            else if (Name is not null && !Name.Equals(other.Name, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((Tag is null) ^ (other.Tag is null))
                return false;
            else if (Tag is not null && !Tag.Equals(other.Tag, StringComparison.OrdinalIgnoreCase))
                return false;

            // TODO: Figure out how to properly check arrays

            return true;
        }
    }
}
