using System;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("blank"), XmlRoot("blank")]
    public class Blank : DatItem, ICloneable, IEquatable<Blank>
    {
        public Blank() => ItemType = ItemType.Blank;

        /// <inheritdoc/>
        public object Clone() => new Blank();

        /// <inheritdoc/>
        public bool Equals(Blank? other) => other is not null;
    }
}
