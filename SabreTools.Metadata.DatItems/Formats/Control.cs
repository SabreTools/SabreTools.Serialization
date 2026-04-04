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
        #region Properties

        public long? Buttons
        {
            get => (_internal as Data.Models.Metadata.Control)?.Buttons;
            set => (_internal as Data.Models.Metadata.Control)?.Buttons = value;
        }

        public Data.Models.Metadata.ControlType? ControlType
        {
            get => (_internal as Data.Models.Metadata.Control)?.ControlType;
            set => (_internal as Data.Models.Metadata.Control)?.ControlType = value;
        }

        /// <inheritdoc>/>
        public override Data.Models.Metadata.ItemType ItemType
            => Data.Models.Metadata.ItemType.Control;

        public long? KeyDelta
        {
            get => (_internal as Data.Models.Metadata.Control)?.KeyDelta;
            set => (_internal as Data.Models.Metadata.Control)?.KeyDelta = value;
        }

        public long? Maximum
        {
            get => (_internal as Data.Models.Metadata.Control)?.Maximum;
            set => (_internal as Data.Models.Metadata.Control)?.Maximum = value;
        }

        public long? Minimum
        {
            get => (_internal as Data.Models.Metadata.Control)?.Minimum;
            set => (_internal as Data.Models.Metadata.Control)?.Minimum = value;
        }

        public long? Player
        {
            get => (_internal as Data.Models.Metadata.Control)?.Player;
            set => (_internal as Data.Models.Metadata.Control)?.Player = value;
        }

        public long? ReqButtons
        {
            get => (_internal as Data.Models.Metadata.Control)?.ReqButtons;
            set => (_internal as Data.Models.Metadata.Control)?.ReqButtons = value;
        }

        public bool? Reverse
        {
            get => (_internal as Data.Models.Metadata.Control)?.Reverse;
            set => (_internal as Data.Models.Metadata.Control)?.Reverse = value;
        }

        public long? Sensitivity
        {
            get => (_internal as Data.Models.Metadata.Control)?.Sensitivity;
            set => (_internal as Data.Models.Metadata.Control)?.Sensitivity = value;
        }

        public string? Ways
        {
            get => (_internal as Data.Models.Metadata.Control)?.Ways;
            set => (_internal as Data.Models.Metadata.Control)?.Ways = value;
        }

        public string? Ways2
        {
            get => (_internal as Data.Models.Metadata.Control)?.Ways2;
            set => (_internal as Data.Models.Metadata.Control)?.Ways2 = value;
        }

        public string? Ways3
        {
            get => (_internal as Data.Models.Metadata.Control)?.Ways3;
            set => (_internal as Data.Models.Metadata.Control)?.Ways3 = value;
        }

        #endregion

        #region Constructors

        public Control() : base() { }

        public Control(Data.Models.Metadata.Control item) : base(item) { }

        public Control(Data.Models.Metadata.Control item, Machine machine, Source source) : this(item)
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
        public override object Clone() => new Control(GetInternalClone());

        /// <inheritdoc/>
        public override Data.Models.Metadata.Control GetInternalClone()
            => (_internal as Data.Models.Metadata.Control)?.Clone() as Data.Models.Metadata.Control ?? [];

        #endregion
    }
}
