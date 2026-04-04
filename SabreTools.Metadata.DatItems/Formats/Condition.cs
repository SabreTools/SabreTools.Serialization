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
        public override Data.Models.Metadata.ItemType ItemType
            => Data.Models.Metadata.ItemType.Condition;

        public string? Mask
        {
            get => (_internal as Data.Models.Metadata.Condition)?.Mask;
            set => (_internal as Data.Models.Metadata.Condition)?.Mask = value;
        }

        public Data.Models.Metadata.Relation? Relation
        {
            get => (_internal as Data.Models.Metadata.Condition)?.Relation;
            set => (_internal as Data.Models.Metadata.Condition)?.Relation = value;
        }

        public string? Tag
        {
            get => (_internal as Data.Models.Metadata.Condition)?.Tag;
            set => (_internal as Data.Models.Metadata.Condition)?.Tag = value;
        }

        public string? Value
        {
            get => (_internal as Data.Models.Metadata.Condition)?.Value;
            set => (_internal as Data.Models.Metadata.Condition)?.Value = value;
        }

        #endregion

        #region Constructors

        public Condition() : base() { }

        public Condition(Data.Models.Metadata.Condition item) : base(item) { }

        public Condition(Data.Models.Metadata.Condition item, Machine machine, Source source) : this(item)
        {
            Source = source;
            CopyMachineInformation(machine);
        }

        #endregion

        #region Accessors

        /// <inheritdoc/>
        public override string? GetName() => null;

        /// <inheritdoc/>
        public override void SetName(string? name) { }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone() => new Condition(_internal.DeepClone() as Data.Models.Metadata.Condition ?? []);

        #endregion
    }
}
