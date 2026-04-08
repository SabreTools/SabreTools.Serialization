using System;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("analog"), XmlRoot("analog")]
    public class Analog : DatItem, ICloneable, IEquatable<Analog>
    {
        #region Properties

        public string? Mask { get; set; }

        #endregion

        public Analog() => ItemType = ItemType.Analog;

        /// <inheritdoc/>
        public object Clone()
        {
            var obj = new Analog();

            obj.Mask = Mask;

            return obj;
        }

        /// <inheritdoc/>
        public bool Equals(Analog? other)
        {
            // Null never matches
            if (other is null)
                return false;

            // Properties
            if ((Mask is null) ^ (other.Mask is null))
                return false;
            else if (Mask is not null && !Mask.Equals(other.Mask, StringComparison.OrdinalIgnoreCase))
                return false;

            return true;
        }
    }
}
