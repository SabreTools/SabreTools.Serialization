using System;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("release"), XmlRoot("release")]
    public class Release : DatItem, ICloneable, IEquatable<Release>
    {
        #region Properties

        public string? Date { get; set; }

        /// <remarks>(yes|no) "no"</remarks>
        public bool? Default { get; set; }

        public string? Language { get; set; }

        public string? Name { get; set; }

        public string? Region { get; set; }

        #endregion

        public Release() => ItemType = ItemType.Release;

        /// <inheritdoc/>
        public object Clone()
        {
            var obj = new Release();

            obj.Date = Date;
            obj.Default = Default;
            obj.Language = Language;
            obj.Name = Name;
            obj.Region = Region;

            return obj;
        }

        /// <inheritdoc/>
        public bool Equals(Release? other)
        {
            // Null never matches
            if (other is null)
                return false;

            // Properties
            if ((Date is null) ^ (other.Date is null))
                return false;
            else if (Date is not null && !Date.Equals(other.Date, StringComparison.OrdinalIgnoreCase))
                return false;

            if (Default != other.Default)
                return false;

            if ((Language is null) ^ (other.Language is null))
                return false;
            else if (Language is not null && !Language.Equals(other.Language, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((Name is null) ^ (other.Name is null))
                return false;
            else if (Name is not null && !Name.Equals(other.Name, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((Region is null) ^ (other.Region is null))
                return false;
            else if (Region is not null && !Region.Equals(other.Region, StringComparison.OrdinalIgnoreCase))
                return false;

            return true;
        }
    }
}
