using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Metadata.DatItems.Formats
{
    /// <summary>
    /// Represents a single port on a machine
    /// </summary>
    [JsonObject("port"), XmlRoot("port")]
    public sealed class Port : DatItem<Data.Models.Metadata.Port>
    {
        #region Properties

        public string[]? AnalogMask
        {
            get => _internal.AnalogMask;
            set => _internal.AnalogMask = value;
        }

        /// <inheritdoc>/>
        public override Data.Models.Metadata.ItemType ItemType
            => Data.Models.Metadata.ItemType.Port;

        public string? Tag
        {
            get => _internal.Tag;
            set => _internal.Tag = value;
        }

        #endregion

        #region Constructors

        public Port() : base() { }

        public Port(Data.Models.Metadata.Port item) : base(item) { }

        public Port(Data.Models.Metadata.Port item, Machine machine, Source source) : this(item)
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
        public override object Clone() => new Port(GetInternalClone());

        /// <inheritdoc/>
        public override Data.Models.Metadata.Port GetInternalClone()
            => _internal.Clone() as Data.Models.Metadata.Port ?? new();

        #endregion

        #region Comparision Methods

        /// <inheritdoc/>
        public override bool Equals(DatItem? other)
        {
            // If the other item is null
            if (other is null)
                return false;

            // If the type matches
            if (other is Port otherPort)
                return _internal.Equals(otherPort._internal);

            // Everything else fails
            return false;
        }

        #endregion
    }
}
