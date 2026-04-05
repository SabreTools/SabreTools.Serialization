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
            get => _internal.Buttons;
            set => _internal.Buttons = value;
        }

        public Data.Models.Metadata.ControlType? ControlType
        {
            get => _internal.ControlType;
            set => _internal.ControlType = value;
        }

        /// <inheritdoc>/>
        public override Data.Models.Metadata.ItemType ItemType
            => Data.Models.Metadata.ItemType.Control;

        public long? KeyDelta
        {
            get => _internal.KeyDelta;
            set => _internal.KeyDelta = value;
        }

        public long? Maximum
        {
            get => _internal.Maximum;
            set => _internal.Maximum = value;
        }

        public long? Minimum
        {
            get => _internal.Minimum;
            set => _internal.Minimum = value;
        }

        public long? Player
        {
            get => _internal.Player;
            set => _internal.Player = value;
        }

        public long? ReqButtons
        {
            get => _internal.ReqButtons;
            set => _internal.ReqButtons = value;
        }

        public bool? Reverse
        {
            get => _internal.Reverse;
            set => _internal.Reverse = value;
        }

        public long? Sensitivity
        {
            get => _internal.Sensitivity;
            set => _internal.Sensitivity = value;
        }

        public string? Ways
        {
            get => _internal.Ways;
            set => _internal.Ways = value;
        }

        public string? Ways2
        {
            get => _internal.Ways2;
            set => _internal.Ways2 = value;
        }

        public string? Ways3
        {
            get => _internal.Ways3;
            set => _internal.Ways3 = value;
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
            => _internal.Clone() as Data.Models.Metadata.Control ?? new();

        #endregion

        #region Comparision Methods

        /// <inheritdoc/>
        public override bool Equals(DatItem? other)
        {
            // If the other item is null
            if (other is null)
                return false;

            // If the type matches
            if (other is Control otherControl)
                return _internal.Equals(otherControl._internal);

            // Everything else fails
            return false;
        }

        /// <inheritdoc/>
        public override bool Equals(DatItem<Data.Models.Metadata.Control>? other)
        {
            // If the other value is invalid
            if (other is null)
                return false;

            // If the type matches
            if (other is Control otherControl)
                return _internal.Equals(otherControl._internal);

            // Everything else fails
            return false;
        }

        #endregion
    }
}
