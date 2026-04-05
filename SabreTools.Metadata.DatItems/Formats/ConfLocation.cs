using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Metadata.DatItems.Formats
{
    /// <summary>
    /// Represents one conflocation
    /// </summary>
    [JsonObject("conflocation"), XmlRoot("conflocation")]
    public sealed class ConfLocation : DatItem<Data.Models.Metadata.ConfLocation>
    {
        #region Properties

        /// <inheritdoc>/>
        public override Data.Models.Metadata.ItemType ItemType
            => Data.Models.Metadata.ItemType.ConfLocation;

        public bool? Inverted
        {
            get => (_internal as Data.Models.Metadata.ConfLocation)?.Inverted;
            set => (_internal as Data.Models.Metadata.ConfLocation)?.Inverted = value;
        }

        public string? Name
        {
            get => (_internal as Data.Models.Metadata.ConfLocation)?.Name;
            set => (_internal as Data.Models.Metadata.ConfLocation)?.Name = value;
        }

        public long? Number
        {
            get => (_internal as Data.Models.Metadata.ConfLocation)?.Number;
            set => (_internal as Data.Models.Metadata.ConfLocation)?.Number = value;
        }

        #endregion

        #region Constructors

        public ConfLocation() : base() { }

        public ConfLocation(Data.Models.Metadata.ConfLocation item) : base(item) { }

        public ConfLocation(Data.Models.Metadata.ConfLocation item, Machine machine, Source source) : this(item)
        {
            Source = source;
            CopyMachineInformation(machine);
        }

        #endregion

        #region Accessors

        /// <inheritdoc/>
        public override string? GetName() => Name;

        /// <inheritdoc/>
        public override void SetName(string? name) => Name = name;

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone() => new ConfLocation(GetInternalClone());

        /// <inheritdoc/>
        public override Data.Models.Metadata.ConfLocation GetInternalClone()
            => (_internal as Data.Models.Metadata.ConfLocation)?.Clone() as Data.Models.Metadata.ConfLocation ?? new();

        #endregion

        #region Comparision Methods

        /// <inheritdoc/>
        public override bool Equals(ModelBackedItem? other)
        {
            // If the other item is null
            if (other is null)
                return false;

            // If the type matches
            if (other is ConfLocation otherConfLocation)
                return ((Data.Models.Metadata.ConfLocation)_internal).Equals((Data.Models.Metadata.ConfLocation)otherConfLocation._internal);

            // Everything else fails
            return false;
        }

        /// <inheritdoc/>
        public override bool Equals(ModelBackedItem<Data.Models.Metadata.DatItem>? other)
        {
            // If the other item is null
            if (other is null)
                return false;

            // If the type matches
            if (other is ConfLocation otherConfLocation)
                return ((Data.Models.Metadata.ConfLocation)_internal).Equals((Data.Models.Metadata.ConfLocation)otherConfLocation._internal);

            // Everything else fails
            return false;
        }

        /// <inheritdoc/>
        public override bool Equals(DatItem? other)
        {
            // If the other item is null
            if (other is null)
                return false;

            // If the type matches
            if (other is ConfLocation otherConfLocation)
                return ((Data.Models.Metadata.ConfLocation)_internal).Equals((Data.Models.Metadata.ConfLocation)otherConfLocation._internal);

            // Everything else fails
            return false;
        }

        /// <inheritdoc/>
        public override bool Equals(DatItem<Data.Models.Metadata.ConfLocation>? other)
        {
            // If the other value is invalid
            if (other is null)
                return false;

            // If the type matches
            if (other is ConfLocation otherConfLocation)
                return ((Data.Models.Metadata.ConfLocation)_internal).Equals((Data.Models.Metadata.ConfLocation)otherConfLocation._internal);

            // Everything else fails
            return false;
        }

        #endregion
    }
}
