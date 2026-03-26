using System.Xml.Serialization;
using Newtonsoft.Json;

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
            if (ReadLong(Data.Models.Metadata.Control.ButtonsKey) is not null)
                Write<string?>(Data.Models.Metadata.Control.ButtonsKey, ReadLong(Data.Models.Metadata.Control.ButtonsKey).ToString());
            if (ReadLong(Data.Models.Metadata.Control.KeyDeltaKey) is not null)
                Write<string?>(Data.Models.Metadata.Control.KeyDeltaKey, ReadLong(Data.Models.Metadata.Control.KeyDeltaKey).ToString());
            if (ReadLong(Data.Models.Metadata.Control.MaximumKey) is not null)
                Write<string?>(Data.Models.Metadata.Control.MaximumKey, ReadLong(Data.Models.Metadata.Control.MaximumKey).ToString());
            if (ReadLong(Data.Models.Metadata.Control.MinimumKey) is not null)
                Write<string?>(Data.Models.Metadata.Control.MinimumKey, ReadLong(Data.Models.Metadata.Control.MinimumKey).ToString());
            if (ReadLong(Data.Models.Metadata.Control.PlayerKey) is not null)
                Write<string?>(Data.Models.Metadata.Control.PlayerKey, ReadLong(Data.Models.Metadata.Control.PlayerKey).ToString());
            if (ReadLong(Data.Models.Metadata.Control.ReqButtonsKey) is not null)
                Write<string?>(Data.Models.Metadata.Control.ReqButtonsKey, ReadLong(Data.Models.Metadata.Control.ReqButtonsKey).ToString());
            if (ReadBool(Data.Models.Metadata.Control.ReverseKey) is not null)
                Write<string?>(Data.Models.Metadata.Control.ReverseKey, ReadBool(Data.Models.Metadata.Control.ReverseKey).FromYesNo());
            if (ReadLong(Data.Models.Metadata.Control.SensitivityKey) is not null)
                Write<string?>(Data.Models.Metadata.Control.SensitivityKey, ReadLong(Data.Models.Metadata.Control.SensitivityKey).ToString());
            if (ReadString(Data.Models.Metadata.Control.ControlTypeKey) is not null)
                Write<string?>(Data.Models.Metadata.Control.ControlTypeKey, ReadString(Data.Models.Metadata.Control.ControlTypeKey).AsControlType().AsStringValue());
        }

        public Control(Data.Models.Metadata.Control item, Machine machine, Source source) : this(item)
        {
            Write<Source?>(SourceKey, source);
            CopyMachineInformation(machine);
        }

        #endregion
    }
}
