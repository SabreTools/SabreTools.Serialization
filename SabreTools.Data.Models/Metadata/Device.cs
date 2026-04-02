using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("device"), XmlRoot("device")]
    public class Device : DatItem
    {
        #region Properties

        /// <remarks>(unknown|cartridge|floppydisk|harddisk|cylinder|cassette|punchcard|punchtape|printout|serial|parallel|snapshot|quickload|memcard|cdrom|magtape|romimage|midiin|midiout|picture|vidfile)</remarks>
        public DeviceType? DeviceType { get; set; }

        public string? FixedImage { get; set; }

        public string? Interface { get; set; }

        /// <remarks>(0|1) "0"</remarks>
        public bool? Mandatory { get; set; }

        public string? Tag { get; set; }

        #endregion

        #region Keys

        /// <remarks>Extension[]</remarks>
        [NoFilter]
        public const string ExtensionKey = "extension";

        /// <remarks>Instance</remarks>
        [NoFilter]
        public const string InstanceKey = "instance";

        #endregion

        public Device() => ItemType = ItemType.Device;
    }
}
