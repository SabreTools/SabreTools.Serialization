using System;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("port"), XmlRoot("port")]
    public class Port : DatItem, ICloneable, IEquatable<Port>
    {
        #region Properties

        public string[]? AnalogMask { get; set; }

        public string? Tag { get; set; }

        #endregion

        public Port() => ItemType = ItemType.Port;

        /// <inheritdoc/>
        public object Clone()
        {
            var obj = new Port();

            if (AnalogMask is not null)
                obj.AnalogMask = Array.ConvertAll(AnalogMask, i => i);
            obj.Tag = Tag;

            return obj;
        }

        /// <inheritdoc/>
        public bool Equals(Port? other)
        {
            // Null never matches
            if (other is null)
                return false;

            // Properties
            if ((Tag is null) ^ (other.Tag is null))
                return false;
            else if (Tag is not null && !Tag.Equals(other.Tag, StringComparison.OrdinalIgnoreCase))
                return false;

            // Sub-items
            // TODO: Figure out how to properly check arrays

            return true;
        }
    }
}
