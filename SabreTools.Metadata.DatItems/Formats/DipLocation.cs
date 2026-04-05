using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Metadata.DatItems.Formats
{
    /// <summary>
    /// Represents one diplocation
    /// </summary>
    [JsonObject("diplocation"), XmlRoot("diplocation")]
    public sealed class DipLocation : DatItem<Data.Models.Metadata.DipLocation>
    {
        #region Properties

        public bool? Inverted
        {
            get => _internal.Inverted;
            set => _internal.Inverted = value;
        }

        /// <inheritdoc>/>
        public override Data.Models.Metadata.ItemType ItemType
            => Data.Models.Metadata.ItemType.DipLocation;

        public string? Name
        {
            get => _internal.Name;
            set => _internal.Name = value;
        }

        public long? Number
        {
            get => _internal.Number;
            set => _internal.Number = value;
        }

        #endregion

        #region Constructors

        public DipLocation() : base() { }

        public DipLocation(Data.Models.Metadata.DipLocation item) : base(item) { }

        public DipLocation(Data.Models.Metadata.DipLocation item, Machine machine, Source source) : this(item)
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
        public override object Clone() => new DipLocation(GetInternalClone());

        /// <inheritdoc/>
        public override Data.Models.Metadata.DipLocation GetInternalClone()
            => _internal.Clone() as Data.Models.Metadata.DipLocation ?? new();

        #endregion

        #region Comparision Methods

        /// <inheritdoc/>
        public override bool Equals(DatItem? other)
        {
            // If the other item is null
            if (other is null)
                return false;

            // If the type matches
            if (other is DipLocation otherDipLocation)
                return _internal.Equals(otherDipLocation._internal);

            // Everything else fails
            return false;
        }

        /// <inheritdoc/>
        public override bool Equals(DatItem<Data.Models.Metadata.DipLocation>? other)
        {
            // If the other value is invalid
            if (other is null)
                return false;

            // If the type matches
            if (other is DipLocation otherDipLocation)
                return _internal.Equals(otherDipLocation._internal);

            // Everything else fails
            return false;
        }

        #endregion
    }
}
