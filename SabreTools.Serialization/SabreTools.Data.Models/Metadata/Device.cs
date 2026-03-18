using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Data.Attributes;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("device"), XmlRoot("device")]
    public class Device : DatItem
    {
        #region Keys

        /// <remarks>Extension[]</remarks>
        [NoFilter]
        public const string ExtensionKey = "extension";

        /// <remarks>string</remarks>
        public const string FixedImageKey = "fixed_image";

        /// <remarks>Instance</remarks>
        [NoFilter]
        public const string InstanceKey = "instance";

        /// <remarks>string</remarks>
        public const string InterfaceKey = "interface";

        /// <remarks>(0|1) "0"</remarks>
        public const string MandatoryKey = "mandatory";

        /// <remarks>string</remarks>
        public const string TagKey = "tag";

        /// <remarks>(unknown|cartridge|floppydisk|harddisk|cylinder|cassette|punchcard|punchtape|printout|serial|parallel|snapshot|quickload|memcard|cdrom|magtape|romimage|midiin|midiout|picture|vidfile)</remarks>
        public const string DeviceTypeKey = "type";

        #endregion

        public Device() => Type = ItemType.Device;
    }
}
