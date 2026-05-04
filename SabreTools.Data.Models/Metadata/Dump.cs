using System;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("dump"), XmlRoot("dump")]
    public class Dump : DatItem, ICloneable, IEquatable<Dump>
    {
        #region Properties

        public string? Boot { get; set; }

        public Rom? MegaRom { get; set; }

        public Original? Original { get; set; }

        public Rom? Rom { get; set; }

        public Rom? SCCPlusCart { get; set; }

        #endregion

        public Dump() => ItemType = ItemType.Dump;

        /// <inheritdoc/>
        public object Clone()
        {
            var obj = new Dump();

            obj.Boot = Boot;
            if (MegaRom is not null)
                obj.MegaRom = MegaRom.Clone() as Rom;
            if (Original is not null)
                obj.Original = Original.Clone() as Original;
            if (Rom is not null)
                obj.Rom = Rom.Clone() as Rom;
            if (SCCPlusCart is not null)
                obj.SCCPlusCart = SCCPlusCart.Clone() as Rom;

            return obj;
        }

        /// <inheritdoc/>
        public bool Equals(Dump? other)
        {
            // Null never matches
            if (other is null)
                return false;

            if ((Boot is null) ^ (other.Boot is null))
                return false;
            else if (Boot is not null && !Boot.Equals(other.Boot, StringComparison.OrdinalIgnoreCase))
                return false;

            // Sub-items
            if ((MegaRom is null) ^ (other.MegaRom is null))
                return false;
            else if (MegaRom is not null && other.MegaRom is not null && MegaRom.Equals(other.MegaRom))
                return false;

            if ((Original is null) ^ (other.Original is null))
                return false;
            else if (Original is not null && other.Original is not null && Original.Equals(other.Original))
                return false;

            if ((Rom is null) ^ (other.Rom is null))
                return false;
            else if (Rom is not null && other.Rom is not null && Rom.Equals(other.Rom))
                return false;

            if ((SCCPlusCart is null) ^ (other.SCCPlusCart is null))
                return false;
            else if (SCCPlusCart is not null && other.SCCPlusCart is not null && SCCPlusCart.Equals(other.SCCPlusCart))
                return false;

            return true;
        }
    }
}
