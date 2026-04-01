using System.Xml.Serialization;

namespace SabreTools.Data.Models.NoIntroDatabase
{
    [XmlRoot("serials")]
    public class Serials
    {
        [XmlAttribute("media_serial1")]
        public string? MediaSerial1 { get; set; }

        [XmlAttribute("media_serial2")]
        public string? MediaSerial2 { get; set; }

        [XmlAttribute("media_serial3")]
        public string? MediaSerial3 { get; set; }

        [XmlAttribute("pcb_serial")]
        public string? PCBSerial { get; set; }

        [XmlAttribute("romchip_serial1")]
        public string? RomChipSerial1 { get; set; }

        [XmlAttribute("romchip_serial2")]
        public string? RomChipSerial2 { get; set; }

        [XmlAttribute("lockout_serial")]
        public string? LockoutSerial { get; set; }

        [XmlAttribute("savechip_serial")]
        public string? SaveChipSerial { get; set; }

        [XmlAttribute("chip_serial")]
        public string? ChipSerial { get; set; }

        [XmlAttribute("box_serial")]
        public string? BoxSerial { get; set; }

        [XmlAttribute("mediastamp")]
        public string? MediaStamp { get; set; }

        [XmlAttribute("box_barcode")]
        public string? BoxBarcode { get; set; }

        [XmlAttribute("digital_serial1")]
        public string? DigitalSerial1 { get; set; }

        [XmlAttribute("digital_serial2")]
        public string? DigitalSerial2 { get; set; }
    }
}
