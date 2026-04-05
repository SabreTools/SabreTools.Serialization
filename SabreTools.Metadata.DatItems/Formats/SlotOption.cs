using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Metadata.DatItems.Formats
{
    /// <summary>
    /// Represents one ListXML slotoption
    /// </summary>
    [JsonObject("slotoption"), XmlRoot("slotoption")]
    public sealed class SlotOption : DatItem<Data.Models.Metadata.SlotOption>
    {
        #region Properties

        public bool? Default
        {
            get => (_internal as Data.Models.Metadata.SlotOption)?.Default;
            set => (_internal as Data.Models.Metadata.SlotOption)?.Default = value;
        }

        public string? DevName
        {
            get => (_internal as Data.Models.Metadata.SlotOption)?.DevName;
            set => (_internal as Data.Models.Metadata.SlotOption)?.DevName = value;
        }

        /// <inheritdoc>/>
        public override Data.Models.Metadata.ItemType ItemType
            => Data.Models.Metadata.ItemType.SlotOption;

        public string? Name
        {
            get => (_internal as Data.Models.Metadata.SlotOption)?.Name;
            set => (_internal as Data.Models.Metadata.SlotOption)?.Name = value;
        }

        #endregion

        #region Constructors

        public SlotOption() : base() { }

        public SlotOption(Data.Models.Metadata.SlotOption item) : base(item) { }

        public SlotOption(Data.Models.Metadata.SlotOption item, Machine machine, Source source) : this(item)
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
        public override object Clone() => new SlotOption(GetInternalClone());

        /// <inheritdoc/>
        public override Data.Models.Metadata.SlotOption GetInternalClone()
            => (_internal as Data.Models.Metadata.SlotOption)?.Clone() as Data.Models.Metadata.SlotOption ?? new();

        #endregion

        #region Comparision Methods

        /// <inheritdoc/>
        public override bool Equals(ModelBackedItem? other)
        {
            // If the other item is null
            if (other is null)
                return false;

            // If the type matches
            if (other is SlotOption otherSlotOption)
                return ((Data.Models.Metadata.SlotOption)_internal).Equals((Data.Models.Metadata.SlotOption)otherSlotOption._internal);

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
            if (other is SlotOption otherSlotOption)
                return ((Data.Models.Metadata.SlotOption)_internal).Equals((Data.Models.Metadata.SlotOption)otherSlotOption._internal);

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
            if (other is SlotOption otherSlotOption)
                return ((Data.Models.Metadata.SlotOption)_internal).Equals((Data.Models.Metadata.SlotOption)otherSlotOption._internal);

            // Everything else fails
            return false;
        }

        /// <inheritdoc/>
        public override bool Equals(DatItem<Data.Models.Metadata.SlotOption>? other)
        {
            // If the other value is invalid
            if (other is null)
                return false;

            // If the type matches
            if (other is SlotOption otherSlotOption)
                return ((Data.Models.Metadata.SlotOption)_internal).Equals((Data.Models.Metadata.SlotOption)otherSlotOption._internal);

            // Everything else fails
            return false;
        }

        #endregion
    }
}
