using System;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("slotoption"), XmlRoot("slotoption")]
    public class SlotOption : DatItem, ICloneable
    {
        #region Properties

        /// <remarks>(yes|no) "no"</remarks>
        public bool? Default { get; set; }

        public string? DevName { get; set; }

        public string? Name { get; set; }

        #endregion

        public SlotOption() => ItemType = ItemType.SlotOption;

        /// <inheritdoc/>
        public object Clone()
        {
            var obj = new SlotOption();

            obj.Default = Default;
            obj.DevName = DevName;
            obj.Name = Name;

            return obj;
        }
    }
}
