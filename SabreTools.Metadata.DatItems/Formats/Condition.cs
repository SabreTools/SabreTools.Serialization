using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Data.Extensions;

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
            string? condition = ReadString(Data.Models.Metadata.Condition.RelationKey);
            if (condition is not null)
                Write<string?>(Data.Models.Metadata.Condition.RelationKey, condition.AsRelation()?.AsStringValue());
        }

        public Condition(Data.Models.Metadata.Condition item, Machine machine, Source source) : this(item)
        {
            Write<Source?>(SourceKey, source);
            CopyMachineInformation(machine);
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone() => new Condition(_internal.Clone() as Data.Models.Metadata.Condition ?? []);

        #endregion
    }
}
