using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("sound"), XmlRoot("sound")]
    public class Sound : DatItem
    {
        #region Properties

        public long? Channels { get; set; }

        #endregion

        public Sound() => ItemType = ItemType.Sound;
    }
}
