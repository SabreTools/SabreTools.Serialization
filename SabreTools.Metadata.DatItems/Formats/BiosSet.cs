using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Metadata.DatItems.Formats
{
    /// <summary>
    /// Represents which BIOS(es) is associated with a set
    /// </summary>
    [JsonObject("biosset"), XmlRoot("biosset")]
    public sealed class BiosSet : DatItem<Data.Models.Metadata.BiosSet>
    {
        #region Properties

        public bool? Default
        {
            get => _internal.Default;
            set => _internal.Default = value;
        }

        public string? Description
        {
            get => _internal.Description;
            set => _internal.Description = value;
        }

        /// <inheritdoc>/>
        public override Data.Models.Metadata.ItemType ItemType
            => Data.Models.Metadata.ItemType.BiosSet;

        public string? Name
        {
            get => _internal.Name;
            set => _internal.Name = value;
        }

        #endregion

        #region Constructors

        public BiosSet() : base() { }

        public BiosSet(Data.Models.Metadata.BiosSet item) : base(item) { }

        public BiosSet(Data.Models.Metadata.BiosSet item, Machine machine, Source source) : this(item)
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
        public override object Clone() => new BiosSet(GetInternalClone());

        /// <inheritdoc/>
        public override Data.Models.Metadata.BiosSet GetInternalClone()
            => _internal.Clone() as Data.Models.Metadata.BiosSet ?? new();

        #endregion

        #region Comparision Methods

        /// <inheritdoc/>
        public override bool Equals(DatItem? other)
        {
            // If the other item is null
            if (other is null)
                return false;

            // If the type matches
            if (other is BiosSet otherBiosSet)
                return _internal.Equals(otherBiosSet._internal);

            // Everything else fails
            return false;
        }

        /// <inheritdoc/>
        public override bool Equals(DatItem<Data.Models.Metadata.BiosSet>? other)
        {
            // If the other value is invalid
            if (other is null)
                return false;

            // If the type matches
            if (other is BiosSet otherBiosSet)
                return _internal.Equals(otherBiosSet._internal);

            // Everything else fails
            return false;
        }

        #endregion
    }
}
