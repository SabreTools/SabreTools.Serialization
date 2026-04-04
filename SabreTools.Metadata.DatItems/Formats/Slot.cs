using System;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Metadata.DatItems.Formats
{
    /// <summary>
    /// Represents which Slot(s) is associated with a set
    /// </summary>
    [JsonObject("slot"), XmlRoot("slot")]
    public sealed class Slot : DatItem<Data.Models.Metadata.Slot>
    {
        #region Properties

        /// <inheritdoc>/>
        public override Data.Models.Metadata.ItemType ItemType
            => Data.Models.Metadata.ItemType.Slot;

        public string? Name
        {
            get => (_internal as Data.Models.Metadata.Slot)?.Name;
            set => (_internal as Data.Models.Metadata.Slot)?.Name = value;
        }

        public SlotOption[]? SlotOption { get; set; }

        [JsonIgnore]
        public bool SlotOptionSpecified => SlotOption is not null && SlotOption.Length > 0;

        #endregion

        #region Constructors

        public Slot() : base() { }

        public Slot(Data.Models.Metadata.Slot item) : base(item)
        {
            // Handle subitems
            if (item.SlotOption is not null)
                SlotOption = Array.ConvertAll(item.SlotOption, slotOption => new SlotOption(slotOption)); ;
        }

        public Slot(Data.Models.Metadata.Slot item, Machine machine, Source source) : this(item)
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
        public override object Clone() => new Slot(GetInternalClone());

        /// <inheritdoc/>
        public override Data.Models.Metadata.Slot GetInternalClone()
        {
            var slotItem = (_internal as Data.Models.Metadata.Slot)?.Clone() as Data.Models.Metadata.Slot ?? [];

            if (SlotOption is not null)
                slotItem.SlotOption = Array.ConvertAll(SlotOption, slotOption => slotOption.GetInternalClone());

            return slotItem;
        }

        #endregion
    }
}
