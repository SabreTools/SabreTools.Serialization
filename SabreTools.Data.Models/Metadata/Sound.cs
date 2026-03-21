using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("sound"), XmlRoot("sound")]
    public class Sound : DatItem
    {
        #region Keys

        /// <remarks>long</remarks>
        public const string ChannelsKey = "channels";

        #endregion

        public Sound() => Type = ItemType.Sound;
    }
}
