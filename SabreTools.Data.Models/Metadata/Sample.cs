using System;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("sample"), XmlRoot("sample")]
    public class Sample : DatItem, ICloneable
    {
        #region Properties

        public string? Name { get; set; }

        #endregion

        public Sample() => ItemType = ItemType.Sample;

        /// <inheritdoc/>
        public object Clone()
        {
            var obj = new Sample();

            obj.Name = Name;

            return obj;
        }
    }
}
