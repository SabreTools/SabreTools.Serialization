using System;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("instance"), XmlRoot("instance")]
    public class Instance : DatItem, ICloneable, IEquatable<Instance>
    {
        #region Properties

        public string? BriefName { get; set; }

        public string? Name { get; set; }

        #endregion

        public Instance() => ItemType = ItemType.Instance;

        /// <inheritdoc/>
        public object Clone()
        {
            var obj = new Instance();

            obj.BriefName = BriefName;
            obj.Name = Name;

            return obj;
        }

        /// <inheritdoc/>
        public bool Equals(Instance? other)
        {
            // Null never matches
            if (other is null)
                return false;

            // Properties
            if ((BriefName is null) ^ (other.BriefName is null))
                return false;
            else if (BriefName is not null && !BriefName.Equals(other.BriefName, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((Name is null) ^ (other.Name is null))
                return false;
            else if (Name is not null && !Name.Equals(other.Name, StringComparison.OrdinalIgnoreCase))
                return false;

            return true;
        }
    }
}
