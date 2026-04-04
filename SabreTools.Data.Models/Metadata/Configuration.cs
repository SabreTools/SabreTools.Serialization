using System;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("configuration"), XmlRoot("configuration")]
    public class Configuration : DatItem, ICloneable, IEquatable<Configuration>
    {
        #region Properties

        public Condition? Condition { get; set; }

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

            obj.Condition = Condition?.Clone() as Condition;
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

            // Sub-items
            if ((Condition is null) ^ (other.Condition is null))
                return false;
            else if (Condition is not null && other.Condition is not null && Condition.Equals(other.Condition))
                return false;

            // TODO: Figure out how to properly check arrays

            return true;
        }
    }
}
