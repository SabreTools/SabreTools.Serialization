using System;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    // TODO: IEquatable<Media>
    [JsonObject("media"), XmlRoot("media")]
    public class Media : DatItem, ICloneable
    {
        #region Properties

        public string? MD5 { get; set; }

        public string? Name { get; set; }

        public string? SHA1 { get; set; }

        public string? SHA256 { get; set; }

        public string? SpamSum { get; set; }

        #endregion

        public Media() => ItemType = ItemType.Media;

        /// <inheritdoc/>
        public object Clone()
        {
            var obj = new Media();

            obj.MD5 = MD5;
            obj.Name = Name;
            obj.SHA1 = SHA1;
            obj.SHA256 = SHA256;
            obj.SpamSum = SpamSum;

            return obj;
        }
    }
}
