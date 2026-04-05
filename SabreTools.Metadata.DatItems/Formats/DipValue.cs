using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Metadata.DatItems.Formats
{
    /// <summary>
    /// Represents one ListXML dipvalue
    /// </summary>
    [JsonObject("dipvalue"), XmlRoot("dipvalue")]
    public sealed class DipValue : DatItem<Data.Models.Metadata.DipValue>
    {
        #region Properties

        public Condition? Condition { get; set; }

        [JsonIgnore]
        public bool ConditionSpecified => Condition is not null;

        public bool? Default
        {
            get => _internal.Default;
            set => _internal.Default = value;
        }

        /// <inheritdoc>/>
        public override Data.Models.Metadata.ItemType ItemType
            => Data.Models.Metadata.ItemType.DipValue;

        public string? Name
        {
            get => _internal.Name;
            set => _internal.Name = value;
        }

        public string? Value
        {
            get => _internal.Value;
            set => _internal.Value = value;
        }

        #endregion

        #region Constructors

        public DipValue() : base() { }

        public DipValue(Data.Models.Metadata.DipValue item) : base(item)
        {
            // Handle subitems
            if (item.Condition is not null)
                Condition = new Condition(item.Condition);
        }

        public DipValue(Data.Models.Metadata.DipValue item, Machine machine, Source source) : this(item)
        {
            Source = source;
            CopyMachineInformation(machine);
        }

        #endregion

        #region Accessors

        /// <inheritdoc/>
        public override string? GetName() => Name;

        /// <inheritdoc/>
        public override void SetName(string? name) => Name = name;

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone() => new DipValue(GetInternalClone());

        /// <inheritdoc/>
        public override Data.Models.Metadata.DipValue GetInternalClone()
        {
            var dipValueItem = _internal.Clone() as Data.Models.Metadata.DipValue ?? new();

            if (Condition is not null)
                dipValueItem.Condition = Condition.GetInternalClone();

            return dipValueItem;
        }

        #endregion

        #region Comparision Methods

        /// <inheritdoc/>
        public override bool Equals(DatItem? other)
        {
            // If the other item is null
            if (other is null)
                return false;

            // If the type matches
            if (other is DipValue otherDipValue)
                return _internal.Equals(otherDipValue._internal);

            // Everything else fails
            return false;
        }

        #endregion
    }
}
