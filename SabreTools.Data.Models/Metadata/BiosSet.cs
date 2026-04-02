using System;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("biosset"), XmlRoot("biosset")]
    public class BiosSet : DatItem, ICloneable, IEquatable<BiosSet>
    {
        #region Properties

        /// <remarks>(yes|no) "no"</remarks>
        public bool? Default { get; set; }

        public string? Description { get; set; }

        public string? Name { get; set; }

        #endregion

        public BiosSet() => ItemType = ItemType.BiosSet;

        /// <inheritdoc/>
        public object Clone()
        {
            var obj = new BiosSet();

            obj.Default = Default;
            obj.Description = Description;
            obj.Name = Name;

            return obj;
        }

        /// <inheritdoc/>
        public bool Equals(BiosSet? other)
        {
            // Null never matches
            if (other is null)
                return false;

            // Properties
            if (Default != other.Default)
                return false;

            if ((Description is null) ^ (other.Description is null))
                return false;
            else if (Description is not null && !Description.Equals(other.Description, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((Name is null) ^ (other.Name is null))
                return false;
            else if (Name is not null && !Name.Equals(other.Name, StringComparison.OrdinalIgnoreCase))
                return false;

            return true;
        }
    }
}
