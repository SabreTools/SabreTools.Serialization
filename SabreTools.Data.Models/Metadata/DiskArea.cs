using System;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("diskarea"), XmlRoot("diskarea")]
    public class DiskArea : DatItem, ICloneable, IEquatable<DiskArea>
    {
        #region Properties

        public Disk[]? Disk { get; set; }

        public string? Name { get; set; }

        #endregion

        public DiskArea() => ItemType = ItemType.DiskArea;

        /// <inheritdoc/>
        public object Clone()
        {
            var obj = new DiskArea();

            if (Disk is not null)
                obj.Disk = Array.ConvertAll(Disk, i => (Disk)i.Clone());
            obj.Name = Name;

            return obj;
        }

        /// <inheritdoc/>
        public bool Equals(DiskArea? other)
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
