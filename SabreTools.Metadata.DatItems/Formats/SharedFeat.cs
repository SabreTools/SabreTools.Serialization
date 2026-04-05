using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Metadata.DatItems.Formats
{
    /// <summary>
    /// Represents one shared feature object
    /// </summary>
    [JsonObject("sharedfeat"), XmlRoot("sharedfeat")]
    public sealed class SharedFeat : DatItem<Data.Models.Metadata.SharedFeat>
    {
        #region Properties

        /// <inheritdoc>/>
        public override Data.Models.Metadata.ItemType ItemType
            => Data.Models.Metadata.ItemType.SharedFeat;

        public string? Name
        {
            get => (_internal as Data.Models.Metadata.SharedFeat)?.Name;
            set => (_internal as Data.Models.Metadata.SharedFeat)?.Name = value;
        }

        public string? Value
        {
            get => (_internal as Data.Models.Metadata.SharedFeat)?.Value;
            set => (_internal as Data.Models.Metadata.SharedFeat)?.Value = value;
        }

        #endregion

        #region Constructors

        public SharedFeat() : base() { }

        public SharedFeat(Data.Models.Metadata.SharedFeat item) : base(item) { }

        public SharedFeat(Data.Models.Metadata.SharedFeat item, Machine machine, Source source) : this(item)
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
        public override object Clone() => new SharedFeat(GetInternalClone());

        /// <inheritdoc/>
        public override Data.Models.Metadata.SharedFeat GetInternalClone()
            => (_internal as Data.Models.Metadata.SharedFeat)?.Clone() as Data.Models.Metadata.SharedFeat ?? new();

        #endregion

        #region Comparision Methods

        /// <inheritdoc/>
        public override bool Equals(ModelBackedItem? other)
        {
            // If the other item is null
            if (other is null)
                return false;

            // If the type matches
            if (other is SharedFeat otherSharedFeat)
                return ((Data.Models.Metadata.SharedFeat)_internal).Equals((Data.Models.Metadata.SharedFeat)otherSharedFeat._internal);

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
            if (other is SharedFeat otherSharedFeat)
                return ((Data.Models.Metadata.SharedFeat)_internal).Equals((Data.Models.Metadata.SharedFeat)otherSharedFeat._internal);

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
            if (other is SharedFeat otherSharedFeat)
                return ((Data.Models.Metadata.SharedFeat)_internal).Equals((Data.Models.Metadata.SharedFeat)otherSharedFeat._internal);

            // Everything else fails
            return false;
        }

        /// <inheritdoc/>
        public override bool Equals(DatItem<Data.Models.Metadata.SharedFeat>? other)
        {
            // If the other value is invalid
            if (other is null)
                return false;

            // If the type matches
            if (other is SharedFeat otherSharedFeat)
                return ((Data.Models.Metadata.SharedFeat)_internal).Equals((Data.Models.Metadata.SharedFeat)otherSharedFeat._internal);

            // Everything else fails
            return false;
        }

        #endregion
    }
}
