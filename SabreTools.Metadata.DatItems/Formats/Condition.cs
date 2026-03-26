using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Metadata.DatItems.Formats
{
    /// <summary>
    /// Represents a condition on a machine or other item
    /// </summary>
    [JsonObject("condition"), XmlRoot("condition")]
    public sealed class Condition : DatItem<Data.Models.Metadata.Condition>
    {
        #region Fields

        /// <inheritdoc>/>
        protected override ItemType ItemType => ItemType.Condition;

        #endregion

        #region Constructors

        public Condition() : base() { }

        public Condition(Data.Models.Metadata.Condition item) : base(item)
        {
            // Process flag values
            if (ReadString(Data.Models.Metadata.Condition.RelationKey) is not null)
                Write<string?>(Data.Models.Metadata.Condition.RelationKey, ReadString(Data.Models.Metadata.Condition.RelationKey).AsRelation().AsStringValue());
        }

        public Condition(Data.Models.Metadata.Condition item, Machine machine, Source source) : this(item)
        {
            Write<Source?>(SourceKey, source);
            CopyMachineInformation(machine);
        }

        #endregion
    }
}
