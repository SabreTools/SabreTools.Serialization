using System;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("device"), XmlRoot("device")]
    public class Device : DatItem, ICloneable, IEquatable<Device>
    {
        #region Properties

        /// <remarks>(unknown|cartridge|floppydisk|harddisk|cylinder|cassette|punchcard|punchtape|printout|serial|parallel|snapshot|quickload|memcard|cdrom|magtape|romimage|midiin|midiout|picture|vidfile)</remarks>
        public DeviceType? DeviceType { get; set; }

        /// <remarks>Extension subitem</remarks>
        public string[]? ExtensionName { get; set; }

        public string? FixedImage { get; set; }

        public string? InstanceBriefName { get; set; }

        public string? InstanceName { get; set; }

        public string? Interface { get; set; }

        /// <remarks>(0|1) "0"</remarks>
        public bool? Mandatory { get; set; }

        public string? Tag { get; set; }

        #endregion

        public Device() => ItemType = ItemType.Device;

        /// <inheritdoc/>
        public object Clone()
        {
            var obj = new Device();

            obj.DeviceType = DeviceType;
            if (ExtensionName is not null)
                obj.ExtensionName = Array.ConvertAll(ExtensionName, i => i);
            obj.FixedImage = FixedImage;
            obj.InstanceBriefName = InstanceBriefName;
            obj.InstanceName = InstanceName;
            obj.Interface = Interface;
            obj.Mandatory = Mandatory;
            obj.Tag = Tag;

            return obj;
        }

        /// <inheritdoc/>
        public bool Equals(Device? other)
        {
            // Null never matches
            if (other is null)
                return false;

            // Properties
            if (DeviceType != other.DeviceType)
                return false;

            if ((FixedImage is null) ^ (other.FixedImage is null))
                return false;
            else if (FixedImage is not null && !FixedImage.Equals(other.FixedImage, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((InstanceBriefName is null) ^ (other.InstanceBriefName is null))
                return false;
            else if (InstanceBriefName is not null && !InstanceBriefName.Equals(other.InstanceBriefName, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((InstanceName is null) ^ (other.InstanceName is null))
                return false;
            else if (InstanceName is not null && !InstanceName.Equals(other.InstanceName, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((Interface is null) ^ (other.Interface is null))
                return false;
            else if (Interface is not null && !Interface.Equals(other.Interface, StringComparison.OrdinalIgnoreCase))
                return false;

            if (Mandatory != other.Mandatory)
                return false;

            if ((Tag is null) ^ (other.Tag is null))
                return false;
            else if (Tag is not null && !Tag.Equals(other.Tag, StringComparison.OrdinalIgnoreCase))
                return false;

            // TODO: Figure out how to properly check arrays

            return true;
        }
    }
}
