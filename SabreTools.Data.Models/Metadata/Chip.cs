using System;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("chip"), XmlRoot("chip")]
    public class Chip : DatItem, ICloneable, IEquatable<Chip>
    {
        #region Properties

        /// <remarks>(cpu|audio)</remarks>
        public ChipType? ChipType { get; set; }

        public long? Clock { get; set; }

        public string? Flags { get; set; }

        public string? Name { get; set; }

        /// <remarks>(yes|no) "no"</remarks>
        public bool? SoundOnly { get; set; }

        public string? Tag { get; set; }

        #endregion

        public Chip() => ItemType = ItemType.Chip;

        /// <inheritdoc/>
        public object Clone()
        {
            var obj = new Chip();

            obj.ChipType = ChipType;
            obj.Clock = Clock;
            obj.Flags = Flags;
            obj.Name = Name;
            obj.SoundOnly = SoundOnly;
            obj.Tag = Tag;

            return obj;
        }

        /// <inheritdoc/>
        public bool Equals(Chip? other)
        {
            // Null never matches
            if (other is null)
                return false;

            // Properties
            if (ChipType != other.ChipType)
                return false;

            if (Clock != other.Clock)
                return false;

            if ((Flags is null) ^ (other.Flags is null))
                return false;
            else if (Flags is not null && !Flags.Equals(other.Flags, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((Name is null) ^ (other.Name is null))
                return false;
            else if (Name is not null && !Name.Equals(other.Name, StringComparison.OrdinalIgnoreCase))
                return false;

            if (SoundOnly != other.SoundOnly)
                return false;

            if ((Tag is null) ^ (other.Tag is null))
                return false;
            else if (Tag is not null && !Tag.Equals(other.Tag, StringComparison.OrdinalIgnoreCase))
                return false;

            return true;
        }
    }
}
