using System;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("slot"), XmlRoot("slot")]
    public class Slot : DatItem, ICloneable, IEquatable<Slot>
    {
        #region Properties

        public string? Name { get; set; }

        public SlotOption[]? SlotOption { get; set; }

        #endregion

        public Slot() => ItemType = ItemType.Slot;

        /// <inheritdoc/>
        public object Clone()
        {
            var obj = new Slot();

            obj.Name = Name;
            if (SlotOption is not null)
                obj.SlotOption = Array.ConvertAll(SlotOption, i => (SlotOption)i.Clone());

            return obj;
        }

        /// <inheritdoc/>
        public bool Equals(Slot? other)
        {
            // Null never matches
            if (other is null)
                return false;

            // Properties
            if ((Name is null) ^ (other.Name is null))
                return false;
            else if (Name is not null && !Name.Equals(other.Name, StringComparison.OrdinalIgnoreCase))
                return false;

            // Sub-items
            // TODO: Figure out how to properly check arrays

            return true;
        }
    }
}
