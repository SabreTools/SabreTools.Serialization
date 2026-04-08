using System;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("dipswitch"), XmlRoot("dipswitch")]
    public class DipSwitch : DatItem, ICloneable, IEquatable<DipSwitch>
    {
        #region Properties

        public Condition? Condition { get; set; }

        /// <remarks>(yes|no) "no"</remarks>
        public bool? Default { get; set; }

        public DipLocation[]? DipLocation { get; set; }

        public DipValue[]? DipValue { get; set; }

        public string[]? Entry { get; set; }

        public string? Mask { get; set; }

        public string? Name { get; set; }

        public string? Tag { get; set; }

        #endregion

        public DipSwitch() => ItemType = ItemType.DipSwitch;

        /// <inheritdoc/>
        public object Clone()
        {
            var obj = new DipSwitch();

            obj.Condition = Condition?.Clone() as Condition;
            obj.Default = Default;
            if (DipLocation is not null)
                obj.DipLocation = Array.ConvertAll(DipLocation, i => (DipLocation)i.Clone());
            if (DipValue is not null)
                obj.DipValue = Array.ConvertAll(DipValue, i => (DipValue)i.Clone());
            if (Entry is not null)
                obj.Entry = Array.ConvertAll(Entry, i => i);
            obj.Mask = Mask;
            obj.Name = Name;
            obj.Tag = Tag;

            return obj;
        }

        /// <inheritdoc/>
        public bool Equals(DipSwitch? other)
        {
            // Null never matches
            if (other is null)
                return false;

            // Properties
            if ((Mask is null) ^ (other.Mask is null))
                return false;
            else if (Mask is not null && !Mask.Equals(other.Mask, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((Name is null) ^ (other.Name is null))
                return false;
            else if (Name is not null && !Name.Equals(other.Name, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((Tag is null) ^ (other.Tag is null))
                return false;
            else if (Tag is not null && !Tag.Equals(other.Tag, StringComparison.OrdinalIgnoreCase))
                return false;

            // Sub-items
            if ((Condition is null) ^ (other.Condition is null))
                return false;
            else if (Condition is not null && other.Condition is not null && Condition.Equals(other.Condition))
                return false;

            // TODO: Figure out how to properly check arrays

            return true;
        }
    }
}
