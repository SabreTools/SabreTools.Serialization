using System;
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

        public Analog[]? Analog { get; set; }

        [JsonIgnore]
        public bool AnalogSpecified => Analog is not null && Analog.Length > 0;

        /// <inheritdoc>/>
        public override Data.Models.Metadata.ItemType ItemType
            => Data.Models.Metadata.ItemType.Port;

        public string? Tag
        {
            get => (_internal as Data.Models.Metadata.Port)?.Tag;
            set => (_internal as Data.Models.Metadata.Port)?.Tag = value;
        }

        #endregion

        #region Constructors

        public Port() : base() { }

        public Port(Data.Models.Metadata.Port item) : base(item)
        {
            // Handle subitems
            if (item.Analog is not null)
                Analog = Array.ConvertAll(item.Analog, analog => new Analog(analog)); ;
        }

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
        {
            var portItem = (_internal as Data.Models.Metadata.Port)?.Clone() as Data.Models.Metadata.Port ?? new();

            if (Analog is not null)
                portItem.Analog = Array.ConvertAll(Analog, analog => analog.GetInternalClone());

            return portItem;
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
            if (other is Port otherPort)
                return ((Data.Models.Metadata.Port)_internal).Equals((Data.Models.Metadata.Port)otherPort._internal);

            // Everything else fails
            return false;
        }

        /// <inheritdoc/>
        public override bool Equals(DatItem<Data.Models.Metadata.Port>? other)
        {
            // If the other value is invalid
            if (other is null)
                return false;

            // If the type matches
            if (other is Port otherPort)
                return ((Data.Models.Metadata.Port)_internal).Equals((Data.Models.Metadata.Port)otherPort._internal);

            // Everything else fails
            return false;
        }

        #endregion
    }
}
