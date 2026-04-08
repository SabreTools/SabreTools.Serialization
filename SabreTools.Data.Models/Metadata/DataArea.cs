using System;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("dataarea"), XmlRoot("dataarea")]
    public class DataArea : DatItem, ICloneable, IEquatable<DataArea>
    {
        #region Properties

        /// <remarks>(big|little) "little"</remarks>
        public Endianness? Endianness { get; set; }

        public string? Name { get; set; }

        public Rom[]? Rom { get; set; }

        public long? Size { get; set; }

        /// <remarks>(8|16|32|64) "8"</remarks>
        public Width? Width { get; set; }

        #endregion

        public DataArea() => ItemType = ItemType.DataArea;

        /// <inheritdoc/>
        public object Clone()
        {
            var obj = new DataArea();

            obj.Endianness = Endianness;
            obj.Name = Name;
            if (Rom is not null)
                obj.Rom = Array.ConvertAll(Rom, i => (Rom)i.Clone());
            obj.Size = Size;
            obj.Width = Width;

            return obj;
        }

        /// <inheritdoc/>
        public bool Equals(DataArea? other)
        {
            // Null never matches
            if (other is null)
                return false;

            // Properties
            if (Endianness != other.Endianness)
                return false;

            if ((Name is null) ^ (other.Name is null))
                return false;
            else if (Name is not null && !Name.Equals(other.Name, StringComparison.OrdinalIgnoreCase))
                return false;

            if (Size != other.Size)
                return false;

            if (Width != other.Width)
                return false;

            // Sub-items
            // TODO: Figure out how to properly check arrays

            return true;
        }
    }
}
