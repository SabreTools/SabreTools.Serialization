using System;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("info"), XmlRoot("info")]
    public class Info : DatItem, ICloneable
    {
        #region Properties

        public string? Name { get; set; }

        public string? Value { get; set; }

        #endregion

        public Info() => ItemType = ItemType.Info;

        /// <inheritdoc/>
        public object Clone()
        {
            var obj = new Info();

            obj.Name = Name;
            obj.Value = Value;

            return obj;
        }
    }
}
