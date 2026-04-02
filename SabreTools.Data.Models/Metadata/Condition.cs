using System;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("condition"), XmlRoot("condition")]
    public class Condition : DatItem, ICloneable
    {
        #region Properties

        public string? Mask { get; set; }

        /// <remarks>(eq|ne|gt|le|lt|ge)</remarks>
        public Relation? Relation { get; set; }

        public string? Tag { get; set; }

        public string? Value { get; set; }

        #endregion

        public Condition() => ItemType = ItemType.Condition;

        /// <inheritdoc/>
        public object Clone()
        {
            var obj = new Condition();

            obj.Mask = Mask;
            obj.Relation = Relation;
            obj.Tag = Tag;
            obj.Value = Value;

            return obj;
        }
    }
}
