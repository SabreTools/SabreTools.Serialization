using System;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("extension"), XmlRoot("extension")]
    public class Extension : DatItem, ICloneable, IEquatable<Extension>
    {
        #region Properties

        public string? Name { get; set; }

        #endregion

        public Extension() => ItemType = ItemType.Extension;

        /// <inheritdoc/>
        public object Clone()
        {
            var obj = new Extension();

            obj.Name = Name;

            return obj;
        }

        /// <inheritdoc/>
        public bool Equals(Extension? other)
        {
            // Null never matches
            if (other is null)
                return false;

            // Properties
            if ((Name is null) ^ (other.Name is null))
                return false;
            else if (Name is not null && !Name.Equals(other.Name, StringComparison.OrdinalIgnoreCase))
                return false;

            return true;
        }
    }
}
