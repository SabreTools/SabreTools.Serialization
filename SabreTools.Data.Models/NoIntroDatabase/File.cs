using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Data.Models.NoIntroDatabase
{
    [XmlRoot("file")]
    public class File
    {
        [XmlAttribute("id")]
        public string? Id { get; set; }

        [XmlAttribute("append_to_source_id")]
        public string? AppendToSourceId { get; set; }

        [XmlAttribute("forcename")]
        public string? ForceName { get; set; }

        [XmlAttribute("forcescenename")]
        public string? ForceSceneName { get; set; }

        /// <remarks>byte</remarks>
        [XmlAttribute("emptydir")]
        public string? EmptyDir { get; set; }

        [XmlAttribute("extension")]
        public string? Extension { get; set; }

        [XmlAttribute("item")]
        public string? Item { get; set; }

        [XmlAttribute("date")]
        public string? Date { get; set; }

        [XmlAttribute("format")]
        public string? Format { get; set; }

        [XmlAttribute("note")]
        public string? Note { get; set; }

        [XmlAttribute("filter")]
        public string? Filter { get; set; }

        [XmlAttribute("version")]
        public string? Version { get; set; }

        [XmlAttribute("update_type")]
        public string? UpdateType { get; set; }

        [XmlAttribute("size")]
        public string? Size { get; set; }

        [XmlAttribute("crc32")]
        public string? CRC32 { get; set; }

        [XmlAttribute("md5")]
        public string? MD5 { get; set; }

        [XmlAttribute("sha1")]
        public string? SHA1 { get; set; }

        [XmlAttribute("sha256")]
        public string? SHA256 { get; set; }

        [XmlAttribute("serial")]
        public string? Serial { get; set; }

        [XmlAttribute("header")]
        public string? Header { get; set; }

        /// <remarks>byte</remarks>
        [XmlAttribute("bad")]
        public string? Bad { get; set; }

        /// <remarks>byte</remarks>
        [XmlAttribute("mia")]
        public string? MIA { get; set; }

        /// <remarks>byte</remarks>
        [XmlAttribute("unique")]
        public string? Unique { get; set; }

        [XmlAttribute("mergename")]
        public string? MergeName { get; set; }

        [XmlAttribute("unique_attachment")]
        public string? UniqueAttachment { get; set; }
    }
}
