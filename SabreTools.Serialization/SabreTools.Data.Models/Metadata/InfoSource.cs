using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("infosource"), XmlRoot("infosource")]
    public class InfoSource : DictionaryBase
    {
        #region Keys

        /// <remarks>string[]</remarks>
        public const string SourceKey = "source";

        #endregion
    }
}
