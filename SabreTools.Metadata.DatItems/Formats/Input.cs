using System;
using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Data.Extensions;

namespace SabreTools.Metadata.DatItems.Formats
{
    /// <summary>
    /// Represents one ListXML input
    /// </summary>
    [JsonObject("input"), XmlRoot("input")]
    public sealed class Input : DatItem<Data.Models.Metadata.Input>
    {
        #region Fields

        public long? Buttons
        {
            get => (_internal as Data.Models.Metadata.Input)?.Buttons;
            set => (_internal as Data.Models.Metadata.Input)?.Buttons = value;
        }

        public long? Coins
        {
            get => (_internal as Data.Models.Metadata.Input)?.Coins;
            set => (_internal as Data.Models.Metadata.Input)?.Coins = value;
        }

        public Control[]? Control { get; set; }

        [JsonIgnore]
        public bool ControlSpecified => Control is not null && Control.Length > 0;

        public string? ControlAttr
        {
            get => (_internal as Data.Models.Metadata.Input)?.ControlAttr;
            set => (_internal as Data.Models.Metadata.Input)?.ControlAttr = value;
        }

        /// <inheritdoc>/>
        public override Data.Models.Metadata.ItemType ItemType
            => Data.Models.Metadata.ItemType.Input;

        public long? Players
        {
            get => (_internal as Data.Models.Metadata.Input)?.Players;
            set => (_internal as Data.Models.Metadata.Input)?.Players = value;
        }

        public bool? Service
        {
            get => (_internal as Data.Models.Metadata.Input)?.Service;
            set => (_internal as Data.Models.Metadata.Input)?.Service = value;
        }

        public bool? Tilt
        {
            get => (_internal as Data.Models.Metadata.Input)?.Tilt;
            set => (_internal as Data.Models.Metadata.Input)?.Tilt = value;
        }

        #endregion

        #region Constructors

        public Input() : base() { }

        public Input(Data.Models.Metadata.Input item) : base(item)
        {
            // Handle subitems
            if (item.Control is not null)
                Control = Array.ConvertAll(item.Control, control => new Control(control));
        }

        public Input(Data.Models.Metadata.Input item, Machine machine, Source source) : this(item)
        {
            Source = source;
            CopyMachineInformation(machine);
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone() => new Input(_internal.DeepClone() as Data.Models.Metadata.Input ?? []);

        /// <inheritdoc/>
        public override Data.Models.Metadata.Input GetInternalClone()
        {
            var inputItem = base.GetInternalClone();

            if (Control is not null)
                inputItem.Control = Array.ConvertAll(Control, control => control.GetInternalClone());

            return inputItem;
        }

        #endregion
    }
}
