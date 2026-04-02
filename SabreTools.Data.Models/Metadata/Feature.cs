using System;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("feature"), XmlRoot("feature")]
    public class Feature : DatItem, ICloneable, IEquatable<Feature>
    {
        #region Properties

        /// <remarks>(protection|timing|graphics|palette|sound|capture|camera|microphone|controls|keyboard|mouse|media|disk|printer|tape|punch|drum|rom|comms|lan|wan)</remarks>
        public FeatureType? FeatureType { get; set; }

        public string? Name { get; set; }

        /// <remarks>(unemulated|imperfect)</remarks>
        public FeatureStatus? Overall { get; set; }

        /// <remarks>(unemulated|imperfect)</remarks>
        public FeatureStatus? Status { get; set; }

        public string? Value { get; set; }

        #endregion

        public Feature() => ItemType = ItemType.Feature;

        /// <inheritdoc/>
        public object Clone()
        {
            var obj = new Feature();

            obj.FeatureType = FeatureType;
            obj.Name = Name;
            obj.Overall = Overall;
            obj.Status = Status;
            obj.Value = Value;

            return obj;
        }

        /// <inheritdoc/>
        public bool Equals(Feature? other)
        {
            // Null never matches
            if (other is null)
                return false;

            // Properties
            if (FeatureType != other.FeatureType)
                return false;

            if ((Name is null) ^ (other.Name is null))
                return false;
            else if (Name is not null && !Name.Equals(other.Name, StringComparison.OrdinalIgnoreCase))
                return false;

            if (Overall != other.Overall)
                return false;

            if (Status != other.Status)
                return false;

            if ((Value is null) ^ (other.Value is null))
                return false;
            else if (Value is not null && !Value.Equals(other.Value, StringComparison.OrdinalIgnoreCase))
                return false;

            return true;
        }
    }
}
