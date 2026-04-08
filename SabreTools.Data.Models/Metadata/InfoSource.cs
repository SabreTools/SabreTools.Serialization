using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("infosource"), XmlRoot("infosource")]
    public class InfoSource
    {
        public string[]? Source { get; set; }
    }
}
