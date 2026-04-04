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
            get => (_internal as Data.Models.Metadata.Feature)?.FeatureType;
            set => (_internal as Data.Models.Metadata.Feature)?.FeatureType = value;
        }

        /// <inheritdoc>/>
        public override Data.Models.Metadata.ItemType ItemType
            => Data.Models.Metadata.ItemType.Feature;

        public string? Name
        {
            get => (_internal as Data.Models.Metadata.Feature)?.Name;
            set => (_internal as Data.Models.Metadata.Feature)?.Name = value;
        }

        public Data.Models.Metadata.FeatureStatus? Overall
        {
            get => (_internal as Data.Models.Metadata.Feature)?.Overall;
            set => (_internal as Data.Models.Metadata.Feature)?.Overall = value;
        }

        public Data.Models.Metadata.FeatureStatus? Status
        {
            get => (_internal as Data.Models.Metadata.Feature)?.Status;
            set => (_internal as Data.Models.Metadata.Feature)?.Status = value;
        }

        public string? Value
        {
            get => (_internal as Data.Models.Metadata.Feature)?.Value;
            set => (_internal as Data.Models.Metadata.Feature)?.Value = value;
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
            => (_internal as Data.Models.Metadata.Feature)?.Clone() as Data.Models.Metadata.Feature ?? [];

        #endregion
    }
}
