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

        public Data.Models.Metadata.ControlType? ControlType
        {
            get => (_internal as Data.Models.Metadata.Control)?.ControlType;
            set => (_internal as Data.Models.Metadata.Control)?.ControlType = value;
        }

        public bool? Reverse
        {
            get => (_internal as Data.Models.Metadata.Control)?.Reverse;
            set => (_internal as Data.Models.Metadata.Control)?.Reverse = value;
        }

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

            long? sensitivity = ReadLong(Data.Models.Metadata.Control.SensitivityKey);
            if (sensitivity is not null)
                Write<string?>(Data.Models.Metadata.Control.SensitivityKey, sensitivity.ToString());
        }

        public Control(Data.Models.Metadata.Control item, Machine machine, Source source) : this(item)
        {
            Source = source;
            CopyMachineInformation(machine);
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone() => new Control(_internal.DeepClone() as Data.Models.Metadata.Control ?? []);

        #endregion
    }
}
