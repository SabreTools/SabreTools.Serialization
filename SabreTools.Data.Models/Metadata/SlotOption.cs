using System;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("slotoption"), XmlRoot("slotoption")]
    public class SlotOption : DatItem, ICloneable, IEquatable<SlotOption>
    {
        #region Properties

        /// <remarks>(yes|no) "no"</remarks>
        public bool? Default { get; set; }

        public string? DevName { get; set; }

        public string? Name { get; set; }

        #endregion

        public SlotOption() => ItemType = ItemType.SlotOption;

        /// <inheritdoc/>
        public object Clone()
        {
            var obj = new SlotOption();

            obj.Default = Default;
            obj.DevName = DevName;
            obj.Name = Name;

            return obj;
        }

        /// <inheritdoc/>
        public bool Equals(SlotOption? other)
        {
            // Null never matches
            if (other is null)
                return false;

            // Properties
            if (Default != other.Default)
                return false;

            if ((DevName is null) ^ (other.DevName is null))
                return false;
            else if (DevName is not null && !DevName.Equals(other.DevName, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((Name is null) ^ (other.Name is null))
                return false;
            else if (Name is not null && !Name.Equals(other.Name, StringComparison.OrdinalIgnoreCase))
                return false;

            return true;
        }
    }
}
