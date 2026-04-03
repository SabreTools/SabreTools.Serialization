using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    // TODO: ICloneable
    // TODO: IEquatable
    [JsonObject("media"), XmlRoot("media")]
    public class Media : DatItem
    {
        #region Properties

        public string? MD5 { get; set; }

        public string? Name { get; set; }

        public string? SHA1 { get; set; }

        public string? SHA256 { get; set; }

        public string? SpamSum { get; set; }

        #endregion

        public Media() => ItemType = ItemType.Media;
    }
}
