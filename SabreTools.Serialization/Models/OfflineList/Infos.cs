using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Data.Models.OfflineList
{
    [XmlRoot("infos")]
    public class Infos
    {
        [XmlElement("title")]
        public Title? Title { get; set; }

        [XmlElement("location")]
        public Location? Location { get; set; }

        [XmlElement("publisher")]
        public Publisher? Publisher { get; set; }

        [XmlElement("sourceRom")]
        public SourceRom? SourceRom { get; set; }

        [XmlElement("saveType")]
        public SaveType? SaveType { get; set; }

        [XmlElement("romSize")]
        public RomSize? RomSize { get; set; }

        [XmlElement("releaseNumber")]
        public ReleaseNumber? ReleaseNumber { get; set; }

        [XmlElement("imageNumber")]
        public ImageNumber? ImageNumber { get; set; }

        [XmlElement("languageNumber")]
        public LanguageNumber? LanguageNumber { get; set; }

        [XmlElement("comment")]
        public Comment? Comment { get; set; }

        [XmlElement("romCRC")]
        public RomCRC? RomCRC { get; set; }

        [XmlElement("im1CRC")]
        public Im1CRC? Im1CRC { get; set; }

        [XmlElement("im2CRC")]
        public Im2CRC? Im2CRC { get; set; }

        [XmlElement("languages")]
        public Languages? Languages { get; set; }
    }
}