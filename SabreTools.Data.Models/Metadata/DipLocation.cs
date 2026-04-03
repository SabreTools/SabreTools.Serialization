using System;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("diplocation"), XmlRoot("diplocation")]
    public class DipLocation : DatItem, ICloneable, IEquatable<DipLocation>
    {
        #region Properties

        /// <remarks>(yes|no) "no"</remarks>
        public bool? Inverted { get; set; }

        public string? Name { get; set; }

        public long? Number { get; set; }

        #endregion

        public DipLocation() => ItemType = ItemType.DipLocation;

        /// <inheritdoc/>
        public object Clone()
        {
            var obj = new DipLocation();

            obj.Inverted = Inverted;
            obj.Name = Name;
            obj.Number = Number;

            return obj;
        }

        /// <inheritdoc/>
        public bool Equals(DipLocation? other)
        {
            // Null never matches
            if (other is null)
                return false;

            // Properties
            if (Inverted != other.Inverted)
                return false;

            if ((Name is null) ^ (other.Name is null))
                return false;
            else if (Name is not null && !Name.Equals(other.Name, StringComparison.OrdinalIgnoreCase))
                return false;

            if (Number != other.Number)
                return false;

            return true;
        }
    }
}
