using System;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("sample"), XmlRoot("sample")]
    public class Sample : DatItem, IEquatable<Sample>
    {
        #region Properties

        public string? Name { get; set; }

        #endregion

        public Sample() => ItemType = ItemType.Sample;

        /// <inheritdoc/>
        public object Clone()
        {
            var obj = new Sample();

            obj.Name = Name;

            return obj;
        }

        /// <inheritdoc/>
        public bool Equals(Sample? other)
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
