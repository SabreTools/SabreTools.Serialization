using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Data.Extensions;

namespace SabreTools.Metadata.DatItems.Formats
{
    /// <summary>
    /// Represents control for an input
    /// </summary>
    [JsonObject("control"), XmlRoot("control")]
    public sealed class Control : DatItem<Data.Models.Metadata.Control>
    {
        #region Fields

        /// <inheritdoc>/>
        protected override ItemType ItemType => ItemType.Control;

        #endregion

        #region Constructors

        public Control() : base() { }

        public Control(Data.Models.Metadata.Control item) : base(item)
        {
            // Process flag values
            long? buttons = ReadLong(Data.Models.Metadata.Control.ButtonsKey);
            if (buttons is not null)
                Write<string?>(Data.Models.Metadata.Control.ButtonsKey, buttons.ToString());

            long? keyDelta = ReadLong(Data.Models.Metadata.Control.KeyDeltaKey);
            if (keyDelta is not null)
                Write<string?>(Data.Models.Metadata.Control.KeyDeltaKey, keyDelta.ToString());

            long? maximum = ReadLong(Data.Models.Metadata.Control.MaximumKey);
            if (maximum is not null)
                Write<string?>(Data.Models.Metadata.Control.MaximumKey, maximum.ToString());

            long? minimum = ReadLong(Data.Models.Metadata.Control.MinimumKey);
            if (minimum is not null)
                Write<string?>(Data.Models.Metadata.Control.MinimumKey, minimum.ToString());

            long? player = ReadLong(Data.Models.Metadata.Control.PlayerKey);
            if (player is not null)
                Write<string?>(Data.Models.Metadata.Control.PlayerKey, player.ToString());

            long? reqButtons = ReadLong(Data.Models.Metadata.Control.ReqButtonsKey);
            if (reqButtons is not null)
                Write<string?>(Data.Models.Metadata.Control.ReqButtonsKey, reqButtons.ToString());

            bool? reverse = ReadBool(Data.Models.Metadata.Control.ReverseKey);
            if (reverse is not null)
                Write<string?>(Data.Models.Metadata.Control.ReverseKey, reverse.FromYesNo());

            long? sensitivity = ReadLong(Data.Models.Metadata.Control.SensitivityKey);
            if (sensitivity is not null)
                Write<string?>(Data.Models.Metadata.Control.SensitivityKey, sensitivity.ToString());

            string? controlType = ReadString(Data.Models.Metadata.Control.ControlTypeKey);
            if (controlType is not null)
                Write<string?>(Data.Models.Metadata.Control.ControlTypeKey, controlType.AsControlType()?.AsStringValue());
        }

        public Control(Data.Models.Metadata.Control item, Machine machine, Source source) : this(item)
        {
            Write<Source?>(SourceKey, source);
            CopyMachineInformation(machine);
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone() => new Control(_internal.Clone() as Data.Models.Metadata.Control ?? []);

        #endregion
    }
}
