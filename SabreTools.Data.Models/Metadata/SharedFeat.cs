using System;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("sharedfeat"), XmlRoot("sharedfeat")]
    public class SharedFeat : DatItem, ICloneable, IEquatable<SharedFeat>
    {
        #region Properties

        public string? Name { get; set; }

        public string? Value { get; set; }

        #endregion

        public SharedFeat() => ItemType = ItemType.SharedFeat;

        /// <inheritdoc/>
        public object Clone()
        {
            var obj = new SharedFeat();

            obj.Name = Name;
            obj.Value = Value;

            return obj;
        }

        /// <inheritdoc/>
        public bool Equals(SharedFeat? other)
        {
            // Null never matches
            if (other is null)
                return false;

            // Properties
            if ((Name is null) ^ (other.Name is null))
                return false;
            else if (Name is not null && !Name.Equals(other.Name, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((Value is null) ^ (other.Value is null))
                return false;
            else if (Value is not null && !Value.Equals(other.Value, StringComparison.OrdinalIgnoreCase))
                return false;

            return true;
        }
    }
}
