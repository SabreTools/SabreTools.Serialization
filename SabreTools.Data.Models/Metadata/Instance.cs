using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("instance"), XmlRoot("instance")]
    public class Instance : DatItem
    {
        #region Properties

        public string? BriefName { get; set; }

        public string? Name { get; set; }

        #endregion

        public Instance() => ItemType = ItemType.Instance;
    }
}
