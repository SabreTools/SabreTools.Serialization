using System;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("instance"), XmlRoot("instance")]
    public class Instance : DatItem, ICloneable
    {
        #region Properties

        public string? BriefName { get; set; }

        public string? Name { get; set; }

        #endregion

        public Instance() => ItemType = ItemType.Instance;

        /// <inheritdoc/>
        public object Clone()
        {
            var obj = new Instance();

            obj.BriefName = BriefName;
            obj.Name = Name;

            return obj;
        }
    }
}
