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
            get => _internal.Default;
            set => _internal.Default = value;
        }

        public string? DevName
        {
            get => _internal.DevName;
            set => _internal.DevName = value;
        }

        /// <inheritdoc>/>
        public override Data.Models.Metadata.ItemType ItemType
            => Data.Models.Metadata.ItemType.SlotOption;

        public string? Name
        {
            get => _internal.Name;
            set => _internal.Name = value;
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

        public SlotOption(Data.Models.Metadata.SlotOption item, long machineIndex, long sourceIndex) : this(item)
        {
            SourceIndex = sourceIndex;
            MachineIndex = machineIndex;
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
            => _internal.Clone() as Data.Models.Metadata.SlotOption ?? new();

        #endregion

        #region Comparision Methods

        /// <inheritdoc/>
        public override bool Equals(DatItem? other)
        {
            // If the other item is null
            if (other is null)
                return false;

            // If the type matches
            if (other is SlotOption otherSlotOption)
                return _internal.Equals(otherSlotOption._internal);

            // Everything else fails
            return false;
        }

        #endregion
    }
}
