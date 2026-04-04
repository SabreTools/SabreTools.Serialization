using System;
using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Data.Extensions;

namespace SabreTools.Metadata.DatItems.Formats
{
    /// <summary>
    /// Represents which Slot(s) is associated with a set
    /// </summary>
    [JsonObject("slot"), XmlRoot("slot")]
    public sealed class Slot : DatItem<Data.Models.Metadata.Slot>
    {
        #region Fields

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
                SlotOption = Array.ConvertAll(item.SlotOption, slotOption => new SlotOption(slotOption));;
        }

        public Slot(Data.Models.Metadata.Slot item, Machine machine, Source source) : this(item)
        {
            Source = source;
            CopyMachineInformation(machine);
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone() => new Slot(_internal.DeepClone() as Data.Models.Metadata.Slot ?? []);

        /// <inheritdoc/>
        public override Data.Models.Metadata.Slot GetInternalClone()
        {
            var slotItem = base.GetInternalClone();

            if (SlotOption is not null)
                slotItem.SlotOption = Array.ConvertAll(SlotOption, slotOption => slotOption.GetInternalClone());

            return slotItem;
        }

        #endregion
    }
}
