using System;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("softwarelist"), XmlRoot("softwarelist")]
    public class SoftwareList : DatItem, ICloneable, IEquatable<SoftwareList>
    {
        #region Properties

        public string? Filter { get; set; }

        public string? Name { get; set; }

        /// <remarks>(original|compatible)</remarks>
        public SoftwareListStatus? Status { get; set; }

        public string? Tag { get; set; }

        #endregion

        public SoftwareList() => ItemType = ItemType.SoftwareList;

        /// <inheritdoc/>
        public object Clone()
        {
            var obj = new SoftwareList();

            obj.Filter = Filter;
            obj.Name = Name;
            obj.Status = Status;
            obj.Tag = Tag;

            return obj;
        }

        /// <inheritdoc/>
        public bool Equals(SoftwareList? other)
        {
            // Null never matches
            if (other is null)
                return false;

            // Properties
            if ((Filter is null) ^ (other.Filter is null))
                return false;
            else if (Filter is not null && !Filter.Equals(other.Filter, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((Name is null) ^ (other.Name is null))
                return false;
            else if (Name is not null && !Name.Equals(other.Name, StringComparison.OrdinalIgnoreCase))
                return false;

            if (Status != other.Status)
                return false;

            if ((Tag is null) ^ (other.Tag is null))
                return false;
            else if (Tag is not null && !Tag.Equals(other.Tag, StringComparison.OrdinalIgnoreCase))
                return false;

            return true;
        }
    }
}
