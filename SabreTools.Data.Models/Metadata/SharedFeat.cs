using System;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("sharedfeat"), XmlRoot("sharedfeat")]
    public class SharedFeat : DatItem, ICloneable
    {
        #region Properties

        public string? Name { get; set; }

        public string? Value { get; set; }

        #endregion

        public SharedFeat() => ItemType = ItemType.SharedFeat;

        /// <inheritdoc/>
        public object Clone()
        {
            var obj = new SharedFeat();

            obj.Name = Name;
            obj.Value = Value;

            return obj;
        }
    }
}
