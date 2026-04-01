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

        /// <inheritdoc>/>
        internal override Data.Models.Metadata.ItemType ItemType => Data.Models.Metadata.ItemType.Input;

        [JsonIgnore]
        public bool ControlsSpecified
        {
            get
            {
                var controls = Read<Control[]?>(Data.Models.Metadata.Input.ControlKey);
                return controls is not null && controls.Length > 0;
            }
        }

        #endregion

        #region Constructors

        public Input() : base() { }

        public Input(Data.Models.Metadata.Input item) : base(item)
        {
            // Process flag values
            long? buttons = ReadLong(Data.Models.Metadata.Input.ButtonsKey);
            if (buttons is not null)
                Write<string?>(Data.Models.Metadata.Input.ButtonsKey, buttons.ToString());

            long? coins = ReadLong(Data.Models.Metadata.Input.CoinsKey);
            if (coins is not null)
                Write<string?>(Data.Models.Metadata.Input.CoinsKey, coins.ToString());

            long? players = ReadLong(Data.Models.Metadata.Input.PlayersKey);
            if (players is not null)
                Write<string?>(Data.Models.Metadata.Input.PlayersKey, players.ToString());

            bool? service = ReadBool(Data.Models.Metadata.Input.ServiceKey);
            if (service is not null)
                Write<string?>(Data.Models.Metadata.Input.ServiceKey, service.FromYesNo());

            bool? tilt = ReadBool(Data.Models.Metadata.Input.TiltKey);
            if (tilt is not null)
                Write<string?>(Data.Models.Metadata.Input.TiltKey, tilt.FromYesNo());

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
            Write<Source?>(SourceKey, source);
            CopyMachineInformation(machine);
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone() => new Input(_internal.Clone() as Data.Models.Metadata.Input ?? []);

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
