using System;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Metadata.DatItems.Formats
{
    /// <summary>
    /// Represents one ListXML input
    /// </summary>
    [JsonObject("input"), XmlRoot("input")]
    public sealed class Input : DatItem<Data.Models.Metadata.Input>
    {
        #region Properties

        public long? Buttons
        {
            get => _internal.Buttons;
            set => _internal.Buttons = value;
        }

        public long? Coins
        {
            get => _internal.Coins;
            set => _internal.Coins = value;
        }

        public Control[]? Control { get; set; }

        [JsonIgnore]
        public bool ControlSpecified => Control is not null && Control.Length > 0;

        public string? ControlAttr
        {
            get => _internal.ControlAttr;
            set => _internal.ControlAttr = value;
        }

        /// <inheritdoc>/>
        public override Data.Models.Metadata.ItemType ItemType
            => Data.Models.Metadata.ItemType.Input;

        public long? Players
        {
            get => _internal.Players;
            set => _internal.Players = value;
        }

        public bool? Service
        {
            get => _internal.Service;
            set => _internal.Service = value;
        }

        public bool? Tilt
        {
            get => _internal.Tilt;
            set => _internal.Tilt = value;
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

        #region Accessors

        /// <inheritdoc/>
        public override string? GetName() => null;

        /// <inheritdoc/>
        public override void SetName(string? name) { }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone() => new Input(GetInternalClone());

        /// <inheritdoc/>
        public override Data.Models.Metadata.Input GetInternalClone()
        {
            var inputItem = _internal.Clone() as Data.Models.Metadata.Input ?? new();

            if (Control is not null)
                inputItem.Control = Array.ConvertAll(Control, control => control.GetInternalClone());

            return inputItem;
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
            if (other is Input otherInput)
                return _internal.Equals(otherInput._internal);

            // Everything else fails
            return false;
        }

        #endregion
    }
}
