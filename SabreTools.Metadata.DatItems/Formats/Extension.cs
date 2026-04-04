using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Metadata.DatItems.Formats
{
    /// <summary>
    /// Represents a matchable extension
    /// </summary>
    [JsonObject("extension"), XmlRoot("extension")]
    public sealed class Extension : DatItem<Data.Models.Metadata.Extension>
    {
        #region Properties

        /// <inheritdoc>/>
        public override Data.Models.Metadata.ItemType ItemType
            => Data.Models.Metadata.ItemType.Extension;

        public string? Name
        {
            get => (_internal as Data.Models.Metadata.Extension)?.Name;
            set => (_internal as Data.Models.Metadata.Extension)?.Name = value;
        }

        #endregion

        #region Constructors

        public Extension() : base() { }

        public Extension(Data.Models.Metadata.Extension item) : base(item) { }

        public Extension(Data.Models.Metadata.Extension item, Machine machine, Source source) : this(item)
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
        public override object Clone() => new Extension(GetInternalClone());

        /// <inheritdoc/>
        public override Data.Models.Metadata.Extension GetInternalClone()
            => (_internal as Data.Models.Metadata.Extension)?.Clone() as Data.Models.Metadata.Extension ?? [];

        #endregion

        #region Comparision Methods

        /// <inheritdoc/>
        public override bool Equals(DatItem? other)
        {
            // If the other item is null
            if (other is null)
                return false;

            // If the type matches
            if (other is Extension otherExtension)
                return ((Data.Models.Metadata.Extension)_internal).Equals((Data.Models.Metadata.Extension)otherExtension._internal);

            // Everything else fails
            return false;
        }

        /// <inheritdoc/>
        public override bool Equals(DatItem<Data.Models.Metadata.Extension>? other)
        {
            // If the other value is invalid
            if (other is null)
                return false;

            // If the type matches
            if (other is Extension otherExtension)
                return ((Data.Models.Metadata.Extension)_internal).Equals((Data.Models.Metadata.Extension)otherExtension._internal);

            // Everything else fails
            return false;
        }

        #endregion
    }
}
