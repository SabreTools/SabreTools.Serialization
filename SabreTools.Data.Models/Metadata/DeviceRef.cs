using System;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("device_ref"), XmlRoot("device_ref")]
    public class DeviceRef : DatItem, ICloneable, IEquatable<DeviceRef>
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

        /// <inheritdoc/>
        public bool Equals(DeviceRef? other)
        {
            // Null never matches
            if (other is null)
                return false;

            // Properties
            if ((Name is null) ^ (other.Name is null))
                return false;
            else if (Name is not null && !Name.Equals(other.Name, StringComparison.OrdinalIgnoreCase))
                return false;

            return true;
        }
    }
}
