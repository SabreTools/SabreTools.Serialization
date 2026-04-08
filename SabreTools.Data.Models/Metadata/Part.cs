using System;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("part"), XmlRoot("part")]
    public class Part : DatItem, ICloneable, IEquatable<Part>
    {
        #region Properties

        public DataArea[]? DataArea { get; set; }

        public DiskArea[]? DiskArea { get; set; }

        public DipSwitch[]? DipSwitch { get; set; }

        public Feature[]? Feature { get; set; }

        public string? Interface { get; set; }

        public string? Name { get; set; }

        #endregion

        public Part() => ItemType = ItemType.Part;

        /// <inheritdoc/>
        public object Clone()
        {
            var obj = new Part();

            if (DataArea is not null)
                obj.DataArea = Array.ConvertAll(DataArea, i => (DataArea)i.Clone());
            if (DiskArea is not null)
                obj.DiskArea = Array.ConvertAll(DiskArea, i => (DiskArea)i.Clone());
            if (DipSwitch is not null)
                obj.DipSwitch = Array.ConvertAll(DipSwitch, i => (DipSwitch)i.Clone());
            if (Feature is not null)
                obj.Feature = Array.ConvertAll(Feature, i => (Feature)i.Clone());
            obj.Interface = Interface;
            obj.Name = Name;

            return obj;
        }

        /// <inheritdoc/>
        public bool Equals(Part? other)
        {
            // Null never matches
            if (other is null)
                return false;

            // Properties
            if ((Interface is null) ^ (other.Interface is null))
                return false;
            else if (Interface is not null && !Interface.Equals(other.Interface, StringComparison.OrdinalIgnoreCase))
                return false;

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
