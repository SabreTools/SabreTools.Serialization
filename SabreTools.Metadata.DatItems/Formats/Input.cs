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

        [JsonIgnore]
        public bool ControlsSpecified
        {
            get
            {
                var controls = Read<Control[]?>(Data.Models.Metadata.Input.ControlKey);
                return controls is not null && controls.Length > 0;
            }
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
            var controls = item.ReadArray<Data.Models.Metadata.Control>(Data.Models.Metadata.Input.ControlKey);
            if (controls is not null)
            {
                Control[] controlItems = Array.ConvertAll(controls, control => new Control(control));
                Write<Control[]?>(Data.Models.Metadata.Input.ControlKey, controlItems);
            }
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

            var controls = Read<Control[]?>(Data.Models.Metadata.Input.ControlKey);
            if (controls is not null)
            {
                Data.Models.Metadata.Control[] controlItems = Array.ConvertAll(controls, control => control.GetInternalClone());
                inputItem[Data.Models.Metadata.Input.ControlKey] = controlItems;
            }

            return inputItem;
        }

        #endregion
    }
}
