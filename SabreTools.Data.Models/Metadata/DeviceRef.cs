using System;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("device_ref"), XmlRoot("device_ref")]
    public class DeviceRef : DatItem, ICloneable
    {
        #region Properties

        public string? Name { get; set; }

        #endregion

        public DeviceRef() => ItemType = ItemType.DeviceRef;

        /// <inheritdoc/>
        public object Clone()
        {
            var obj = new DeviceRef();

            obj.Name = Name;

            return obj;
        }
    }
}
