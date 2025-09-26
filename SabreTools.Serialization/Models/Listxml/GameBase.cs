using System.Xml;
using System.Xml.Serialization;
using SabreTools.Data.Attributes;

namespace SabreTools.Data.Models.Listxml
{
    /// <summary>
    /// Base class to unify the various game-like types
    /// </summary>
    public abstract class GameBase
    {
        [Required]
        [XmlAttribute("name")]
        public string? Name { get; set; }

        /// <remarks>Machine only</remarks>
        [XmlAttribute("sourcefile")]
        public string? SourceFile { get; set; }

        /// <remarks>(yes|no) "no", Machine only</remarks>
        [XmlAttribute("isbios")]
        public string? IsBios { get; set; }

        /// <remarks>(yes|no) "no", Machine only</remarks>
        [XmlAttribute("isdevice")]
        public string? IsDevice { get; set; }

        /// <remarks>(yes|no) "no", Machine only</remarks>
        [XmlAttribute("ismechanical")]
        public string? IsMechanical { get; set; }

        /// <remarks>(yes|no) "no"</remarks>
        [XmlAttribute("runnable")]
        public string? Runnable { get; set; }

        [XmlAttribute("cloneof")]
        public string? CloneOf { get; set; }

        [XmlAttribute("romof")]
        public string? RomOf { get; set; }

        [XmlAttribute("sampleof")]
        public string? SampleOf { get; set; }

        [Required]
        [XmlElement("description")]
        public string? Description { get; set; }

        [XmlElement("year")]
        public string? Year { get; set; }

        [XmlElement("manufacturer")]
        public string? Manufacturer { get; set; }

        /// <remarks>Game only</remarks>
        [XmlElement("history")]
        public string? History { get; set; }

        [XmlElement("biosset")]
        public BiosSet[]? BiosSet { get; set; }

        [XmlElement("rom")]
        public Rom[]? Rom { get; set; }

        [XmlElement("disk")]
        public Disk[]? Disk { get; set; }

        [XmlElement("device_ref")]
        public DeviceRef[]? DeviceRef { get; set; }

        [XmlElement("sample")]
        public Sample[]? Sample { get; set; }

        [XmlElement("chip")]
        public Chip[]? Chip { get; set; }

        [XmlElement("display")]
        public Display[]? Display { get; set; }

        /// <remarks>Only present in older versions</remarks>
        [XmlElement("video")]
        public Video[]? Video { get; set; }

        [XmlElement("sound")]
        public Sound? Sound { get; set; }

        [XmlElement("input")]
        public Input? Input { get; set; }

        [XmlElement("dipswitch")]
        public DipSwitch[]? DipSwitch { get; set; }

        [XmlElement("configuration")]
        public Configuration[]? Configuration { get; set; }

        [XmlElement("port")]
        public Port[]? Port { get; set; }

        [XmlElement("adjuster")]
        public Adjuster[]? Adjuster { get; set; }

        [XmlElement("driver")]
        public Driver? Driver { get; set; }

        [XmlElement("feature")]
        public Feature[]? Feature { get; set; }

        [XmlElement("device")]
        public Device[]? Device { get; set; }

        [XmlElement("slot")]
        public Slot[]? Slot { get; set; }

        [XmlElement("softwarelist")]
        public SoftwareList[]? SoftwareList { get; set; }

        [XmlElement("ramoption")]
        public RamOption[]? RamOption { get; set; }
    }
}