using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Metadata.DatItems.Formats
{
    /// <summary>
    /// Represents which RAM option(s) is associated with a set
    /// </summary>
    [JsonObject("ramoption"), XmlRoot("ramoption")]
    public sealed class RamOption : DatItem<Data.Models.Metadata.RamOption>
    {
        #region Properties

        public string? Content
        {
            get => (_internal as Data.Models.Metadata.RamOption)?.Content;
            set => (_internal as Data.Models.Metadata.RamOption)?.Content = value;
        }

        public bool? Default
        {
            get => (_internal as Data.Models.Metadata.RamOption)?.Default;
            set => (_internal as Data.Models.Metadata.RamOption)?.Default = value;
        }

        /// <inheritdoc>/>
        public override Data.Models.Metadata.ItemType ItemType
            => Data.Models.Metadata.ItemType.RamOption;

        public string? Name
        {
            get => (_internal as Data.Models.Metadata.RamOption)?.Name;
            set => (_internal as Data.Models.Metadata.RamOption)?.Name = value;
        }

        #endregion

        #region Constructors

        public RamOption() : base() { }

        public RamOption(Data.Models.Metadata.RamOption item) : base(item) { }

        public RamOption(Data.Models.Metadata.RamOption item, Machine machine, Source source) : this(item)
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
        public override object Clone() => new RamOption(GetInternalClone());

        /// <inheritdoc/>
        public override Data.Models.Metadata.RamOption GetInternalClone()
            => (_internal as Data.Models.Metadata.RamOption)?.Clone() as Data.Models.Metadata.RamOption ?? [];

        #endregion

        #region Comparision Methods

        /// <inheritdoc/>
        public override bool Equals(DatItem? other)
        {
            // If the other item is null
            if (other is null)
                return false;

            // If the type matches
            if (other is RamOption otherRamOption)
                return ((Data.Models.Metadata.RamOption)_internal).Equals((Data.Models.Metadata.RamOption)otherRamOption._internal);

            // Everything else fails
            return false;
        }

        /// <inheritdoc/>
        public override bool Equals(DatItem<Data.Models.Metadata.RamOption>? other)
        {
            // If the other value is invalid
            if (other is null)
                return false;

            // If the type matches
            if (other is RamOption otherRamOption)
                return ((Data.Models.Metadata.RamOption)_internal).Equals((Data.Models.Metadata.RamOption)otherRamOption._internal);

            // Everything else fails
            return false;
        }

        #endregion
    }
}
