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
        protected override ItemType ItemType => ItemType.Input;

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
            if (ReadLong(Data.Models.Metadata.Input.ButtonsKey) is not null)
                Write<string?>(Data.Models.Metadata.Input.ButtonsKey, ReadLong(Data.Models.Metadata.Input.ButtonsKey).ToString());
            if (ReadLong(Data.Models.Metadata.Input.CoinsKey) is not null)
                Write<string?>(Data.Models.Metadata.Input.CoinsKey, ReadLong(Data.Models.Metadata.Input.CoinsKey).ToString());
            if (ReadLong(Data.Models.Metadata.Input.PlayersKey) is not null)
                Write<string?>(Data.Models.Metadata.Input.PlayersKey, ReadLong(Data.Models.Metadata.Input.PlayersKey).ToString());
            if (ReadBool(Data.Models.Metadata.Input.ServiceKey) is not null)
                Write<string?>(Data.Models.Metadata.Input.ServiceKey, ReadBool(Data.Models.Metadata.Input.ServiceKey).FromYesNo());
            if (ReadBool(Data.Models.Metadata.Input.TiltKey) is not null)
                Write<string?>(Data.Models.Metadata.Input.TiltKey, ReadBool(Data.Models.Metadata.Input.TiltKey).FromYesNo());

            // Handle subitems
            var controls = item.ReadItemArray<Data.Models.Metadata.Control>(Data.Models.Metadata.Input.ControlKey);
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
