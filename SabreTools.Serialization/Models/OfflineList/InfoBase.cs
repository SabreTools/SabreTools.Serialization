using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Data.Models.OfflineList
{
    public class InfoBase
    {
        /// <remarks>Boolean</remarks>
        [XmlAttribute("visible")]
        public string? Visible { get; set; }

        /// <remarks>Boolean</remarks>
        [XmlAttribute("inNamingOption")]
        public string? InNamingOption { get; set; }

        /// <remarks>Boolean</remarks>
        [XmlAttribute("default")]
        public string? Default { get; set; }
    }

    [XmlRoot("title")]
    public class Title : InfoBase { }

    [XmlRoot("location")]
    public class Location : InfoBase { }

    [XmlRoot("publisher")]
    public class Publisher : InfoBase { }

    [XmlRoot("sourceRom")]
    public class SourceRom : InfoBase { }

    [XmlRoot("saveType")]
    public class SaveType : InfoBase { }

    [XmlRoot("romSize")]
    public class RomSize : InfoBase { }

    [XmlRoot("releaseNumber")]
    public class ReleaseNumber : InfoBase { }

    [XmlRoot("imageNumber")]
    public class ImageNumber : InfoBase { }

    [XmlRoot("languageNumber")]
    public class LanguageNumber : InfoBase { }

    [XmlRoot("comment")]
    public class Comment : InfoBase { }

    [XmlRoot("romCRC")]
    public class RomCRC : InfoBase { }

    [XmlRoot("im1CRC")]
    public class Im1CRC : InfoBase { }

    [XmlRoot("im2CRC")]
    public class Im2CRC : InfoBase { }

    [XmlRoot("languages")]
    public class Languages : InfoBase { }
}