using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Serialization.Models.OfflineList
{
    [XmlRoot("configuration")]
    public class Configuration
    {
        [XmlElement("datName")]
        public string? DatName { get; set; }

        [XmlElement("imFolder")]
        public string? ImFolder { get; set; }

        [XmlElement("datVersion")]
        public string? DatVersion { get; set; }

        [XmlElement("system")]
        public string? System { get; set; }

        /// <remarks>Numeric?</remarks>
        [XmlElement("screenshotsWidth")]
        public string? ScreenshotsWidth { get; set; }

        /// <remarks>Numeric?</remarks>
        [XmlElement("screenshotsHeight")]
        public string? ScreenshotsHeight { get; set; }

        [XmlElement("infos")]
        public Infos? Infos { get; set; }

        [XmlElement("canOpen")]
        public CanOpen? CanOpen { get; set; }

        [XmlElement("newDat")]
        public NewDat? NewDat { get; set; }

        [XmlElement("search")]
        public Search? Search { get; set; }

        [XmlElement("romTitle")]
        public string? RomTitle { get; set; }
    }
}