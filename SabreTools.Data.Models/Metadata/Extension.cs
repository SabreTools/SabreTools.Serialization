using System;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("extension"), XmlRoot("extension")]
    public class Extension : DatItem, ICloneable
    {
        #region Properties

        public string? Name { get; set; }

        #endregion

        public Extension() => ItemType = ItemType.Extension;

        /// <inheritdoc/>
        public object Clone()
        {
            var obj = new Extension();

            obj.Name = Name;

            return obj;
        }
    }
}
