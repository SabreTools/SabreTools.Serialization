using System;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("biosset"), XmlRoot("biosset")]
    public class BiosSet : DatItem, ICloneable
    {
        #region Properties

        /// <remarks>(yes|no) "no"</remarks>
        public bool? Default { get; set; }

        public string? Description { get; set; }

        public string? Name { get; set; }

        #endregion

        public BiosSet() => ItemType = ItemType.BiosSet;

        /// <inheritdoc/>
        public object Clone()
        {
            var obj = new BiosSet();

            obj.Default = Default;
            obj.Description = Description;
            obj.Name = Name;

            return obj;
        }
    }
}
