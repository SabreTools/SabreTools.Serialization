using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Metadata.DatItems.Formats
{
    /// <summary>
    /// Represents the a feature of the machine
    /// </summary>
    [JsonObject("feature"), XmlRoot("feature")]
    public sealed class Feature : DatItem<Data.Models.Metadata.Feature>
    {
        #region Properties

        public Data.Models.Metadata.FeatureType? FeatureType
        {
            get => _internal.FeatureType;
            set => _internal.FeatureType = value;
        }

        /// <inheritdoc>/>
        public override Data.Models.Metadata.ItemType ItemType
            => Data.Models.Metadata.ItemType.Feature;

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

        public Feature() : base() { }

        public Feature(Data.Models.Metadata.Feature item) : base(item) { }

        public Feature(Data.Models.Metadata.Feature item, Machine machine, Source source) : this(item)
        {
            Source = source;
            CopyMachineInformation(machine);
        }

        public Feature(Data.Models.Metadata.Feature item, long machineIndex, long sourceIndex) : this(item)
        {
            SourceIndex = sourceIndex;
            MachineIndex = machineIndex;
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
        public override object Clone() => new Feature(GetInternalClone());

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
            if (other is Feature otherFeature)
                return _internal.Equals(otherFeature._internal);

            // Everything else fails
            return false;
        }

        #endregion
    }
}
