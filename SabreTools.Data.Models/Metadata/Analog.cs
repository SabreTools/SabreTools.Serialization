using System;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("analog"), XmlRoot("analog")]
    public class Analog : DatItem, ICloneable
    {
        #region Properties

        public string? Mask { get; set; }

        #endregion

        public Analog() => ItemType = ItemType.Analog;

        /// <inheritdoc/>
        public object Clone()
        {
            var obj = new Analog();

            obj.Mask = Mask;

            return obj;
        }
    }
}
