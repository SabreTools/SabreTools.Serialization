using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Data.Extensions;

namespace SabreTools.Metadata.DatItems.Formats
{
    /// <summary>
    /// Represents one ListXML dipvalue
    /// </summary>
    [JsonObject("dipvalue"), XmlRoot("dipvalue")]
    public sealed class DipValue : DatItem<Data.Models.Metadata.DipValue>
    {
        #region Fields

        [JsonIgnore]
        public bool ConditionsSpecified
        {
            get
            {
                var conditions = Read<Condition[]?>(Data.Models.Metadata.DipValue.ConditionKey);
                return conditions is not null && conditions.Length > 0;
            }
        }

        public bool? Default
        {
            get => (_internal as Data.Models.Metadata.DipValue)?.Default;
            set => (_internal as Data.Models.Metadata.DipValue)?.Default = value;
        }

        /// <inheritdoc>/>
        public override Data.Models.Metadata.ItemType ItemType
            => Data.Models.Metadata.ItemType.DipValue;

        public string? Name
        {
            get => (_internal as Data.Models.Metadata.DipValue)?.Name;
            set => (_internal as Data.Models.Metadata.DipValue)?.Name = value;
        }

        public string? Value
        {
            get => (_internal as Data.Models.Metadata.DipValue)?.Value;
            set => (_internal as Data.Models.Metadata.DipValue)?.Value = value;
        }

        #endregion

        #region Constructors

        public DipValue() : base() { }

        public DipValue(Data.Models.Metadata.DipValue item) : base(item)
        {
            // Handle subitems
            var condition = Read<Data.Models.Metadata.Condition>(Data.Models.Metadata.DipValue.ConditionKey);
            if (condition is not null)
                Write<Condition?>(Data.Models.Metadata.DipValue.ConditionKey, new Condition(condition));
        }

        public DipValue(Data.Models.Metadata.DipValue item, Machine machine, Source source) : this(item)
        {
            Source = source;
            CopyMachineInformation(machine);
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone() => new DipValue(_internal.DeepClone() as Data.Models.Metadata.DipValue ?? []);

        /// <inheritdoc/>
        public override Data.Models.Metadata.DipValue GetInternalClone()
        {
            var dipValueItem = base.GetInternalClone();

            // Handle subitems
            var subCondition = Read<Condition>(Data.Models.Metadata.DipValue.ConditionKey);
            if (subCondition is not null)
                dipValueItem[Data.Models.Metadata.DipValue.ConditionKey] = subCondition.GetInternalClone();

            return dipValueItem;
        }

        #endregion
    }
}
