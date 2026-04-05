using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Metadata.DatItems.Formats
{
    /// <summary>
    /// Represents one part feature object
    /// </summary>
    [JsonObject("part_feature"), XmlRoot("part_feature")]
    public sealed class PartFeature : DatItem<Data.Models.Metadata.Feature>
    {
        #region Properties

        public Data.Models.Metadata.FeatureType? FeatureType
        {
            get => _internal.FeatureType;
            set => _internal.FeatureType = value;
        }

        /// <inheritdoc>/>
        public override Data.Models.Metadata.ItemType ItemType
            => Data.Models.Metadata.ItemType.PartFeature;

        public string? Name
        {
            get => _internal.Name;
            set => _internal.Name = value;
        }

        public Data.Models.Metadata.FeatureStatus? Overall
        {
            get => _internal.Overall;
            set => _internal.Overall = value;
        }

        public Part? Part { get; set; }

        [JsonIgnore]
        public bool PartSpecified => Part is not null;

        public Data.Models.Metadata.FeatureStatus? Status
        {
            get => _internal.Status;
            set => _internal.Status = value;
        }

        public string? Value
        {
            get => _internal.Value;
            set => _internal.Value = value;
        }

        #endregion

        #region Constructors

        public PartFeature() : base() { }

        public PartFeature(Data.Models.Metadata.Feature item) : base(item) { }

        public PartFeature(Data.Models.Metadata.Feature item, Machine machine, Source source) : this(item)
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
        public override object Clone() => new PartFeature(GetInternalClone());

        /// <inheritdoc/>
        public override Data.Models.Metadata.Feature GetInternalClone()
            => _internal.Clone() as Data.Models.Metadata.Feature ?? new();

        #endregion

        #region Comparision Methods

        /// <inheritdoc/>
        public override bool Equals(DatItem? other)
        {
            // If the other item is null
            if (other is null)
                return false;

            // If the type matches
            if (other is PartFeature otherPartFeature)
                return _internal.Equals(otherPartFeature._internal);

            // Everything else fails
            return false;
        }

        /// <inheritdoc/>
        public override bool Equals(DatItem<Data.Models.Metadata.Feature>? other)
        {
            // If the other value is invalid
            if (other is null)
                return false;

            // If the type matches
            if (other is PartFeature otherPartFeature)
                return _internal.Equals(otherPartFeature._internal);

            // Everything else fails
            return false;
        }

        #endregion
    }
}
