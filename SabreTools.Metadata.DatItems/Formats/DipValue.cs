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
            var dipValueItem = (_internal as Data.Models.Metadata.DipValue)?.Clone() as Data.Models.Metadata.DipValue ?? new();

            if (Condition is not null)
                dipValueItem.Condition = Condition.GetInternalClone();

            return dipValueItem;
        }

        #endregion

        #region Comparision Methods

        /// <inheritdoc/>
        public override bool Equals(ModelBackedItem? other)
        {
            // If the other item is null
            if (other is null)
                return false;

            // If the type matches
            if (other is DipValue otherDipValue)
                return ((Data.Models.Metadata.DipValue)_internal).Equals((Data.Models.Metadata.DipValue)otherDipValue._internal);

            // Everything else fails
            return false;
        }

        /// <inheritdoc/>
        public override bool Equals(ModelBackedItem<Data.Models.Metadata.DatItem>? other)
        {
            // If the other item is null
            if (other is null)
                return false;

            // If the type matches
            if (other is DipValue otherDipValue)
                return ((Data.Models.Metadata.DipValue)_internal).Equals((Data.Models.Metadata.DipValue)otherDipValue._internal);

            // Everything else fails
            return false;
        }

        /// <inheritdoc/>
        public override bool Equals(DatItem? other)
        {
            // If the other item is null
            if (other is null)
                return false;

            // If the type matches
            if (other is DipValue otherDipValue)
                return ((Data.Models.Metadata.DipValue)_internal).Equals((Data.Models.Metadata.DipValue)otherDipValue._internal);

            // Everything else fails
            return false;
        }

        /// <inheritdoc/>
        public override bool Equals(DatItem<Data.Models.Metadata.DipValue>? other)
        {
            // If the other value is invalid
            if (other is null)
                return false;

            // If the type matches
            if (other is DipValue otherDipValue)
                return ((Data.Models.Metadata.DipValue)_internal).Equals((Data.Models.Metadata.DipValue)otherDipValue._internal);

            // Everything else fails
            return false;
        }

        #endregion
    }
}
