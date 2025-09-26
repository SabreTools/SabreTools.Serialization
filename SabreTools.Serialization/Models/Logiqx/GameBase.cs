using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Serialization.Models.Logiqx
{
    /// <summary>
    /// Base class to unify the various game-like types
    /// </summary>
    public abstract class GameBase
    {
        [SabreTools.Models.Required]
        [XmlAttribute("name")]
        public string? Name { get; set; }

        [XmlAttribute("sourcefile")]
        public string? SourceFile { get; set; }

        /// <remarks>(yes|no) "no"</remarks>
        [XmlAttribute("isbios")]
        public string? IsBios { get; set; }

        /// <remarks>(yes|no) "no", MAME extension</remarks>
        [XmlAttribute("isdevice")]
        public string? IsDevice { get; set; }

        /// <remarks>(yes|no) "no", MAME extension</remarks>
        [XmlAttribute("ismechanical")]
        public string? IsMechanical { get; set; }

        [XmlAttribute("cloneof")]
        public string? CloneOf { get; set; }

        [XmlAttribute("romof")]
        public string? RomOf { get; set; }

        [XmlAttribute("sampleof")]
        public string? SampleOf { get; set; }

        [XmlAttribute("board")]
        public string? Board { get; set; }

        [XmlAttribute("rebuildto")]
        public string? RebuildTo { get; set; }

        /// <remarks>No-Intro extension</remarks>
        [XmlAttribute("id")]
        public string? Id { get; set; }

        /// <remarks>No-Intro extension</remarks>
        [XmlAttribute("cloneofid")]
        public string? CloneOfId { get; set; }

        /// <remarks>(no|partial|yes) "no"</remarks>
        [XmlAttribute("runnable")]
        public string? Runnable { get; set; }

        [XmlElement("comment")]
        public string[]? Comment { get; set; }

        [SabreTools.Models.Required]
        [XmlElement("description")]
        public string? Description { get; set; }

        [XmlElement("year")]
        public string? Year { get; set; }

        [XmlElement("manufacturer")]
        public string? Manufacturer { get; set; }

        [XmlElement("publisher")]
        public string? Publisher { get; set; }

        /// <remarks>No-Intro extension includes more than 1 instance</remarks>
        [XmlElement("category")]
        public string[]? Category { get; set; }

        /// <remarks>Trurip extension</remarks>
        [XmlElement("trurip")]
        public Trurip? Trurip { get; set; }

        [XmlElement(elementName: "release")]
        public Release[]? Release { get; set; }

        [XmlElement("biosset")]
        public BiosSet[]? BiosSet { get; set; }

        [XmlElement("rom")]
        public Rom[]? Rom { get; set; }

        [XmlElement("disk")]
        public Disk[]? Disk { get; set; }

        /// <remarks>Aaru extension</remarks>
        [XmlElement("media")]
        public Media[]? Media { get; set; }

        /// <remarks>MAME extension</remarks>
        [XmlElement("device_ref")]
        public DeviceRef[]? DeviceRef { get; set; }

        [XmlElement("sample")]
        public Sample[]? Sample { get; set; }

        [XmlElement("archive")]
        public Archive[]? Archive { get; set; }

        /// <remarks>MAME extension</remarks>
        [XmlElement("driver")]
        public Driver? Driver { get; set; }

        /// <remarks>MAME extension</remarks>
        [XmlElement("softwarelist")]
        public SoftwareList[]? SoftwareList { get; set; }

        /// <remarks>RetroAchievements extension</remarks>
        [XmlAttribute("url")]
        public string? Url { get; set; }

        /// <remarks>RetroAchievements extension</remarks>
        [XmlAttribute("hash")]
        public string? Hash { get; set; }
    }
}