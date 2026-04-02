using System;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("feature"), XmlRoot("feature")]
    public class Feature : DatItem, ICloneable
    {
        #region Properties

        /// <remarks>(protection|timing|graphics|palette|sound|capture|camera|microphone|controls|keyboard|mouse|media|disk|printer|tape|punch|drum|rom|comms|lan|wan)</remarks>
        public FeatureType? FeatureType { get; set; }

        public string? Name { get; set; }

        /// <remarks>(unemulated|imperfect)</remarks>
        public FeatureStatus? Overall { get; set; }

        /// <remarks>(unemulated|imperfect)</remarks>
        public FeatureStatus? Status { get; set; }

        public string? Value { get; set; }

        #endregion

        public Feature() => ItemType = ItemType.Feature;

        /// <inheritdoc/>
        public object Clone()
        {
            var obj = new Feature();

            obj.FeatureType = FeatureType;
            obj.Name = Name;
            obj.Overall = Overall;
            obj.Status = Status;
            obj.Value = Value;

            return obj;
        }
    }
}
